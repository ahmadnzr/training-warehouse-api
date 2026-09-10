using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Notifications;
using WarehouseWeb.Api.Models;
using WarehouseWeb.Api.Models.Enums;
using WarehouseWeb.Api.Repositories;

namespace WarehouseWeb.Api.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationLogRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationLogRepository repository,
            ICacheService cacheService,
            ILogger<NotificationService> logger)
        {
            _repository = repository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<PaginatedResponse<NotificationDto>> ListUserNotificationsAsync(
            Guid userId,
            string role,
            PaginationRequest request)
        {
            request.Validate();

            var cacheKey = $"notifications:user:{userId}:page={request.Page}:per_page={request.PerPage}";

            return await _cacheService.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var items = await _repository.ListByRecipientAsync(
                        userId,
                        role,
                        request.GetOffset(),
                        request.PerPage
                    );

                    var total = await _repository.CountByRecipientAsync(userId, role);

                    return new PaginatedResponse<NotificationDto>
                    {
                        Items = items.Select(MapToDto).ToList(),
                        Meta = new PaginationMeta
                        {
                            Page = request.Page,
                            PerPage = request.PerPage,
                            Total = total,
                            TotalPage = (int)Math.Ceiling(total / (double)request.PerPage)
                        }
                    };
                },
                TimeSpan.FromMinutes(3)
            );
        }

        public async Task<NotificationDto> CreateAsync(CreateNotificationRequestDto request)
        {
            var log = new NotificationLog
            {
                Title = request.Title,
                Message = request.Message,
                TargetRole = request.TargetRole ?? string.Empty,
                RecipientUserId = request.RecipientUserId,
                Status = NotificationStatus.Sent,
                CreatedAt = DateTime.UtcNow,
                SentAt = DateTime.UtcNow
            };

            await _repository.AddAsync(log);

            if (request.RecipientUserId.HasValue)
            {
                await _cacheService.RemoveByPrefixAsync($"notifications:user:{request.RecipientUserId.Value}:");
            }

            return MapToDto(log);
        }

        public async Task MarkAsReadAsync(Guid id, Guid userId, string role)
        {
            var log = await _repository.FindByIdAsync(id);
            if (log == null) throw new NotFoundException("Notification not found");

            var roleName = role.ToLowerInvariant();
            if (log.RecipientUserId != userId && log.TargetRole.ToLower() != roleName)
            {
                throw new ForbiddenException("You do not have access to this notification");
            }

            log.Status = NotificationStatus.Read;
            await _repository.UpdateAsync(log);

            await _cacheService.RemoveByPrefixAsync($"notifications:user:{userId}:");
        }

        public async Task DeleteAsync(Guid id, Guid userId, string role)
        {
            var log = await _repository.FindByIdAsync(id);
            if (log == null) throw new NotFoundException("Notification not found");

            var roleName = role.ToLowerInvariant();
            if (log.RecipientUserId != userId && log.TargetRole.ToLower() != roleName)
            {
                throw new ForbiddenException("You do not have access to this notification");
            }

            await _repository.DeleteAsync(log);

            await _cacheService.RemoveByPrefixAsync($"notifications:user:{userId}:");
        }

        public async Task NotifySupervisorsMovementCompletedAsync(
            Guid movementId,
            string movementNumber,
            string movementType)
        {
            var log = new NotificationLog
            {
                Title = "Stock movement completed",
                Message = $"Movement {movementNumber} ({movementType}) has been completed.",
                TargetRole = "supervisor",
                RelatedMovementId = movementId,
                Status = NotificationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                log.Status = NotificationStatus.Sent;
                log.SentAt = DateTime.UtcNow;
                await _repository.AddAsync(log);

                _logger.LogInformation(
                    "Notification sent to supervisors for movement {MovementNumber}",
                    movementNumber);
            }
            catch (Exception ex)
            {
                log.Status = NotificationStatus.Failed;
                log.ErrorMessage = ex.Message;
                log.SentAt = DateTime.UtcNow;
                await _repository.AddAsync(log);
                _logger.LogError(ex, "Failed to notify supervisors for {MovementNumber}", movementNumber);
            }
        }

        private static NotificationDto MapToDto(NotificationLog entity)
        {
            return new NotificationDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Message = entity.Message,
                TargetRole = entity.TargetRole,
                RelatedMovementId = entity.RelatedMovementId,
                RecipientUserId = entity.RecipientUserId,
                Status = entity.Status.ToString().ToLowerInvariant(),
                CreatedAt = entity.CreatedAt,
                SentAt = entity.SentAt
            };
        }
    }
}

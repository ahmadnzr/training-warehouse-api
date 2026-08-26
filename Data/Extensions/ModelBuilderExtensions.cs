using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WarehouseWeb.Api.Common;

namespace WarehouseWeb.Api.Data.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplySoftDeleteQueryFilter(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var propertyMethodInfo = typeof(EF).GetMethod(nameof(EF.Property), [typeof(object), typeof(string)])!
                    .MakeGenericMethod(typeof(DateTime?));
                var deletedAtProperty = Expression.Call(propertyMethodInfo, parameter, Expression.Constant(nameof(ISoftDeletable.DeletedAt)));
                var compareExpression = Expression.MakeBinary(ExpressionType.Equal, deletedAtProperty, Expression.Constant(null, typeof(DateTime?)));
                var lambda = Expression.Lambda(compareExpression, parameter);

                entityType.SetQueryFilter(lambda);
            }
        }
    }
}

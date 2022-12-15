using Microsoft.OData.Client;
using Sandbox.OData.Client;

namespace SandboxODataClient.Helpers;

public static class GeneralContextExtensions
{
    public static void AddEntity<TEntity>(this Container context, TEntity entity)
        where TEntity : class
    {
        context.AddObject(entity.GetEntitySetName(), entity);
    }

    public static void AddAndTrackEntity<TEntity>(this Container context, TEntity entity)
        where TEntity : class
    {
        context.AddEntity(entity);
        var collection = new DataServiceCollection<TEntity>(context);
        var attachedEntity = (TEntity) context.GetEntityDescriptor(entity).Entity;
        collection.Load(attachedEntity);
    }

    /// <inheritdoc cref="DataServiceContext.AttachTo(string, object)"/>
    public static void AttachEntity<TEntity>(this Container context, TEntity entity)
        where TEntity : class
    {
        context.AttachTo(entity.GetEntitySetName(), entity);
    }

    /// <summary>
    /// Attach and track an entity to be able to make use of <see cref="SaveChangesOptions"/>, e.g. <see cref="SaveChangesOptions.PostOnlySetProperties"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="context"></param>
    /// <param name="entity"></param>
    public static void AttachAndTrackEntity<TEntity>(this Container context, TEntity entity)
        where TEntity : class
    {
        context.AttachTo(entity.GetEntitySetName(), entity);
        var collection = new DataServiceCollection<TEntity>(context);
        var attachedEntity = (TEntity) context.GetEntityDescriptor(entity).Entity;
        collection.Load(attachedEntity);
    }

    /// <summary>
    ///     Gets the entityset name to which the entity belongs.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static string GetEntitySetName<TEntity>(this TEntity entity)
        where TEntity : class
    {
        return entity.GetType().GetCustomAttributes(true)
            .Where(a => a is EntitySetAttribute t).Cast<EntitySetAttribute>()
            .FirstOrDefault()!.EntitySet;
    } 

    /// <summary>
    ///     Resets tracked entities.
    /// </summary>
    /// <param name="context"></param>
    public static void Clear(this Container context)
    {
        var trackedEntities = context.EntityTracker.Entities;
        foreach (var entity in trackedEntities)
        {
            if (!context.Detach(entity.Entity))
            {
                throw new Exception($"Unable to detach entity: {entity.SelfLink}");
            }
        }
    }
}

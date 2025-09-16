using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace RankingPadelAPI.Repositories;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    protected readonly DbSet<TEntity> _entities;

    protected readonly DbContext _context;

    public Repository(DbContext context)
    {
        _context = context;
        _entities = context.Set<TEntity>();
    }

    public bool Exist(Expression<Func<TEntity, bool>> predicate)
    {
        return _entities.Any(predicate);
    }

    public void Add(TEntity entity)
    {
        _entities.Add(entity);

        _context.SaveChanges();
    }

    public virtual TEntity Get(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _entities;

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        var entity = query.FirstOrDefault(predicate);

        if (entity == null)
        {
            throw new InvalidOperationException($"Entity {typeof(TEntity).Name} not found");
        }

        return entity;
    }

    public void Remove(TEntity entity)
    {
        _entities.Remove(entity);

        _context.SaveChanges();
    }

    public virtual void Update(TEntity entity)
    {
        _entities.Update(entity);

        _context.SaveChanges();
    }

    public virtual IQueryable<TEntity> GetAll()
    {
        return _entities.AsQueryable();
    }

    public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> expression)
    {
        return _entities.Where(expression);
    }
}

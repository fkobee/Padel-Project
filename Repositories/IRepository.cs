using System.Linq.Expressions;

namespace RankingPadelAPI.Repositories;

public interface IRepository<TEntity>
    where TEntity : class
{
    bool Exist(Expression<Func<TEntity, bool>> expression);

    void Add(TEntity entity);

    IQueryable<TEntity> GetAll();
    IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> expression);

    TEntity Get(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes);

    void Remove(TEntity entity);

    void Update(TEntity entity);
}

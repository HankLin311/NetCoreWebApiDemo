using Demo.Repository.implements.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Demo.Repository.implements
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _dbContext;

        public GenericRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 新增一筆資料
        /// </summary>
        public virtual void Add(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("新增資料為空");
            }

            _dbContext.Set<TEntity>().Add(entity);
        }

        /// <summary>
        /// 查詢第一筆資料
        /// </summary>
        public virtual TEntity? FindFirst(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// 查詢所有資料
        /// </summary>
        public virtual ICollection<TEntity> FindAll()
        {
            return _dbContext.Set<TEntity>().ToList();
        }

        /// <summary>
        /// 條件查詢資料
        /// </summary>
        public virtual ICollection<TEntity> FindConditions(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().Where(predicate).ToList();
        }

        /// <summary>
        /// 查詢單筆資料
        /// </summary>
        public virtual TEntity? FindSingle(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().SingleOrDefault(predicate);
        }

        /// <summary>
        /// 刪除一筆資料
        /// </summary>
        public virtual void Remove(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("刪除資料為空");
            }

            _dbContext.Set<TEntity>().Remove(entity);
        }

        /// <summary>
        /// 條件刪除資料
        /// </summary>
        public virtual void RemoveConditions(Expression<Func<TEntity, bool>> predicate)
        {
            ICollection<TEntity> entities = FindConditions(predicate);

            if (entities.Count > 0)
            {
                _dbContext.Set<TEntity>().RemoveRange(entities);
            }
        }

        /// <summary>
        /// 更新一筆資料
        /// </summary>
        public virtual void Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("修改資料為空");
            }

            _dbContext.Entry(entity).CurrentValues.SetValues(entity);
        }
    }
}

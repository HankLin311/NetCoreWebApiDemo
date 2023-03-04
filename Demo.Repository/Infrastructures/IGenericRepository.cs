using System.Linq.Expressions;

namespace Demo.Repository.Infrastructures
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// 新增一筆資料
        /// </summary>
        public void Add(TEntity entity);

        /// <summary>
        /// 查詢第一筆資料
        /// </summary>
        public TEntity? FindFirst(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 查詢單筆資料
        /// </summary>
        public TEntity? FindSingle(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 條件查詢資料
        /// </summary>
        public ICollection<TEntity> FindConditions(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 查詢所有資料
        /// </summary>
        public ICollection<TEntity> FindAll();

        /// <summary>
        /// 刪除一筆資料
        /// </summary>
        void Remove(TEntity entity);

        /// <summary>
        /// 條件刪除資料
        /// </summary>
        void RemoveConditions(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 更新一筆資料
        /// </summary>
        void Update(TEntity entity);
    }
}
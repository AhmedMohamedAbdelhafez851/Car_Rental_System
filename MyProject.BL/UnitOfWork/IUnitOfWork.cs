// MyProject.BL/UnitOfWork/IUnitOfWork.cs
using Microsoft.EntityFrameworkCore.Storage;
using MyProject.BL.Repositories;
namespace MyProject.BL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        void Commit();

        void Rollback();
    }
}
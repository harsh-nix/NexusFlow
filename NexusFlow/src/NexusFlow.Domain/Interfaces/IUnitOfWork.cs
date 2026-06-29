using System;
using System.Collections.Generic;
using System.Text;

using NexusFlow.Domain.Interfaces.Repositories;

namespace NexusFlow.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;
        Task<int> SaveChangesAsync();
    }
}
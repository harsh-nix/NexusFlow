using System;
using System.Collections.Generic;
using System.Text;

using NexusFlow.Domain.Interfaces;
using NexusFlow.Domain.Interfaces.Repositories;
using NexusFlow.Infrastructure.Data;
using NexusFlow.Infrastructure.Repositories;

namespace NexusFlow.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NexusFlowDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(NexusFlowDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
                _repositories[type] = new GenericRepository<T>(_context);

            return (IGenericRepository<T>)_repositories[type];
        }

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();
    }
}
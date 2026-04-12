using Microsoft.EntityFrameworkCore;
using Pms.Core.Entities;
using Pms.Core.Interfaces;
using Pms.Data;

namespace Pms.Dal
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly IDbContextFactory<ApplicationDbContext> _factory;

        public Repository(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async virtual Task<IEnumerable<T>> GetAllAsync()
        {
            using var context = await _factory.CreateDbContextAsync();
            return await context.Set<T>().AsNoTracking().ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            using var context = await _factory.CreateDbContextAsync();
            return await context.Set<T>().FindAsync(id);
        }

        public virtual async Task CreateAsync(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            using var context = await _factory.CreateDbContextAsync();
            await context.Set<T>().AddAsync(item);
            await context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            using var context = await _factory.CreateDbContextAsync();
            var existing = await context.Set<T>().FindAsync(item.Id);
            if (existing != null)
            {
                context.Entry(existing).CurrentValues.SetValues(item);

                await context.SaveChangesAsync();
            }
        }

        public virtual async Task DeleteAsync(int id)
        {
            using var context = await _factory.CreateDbContextAsync();
            await context.Set<T>()
                    .Where(x => x.Id == id)
                    .ExecuteDeleteAsync();
        }
    }
}

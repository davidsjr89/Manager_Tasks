using Domain.Interfaces.Generics;
using Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace Infrastructure.Repository.Generics
{
    public class RepositoryGenerics<T> : IGeneric<T>, IDisposable where T : class
    {
        private bool disposed = false;
        private SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        private readonly DbContextOptions<ContextBase> _optionsBuilder;

        public RepositoryGenerics()
        {
            _optionsBuilder = new DbContextOptions<ContextBase>();
        }

        public async Task Add(T item)
        {
            try
            {
                using(var data = new ContextBase(_optionsBuilder))
                {
                    using var transaction = data.Database.BeginTransaction();
                
                    await data.Set<T>().AddAsync(item);
                    await data.SaveChangesAsync();
                
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Problema ao adicionar no banco de dados", ex.InnerException);
            }
        }

        public async Task Delete(T item)
        {
            try
            {
                using (var data = new ContextBase(_optionsBuilder))
                {
                    using var transaction = data.Database.BeginTransaction();
                
                    data.Set<T>().Remove(item);
                    await data.SaveChangesAsync();

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Problema ao deletar no banco de dados", ex.InnerException);
            }
        }

        public async Task<T> GetEntityById(int id)
        {
            try
            {
                using (var data = new ContextBase(_optionsBuilder))
                {
                    return await data.Set<T>().FindAsync(id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Problema ao fazer consulta no banco de dados", ex.InnerException);
            }
        }

        public async Task<IList<T>> List()
        {
            try
            {
                using (var data = new ContextBase(_optionsBuilder))
                {
                    return await data.Set<T>().ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Problema ao fazer consulta no banco de dados", ex.InnerException);
            }
        }

        public async Task Update(T item)
        {
            try
            {
                using (var data = new ContextBase(_optionsBuilder))
                {
                    using var transaction = data.Database.BeginTransaction();

                    data.Set<T>().Update(item);
                    await data.SaveChangesAsync();

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Problema ao fazer consulta no banco de dados", ex.InnerException);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if(disposed) 
                return;

            if (disposing)
            {
                handle.Dispose();
            }

            disposed = true;
        }
    }
}

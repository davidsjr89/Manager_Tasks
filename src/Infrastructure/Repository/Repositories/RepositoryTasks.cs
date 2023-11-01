using Domain.Interfaces;
using Entities.Enum;
using Entities.Model;
using Infrastructure.Configuration;
using Infrastructure.Repository.Generics;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructure.Repository.Repositories
{
    public class RepositoryTasks : RepositoryGenerics<Tasks>, ITasks
    {
        private readonly DbContextOptions<ContextBase> _optionsBuilder;

        public RepositoryTasks()
        {
            _optionsBuilder = new DbContextOptions<ContextBase>();
        }

        public async Task<IList<Tasks>> GetEntityByStatus(Expression<Func<Tasks, bool>> exTasks)
        {
            try
            {
                using (var data = new ContextBase(_optionsBuilder))
                {
                    return await data.Tasks.Where(exTasks).AsNoTracking().ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Problema ao fazer consulta no banco de dados", ex.InnerException);
            }
        }
    }
}

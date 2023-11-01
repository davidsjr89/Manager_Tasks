using Domain.Interfaces.Generics;
using Entities.Model;
using System.Linq.Expressions;

namespace Domain.Interfaces
{
    public interface ITasks : IGeneric<Tasks>
    {
        Task<IList<Tasks>> GetEntityByStatus(Expression<Func<Tasks, bool>> exTasks);
    }
}
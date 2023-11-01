using Entities.Enum;
using Entities.Model;

namespace Domain.Interfaces.InterfaceServices
{
    public interface IServiceTasks
    {
        Task Add(Tasks item);
        Task Update(Tasks item);
        Task<IList<Tasks>> GetEntityByStatus(Status status);
    }
}

using Domain.Interfaces;
using Domain.Interfaces.InterfaceServices;
using Entities.Enum;
using Entities.Model;

namespace Domain.Services
{
    public class ServiceTasks : IServiceTasks
    {
        private readonly ITasks _iTasks;

        public ServiceTasks(ITasks iTasks)
        {
            _iTasks = iTasks;
        }

        public async Task Add(Tasks item)
        {
            var validateTitle = item.IsPropertyString(item.Title, "Title");
            var validateDescription = item.IsPropertyString(item.Description, "Description");

            if (validateTitle && validateDescription)
            {
                item.Created = DateTime.UtcNow;
                await _iTasks.Add(item);
            }
        }

        public async Task<IList<Tasks>> GetEntityByStatus(Status status)
        {
            return await _iTasks.GetEntityByStatus(x => x.Status == status);
        }

        public async Task Update(Tasks item)
        {
            var validateTitle = item.IsPropertyString(item.Title, "Title");
            var validateDescription = item.IsPropertyString(item.Description, "Description");

            if (validateTitle && validateDescription)
            {
                await _iTasks.Update(item);
            }
        }
    }
}

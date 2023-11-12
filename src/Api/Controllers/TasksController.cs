using Api.Models;
using AutoMapper;
using Domain.Interfaces;
using Domain.Interfaces.InterfaceServices;
using Entities.Enum;
using Entities.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IMapper _iMapper;
        private readonly ITasks _tasks;
        private readonly IServiceTasks _serviceTasks;
        private readonly ILogger<TasksController> _logger;

        public TasksController(
            IMapper iMapper, 
            ITasks tasks, 
            IServiceTasks serviceTasks,
            ILogger<TasksController> logger)
        {
            _iMapper = iMapper;
            _tasks = tasks;
            _serviceTasks = serviceTasks;
            _logger = logger;
        }

        [Authorize]
        [Produces("application/json")]
        [HttpPost("/api/add")]
        public async Task<ActionResult<List<Notifies>>> Add(TasksAddViewModel tasks)
        {
            try
            {
                tasks.UserId = await IdUserLogin();

                var tasksMap = _iMapper.Map<Tasks>(tasks);
                await _serviceTasks.Add(tasksMap);

                _logger.LogInformation("Adicionado uma nova tarefa", tasks.Id);
                return StatusCode((int)HttpStatusCode.Created, tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        [Authorize]
        [Produces("application/json")]
        [HttpPut("/api/update")]
        public async Task<ActionResult<List<Notifies>>> Update(TasksAddViewModel tasks)
        {
            try
            {
                if(IsModified(tasks.UserId))
                {
                    var tasksMap = _iMapper.Map<Tasks>(tasks);
                    await _serviceTasks.Update(tasksMap);

                    _logger.LogInformation("Atualizado item com sucesso", tasks.Id);
                    return StatusCode((int)HttpStatusCode.Unauthorized, tasks);
                }

                _logger.LogWarning("Usuário não têm acesso a modificar o item: " + tasks.Id, tasks.UserId);
                return StatusCode((int)HttpStatusCode.BadRequest, "Usuário não é permitido atualizar");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        [Authorize]
        [Produces("application/json")]
        [HttpDelete("/api/delete")]
        public async Task<ActionResult<List<Notifies>>> Delete(TasksViewModel tasks)
        {
            try
            {
                if (IsModified(tasks.UserId))
                {
                    var tasksMap = _iMapper.Map<Tasks>(tasks);
                    await _tasks.Delete(tasksMap);

                    _logger.LogInformation("Item deletado com sucesso", tasks.Id);
                    return StatusCode((int)HttpStatusCode.OK, tasks);
                }

                _logger.LogWarning("Usuário não têm acesso a modificar o item: " + tasks.Id, tasks.UserId);
                return StatusCode((int)HttpStatusCode.BadRequest, "Usuário não é permitido deletar");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        [Authorize]
        [Produces("application/json")]
        [HttpGet("/api/getentitybyid")]
        public async Task<TasksViewModel> GetEntityById(int id)
        {
            try
            {
                var tasks = await _tasks.GetEntityById(id);
                var tasksMap = _iMapper.Map<TasksViewModel>(tasks);

                return tasksMap;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        [Authorize]
        [Produces("application/json")]
        [HttpGet("/api/getentitybystatus")]
        public async Task<List<TasksViewModel>> GetEntityByStatus(Status status)
        {
            try
            {
                var tasks = await _serviceTasks.GetEntityByStatus(status);
                var tasksMap = _iMapper.Map<List<TasksViewModel>>(tasks);

                return tasksMap;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        [Authorize]
        [Produces("application/json")]
        [HttpGet("/api/list")]
        public async Task<List<TasksViewModel>> List()
        {
            try
            {
                var tasks = await _tasks.List();
                var tasksMap = _iMapper.Map<List<TasksViewModel>>(tasks);

                return tasksMap;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private async Task<string> IdUserLogin()
        {
            if(User != null)
            {
                var idUser = User.FindFirst("idUser");
                return idUser.Value;
            }

            return string.Empty;
        }

        private bool IsModified(string id)
        {
            var idUser = User.Claims.First(c => c.Type == "idUser").Value;
            return idUser == id;
        }
    }
}

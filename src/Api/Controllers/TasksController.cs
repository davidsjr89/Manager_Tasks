using Api.Models;
using AutoMapper;
using Domain.Interfaces;
using Domain.Interfaces.InterfaceServices;
using Entities.Enum;
using Entities.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IMapper _iMapper;
        private readonly ITasks _tasks;
        private readonly IServiceTasks _serviceTasks;
        private readonly ILogger _logger;

        public TasksController(
            IMapper iMapper, 
            ITasks tasks, 
            IServiceTasks serviceTasks, 
            ILogger logger)
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
            tasks.UserId = await IdUserLogin();

            var tasksMap = _iMapper.Map<Tasks>(tasks);
            await _serviceTasks.Add(tasksMap);

            return StatusCode((int)HttpStatusCode.Created, tasks);
        }

        [Authorize]
        [Produces("application/json")]
        [HttpPut("/api/update")]
        public async Task<ActionResult<List<Notifies>>> Update(TasksAddViewModel tasks)
        {
            if(IsModified(tasks.UserId))
            {
                var tasksMap = _iMapper.Map<Tasks>(tasks);
                await _serviceTasks.Update(tasksMap);

                return StatusCode((int)HttpStatusCode.OK, tasks);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, "Usuário não é permitido atualizar");
        }

        [Authorize]
        [Produces("application/json")]
        [HttpDelete("/api/delete")]
        public async Task<ActionResult<List<Notifies>>> Delete(TasksViewModel tasks)
        {
            if (IsModified(tasks.UserId))
            {
                var tasksMap = _iMapper.Map<Tasks>(tasks);
                await _tasks.Delete(tasksMap);

                return StatusCode((int)HttpStatusCode.OK, tasks);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, "Usuário não é permitido deletar");
        }

        [Authorize]
        [Produces("application/json")]
        [HttpPost("/api/getentitybyid")]
        public async Task<TasksViewModel> GetEntityById(int id)
        {
            var tasks = await _tasks.GetEntityById(id);
            var tasksMap = _iMapper.Map<TasksViewModel>(tasks);

            return tasksMap;
        }

        [Authorize]
        [Produces("application/json")]
        [HttpPost("/api/getentitybystatus")]
        public async Task<TasksViewModel> GetEntityByStatus(Status status)
        {
            var tasks = await _serviceTasks.GetEntityByStatus(status);
            var tasksMap = _iMapper.Map<TasksViewModel>(tasks);

            return tasksMap;
        }

        [Authorize]
        [Produces("application/json")]
        [HttpPost("/api/list")]
        public async Task<List<TasksViewModel>> List()
        {
            var tasks = await _tasks.List();
            var tasksMap = _iMapper.Map<List<TasksViewModel>>(tasks);

            return tasksMap;
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

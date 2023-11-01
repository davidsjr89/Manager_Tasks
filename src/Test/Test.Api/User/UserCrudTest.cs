using Api.Controllers;
using Api.Models;
using AutoMapper;
using Entities.Model;
using Infrastructure.Configuration;
using Infrastructure.Repository.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Test.Api.User
{
    public class UserCrudTest : BaseTest, IClassFixture<DbTest>
    {
        private ServiceProvider _serviceProvider;
        private UserController _userController;

        public UserCrudTest(DbTest dbTest)
        {
            _serviceProvider = dbTest.ServiceProvider;
            _userController = new UserController(
                new Mock<UserManager<ApplicationUser>>().Object,
                new Mock<SignInManager< ApplicationUser>>().Object,
                new Mock<ILogger<UserController>>().Object
            );
        }

        [Fact]
        public async Task Is_CRUD()
        {
            Login login = new Login
            {
                Email = "david.david.com",
                Password = "David123!"
            };
            
            var t = _userController.AddUserIdentity(login );


            Tasks tasks = new Tasks()
            {
                Description = "Curso",
                Title = "React",
                UserId = Guid.NewGuid().ToString()
            };

            using(var context = _serviceProvider.GetService<ContextBase>())
            {
                var user = new ApplicationUser
                {
                    Email = login.Email,
                    UserName = login.Email
                };

                //var result = await _userManager.CreateAsync(user, login.Password);

                RepositoryTasks _repositoryTasks = new RepositoryTasks();
                await _repositoryTasks.Add(tasks);

                var tasksSelect = await _repositoryTasks.GetEntityById(1);

                Assert.Equal(tasksSelect.Id, tasks.Id);
            }
        }
    }
}

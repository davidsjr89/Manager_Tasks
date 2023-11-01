using Api.Models;
using Api.Token;
using Entities.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Text;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("/api/login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            if(string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.Password))
            {
                _logger.LogInformation("Usuário ou senha não preenchidos ", login.Email);
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }

            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);

            if (result.Succeeded)
            {
                var userCurrent = await _userManager.FindByEmailAsync(login.Email);
                var idUser = userCurrent.Id;

                var token = new TokenJwtBuilder()
                    .AddSecurityKey(JwtSecurityKey.Create("eX|Isq-G|F8f(4_+QA;p8uAzQn>&W/ijbSPx7}JR&4<X9O,V01s==2;(UB;IJ`9"))
                    .AddSubject("Empresa")
                    .AddIssuer("Manager_Tasks.Security.Bearer")
                    .AddAudience("Manager_Tasks.Security.Bearer")
                    .AddClaims("idUser", idUser)
                    .AddExpiry(10)
                    .Builder();

                _logger.LogInformation("Usuário logado com sucesso ", login.Email);
                return StatusCode((int)HttpStatusCode.Created, new { userName = login.Email, id = idUser, token = token.value });
            }

            _logger.LogInformation("Usuário não tem autorização para acessar aplicação ", login.Email);
            return Unauthorized();
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("/api/adduseridentity")]
        public async Task<IActionResult> AddUserIdentity([FromBody] Login login)
        {
            if(string.IsNullOrWhiteSpace(login.Email) && string.IsNullOrWhiteSpace(login.Password))
            {
                _logger.LogError("Falta alguns dados");
                return BadRequest("Falta alguns dados");
            }

            var user = new ApplicationUser
            {
                Email = login.Email,
                UserName = login.Email
            };

            var result = await _userManager.CreateAsync(user, login.Password);

            if (result.Errors.Any())
            {
                _logger.LogInformation(result.Errors.ToString(), login.Email);
                return StatusCode((int)HttpStatusCode.BadRequest, result.Errors);
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var results = await _userManager.ConfirmEmailAsync(user, code);

            if (results.Succeeded)
            {
                _logger.LogInformation("Criado o usuário com sucesso", login.Email);
                return CreatedAtAction(nameof(Login), new { identity = User.Identity }, "Usuário adicionado com sucesso");
            }
            else
            {
                _logger.LogError("Não possível criar usuário", login.Email);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Erro ao confirmar usuário");
            }
        }
    }
}

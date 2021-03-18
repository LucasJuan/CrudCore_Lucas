using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ExemploCore3.Models;
using ExemploCore3.Services;
using ExemploCore3.TokenService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult RecuperarSenha()
        {
            return View();
        }
        public IActionResult ConfirmaCredentials()
        {
            return View();
        }
        [Route("Home/ConfirmCredentials")]
        public async Task<IActionResult> ConfirmarDados([FromBody] Acessa form)
        {
            if (!DAL.Home.VerificaChaveAtiva(form))
            {
                return Json(new { result = false, message = "Chave inválida" });

            }
            var retorno = DAL.Home.ConfirmCredentials(form);
            if(retorno != null)
            {
                return Json(new { result = true, message = "Usuário validado com sucesso." });
            }
            else
            {
                return Json(new { result = false, message = "Não foi possível validar o usuário, verifique suas credenciais" });
            }

        }
        [HttpPost]
        [Authorize]
        public IActionResult Cadastrar([FromBody] Acessa form)
        {
            try
            {
                if (User.Identity.Name != null)
                {
                    var i = HttpContext.Request.Headers;
                    var b = HttpContext.User.Claims;

                    var db = new dbcrudContext();
                    if(form.Id <=0)
                    {
                        var email = db.Acessa.Where(c => c.Email.ToUpper() == form.Email.ToUpper() && c.DataExclusao == null).Select(c => c.Email).FirstOrDefault();

                        if (!string.IsNullOrEmpty(email))
                        {
                            return Json(new { result = false, message = "E-mail Já cadastrado no sistema" });
                        }
                    }
                    
                    var retorno = DAL.Home.set(form);
                    return Json(new { result = true, retorno = retorno });
                }
                else
                    return Unauthorized();

            }
            catch (Exception e)
            {

                throw;
            }

        }
        [HttpPost]
        [Route("Home/login")]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] Acessa model)
        {

            var user = DAL.Home.validaUsuario(model);
            var ativo = DAL.Home.validaUsuarioAtivo(model);
            if(ativo)
            {
                // Verifica se o usuário existe
                if (user == null)
                    return NotFound(new { result = false, message = "Usuário ou senha inválidos" });
                //verifica se o usuário está ativo

                var claim = new ClaimsIdentity(new Claim[]
                        {
                    new Claim(ClaimTypes.Name, user.Email.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                    }, CookieAuthenticationDefaults.AuthenticationScheme);
                // Gera o Token
                var token = TokenService.GenerateToken(user, claim);
                var authProp = new AuthenticationProperties
                {
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(25),
                    IsPersistent = true
                };
                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claim),
                    authProp
                    );

                // Oculta a senha
                user.Senha = "";

                // Retorna os dados
                return new
                {
                    result = true,
                    user = user,
                    token = token,
                    message = "Seja bem vindo " + user.Nome
                };
            }
            else
            {
                DAL.Home.EmailConfirmacao(model.Email);
                return Json(new { result = false, message = "Cadastro ainda não está ativo, por favor verifique em seu e-mail o Link para a ativação." });
            }

        }
        public async Task<List<Acessa>> BuscaResultados()
        {
            var retorno = (List<Acessa>)DAL.Home.GetAll();
            JsonResult jsonResult = Json(retorno);
            return retorno;
        }
        [HttpGet]
        [Authorize]
        public async Task<Acessa> BuscaEditar( int codigo)
        {
            var retorno = DAL.Home.BuscaEditar(codigo);
            if (retorno != null)
            {
               // return PartialView("_index", retorno);
                return retorno;
            }
            else
                return null;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<bool> Excluir([FromBody] Acessa form)
        {
            var retorno = DAL.Home.Delet(form.Id);
            JsonResult jsonResult = Json(retorno);
            return retorno;
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {


            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // HttpContext.Session.Clear();
            var a = User.Identity.Name;
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<ActionResult> RecuperarEmail([FromBody] Acessa form)
        {
            bool retorno = DAL.Home.ForgotPassword(form.Email);
            if (retorno)
            {
                return Json(new { result = true, message = "As instruções para alteração de senha foram encaminhada em seu e-mail" });
            }
            else
                return BadRequest(404);
        }
        [HttpPost]
        [Route("Home/RenewPWD")]
        public async Task<ActionResult> AlteraSenha([FromBody] Acessa form)
        {
            var retorno = DAL.Home.RenewPassword(form);
            if (retorno != null)
            {
                return Json(new { result = true, message = "Senha alterada com sucesso!" });
            }
            else
                return Json(new { result = true, message = "Não foi possível alterar a senha, verifique suas credênciais, caso o erro percista solicite uma nova recuperação de senha." });
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

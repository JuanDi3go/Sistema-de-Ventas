using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaDeVenta.BLL.Interfaces;
using SistemaDeVenta.Entity.Entities;
using SistemaDeVenta.WebApplication.Models;
using SistemaDeVenta.WebApplication.Models.ViewModels;
using SistemaDeVenta.WebApplication.Utilities.Response;
using System.Diagnostics;
using System.Security.Claims;

namespace SistemaDeVenta.WebApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IUsuarioService _usurioServicio;
        private readonly IMapper _mapper;


        public HomeController(ILogger<HomeController> logger, IUsuarioService usurioServicio, IMapper mapper)
        {
            _logger = logger;
            _usurioServicio = usurioServicio;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Perfil()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async  Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login","Acceso");
        }

        [HttpGet]
        public  async Task<IActionResult> ObtenerUsuario()
        {
            GenericResponse<VMUsuario> response = new GenericResponse<VMUsuario>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

                VMUsuario usuario = _mapper.Map<VMUsuario>(await _usurioServicio.ObtenerPorId(int.Parse(idUsuario)));


                response.Estado = true;
                response.Object = usuario;
            }
            catch (Exception ex)
            {


                response.Estado = true;
                response.Message = ex.Message;
                throw;
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPerfil([FromBody] VMUsuario modelo)
        {
            GenericResponse<VMUsuario> response = new GenericResponse<VMUsuario>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

                Usuario entidad = _mapper.Map<Usuario>(modelo);

                entidad.IdUsuario = int.Parse(idUsuario);

                bool resultado = await _usurioServicio.GuardarPerfil(entidad);


                response.Estado = resultado;
            }
            catch (Exception ex)
            {


                response.Estado = true;
                response.Message = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }



        [HttpPost]
        public async Task<IActionResult> CambiarClave([FromBody] VMCambiarClave modelo)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

                

                bool resultado = await _usurioServicio.CambiarClave(int.Parse(idUsuario),modelo.ClaveActual, modelo.ClaveNueva);


                response.Estado = resultado;
            }
            catch (Exception ex)
            {


                response.Estado = true;
                response.Message = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

    }
}
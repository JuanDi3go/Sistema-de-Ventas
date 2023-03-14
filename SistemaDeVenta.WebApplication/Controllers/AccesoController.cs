using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SistemaDeVenta.BLL.Interfaces;
using SistemaDeVenta.Entity.Entities;
using SistemaDeVenta.WebApplication.Models.ViewModels;
using System.Security.Claims;

namespace SistemaDeVenta.WebApplication.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        
        public AccesoController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public IActionResult Login()
        {
            ClaimsPrincipal claimuser = HttpContext.User;

            if (claimuser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");   
            }
            return View();
        }

        [HttpPost]
        public async  Task<IActionResult> Login(VMUsuarioLogin modelo)
        {
            Usuario usurioEncontrado = await _usuarioService.ObtenerPorCredenciales(modelo.Correo, modelo.Clave);

            if (usurioEncontrado == null)
            {
                ViewData["Mensaje"] = "no se encontraron Coincidencias";
                return View();
            }
            ViewData["Mensaje"] = null;

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usurioEncontrado.Nombre),
                new Claim(ClaimTypes.NameIdentifier, usurioEncontrado.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, usurioEncontrado.IdRol.ToString()),
                new Claim("UrlFoto", usurioEncontrado.UrlFoto),
            };

            ClaimsIdentity claimsIdentity= new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties propertys = new AuthenticationProperties()
            {
                AllowRefresh= true,
                IsPersistent= modelo.MantenerSesion,
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), propertys);

            return RedirectToAction("Index","Home");
        }



        public IActionResult RestablecerClave()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerClave(VMUsuarioLogin modelo)
        {
            try
            {
                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/RestablecerClave?clave=[clave]";

                bool resultado = await _usuarioService.RestablecerClave(modelo.Correo, urlPlantillaCorreo);

                if (resultado == true)
                {
                    ViewData["Mensaje"] = "Listo su contraseña fue restablecida revise su correo";
                    ViewData["MensajeError"] = null;
                }
                else
                {
                    ViewData["MensajeError"] ="Tenemos problemas porfavor intentelo de nuevo mas tarde";
                    ViewData["Mensaje"] = null;

                }
            }
            catch (Exception ex)
            {
                ViewData["MensajeError"] = ex.Message;
                ViewData["Mensaje"] = null;
            }
            return View();
        }
    }
}

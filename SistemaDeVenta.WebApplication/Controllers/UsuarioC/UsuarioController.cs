using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft;
using SistemaDeVenta.WebApplication.Models.ViewModels;
using SistemaDeVenta.WebApplication.Utilities.Response;
using SistemaDeVenta.BLL.Interfaces;
using SistemaDeVenta.Entity;
using Newtonsoft.Json;
using NuGet.DependencyResolver;
using SistemaDeVenta.Entity.Entities;
using Microsoft.AspNetCore.Authorization;

namespace SistemaDeVenta.WebApplication.Controllers.UsuarioC
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioService _usuarioService;
        private readonly IRolService _rolSercio;

        public UsuarioController(IMapper mapper, IUsuarioService usuarioService, IRolService rolSercio)
        {
            _mapper = mapper;
            _usuarioService = usuarioService;
            _rolSercio = rolSercio;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaRoles()
        {
            List<VMRol> vmListRoles = _mapper.Map<List<VMRol>>(await _rolSercio.Lista());
            return StatusCode(StatusCodes.Status200OK, vmListRoles);
        }
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMUsuario> vmListaUsuario = _mapper.Map<List<VMUsuario>>(await _usuarioService.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmListaUsuario });//para poder usar data table
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();

            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombreEnCodigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?Correo=[correo]&clave=[clave]";

                Usuario usuarioCreado = await _usuarioService.Crear(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto, urlPlantillaCorreo);

                vmUsuario = _mapper.Map<VMUsuario>(usuarioCreado);

                gResponse.Estado = true;
                gResponse.Object = vmUsuario;
            }
            catch (Exception ex)
            {

                gResponse.Estado = false;
                gResponse.Message = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();

            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombreEnCodigo, extension);
                    fotoStream = foto.OpenReadStream();
                }


                Usuario usuarioEditado = await _usuarioService.Editar(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto);

                vmUsuario = _mapper.Map<VMUsuario>(usuarioEditado);

                gResponse.Estado = true;
                gResponse.Object = vmUsuario;
            }
            catch (Exception ex)
            {

                gResponse.Estado = false;
                gResponse.Message = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]

        public async Task<IActionResult> Eliminar(int idUsuario)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _usuarioService.Eliminar(idUsuario);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Message = ex.Message;
                throw;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}

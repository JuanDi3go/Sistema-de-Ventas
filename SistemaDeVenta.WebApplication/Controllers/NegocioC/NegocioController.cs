using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using SistemaDeVenta.WebApplication.Models.ViewModels;
using SistemaDeVenta.WebApplication.Utilities.Response;
using SistemaDeVenta.BLL.Interfaces;
using SistemaDeVenta.Entity;
using SistemaDeVenta.Entity.Entities;
using Microsoft.AspNetCore.Authorization;

namespace SistemaDeVenta.WebApplication.Controllers.NegocioC
{
    [Authorize]
    public class NegocioController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INegocioService _negocioService;

        public NegocioController(INegocioService negocioService, IMapper mapper)
        {
            _negocioService = negocioService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async  Task<IActionResult> Obtener()
        {
            GenericResponse<VMNegocio> gResponse = new GenericResponse<VMNegocio>();

            try
            {
                VMNegocio vMNegocio = _mapper.Map<VMNegocio>(await _negocioService.Obtener());

                gResponse.Estado = true;
                gResponse.Object= vMNegocio;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Message = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarCambios([FromForm]IFormFile logo, [FromForm]string modelo)
        {
            GenericResponse<VMNegocio> gResponse = new GenericResponse<VMNegocio>();

            try
            {
                VMNegocio vMNegocio = JsonConvert.DeserializeObject<VMNegocio>(modelo);

                string nombreLogo = "";
                Stream logoStream = null;

                if (logo != null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N");
                    string extesion = Path.GetExtension(logo.FileName);
                    nombreLogo = string.Concat(nombreEnCodigo, extesion);
                    logoStream = logo.OpenReadStream();
                }
                Negocio negocioEditado = await _negocioService.GuardarCambios(_mapper.Map<Negocio>(vMNegocio), logoStream, nombreLogo);


                vMNegocio = _mapper.Map<VMNegocio>(negocioEditado);
                gResponse.Estado = true;
                gResponse.Object = vMNegocio;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Message = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}

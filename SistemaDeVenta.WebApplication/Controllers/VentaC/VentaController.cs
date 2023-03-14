using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SistemaDeVenta.WebApplication.Models.ViewModels;
using SistemaDeVenta.WebApplication.Utilities.Response;
using SistemaDeVenta.BLL.Interfaces;
using SistemaDeVenta.Entity;
using SistemaDeVenta.Entity.Entities;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SistemaDeVenta.WebApplication.Controllers.VentaCont
{
    [Authorize]
    public class VentaController : Controller
    {
        private readonly  ITipoDocumentoVentaService _tipoDocumentoServicio;
        private readonly  IVentaService _VentaServicio;
        private readonly  IMapper _mapper;
        private readonly IConverter _converter;
        public VentaController(IMapper mapper, IVentaService ventaServicio, ITipoDocumentoVentaService tipoDocumentoServicio, IConverter converter)
        {
            _mapper = mapper;
            _VentaServicio = ventaServicio;
            _tipoDocumentoServicio = tipoDocumentoServicio;
            _converter = converter;
        }

        public IActionResult NuevaVenta()
        {
            return View();
        }
        public IActionResult HistorialVenta()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaTipoDocumentoVenta()
        {
            List<VMTipoDocumentoVenta> vmTipoDocumentos = _mapper.Map<List<VMTipoDocumentoVenta>>(await _tipoDocumentoServicio.Lista());


            return StatusCode(StatusCodes.Status200OK,vmTipoDocumentos);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProductos(string busqueda )
        {
            List<VMProducto> vmListaProductos = _mapper.Map<List<VMProducto>>(await _VentaServicio.ObtenerProductos(busqueda));


            return StatusCode(StatusCodes.Status200OK, vmListaProductos);
        }



        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody] VMVenta modelo)
        {

            GenericResponse<VMVenta> gResponse= new GenericResponse<VMVenta>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

                modelo.IdUsuario = int.Parse(idUsuario);

                Venta ventaCreada = await _VentaServicio.Registrar(_mapper.Map<Venta>(modelo));
                modelo = _mapper.Map<VMVenta>(ventaCreada);

                gResponse.Estado = true;
                gResponse.Object = modelo;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Message = ex.Message ;
              
            }


            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpGet]
        public async Task<IActionResult> Historial(string numeroVenta, string fechaInicio, string fechaFin)
        {
            List<VMVenta> vmHistorialVenta = _mapper.Map<List<VMVenta>>(await _VentaServicio.Historial(numeroVenta,fechaInicio,fechaFin));


            return StatusCode(StatusCodes.Status200OK, vmHistorialVenta);
        }


        public IActionResult  MostrarPDFVenta(string numeroVenta)
        {
            string urlPlantilla = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/PDFVenta?numeroVenta={numeroVenta}";

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                },
                Objects =
                {
                    new ObjectSettings()
                    {
                        Page = urlPlantilla
                    }
                }
               
            };
            var archivoPDF = _converter.Convert(pdf);

            return File(archivoPDF, "application/pdf");
        }
    }
}

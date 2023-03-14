using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SistemaDeVenta.WebApplication.Models.ViewModels;
using SistemaDeVenta.BLL.Interfaces;
namespace SistemaDeVenta.WebApplication.Controllers.Plantilla
{
    public class PlantillaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INegocioService _negocioServicio;
        private readonly IVentaService _ventaServicio;

        public PlantillaController(IVentaService ventaServicio, INegocioService negocioServicio, IMapper mapper)
        {
            _ventaServicio = ventaServicio;
            _negocioServicio = negocioServicio;
            _mapper = mapper;
        }

        public IActionResult EnviarClave(string correo, string clave)
        {
            ViewData["Correo"] = correo;
            ViewData["Clave"] = clave;
            ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";

            return View();
        }
        public async Task<IActionResult> PDFVenta(string numeroVenta)
        {

            VMVenta vmVenta = _mapper.Map<VMVenta>(await _ventaServicio.Detalle(numeroVenta));
            VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioServicio.Obtener());

            VMPDFVenta modelo = new VMPDFVenta();

            modelo.negocio = vmNegocio;
            modelo.venta = vmVenta;

            return View(modelo);

        }

        public IActionResult RestablecerClave(string clave)
        {
        
            ViewData["Clave"] = clave;

            return View();
        }
    }
}

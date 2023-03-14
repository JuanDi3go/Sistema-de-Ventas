using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SistemaDeVenta.WebApplication.Models.ViewModels;
using SistemaDeVenta.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace SistemaDeVenta.WebApplication.Controllers.VentaCont
{
    [Authorize]
    public class ReporteController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IVentaService _ventaServicio;

        public ReporteController(IVentaService ventaServicio, IMapper mapper)
        {
            _ventaServicio = ventaServicio;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ReporteVenta(string fechaInicio, string fechaFin)
        {
            List<VMReporteVenta> vmLista = _mapper.Map<List<VMReporteVenta>>(await _ventaServicio.Reporte(fechaInicio, fechaFin));

            return StatusCode(StatusCodes.Status200OK, new {data = vmLista});
        }
    }
}

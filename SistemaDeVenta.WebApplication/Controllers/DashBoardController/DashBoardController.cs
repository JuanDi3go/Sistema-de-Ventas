using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaDeVenta.BLL.Interfaces;
using SistemaDeVenta.WebApplication.Models.ViewModels;
using SistemaDeVenta.WebApplication.Utilities.Response;

namespace SistemaDeVenta.WebApplication.Controllers.DashBoardController
{
    [Authorize]
    public class DashBoardController : Controller
    {
        private readonly IDashBoardService _dashboardService;

        public DashBoardController(IDashBoardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerResumen()
        {
            GenericResponse<VMDashBoard> gResponse = new GenericResponse<VMDashBoard>();
            try
            {
                VMDashBoard vmDashboard = new VMDashBoard();

                vmDashboard.TotalVentas = await _dashboardService.TotalVentasUltimaSemana();
                vmDashboard.TotalIngresos = await _dashboardService.TotalIngresosUltimaSemana();
                vmDashboard.TotalProductos = await _dashboardService.TotalProductos();
                vmDashboard.TotalCategorias = await _dashboardService.TotalCategorias();

                List<VMVentasSemana> listaVentaSemana = new List<VMVentasSemana>();
                List<VMProductosPorSemana> listaProductosSemana = new List<VMProductosPorSemana>();

                foreach (KeyValuePair<string, int> item in await _dashboardService.VentasUltimaSemana())
                {
                    listaVentaSemana.Add(new VMVentasSemana()
                    {
                        Fecha = item.Key,
                        Total = item.Value,
                    });
                }

                foreach (KeyValuePair<string, int> item in await _dashboardService.ProductosTopUltimaSemana())
                {
                    listaProductosSemana.Add(new VMProductosPorSemana()
                    {
                        Producto = item.Key,
                        Cantidad = item.Value,
                    });
                }


                vmDashboard.VentasSemanaList = listaVentaSemana;
                vmDashboard.ProductosTopUltimaSemana = listaProductosSemana;


                gResponse.Estado = true;
                gResponse.Object = vmDashboard;

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

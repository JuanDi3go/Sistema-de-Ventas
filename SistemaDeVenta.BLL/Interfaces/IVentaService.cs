using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaDeVenta.Entity;
using SistemaDeVenta.Entity.Entities;

namespace SistemaDeVenta.BLL.Interfaces
{
    public interface IVentaService
    {
        Task<List<Producto>> ObtenerProductos(string busqueda);

        Task<Venta> Registrar(Venta entidad);

        Task<List<Venta>> Historial(string numeroVenta, string FechaInicio, string fechaFin);
        Task<Venta> Detalle(string numeroVenta);
        Task<List<DetalleVenta>> Reporte(string FechaInicio, string fechaFin);
    }
}

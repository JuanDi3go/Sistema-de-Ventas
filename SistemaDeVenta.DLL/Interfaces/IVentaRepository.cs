using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaDeVenta.Entity;
using SistemaDeVenta.Entity.Entities;

namespace SistemaDeVenta.DLL.Interfaces
{
    public interface IVentaRepository:IGenericRepository<Venta>
    {
        Task<Venta> Registrar(Venta entidad);
        Task<List<DetalleVenta>> Reporte(DateTime fechaInicio, DateTime fechaFin);
    }
}

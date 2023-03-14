using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaDeVenta.BLL.Interfaces;
using SistemaDeVenta.DLL.Interfaces;
using SistemaDeVenta.Entity;
using Microsoft.EntityFrameworkCore;
using SistemaDeVenta.Entity.Entities;
using System.Globalization;

namespace SistemaDeVenta.BLL.Implementacion
{
    public class VentaService : IVentaService
    {
        private readonly IGenericRepository<Producto> _repositorioProducto;
        private readonly IVentaRepository _repositorioVenta;

        public VentaService(IVentaRepository repositorioVenta, IGenericRepository<Producto> repositorioProducto)
        {
            _repositorioVenta = repositorioVenta;
            _repositorioProducto = repositorioProducto;
        }
        public async Task<List<Producto>> ObtenerProductos(string busqueda)
        {
            IQueryable<Producto> query = await _repositorioProducto.Consultar(
                p => p.EsActivo == true && p.Stock > 0 && string.Concat(p.CodigoBarra, p.Marca, p.Descripcion).Contains(busqueda)
                );

            return query.Include(c => c.IdCategoriaNavigation).ToList();
        }
        public async Task<Venta> Registrar(Venta entidad)
        {
            try
            {
                return await _repositorioVenta.Registrar(entidad);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<Venta>> Historial(string numeroVenta, string FechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = await _repositorioVenta.Consultar();
            FechaInicio = FechaInicio is null ? "" : FechaInicio;
            fechaFin    = fechaFin is null ? "" : fechaFin;

            if (FechaInicio != "" && fechaFin != "")
            {
                DateTime fechaInincioD = DateTime.ParseExact(FechaInicio, "dd/MM/yyyy", new CultureInfo("es-CO"));
                DateTime fechaFinD = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-CO"));

                return query.Where(v => v.FechaRegistro.Value.Date >= fechaInincioD.Date &&
                v.FechaRegistro.Value.Date <= fechaFinD.Date).Include(tdv => tdv.IdTipoDocumentoVentaNavigation).Include(u => u.IdUsuarioNavigation)
                .Include(dv => dv.DetalleVenta).ToList();
            }
            else
            {
                return query.Where(v => v.NumeroVenta == numeroVenta).Include(tdv => tdv.IdTipoDocumentoVentaNavigation).Include(u => u.IdUsuarioNavigation)
     .Include(dv => dv.DetalleVenta).ToList();
            }
            
        }


        public async Task<Venta> Detalle(string numeroVenta)
        {
            IQueryable<Venta> query = await _repositorioVenta.Consultar(v => v.NumeroVenta == numeroVenta);

           return query.Include(tdv => tdv.IdTipoDocumentoVentaNavigation).Include(u => u.IdUsuarioNavigation)
               .Include(dv => dv.DetalleVenta).First();    
        }



        public async Task<List<DetalleVenta>> Reporte(string FechaInicio, string fechaFin)
        {
            DateTime fechaInincioD = DateTime.ParseExact(FechaInicio, "dd/MM/yyyy", new CultureInfo("es-CO"));
            DateTime fechaFinD = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-CO"));

            List<DetalleVenta> lista = await _repositorioVenta.Reporte(fechaInincioD, fechaFinD);

            return lista;
        }
    }
}

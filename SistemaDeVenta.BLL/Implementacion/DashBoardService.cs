using Microsoft.EntityFrameworkCore;
using SistemaDeVenta.BLL.Interfaces;
using SistemaDeVenta.DLL.Context;
using SistemaDeVenta.DLL.Interfaces;
using SistemaDeVenta.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeVenta.BLL.Implementacion
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IVentaRepository _repositorioVenta;
        private readonly IGenericRepository<DetalleVenta> _repositorioDetalle;
        private readonly IGenericRepository<Categoria> _repositorioCategoria;
        private readonly IGenericRepository<Producto> _repositorioProducto;
        private DateTime FechaInicio = DateTime.Now;


        public DashBoardService(IVentaRepository repositorioVenta, IGenericRepository<DetalleVenta> repositorioDetalle, IGenericRepository<Categoria> repositorioCategoria, IGenericRepository<Producto> repositorioProducto)
        {
            _repositorioVenta = repositorioVenta;
            _repositorioDetalle = repositorioDetalle;
            _repositorioCategoria = repositorioCategoria;
            _repositorioProducto = repositorioProducto;

            FechaInicio = FechaInicio.AddDays(-7);
        }

        public async Task<Dictionary<string, int>> ProductosTopUltimaSemana()
        {
            try
            {
                IQueryable<DetalleVenta> query = await _repositorioDetalle.Consultar();

                Dictionary<string, int> result = query.Include(v => v.IdVentaNavigation).Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= FechaInicio.Date)
                    .GroupBy(dv => dv.DescripcionProducto).OrderByDescending(g => g.Count()).Select(dv => new {
                    producto = dv.Key,
                    total = dv.Count()
                }).Take(4).ToDictionary(keySelector: r => r.producto, elementSelector: r => r.total);


                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<int> TotalCategorias()
        {
            try
            {
                IQueryable<Categoria> query = await _repositorioCategoria.Consultar();
                int total = query.Count();
                return total;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<string> TotalIngresosUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _repositorioVenta.Consultar(v => v.FechaRegistro.Value.Date >= FechaInicio.Date);

                decimal resultado = query.Select(v => v.Total).Sum(v => v.Value);


                return Convert.ToString(resultado, new CultureInfo("es-CO"));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<int> TotalProductos()
        {
            try
            {
                IQueryable<Producto> query = await _repositorioProducto.Consultar();
                int total = query.Count();
                return total;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<int> TotalVentasUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _repositorioVenta.Consultar(v => v.FechaRegistro.Value.Date >= FechaInicio.Date);
                int total = query.Count();
                return total;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Dictionary<string, int>> VentasUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _repositorioVenta.Consultar(v => v.FechaRegistro.Value.Date >= FechaInicio.Date);
                
                Dictionary<string,int> result = query.GroupBy(v => v.FechaRegistro.Value.Date).OrderByDescending(g => g.Key).Select(dv => new {
                    fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() }).ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total);


                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

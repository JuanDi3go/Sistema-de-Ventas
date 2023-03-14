using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaDeVenta.DLL.Context;
using SistemaDeVenta.DLL.Interfaces;
using SistemaDeVenta.Entity;
using SistemaDeVenta.Entity.Entities;

namespace SistemaDeVenta.DLL.Implementacion
{
    public class VentaRepository :GenericRepository<Venta>, IVentaRepository
    {
        private readonly DbventaContext _dbcontext;

        public VentaRepository(DbventaContext dbcontext):base(dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<Venta> Registrar(Venta entidad)
        {
            Venta ventaGenerada = new Venta();

            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    foreach (DetalleVenta detalleVenta in entidad.DetalleVenta)
                    {
                        Producto producto_encontrado = _dbcontext.Productos.Where(p => p.IdProducto == detalleVenta.IdProducto).First();
                        producto_encontrado.Stock = producto_encontrado.Stock - detalleVenta.Cantidad;
                        _dbcontext.Productos.Update(producto_encontrado);
                    }
                    await _dbcontext.SaveChangesAsync();

                    NumeroCorrelativo correlativo = _dbcontext.NumeroCorrelativos.Where(n => n.Gestion == "venta").First();

                    correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                    correlativo.FechaActualizacion = DateTime.Now;
                    _dbcontext.NumeroCorrelativos.Update(correlativo);
                    await _dbcontext.SaveChangesAsync();


                    string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value));
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString();
                    numeroVenta.Substring(numeroVenta.Length - correlativo.CantidadDigitos.Value, correlativo.CantidadDigitos.Value);

                    entidad.NumeroVenta = numeroVenta;
                    await _dbcontext.Venta.AddAsync(entidad);
                    await _dbcontext.SaveChangesAsync();

                    ventaGenerada = entidad;

                    transaction.Commit(); // Lo que hace es que espera a que todas las operaciones dentro
                                        // del transaction se realizen si ninguna falla entoces las ejecuta para que se realicen estos procedimientos en la base dee datos
                    
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

            return ventaGenerada;
        }

        public async Task<List<DetalleVenta>> Reporte(DateTime fechaInicio, DateTime fechaFin)
        {
            List<DetalleVenta>listaResumen = await _dbcontext.DetalleVenta.Include(v => v.IdVentaNavigation)
                .ThenInclude(u => u.IdUsuarioNavigation).Include(v => v.IdVentaNavigation).ThenInclude(tdv => tdv.IdTipoDocumentoVentaNavigation)
                .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= fechaInicio.Date && dv.IdVentaNavigation.FechaRegistro.Value.Date 
                <= fechaFin.Date).ToListAsync();

            return listaResumen;
        }
    }
}

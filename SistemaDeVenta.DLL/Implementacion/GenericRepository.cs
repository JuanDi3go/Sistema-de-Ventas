using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaDeVenta.DLL.Context;
using SistemaDeVenta.DLL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace SistemaDeVenta.DLL.Implementacion
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbventaContext _dbContext;
        public GenericRepository(DbventaContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<T> Obtener(Expression<Func<T, bool>> filtro)
        {
            try
            {
                T entidad = await _dbContext.Set<T>().FirstOrDefaultAsync(filtro);
                return entidad;
            }
            catch
            {

                throw;
            }
        }
        public async Task<IQueryable<T>> Consultar(Expression<Func<T, bool>> filtro = null)
        {
            try
            {
                IQueryable<T> queryEntidad = filtro == null ? _dbContext.Set<T>() : _dbContext.Set<T>().Where(filtro);
                return queryEntidad;
            }
            catch
            {

                throw;
            }
        }

        public async Task<T> Crear(T entidad)
        {
            try
            {
              await _dbContext.Set<T>().AddAsync(entidad);
                await _dbContext.SaveChangesAsync();
                return entidad;
            }
            catch
            {

                throw;
            }
        }

        public async Task<bool> Editar(T entidad)
        {
            try
            {
                _dbContext.Set<T>().Update(entidad);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {

                throw;
            }
        }

        public async Task<bool> Eliminar(T entidad)
        {
            try
            {
                _dbContext.Set<T>().Remove(entidad);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {

                throw;
            }
        }
    }
}

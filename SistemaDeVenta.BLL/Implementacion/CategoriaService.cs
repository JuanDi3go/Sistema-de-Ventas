using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaDeVenta.BLL.Interfaces;
using SistemaDeVenta.DLL.Context;
using SistemaDeVenta.DLL.Interfaces;
using SistemaDeVenta.Entity;
using SistemaDeVenta.Entity.Entities;

namespace SistemaDeVenta.BLL.Implementacion
{
    public class CategoriaService : ICategoriaService
    {
        private readonly IGenericRepository<Categoria> _repositorio;

        public CategoriaService(IGenericRepository<Categoria> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<Categoria> Crear(Categoria entidad)
        {
            try
            {
                Categoria categoria_creada = await _repositorio.Crear(entidad);
                if (categoria_creada.IdCategoria == 0)
                    throw new TaskCanceledException("No se pudo crear la categoria");

                return categoria_creada;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Categoria> Editar(Categoria entidad)
        {
            try
            {
                Categoria categoriaEncontrada = await _repositorio.Obtener(c => c.IdCategoria == entidad.IdCategoria);
                categoriaEncontrada.Descripcion = entidad.Descripcion;
                categoriaEncontrada.EsActivo = entidad.EsActivo;
                bool respuesta = await _repositorio.Editar(categoriaEncontrada);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo modificar la categoria");

                return categoriaEncontrada;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public  async Task<bool> Eliminar(int idCategoria)
        {
            try
            {
                Categoria categoriaEncontrada = await _repositorio.Obtener(c => c.IdCategoria == idCategoria);

                if (categoriaEncontrada == null)
                    throw new TaskCanceledException("La categoria no existe");

                bool respuesta = await _repositorio.Eliminar(categoriaEncontrada);

                return respuesta;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Categoria>> Lista()
        {
            IQueryable<Categoria> query = await _repositorio.Consultar();

            return query.ToList<Categoria>();
        }
    }
}

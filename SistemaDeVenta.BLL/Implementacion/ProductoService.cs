using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SistemaDeVenta.BLL.Interfaces;
using SistemaDeVenta.DLL.Interfaces;
using SistemaDeVenta.Entity;
using SistemaDeVenta.Entity.Entities;

namespace SistemaDeVenta.BLL.Implementacion
{
    public class ProductoService:IProductoService
    {
        private readonly IGenericRepository<Producto> _Repository;
        private readonly IFirebaseService _firebaseservice;


        public ProductoService(IGenericRepository<Producto> repository, IFirebaseService firebaseservice, IUtilidadesService utilidadesService)
        {
            _Repository = repository;
            _firebaseservice = firebaseservice;

        }

        public async Task<List<Producto>> Lista()
        {
            IQueryable<Producto> query = await _Repository.Consultar();
            return query.Include(c => c.IdCategoriaNavigation).ToList();
        }
        public async Task<Producto> Crear(Producto entidad, Stream imagen = null, string nombreImagen = "")
        {
            Producto productoexiste = await _Repository.Obtener(p => p.CodigoBarra == entidad.CodigoBarra);

            if (productoexiste != null)
                throw new TaskCanceledException("EL Codigo de barra ya existe");

            try
            {
                entidad.NombreImagen = nombreImagen;
                if (imagen !=null)
                {
                    string urlImagen = await _firebaseservice.SubirStorage(imagen, "carpeta_producto", nombreImagen);
                    entidad.UrlImagen= urlImagen;

                }
                Producto productoCreado = await _Repository.Crear(entidad);

                if(productoCreado.IdProducto == 0)
                    throw new TaskCanceledException("No se pudo crear el producto");

                IQueryable<Producto> query = await _Repository.Consultar(p => p.IdProducto == productoCreado.IdProducto);


                productoCreado = query.Include(c => c.IdCategoriaNavigation).First(); 


                return productoCreado;
            }
            catch (Exception)
            {

                throw;
            }

            
        }

        public async Task<Producto> Editar(Producto entidad, Stream imagen = null, string nombreImagen = "")
        {
            Producto producto_existe = await _Repository.Obtener(p => p.CodigoBarra == entidad.CodigoBarra && p.IdProducto != entidad.IdProducto);

            if(producto_existe != null)
                throw new TaskCanceledException("el codigo de barra ya existe");

            try
            {
                IQueryable<Producto> queryProducto = await _Repository.Consultar(p => p.IdProducto == entidad.IdProducto);

                Producto productoParaEditar = queryProducto.First();

                productoParaEditar.CodigoBarra = entidad.CodigoBarra;
                productoParaEditar.Marca = entidad.Marca;
                productoParaEditar.Descripcion =entidad.Descripcion;
                productoParaEditar.IdCategoria = entidad.IdCategoria;
                productoParaEditar.Stock = entidad.Stock;
                productoParaEditar.Precio = entidad.Precio;
                productoParaEditar.EsActivo = entidad.EsActivo;

                if(productoParaEditar.NombreImagen == "")
                {
                    productoParaEditar.NombreImagen = nombreImagen;
                }

                if(imagen != null)
                {
                    string urlImagen = await _firebaseservice.SubirStorage(imagen,"carpeta_producto",productoParaEditar.NombreImagen);
                    productoParaEditar.UrlImagen = urlImagen;
                }

                bool respuesta = await _Repository.Editar(productoParaEditar);

                if(!respuesta)
                    throw new TaskCanceledException("No se pudo modificar el Producto");


                Producto producto_editado = queryProducto.Include(c => c.IdCategoriaNavigation).First();


                return producto_editado;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Eliminar(int idProducto)
        {
            try
            {
                Producto productoEncontrado = await _Repository.Obtener(p => p.IdProducto == idProducto);
                if (productoEncontrado == null)
                    throw new TaskCanceledException("El producto no existe");

                string nombreImagen = productoEncontrado.NombreImagen;

                bool respuesta = await _Repository.Eliminar(productoEncontrado);

                if (respuesta)
                    await _firebaseservice.EliminarStorage("carpeta_producto", nombreImagen);

                return respuesta;

            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}

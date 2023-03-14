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
    public class NegocioService : INegocioService
    {
        private readonly IGenericRepository<Negocio> _repositorio;
        private readonly IFirebaseService _firebase;

        public NegocioService(IFirebaseService firebase, IGenericRepository<Negocio> repositorio)
        {
            _firebase = firebase;
            _repositorio = repositorio;
        }

        public async Task<Negocio> GuardarCambios(Negocio entidad, Stream logo = null, string NombreLogo = "")
        {
            try
            {
                Negocio negocioEncontrado = await _repositorio.Obtener(n => n.IdNegocio == 1);

                negocioEncontrado.NumeroDocumento = entidad.NumeroDocumento;
                negocioEncontrado.Nombre = entidad.Nombre;
                negocioEncontrado.Correo = entidad.Correo;
                negocioEncontrado.Direccion = entidad.Direccion;
                negocioEncontrado.Telefono = entidad.Telefono;
                negocioEncontrado.PorcentajeImpuesto = entidad.PorcentajeImpuesto;
                negocioEncontrado.SimboloMoneda = entidad.SimboloMoneda;

                negocioEncontrado.NombreLogo = negocioEncontrado.NombreLogo ==""? NombreLogo : entidad.NombreLogo;
                if (logo != null)
                {
                    string urlFoto = await _firebase.SubirStorage(logo, "carpeta_logo", negocioEncontrado.NombreLogo);
                    negocioEncontrado.UrlLogo = urlFoto;
                }

                await _repositorio.Editar(negocioEncontrado);
                return negocioEncontrado;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Negocio> Obtener()
        {
            try
            {
                Negocio negocioEncontrado = await _repositorio.Obtener(n => n.IdNegocio == 1);
                return negocioEncontrado;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

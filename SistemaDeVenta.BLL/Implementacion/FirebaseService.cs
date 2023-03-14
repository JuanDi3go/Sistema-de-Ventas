using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaDeVenta.BLL.Interfaces;
using Firebase.Auth;
using Firebase.Storage;
using SistemaDeVenta.Entity;
using SistemaDeVenta.DLL.Interfaces;
using SistemaDeVenta.Entity.Entities;


namespace SistemaDeVenta.BLL.Implementacion
{
    public class FirebaseService : IFirebaseService
    {
        private readonly IGenericRepository<Configuracion> _repositorio;

        public FirebaseService(IGenericRepository<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<string> SubirStorage(Stream streamArchivo, string carpetaDestino, string nombreDelArchivo)
        {
            string urlImagen = "";
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));

                Dictionary<string, string> config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(config["api_key"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(config["email"], config["clave"]);

                var cancellation =  new CancellationTokenSource();
                var task = new FirebaseStorage(
                    config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    }
                    ).Child(config[carpetaDestino])//enviamos el archivo a firebase
                    .Child(nombreDelArchivo)
                    .PutAsync(streamArchivo, cancellation.Token);

                urlImagen = await task;/// la url que nos devulve firebase
            }
            catch
            {
                urlImagen = "";

            }
            return urlImagen;
        }
        public async Task<bool> EliminarStorage(string carpetaDestino, string nombreDelArchivo)
        {
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));

                Dictionary<string, string> config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(config["api_key"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(config["email"], config["clave"]);

                var cancellation = new CancellationTokenSource();
                var task = new FirebaseStorage(
                    config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    }
                    ).Child(config[carpetaDestino])//enviamos el archivo a firebase
                    .Child(nombreDelArchivo)
                    .DeleteAsync();

                await task;/// la url que nos devulve firebase

                return true;
            }
            catch
            {
                return false;


            }
        }
    }
}

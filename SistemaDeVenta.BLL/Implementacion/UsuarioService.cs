using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net;
using SistemaDeVenta.BLL.Interfaces;
using SistemaDeVenta.DLL.Interfaces;
using SistemaDeVenta.Entity;
using SistemaDeVenta.Entity.Entities;
using System.Runtime.CompilerServices;

namespace SistemaDeVenta.BLL.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _repository;
        private readonly IFirebaseService _firebaseService;
        private readonly IUtilidadesService _utilidadesService;
        private readonly ICorreoService _correoService;
        public UsuarioService(IGenericRepository<Usuario> repository, IFirebaseService firebaseService, IUtilidadesService utilidadesService, ICorreoService correoService)
        {
            _repository = repository;
            _firebaseService = firebaseService;
            _utilidadesService = utilidadesService;
            _correoService = correoService;
        }

        public async Task<List<Usuario>> Lista()
        {
            IQueryable<Usuario> query = await _repository.Consultar();
            return query.Include(r => r.IdRolNavigation).ToList();
        }
        public async Task<Usuario> Crear(Usuario entidad, Stream foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "")
        {
            Usuario usuario_existe = await _repository.Obtener(u => u.Correo == entidad.Correo);

            if (usuario_existe != null)
                throw new TaskCanceledException("El correo ya Existe");

            try
            {
                string clave_generada = _utilidadesService.GenerarClave();
                entidad.Clave = _utilidadesService.ConvertirSha256(clave_generada);
                entidad.NombreFoto = NombreFoto;

                if (foto != null)
                {
                    string urlfoto = await _firebaseService.SubirStorage(foto, "carpeta_usuario", NombreFoto);
                    entidad.UrlFoto= urlfoto;
                   
                }
                Usuario usuario_creado = await _repository.Crear(entidad);

                if (usuario_creado.IdUsuario == 0)
                    throw new TaskCanceledException("No se pudo crear el usuario");

                if (UrlPlantillaCorreo != "")
                {
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[correo]", usuario_creado.Correo)
                        .Replace("[clave]", clave_generada);

                    string htmlCorreo = "";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader streamReader = null;

                            if (response.CharacterSet == null)
                                streamReader = new StreamReader(dataStream);
                            else
                                streamReader= new StreamReader(dataStream,Encoding.GetEncoding(response.CharacterSet));

                        htmlCorreo= streamReader.ReadToEnd();
                            response.Close();
                            streamReader.Close();
                        }
                    }
                    if (htmlCorreo != "")
                        await _correoService.EnviarCorreo(usuario_creado.Correo, "Cuenta Creada", htmlCorreo);
                }
                IQueryable<Usuario> query = await _repository.Consultar(u => u.IdUsuario == usuario_creado.IdUsuario);
                usuario_creado = query.Include(r => r.IdRolNavigation).First();
                return usuario_creado;
            }
            catch(Exception ex)
            {

                throw;
            }

            
        }
        public async Task<Usuario> Editar(Usuario entidad, Stream foto = null, string NombreFoto = "")
        {
            Usuario usuario_existe = await _repository.Obtener(u => u.Correo == entidad.Correo && u.IdUsuario != entidad.IdUsuario);

            if (usuario_existe != null)
                throw new TaskCanceledException("El correo ya Existe");
            try
            {
                IQueryable<Usuario> queryUsuario = await _repository.Consultar(u => u.IdUsuario == entidad.IdUsuario);

                Usuario usuario_editar = queryUsuario.First();
                usuario_editar.Nombre = entidad.Nombre;
                usuario_editar.Correo= entidad.Correo;
                usuario_editar.Telefono= entidad.Telefono;
                usuario_editar.IdRol = entidad.IdRol;
                usuario_editar.EsActivo = entidad.EsActivo;
                if (usuario_editar.NombreFoto == "")
                    usuario_editar.Nombre = NombreFoto;

                if(foto != null)
                {
                    string urlFoto = await _firebaseService.SubirStorage(foto, "carpeta_usuario", usuario_editar.Nombre);
                    usuario_editar.UrlFoto = urlFoto;
                }
                bool respuesta = await _repository.Editar(usuario_editar);

                if (!respuesta)
                    throw new TaskCanceledException("No Se pudo modificar el usuario");

                Usuario usuario_editado = queryUsuario.Include(r => r.IdRolNavigation).First();

                return usuario_editado;
                
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Eliminar(int idUsuario)
        {
            try
            {
                Usuario usuario_encontrado = await _repository.Obtener(u => u.IdUsuario == idUsuario);

                if (usuario_encontrado == null)
                    throw new TaskCanceledException("El usuairo no existe");

                string nombrefoto = usuario_encontrado.NombreFoto;
                bool respuesta = await _repository.Eliminar(usuario_encontrado);

                if (respuesta)
                    await _firebaseService.EliminarStorage("carpeta_usuario", nombrefoto);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {
            string clave_encriptada = _utilidadesService.ConvertirSha256(clave);

            Usuario usuarioEncontrado = await _repository.Obtener(u => u.Correo.Equals(correo) && u.Clave.Equals(clave_encriptada));

            return usuarioEncontrado;
        }

        public async Task<Usuario> ObtenerPorId(int idUsuario)
        {
            IQueryable<Usuario> query = await _repository.Consultar(u => u.IdUsuario == idUsuario);

            Usuario resultado = query.Include(r => r.IdRolNavigation).FirstOrDefault();
            return resultado;
        }
        public async Task<bool> GuardarPerfil(Usuario entidad)
        {
            try
            {
                Usuario usuarioEncontrado = await _repository.Obtener(u => u.IdUsuario == entidad.IdUsuario);
                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El usuario no existe");

                usuarioEncontrado.Correo = entidad.Correo;
                usuarioEncontrado.Telefono = entidad.Telefono;

                bool respuesta = await _repository.Editar(usuarioEncontrado);
                return respuesta;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> CambiarClave(int idUsuario, string claveActual, string claveNueva)
        {
            try
            {
                Usuario usuarioEncontrado = await _repository.Obtener(u => u.IdUsuario == idUsuario);
                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El usuario no existe");

                if(usuarioEncontrado.Clave != _utilidadesService.ConvertirSha256(claveActual))
                    throw new TaskCanceledException("La contraseña actual no coincide");

                usuarioEncontrado.Clave = _utilidadesService.ConvertirSha256(claveNueva);

                bool respuesta = await _repository.Editar(usuarioEncontrado);

                return respuesta;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> RestablecerClave(string Correo, string UrlPlantillaCorreo)
        {
            try
            {
                Usuario usuarioEncontrado = await _repository.Obtener(u => u.Correo == Correo);
                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("No encontramos un correo asociado al correo");

                string claveGenerado = _utilidadesService.GenerarClave();
                usuarioEncontrado.Clave = _utilidadesService.ConvertirSha256(claveGenerado);

                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[clave]", claveGenerado);

                    string htmlCorreo = "";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader streamReader = null;

                            if (response.CharacterSet == null)
                                streamReader = new StreamReader(dataStream);
                            else
                                streamReader = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));

                            htmlCorreo = streamReader.ReadToEnd();
                            response.Close();
                            streamReader.Close();
                        }
                    }

                bool correoEnvido = false;
                    if (htmlCorreo != "")
                    correoEnvido=  await _correoService.EnviarCorreo(Correo, "Contraseña Restablecida", htmlCorreo);

                if (!correoEnvido)
                    throw new TaskCanceledException("Tenemos problemas. Por favor intentalo de nuevo mas tarde");

                bool respuesta = await _repository.Editar(usuarioEncontrado);

                return respuesta;
                }
            catch
            {
                throw;
            }
        }
    }
}

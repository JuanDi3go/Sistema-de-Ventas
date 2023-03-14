using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeVenta.BLL.Interfaces
{
    public interface IFirebaseService
    {
        Task<string> SubirStorage(Stream streamArchivo, string carpetaDestino, string nombreDelArchivo);
        Task<bool> EliminarStorage(string carpetaDestino, string nombreDelArchivo);
    }
}

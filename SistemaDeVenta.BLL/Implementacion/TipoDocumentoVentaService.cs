using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaDeVenta.BLL.Interfaces;
using SistemaDeVenta.DLL.Interfaces;
using SistemaDeVenta.Entity;
using SistemaDeVenta.Entity.Entities;

namespace SistemaDeVenta.BLL.Implementacion
{
    public class TipoDocumentoVentaService : ITipoDocumentoVentaService
    {
        private readonly IGenericRepository<TipoDocumentoVenta> _genericRepository;

        public TipoDocumentoVentaService(IGenericRepository<TipoDocumentoVenta> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<List<TipoDocumentoVenta>> Lista()
        {
            IQueryable<TipoDocumentoVenta> query = await _genericRepository.Consultar();
            return query.ToList();
        }
    }
}

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
    public class RolService : IRolService
    {
        private readonly IGenericRepository<Rol> _repository;

        public RolService(IGenericRepository<Rol> repository)
        {
            _repository = repository;
        }

        public async Task<List<Rol>> Lista()
        {
            IQueryable<Rol> query = await _repository.Consultar();

            return query.ToList();
        }
    }
}

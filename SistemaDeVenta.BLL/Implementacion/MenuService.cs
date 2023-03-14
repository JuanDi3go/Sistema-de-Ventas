using SistemaDeVenta.BLL.Interfaces;
using SistemaDeVenta.DLL.Implementacion;
using SistemaDeVenta.DLL.Interfaces;
using SistemaDeVenta.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeVenta.BLL.Implementacion
{
    public  class MenuService : IMenuService
    {
        private readonly IGenericRepository<Menu> _repositorioMenu; 
        private readonly IGenericRepository<Rol> _repositorioRol; 
        private readonly IGenericRepository<RolMenu> _repositorioRolMenu; 
        private readonly IGenericRepository<Usuario> _repositorioUsuario;

        public MenuService(IGenericRepository<Usuario> repositorioUsuario, IGenericRepository<RolMenu> repositorioRolMenu, IGenericRepository<Rol> repositorioRol, IGenericRepository<Menu> repositorioMenu)
        {
            _repositorioUsuario = repositorioUsuario;
            _repositorioRolMenu = repositorioRolMenu;
            _repositorioRol = repositorioRol;
            _repositorioMenu = repositorioMenu;
        }

        public async Task<List<Menu>> ObtenerMenus(int idUsuario)
        {
            IQueryable<Usuario> tbUsuario = await _repositorioUsuario.Consultar(u => u.IdUsuario == idUsuario);
            IQueryable<RolMenu> tbRolmenu = await _repositorioRolMenu.Consultar();
            IQueryable<Menu> tbMenu = await _repositorioMenu.Consultar();


            IQueryable<Menu> menuPadre = (from u in tbUsuario
                                          join rm in tbRolmenu on u.IdRol equals rm.IdRol
                                          join m in tbMenu on rm.IdMenu equals m.IdMenu
                                          join mpadre in tbMenu on m.IdMenuPadre equals mpadre.IdMenu
                                          select mpadre).Distinct().AsQueryable();

            IQueryable<Menu> menuHijos = (from u in tbUsuario
                                          join rm in tbRolmenu on u.IdRol equals rm.IdRol
                                          join m in tbMenu on rm.IdMenu equals m.IdMenu
                                          where m.IdMenu != m.IdMenuPadre
                                          select m).Distinct().AsQueryable();


            List<Menu> listaMenu = (from mpadre in menuPadre
                                    select new Menu()
                                    {
                                        Descripcion = mpadre.Descripcion,
                                        Icono = mpadre.Icono,
                                        Controlador = mpadre.Controlador,
                                        PaginaAccion = mpadre.PaginaAccion,
                                        InverseIdMenuPadreNavigation = (from mhijo in menuHijos where mhijo.IdMenuPadre == mpadre.IdMenu select mhijo).ToList()
                                    }).ToList();

            return listaMenu;

        }
    }
}

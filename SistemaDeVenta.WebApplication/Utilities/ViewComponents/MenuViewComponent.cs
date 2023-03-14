using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SistemaDeVenta.BLL.Interfaces;
using SistemaDeVenta.WebApplication.Models.ViewModels;
using System.Security.Claims;

namespace SistemaDeVenta.WebApplication.Utilities.ViewComponents
{
    public class MenuViewComponent:ViewComponent
    {
        private readonly IMenuService _menuService;
        private readonly IMapper _mapper;

        public MenuViewComponent(IMapper mapper, IMenuService menuService)
        {
            _mapper = mapper;
            _menuService = menuService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            List<VMMenu> listaMenus = new List<VMMenu>();

            if (claimUser.Identity.IsAuthenticated)
            {
                string idUsuario = claimUser.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

                listaMenus = _mapper.Map<List<VMMenu>>(await _menuService.ObtenerMenus(int.Parse(idUsuario)));

            }
            return View(listaMenus);
        }
    }
}

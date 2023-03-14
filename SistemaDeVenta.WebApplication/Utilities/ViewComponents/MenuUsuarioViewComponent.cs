﻿using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SistemaDeVenta.WebApplication.Utilities.ViewComponents
{
    public class MenuUsuarioViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            string nombreUsuario = "";
            string urlFotoUsuario = "";

            if (claimUser.Identity.IsAuthenticated)
            {
                nombreUsuario = claimUser.Claims.Where(c => c.Type == ClaimTypes.Name)
                    .Select(c => c.Value).SingleOrDefault();

                urlFotoUsuario = ((ClaimsIdentity)claimUser.Identity).FindFirst("UrlFoto").Value;


                ViewData["NombreUsuario"] = nombreUsuario;
                ViewData["UrlFotoUsuario"] = urlFotoUsuario;
                
            }
            return View();
        }
    }
}

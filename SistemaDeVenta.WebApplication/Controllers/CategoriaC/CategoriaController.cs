using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SistemaDeVenta.WebApplication.Models.ViewModels;
using SistemaDeVenta.WebApplication.Utilities.Response;
using SistemaDeVenta.BLL.Interfaces;
using SistemaDeVenta.Entity;
using SistemaDeVenta.Entity.Entities;
using Microsoft.AspNetCore.Authorization;

namespace SistemaDeVenta.WebApplication.Controllers.CategoriaC
{
    [Authorize]
    public class CategoriaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService, IMapper mapper)
        {
            _categoriaService = categoriaService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]

        public async Task<IActionResult> Lista()
        {
            List<VMCategoria> vmCategoriaLista = _mapper.Map<List<VMCategoria>>(await _categoriaService.Lista());
            return StatusCode(StatusCodes.Status200OK, new
            {
                data = vmCategoriaLista
            });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody]VMCategoria modelo)
        {
            GenericResponse<VMCategoria> gResponse = new GenericResponse<VMCategoria>();

            try
            {
                Categoria categoria_creada = await _categoriaService.Crear(_mapper.Map<Categoria>(modelo));
                modelo = _mapper.Map<VMCategoria>(categoria_creada);

                gResponse.Estado = true;
                gResponse.Object = modelo;
            }
            catch (Exception ex)
            {
                gResponse.Estado = true;
                gResponse.Message = ex.Message;

            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] VMCategoria modelo)
        {
            GenericResponse<VMCategoria> gResponse = new GenericResponse<VMCategoria>();

            try
            {
                Categoria categoria_Editada = await _categoriaService.Editar(_mapper.Map<Categoria>(modelo));
                modelo = _mapper.Map<VMCategoria>(categoria_Editada);

                gResponse.Estado = true;
                gResponse.Object = modelo;
            }
            catch (Exception ex)
            {
                gResponse.Estado = true;
                gResponse.Message = ex.Message;

            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]

        public async Task<IActionResult> Eliminar(int idCategoria)
        {
            GenericResponse<VMCategoria> gResponse = new GenericResponse<VMCategoria>();

            try
            {
                gResponse.Estado = await _categoriaService.Eliminar(idCategoria);
                
            }
            catch (Exception ex)
            {
                gResponse.Estado = true;
                gResponse.Message = ex.Message;
                
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using SistemaDeVenta.WebApplication.Models.ViewModels;
using SistemaDeVenta.WebApplication.Utilities.Response;
using SistemaDeVenta.BLL.Interfaces;
using SistemaDeVenta.Entity;
using SistemaDeVenta.Entity.Entities;
using Microsoft.AspNetCore.Authorization;

namespace SistemaDeVenta.WebApplication.Controllers.Productoc
{
    [Authorize]
    public class ProductoController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IProductoService _productoServicio;

        public ProductoController(ICategoriaService categoriaServicio, IProductoService productoServicio, IMapper mapper)
        {

            _productoServicio = productoServicio;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async  Task<IActionResult> Lista()
        {
            List<VMProducto> vmProductoLista = _mapper.Map<List<VMProducto>>(await _productoServicio.Lista());

            return StatusCode(StatusCodes.Status200OK ,new
            {
              data=  vmProductoLista
            });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm]IFormFile imagen, [FromForm]string modelo)
        {
            GenericResponse<VMProducto> gResponse = new GenericResponse<VMProducto>();

            try
            {
                VMProducto vmProducto = JsonConvert.DeserializeObject<VMProducto>(modelo);

                string nombreImagen = "";
                Stream imagenStream = null;

                if (imagen != null) {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(imagen.FileName);
                    nombreImagen = string.Concat(nombreEnCodigo, extension);
                    imagenStream = imagen.OpenReadStream();
                }

                Producto productoCreado = await _productoServicio.Crear(_mapper.Map<Producto>(vmProducto), imagenStream, nombreImagen);

                vmProducto = _mapper.Map<VMProducto>(productoCreado);

                gResponse.Estado= true;
                gResponse.Object = vmProducto;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Message= ex.Message;
                
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile imagen, [FromForm] string modelo)
        {
            GenericResponse<VMProducto> gResponse = new GenericResponse<VMProducto>();

            try
            {
                VMProducto vmProducto = JsonConvert.DeserializeObject<VMProducto>(modelo);

                string nombreImagen = "";
                Stream imagenStream = null;

                if (imagen != null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(imagen.FileName);
                    nombreImagen = string.Concat(nombreEnCodigo, extension);
                    imagenStream = imagen.OpenReadStream();
                }

                Producto productoEditado = await _productoServicio.Editar(_mapper.Map<Producto>(vmProducto), imagenStream,nombreImagen );

                vmProducto = _mapper.Map<VMProducto>(productoEditado);

                gResponse.Estado = true;
                gResponse.Object = vmProducto;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Message = ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]

        public async Task<IActionResult> Eliminar(int idProducto)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _productoServicio.Eliminar(idProducto);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Message = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}

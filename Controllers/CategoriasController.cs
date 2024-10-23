using ApiPeliculas.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApiPeliculas.Models.Dtos;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using ApiPeliculas.Models;
using Microsoft.AspNetCore.Authorization;

namespace ApiPeliculas.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
       private readonly ICategoriaRepository _ctRepo;
       private readonly IMapper _mapper;

        public CategoriasController(ICategoriaRepository ctRepo, IMapper mapper)
        {
            _ctRepo=ctRepo;
            _mapper=mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore =true)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategorias() 
        {
        var listaCategorias = _ctRepo.GetCategorias();
        var listaCategoriasDto = new List <ObtenerCategoriaDto> ();

        foreach (var categoria in listaCategorias) 
            {
                listaCategoriasDto.Add(_mapper.Map<ObtenerCategoriaDto>(categoria));
            
            }
        return Ok(listaCategoriasDto);
        
        }
        [AllowAnonymous]
        [HttpGet("{categoriaId:int}", Name = "GetCategoria")]
        [ResponseCache(CacheProfileName = "PerfilVeinteSeg")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCategoria(int categoriaId)
        {
            var itemCategoria = _ctRepo.GetCategoria(categoriaId);

            if (itemCategoria == null)
            {
                return NotFound();
            }

            var itemCategoriaDto = _mapper.Map<ObtenerCategoriaDto>(itemCategoria);
            return Ok(itemCategoriaDto);

        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CrearCategoriaDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]


        public IActionResult CrearCategoria([FromBody] CrearCategoriaDto crearCategoriaDto) 
        {

            if (!ModelState.IsValid) 
            {
            return BadRequest(ModelState);
            }

            if (crearCategoriaDto == null) 
            { 
                return BadRequest(ModelState);
            }

            if (_ctRepo.ExisteCategoria(crearCategoriaDto.Nombre)) 
            {
                ModelState.AddModelError("","La categoria ya existe");
                return StatusCode(404, ModelState);
            
            }

            var categoria = _mapper.Map<Categoria>(crearCategoriaDto);

            if (!_ctRepo.CrearCategoria(categoria) )
            {
                ModelState.AddModelError("", $"Ha ocurrido un error guardando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);

            }



            return CreatedAtRoute("GetCategoria", new { categoriaId=categoria.Id},categoria);
        
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("{categoriaId:int}", Name = "ActualizarPatchCategoria")]
        [ProducesResponseType(201, Type = typeof(ObtenerCategoriaDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
       
        public IActionResult ActualizarPatchCategoria(int categoriaId, [FromBody] ObtenerCategoriaDto ObtenerCategoriaDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (ObtenerCategoriaDto == null || categoriaId != ObtenerCategoriaDto.Id)
            {
                return BadRequest(ModelState);
            }

        

            var categoria = _mapper.Map<Categoria>(ObtenerCategoriaDto);

            if (!_ctRepo.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Ha ocurrido un error actualizando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);

            }



            return NoContent(); 

        }


        [Authorize(Roles = "admin")]
        [HttpDelete("{categoriaId:int}", Name = "BorrarCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult BorrarCategoria(int categoriaId)
        {

            if (!_ctRepo.ExisteCategoria(categoriaId))
            {
                return NotFound();
            }

            var categoria = _ctRepo.GetCategoria(categoriaId);

            if (!_ctRepo.BorrarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Ha ocurrido un error al borrar el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);

            }


            return NoContent();

        }








    }
}

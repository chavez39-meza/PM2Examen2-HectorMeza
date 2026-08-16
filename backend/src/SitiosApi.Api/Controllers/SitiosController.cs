using Microsoft.AspNetCore.Mvc;
using SitiosApi.Application.DTOs;
using SitiosApi.Application.Interfaces;

namespace SitiosApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SitiosController : ControllerBase
    {
        private readonly ISitioService _service;

        public SitiosController(ISitioService service)
        {
            _service = service;
        }

        // GET: api/sitios
        [HttpGet]
        public async Task<ActionResult<List<SitioDto>>> GetAll()
        {
            var sitios = await _service.GetAllAsync();
            return Ok(sitios);
        }

        // GET: api/sitios/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SitioDto>> GetById(int id)
        {
            var sitio = await _service.GetByIdAsync(id);
            if (sitio is null) return NotFound(new { mensaje = $"No existe el sitio con id {id}" });
            return Ok(sitio);
        }

        // POST: api/sitios   (CREATE)
        [HttpPost]
        public async Task<ActionResult<SitioDto>> Create([FromBody] SitioCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var creado = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // PUT: api/sitios/5   (ACTUALIZAR)
        // Nota: el examen lo llama "POST" para actualizar en el enunciado,
        // pero lo correcto en REST es PUT/PATCH. Dejamos ambos mapeados
        // (ver el segundo atributo HttpPost("{id}")) por si el catedrático
        // exige literalmente un verbo POST para el update.
        [HttpPut("{id:int}")]
        [HttpPost("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SitioUpdateDto dto)
        {
            var actualizado = await _service.UpdateAsync(id, dto);
            if (!actualizado) return NotFound(new { mensaje = $"No existe el sitio con id {id}" });
            return NoContent();
        }

        // DELETE: api/sitios/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _service.DeleteAsync(id);
            if (!eliminado) return NotFound(new { mensaje = $"No existe el sitio con id {id}" });
            return NoContent();
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRACTICA_03.Data;
using PRACTICA_03.Models;

namespace PRACTICA_03.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TareasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TareasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? estado, [FromQuery] string? prioridad, [FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Validate and parse enums
            Estado? estadoEnum = null;
            if (!string.IsNullOrWhiteSpace(estado))
            {
                if (!Enum.TryParse<Estado>(estado, true, out var parsedEstado))
                {
                    return BadRequest(new { Error = "Valor de 'estado' inválido. Valores válidos: Pendiente, EnProceso, Completada." });
                }
                estadoEnum = parsedEstado;
            }

            Prioridad? prioridadEnum = null;
            if (!string.IsNullOrWhiteSpace(prioridad))
            {
                if (!Enum.TryParse<Prioridad>(prioridad, true, out var parsedPrioridad))
                {
                    return BadRequest(new { Error = "Valor de 'prioridad' inválido. Valores válidos: Baja, Media, Alta." });
                }
                prioridadEnum = parsedPrioridad;
            }

            // Validate date range
            if (fechaInicio.HasValue && fechaFin.HasValue && fechaInicio.Value > fechaFin.Value)
            {
                return BadRequest(new { Error = "'fechaInicio' no puede ser mayor que 'fechaFin'." });
            }

            var query = _context.Tareas.AsQueryable();

            if (estadoEnum.HasValue)
            {
                query = query.Where(t => t.Estado == estadoEnum.Value);
            }

            if (prioridadEnum.HasValue)
            {
                query = query.Where(t => t.Prioridad == prioridadEnum.Value);
            }

            if (fechaInicio.HasValue)
            {
                var start = fechaInicio.Value.Date;
                query = query.Where(t => t.FechaVencimiento >= start);
            }

            if (fechaFin.HasValue)
            {
                var end = fechaFin.Value.Date;
                query = query.Where(t => t.FechaVencimiento <= end);
            }

            var items = await query.ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _context.Tareas.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Tarea tarea)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (tarea.FechaVencimiento < DateTime.UtcNow.Date)
            {
                return BadRequest(new { Error = "FechaVencimiento no puede ser menor a la fecha actual." });
            }

            tarea.FechaCreacion = DateTime.UtcNow;
            _context.Tareas.Add(tarea);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = tarea.Id }, tarea);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Tarea tarea)
        {
            if (id != tarea.Id) return BadRequest(new { Error = "Id mismatch." });
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (tarea.FechaVencimiento < DateTime.UtcNow.Date)
            {
                return BadRequest(new { Error = "FechaVencimiento no puede ser menor a la fecha actual." });
            }

            var existing = await _context.Tareas.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Titulo = tarea.Titulo;
            existing.Descripcion = tarea.Descripcion;
            existing.Estado = tarea.Estado;
            existing.Prioridad = tarea.Prioridad;
            existing.FechaVencimiento = tarea.FechaVencimiento;

            _context.Tareas.Update(existing);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.Tareas.FindAsync(id);
            if (existing == null) return NotFound();

            _context.Tareas.Remove(existing);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

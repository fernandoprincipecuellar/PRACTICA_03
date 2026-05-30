using Microsoft.AspNetCore.Mvc;
using PRACTICA_03.Services;

namespace PRACTICA_03.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MlController : ControllerBase
    {
        private readonly SentimientoService _sentimientoService;

        public MlController(SentimientoService sentimientoService)
        {
            _sentimientoService = sentimientoService;
        }

        public class ComentarioDto
        {
            public string Comentario { get; set; } = string.Empty;
        }

        [HttpPost("sentimiento")]
        public IActionResult Sentimiento([FromBody] ComentarioDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Comentario))
            {
                return BadRequest(new { Error = "Se requiere el campo 'comentario'" });
            }

            var resultado = _sentimientoService.Predecir(dto.Comentario);

            return Ok(new { comentario = dto.Comentario, sentimiento = resultado });
        }
    }
}

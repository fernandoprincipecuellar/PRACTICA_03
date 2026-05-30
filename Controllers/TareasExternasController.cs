using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRACTICA_03.Models;

namespace PRACTICA_03.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TareasExternasController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TareasExternasController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("JsonPlaceholder");
                var resp = await client.GetAsync("todos");
                if (!resp.IsSuccessStatusCode)
                {
                    return StatusCode(502, new { Error = "La API externa no está disponible" });
                }

                var content = await resp.Content.ReadAsStringAsync();
                var external = JsonSerializer.Deserialize<List<ExternalTodo>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var list = new List<TareaExternaDto>();
                if (external != null)
                {
                    foreach (var e in external)
                    {
                        list.Add(new TareaExternaDto
                        {
                            ExternalId = e.Id,
                            Titulo = e.Title ?? string.Empty,
                            Completado = e.Completed
                        });
                    }
                }

                return Ok(list);
            }
            catch (HttpRequestException)
            {
                return StatusCode(502, new { Error = "La API externa no está disponible" });
            }
            catch (Exception)
            {
                return StatusCode(502, new { Error = "La API externa no está disponible" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("JsonPlaceholder");
                var resp = await client.GetAsync($"todos/{id}");
                if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound(new { Error = "Tarea externa no encontrada" });
                }

                if (!resp.IsSuccessStatusCode)
                {
                    return StatusCode(502, new { Error = "La API externa no está disponible" });
                }

                var content = await resp.Content.ReadAsStringAsync();
                var external = JsonSerializer.Deserialize<ExternalTodo>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (external == null)
                {
                    return NotFound(new { Error = "Tarea externa no encontrada" });
                }

                var dto = new TareaExternaDto
                {
                    ExternalId = external.Id,
                    Titulo = external.Title ?? string.Empty,
                    Completado = external.Completed
                };

                return Ok(dto);
            }
            catch (HttpRequestException)
            {
                return StatusCode(502, new { Error = "La API externa no está disponible" });
            }
            catch (Exception)
            {
                return StatusCode(502, new { Error = "La API externa no está disponible" });
            }
        }

        private class ExternalTodo
        {
            public int UserId { get; set; }
            public int Id { get; set; }
            public string? Title { get; set; }
            public bool Completed { get; set; }
        }
    }
}

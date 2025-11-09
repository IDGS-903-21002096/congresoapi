using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using congresoAPI.Data;
using congresoAPI.Models;

namespace congresoAPI.Controllers
{
    [Route("api/participante-qr")]
    [ApiController]
    [Tags("Par")]
    public class ParticipanteController : ControllerBase
    {
        private readonly clsParticipanteData _data;

        public ParticipanteController(clsParticipanteData data)
        {
            _data = data;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            try
            {
                var lista = await _data.ListaParticipantes();
                if (lista == null || !lista.Any())
                    return NotFound(new { message = "No se encontraron participantes registrados." });

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error al obtener los participantes", error = ex.Message });
            }
        }

        [HttpGet("{IdParticipante}")]
        public async Task<IActionResult> Obtener(int IdParticipante)
        {
            try
            {
                var participante = await _data.ObtenerParticipante(IdParticipante);
                if (participante == null)
                    return NotFound(new { message = $"No se encontró el participante con ID {IdParticipante}" });

                return Ok(participante);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error al obtener el participante", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] clsParticipante p)
        {
            if (p == null)
                return BadRequest(new { message = "Los datos del participante son inválidos." });

            try
            {
                bool ok = await _data.CrearParticipante(p);
                if (!ok)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { message = "No se pudo registrar el participante." });

                return Ok(new { isSuccess = ok, message = "Participante registrado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error al crear el participante", error = ex.Message });
            }
        }

        [HttpDelete("{IdParticipante}")]
        public async Task<IActionResult> Eliminar(int IdParticipante)
        {
            try
            {
                bool ok = await _data.EliminarParticipante(IdParticipante);
                if (!ok)
                    return NotFound(new { message = $"No se encontró el participante con ID {IdParticipante}" });

                return Ok(new { isSuccess = ok, message = "Participante eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error al eliminar el participante", error = ex.Message });
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using backend.Dtos;
using backend.Services;

namespace backend.Controllers
{
    /// <summary>
    /// Controlador para gerenciar pessoas cadastradas no sistema.
    /// Fornece criação, listagem e exclusão. A exclusão também remove todas
    /// as transações vinculadas à pessoa, garantindo consistência dos dados.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PessoasController : ControllerBase
    {
        private readonly IPessoaService _service;

        public PessoasController(IPessoaService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista todas as pessoas cadastradas.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PessoaResponseDto>>> Listar()
        {
            var pessoas = await _service.ListarAsync();
            return Ok(pessoas);
        }

        /// <summary>
        /// Retorna os totais por pessoa e o total geral do sistema.
        /// </summary>
        [HttpGet("totais")]
        public async Task<IActionResult> ListarTotais()
        {
            var totais = await _service.ListarTotaisAsync();
            return Ok(totais);
        }

        /// <summary>
        /// Cria uma nova pessoa após validar os dados obrigatórios.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarPessoaDto dto)
        {
            var resultado = await _service.CriarAsync(dto);
            return resultado.Success ? Ok(resultado.Value) : BadRequest(resultado.Error);
        }

        /// <summary>
        /// Remove uma pessoa do sistema pelo identificador.
        /// A exclusão é feita em cascata pelo mecanismo de banco de dados configurado.
        /// </summary>
        /// <param name="id">Identificador único da pessoa a ser excluída.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var resultado = await _service.DeletarAsync(id);
            return resultado.Success ? NoContent() : NotFound(resultado.Error);
        }
    }
}
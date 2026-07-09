using Microsoft.AspNetCore.Mvc;
using backend.Dtos;
using backend.Services;

namespace backend.Controllers
{
    /// <summary>
    /// Controlador responsável pelo gerenciamento de transações.
    /// Permite listar todas as transações e criar novas lançamentos de despesas ou receitas.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TransacoesController : ControllerBase
    {
        private readonly ITransacaoService _service;

        public TransacoesController(ITransacaoService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista todas as transações cadastradas, incluindo o nome da pessoa vinculada.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var transacoes = await _service.ListarAsync();
            return Ok(transacoes);
        }

        /// <summary>
        /// Cria uma nova transação após validar a pessoa, o valor e a regra de menoridade.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarTransacaoDto dto)
        {
            var resultado = await _service.CriarAsync(dto);

            return resultado.Success ? Ok(resultado.Value) : BadRequest(resultado.Error);
        }

        /// <summary>
        /// Remove uma transação pelo seu identificador.
        /// Esse endpoint existe principalmente para apoiar possíveis necessidades futuras,
        /// mas não é obrigatório pela especificação principal do sistema.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var resultado = await _service.DeletarAsync(id);
            return resultado.Success ? NoContent() : NotFound(resultado.Error);
        }
    }
}

using Hackathon.Domain.Interfaces;
using Hackathon.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.API.Controllers;

[ApiController]
[Route("produtos")]
public class ProdutosController(IProdutoRepository produtoRepository) : ControllerBase
{
    // debug: verifica qual produto atende a combinação
    [HttpGet("selecionar")]
    public async Task<ActionResult<Produto>> Selecionar([FromQuery] decimal valor, [FromQuery] int prazo, CancellationToken ct)
    {
        var p = await produtoRepository.GetProdutoAdequadoAsync(valor, prazo, ct);
        if (p is null) return NotFound("Nenhum produto atende aos parâmetros.");
        return Ok(p);
    }
}
using Hackathon.API.Domain.Models;
using Hackathon.API.Persistence.Produtos;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.API.Controllers;

[ApiController]
[Route("produtos")]
public class ProdutosController(IProdutoReadRepository produtoReadRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Produto>>> Get(CancellationToken ct)
        => Ok(await produtoReadRepository.ListarAsync(ct));

    // debug: verifica qual produto atende a combinação
    [HttpGet("selecionar")]
    public async Task<ActionResult<Produto>> Selecionar([FromQuery] decimal valor, [FromQuery] int prazo, CancellationToken ct)
    {
        var p = await produtoReadRepository.SelecionarProdutoParaAsync(valor, prazo, ct);
        if (p is null) return NotFound("Nenhum produto atende aos parâmetros.");
        return Ok(p);
    }
}
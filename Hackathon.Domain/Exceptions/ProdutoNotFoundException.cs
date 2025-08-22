namespace Hackathon.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando um produto não é encontrado
/// </summary>
public class ProdutoNotFoundException : DomainException
{
    public string CodigoProduto { get; }

    public ProdutoNotFoundException(string codigoProduto) 
        : base($"Produto com código '{codigoProduto}' não encontrado.")
    {
        CodigoProduto = codigoProduto;
    }
}

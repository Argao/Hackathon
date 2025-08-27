namespace Hackathon.Domain.ValueObjects;

public static class RegrasNegocio
{
    public static class Valores
    {
        public const decimal VALOR_MINIMO_EMPRESTIMO = 0.01m;
        public const decimal VALOR_MAXIMO_EMPRESTIMO = 999_999_999.99m;
        public const decimal VALOR_MINIMO_MONETARIO = 0.00m;
        public const decimal VALOR_MAXIMO_MONETARIO = 999_999_999_999.99m;
    }

    public static class Prazos
    {
        public const int PRAZO_MINIMO_MESES = 1;
        public const int PRAZO_MAXIMO_MESES = 600;
        public const int PRAZO_MAXIMO_API = 360;
    }

    public static class Taxas
    {
        public const decimal TAXA_MINIMA = 0.000001m;
        public const decimal TAXA_MAXIMA = 0.50m;
    }
}

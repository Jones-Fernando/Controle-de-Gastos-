namespace backend.Models
{
    /// <summary>
    /// Constantes e validação para os tipos de transação aceitos pelo sistema.
    /// </summary>
    public static class TipoTransacao
    {
        public const string Receita = "Receita";
        public const string Despesa = "Despesa";

        public static bool IsValido(string? tipo) =>
            string.Equals(tipo, Receita, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(tipo, Despesa, StringComparison.OrdinalIgnoreCase);
    }
}

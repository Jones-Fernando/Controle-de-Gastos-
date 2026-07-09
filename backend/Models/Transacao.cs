namespace backend.Models
{
    public class Transacao
    {
        public int Id { get; set; }
        public required string Descricao { get; set; }
        public decimal Valor { get; set; }
        
        // Deve receber "Receita" ou "Despesa"
        public required string Tipo { get; set; } 

        public int PessoaId { get; set; }
        public Pessoa? Pessoa { get; set; }
    }
}
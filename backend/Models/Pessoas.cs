using System.Text.Json.Serialization;

namespace backend.Models
{
    public class Pessoa
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public int Idade { get; set; }

        [JsonIgnore] // Impede que as transações sejam carregadas recursivamente e quebrem o JSON
        public List<Transacao> Transacoes { get; set; } = new();
    }
}
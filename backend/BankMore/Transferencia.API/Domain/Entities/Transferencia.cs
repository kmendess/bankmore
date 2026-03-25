namespace Transferencia.API.Domain.Entities
{
    public class Transferencia
    {
        public string Id { get; private set; }
        public string ContaOrigemId { get; private set; }
        public string ContaDestinoId { get; private set; }
        public DateTime Data { get; private set; }
        public decimal Valor { get; private set; }

        private Transferencia() { }

        public Transferencia(string contaOrigemId, string contaDestinoId, decimal valor)
        {
            Id = Guid.NewGuid().ToString();
            ContaOrigemId = contaOrigemId;
            ContaDestinoId = contaDestinoId;
            Data = DateTime.Now;
            Valor = valor;
        }
    }
}

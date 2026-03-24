namespace ContaCorrente.API.Domain.Entities
{
    public class Movimento
    {
        public string IdMovimento { get; private set; }
        public string IdContaCorrente { get; private set; }
        public string DataMovimento { get; private set; }
        public string TipoMovimento { get; private set; }
        public decimal Valor { get; private set; }

        public Movimento(string idContaCorrente, string tipo, decimal valor)
        {
            IdMovimento = Guid.NewGuid().ToString();
            IdContaCorrente = idContaCorrente;
            DataMovimento = DateTime.Now.ToString("dd/MM/yyyy");
            TipoMovimento = tipo;
            Valor = valor;
        }
    }
}

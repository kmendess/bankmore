namespace ContaCorrente.API.Application.Queries
{
    public class SaldoResponse
    {
        public int NumeroConta { get; set; }
        public string Nome { get; set; }
        public DateTime DataHoraConsulta { get; set; }
        public decimal Saldo { get; set; }
    }
}

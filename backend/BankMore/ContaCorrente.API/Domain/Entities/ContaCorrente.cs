namespace ContaCorrente.API.Domain.Entities
{
    public class ContaCorrente
    {
        public string Id { get; protected set; }
        public int Numero { get; private set; }
        public string Nome { get; private set; }
        public bool Ativo { get; private set; }
        public string Senha { get; private set; }
        public string Salt { get; private set; }

        private ContaCorrente() { }

        public ContaCorrente(string nome, string senha)
        {
            Id = Guid.NewGuid().ToString();
            Numero = GerarNumeroConta();
            Nome = nome;
            Ativo = true;
            Senha = senha;
            Salt = Guid.NewGuid().ToString();
        }

        private int GerarNumeroConta()
        {
            return new Random().Next(10000, 99999);
        }
    }
}

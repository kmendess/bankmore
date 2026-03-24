namespace ContaCorrente.API.Domain.Interfaces
{
    public interface IContaCorrenteRepository
    {
        Task Criar(Entities.ContaCorrente conta);
        Task<bool> ExistePorCpf(string cpf);
        Task<Entities.ContaCorrente?> ObterPorCpf(string cpf);
        Task<Entities.ContaCorrente?> ObterPorNumero(int numero);
    }
}

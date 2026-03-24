using ContaCorrente.API.Domain.Entities;

namespace ContaCorrente.API.Domain.Interfaces
{
    public interface IMovimentoRepository
    {
        Task Inserir(Movimento movimento);
    }
}

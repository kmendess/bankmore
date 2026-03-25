namespace Transferencia.API.Domain.Interfaces
{
    public interface ITransferenciaRepository
    {
        Task Inserir(Entities.Transferencia transferencia);
    }
}

namespace ContaCorrente.API.Domain.Interfaces
{
    public interface IIdempotenciaRepository
    {
        Task<bool> Existe(string chave);
        Task Salvar(string chave, string requisicao, string resultado);
    }
}

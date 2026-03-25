using Dapper;
using Transferencia.API.Domain.Interfaces;
using Transferencia.API.Infrastructure.Data;

namespace Transferencia.API.Infrastructure.Repositories
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly DbConnectionFactory _connection;

        public IdempotenciaRepository(DbConnectionFactory connection)
        {
            _connection = connection;
        }

        public async Task<bool> Existe(string chave)
        {
            var sql = @"SELECT 1 FROM idempotencia WHERE chave_idempotencia = @Chave";

            using var connection = _connection.CreateConnection();

            var result = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { Chave = chave });

            return result.HasValue;
        }

        public async Task Salvar(string chave, string requisicao, string resultado)
        {
            var sql = @"INSERT INTO idempotencia
                        (chave_idempotencia, requisicao, resultado)
                        VALUES (@Chave, @Requisicao, @Resultado)";

            using var connection = _connection.CreateConnection();

            await connection.ExecuteAsync(sql, new
            {
                Chave = chave,
                Requisicao = requisicao,
                Resultado = resultado
            });
        }
    }
}

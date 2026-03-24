using ContaCorrente.API.Domain.Entities;
using ContaCorrente.API.Domain.Interfaces;
using ContaCorrente.API.Infrastructure.Data;
using Dapper;

namespace ContaCorrente.API.Infrastructure.Repositories
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly DbConnectionFactory _connection;

        public MovimentoRepository(DbConnectionFactory connection)
        {
            _connection = connection;
        }

        public async Task Inserir(Movimento movimento)
        {
            var sql = @"INSERT INTO movimento
                        (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor)
                        VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)";

            using var connection = _connection.CreateConnection();

            await connection.ExecuteAsync(sql, new
            {
                movimento.IdMovimento,
                movimento.IdContaCorrente,
                movimento.DataMovimento,
                movimento.TipoMovimento,
                movimento.Valor
            });
        }
    }
}

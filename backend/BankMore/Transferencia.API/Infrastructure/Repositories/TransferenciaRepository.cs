using Dapper;
using System.Globalization;
using Transferencia.API.Domain.Interfaces;
using Transferencia.API.Infrastructure.Data;

namespace Transferencia.API.Infrastructure.Repositories
{
    public class TransferenciaRepository : ITransferenciaRepository
    {
        private readonly DbConnectionFactory _connection;

        public TransferenciaRepository(DbConnectionFactory connection)
        {
            _connection = connection;
        }

        public async Task Inserir(Domain.Entities.Transferencia transferencia)
        {
            var sql = @"INSERT INTO transferencia
                        (idtransferencia, idcontacorrente_origem, idcontacorrente_destino, datamovimento, valor)
                        VALUES
                        (@Id, @ContaOrigemId, @ContaDestinoId, @Data, @Valor)";

            using var connection = _connection.CreateConnection();

            await connection.ExecuteAsync(sql, new
            {
                transferencia.Id,
                transferencia.ContaOrigemId,
                transferencia.ContaDestinoId,
                Data = transferencia.Data.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                transferencia.Valor
            });
        }
    }
}

using ContaCorrente.API.Domain.Interfaces;
using ContaCorrente.API.Infrastructure.Data;
using Dapper;

namespace ContaCorrente.API.Infrastructure.Repositories
{
    public class ContaCorrenteRepository : IContaCorrenteRepository
    {
        private readonly DbConnectionFactory _connection;

        public ContaCorrenteRepository(DbConnectionFactory connection)
        {
            _connection = connection;
        }

        public async Task Criar(Domain.Entities.ContaCorrente conta)
        {
            var sql = @"INSERT INTO contacorrente.contacorrente
                        (idcontacorrente, numero, nome, ativo, senha, salt)
                        VALUES (@Id, @Numero, @Nome, @Ativo, @Senha, @Salt)";

            using var connection = _connection.CreateConnection();

            await connection.ExecuteAsync(sql, new
            {
                conta.Id,
                conta.Numero,
                conta.Nome,
                Ativo = conta.Ativo ? 1 : 0,
                conta.Senha,
                conta.Salt
            });
        }

        public async Task<bool> ExistePorCpf(string cpf)
        {
            var sql = @"SELECT 1 
                          FROM contacorrente
                         WHERE nome = @Cpf
                         LIMIT 1";

            using var connection = _connection.CreateConnection();

            var result = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { Cpf = cpf });

            return result.HasValue;
        }

        public async Task<Domain.Entities.ContaCorrente?> ObterPorCpf(string cpf)
        {
            var sql = @"SELECT idcontacorrente AS Id,
                               numero AS Numero,
                               nome AS Nome,
                               ativo AS Ativo,
                               senha AS Senha,
                               salt AS Salt
                          FROM contacorrente
                         WHERE nome = @Cpf";

            using var connection = _connection.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Domain.Entities.ContaCorrente?>(sql, new { Cpf = cpf });
        }

        public async Task<Domain.Entities.ContaCorrente?> ObterPorNumero(int numero)
        {
            var sql = @"SELECT idcontacorrente AS Id,
                               numero AS Numero,
                               nome AS Nome,
                               ativo AS Ativo,
                               senha AS Senha,
                               salt AS Salt
                          FROM contacorrente
                         WHERE numero = @Numero";

            using var connection = _connection.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Domain.Entities.ContaCorrente?>(sql, new { Numero = numero });
        }

        public async Task<Domain.Entities.ContaCorrente?> ObterPorId(string id)
        {
            var sql = @"SELECT idcontacorrente AS Id,
                               numero AS Numero,
                               nome AS Nome,
                               ativo AS Ativo,
                               senha AS Senha,
                               salt AS Salt
                          FROM contacorrente
                         WHERE idcontacorrente = @Id";

            using var connection = _connection.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Domain.Entities.ContaCorrente?>(sql, new { Id = id });
        }

        public async Task Inativar(string id)
        {
            var sql = @"UPDATE contacorrente
                           SET ativo = 0
                         WHERE idcontacorrente = @Id";

            using var connection = _connection.CreateConnection();

            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}

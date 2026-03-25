using BankMore.Application.Models;
using BankMore.Domain.Enums;
using MediatR;
using Transferencia.API.Application.Commands;
using Transferencia.API.Domain.Interfaces;
using Transferencia.API.Infrastructure.Services;

namespace Transferencia.API.Application.Handlers
{
    public class TransferirHandler : IRequestHandler<TransferirCommand, Response>
    {
        private readonly ContaCorrenteClient _client;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ITransferenciaRepository _transferenciaRepository;
        private readonly IIdempotenciaRepository _idempotenciaRepository;

        public TransferirHandler(
            ContaCorrenteClient client,
            IHttpContextAccessor httpContext,
            ITransferenciaRepository transferenciaRepository,
            IIdempotenciaRepository idempotenciaRepository)
        {
            _client = client;
            _httpContext = httpContext;
            _transferenciaRepository = transferenciaRepository;
            _idempotenciaRepository = idempotenciaRepository;
        }

        public async Task<Response> Handle(TransferirCommand request, CancellationToken cancellationToken)
        {
            var authHeader = _httpContext.HttpContext?.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader))
                return Response.Error(ErrorType.USER_UNAUTHORIZED, "Token inválido");

            var token = authHeader.Replace("Bearer ", "");
            _client.SetToken(token);

            if (string.IsNullOrEmpty(request.Id))
                return Response.Error(ErrorType.INVALID_DOCUMENT, "Identificador da requisição inválido");

            if (await _idempotenciaRepository.Existe(request.Id))
                return Response.Success();

            var contaOrigem = await _client.ObterContaLogada();

            if (contaOrigem == null)
                return Response.Error(ErrorType.INVALID_ACCOUNT, "Conta de origem inválida");

            if (!contaOrigem.Ativo)
                return Response.Error(ErrorType.INVALID_ACCOUNT, "Conta de origem inativa");

            if (request.Valor <= 0)
                return Response.Error(ErrorType.INVALID_VALUE, "Valor inválido");

            var contaDestino = await _client.ObterContaPorNumero(request.NumeroContaDestino);

            if (contaDestino == null)
                return Response.Error(ErrorType.INVALID_ACCOUNT, "Conta de destino inválida");

            if (!contaDestino.Ativo)
                return Response.Error(ErrorType.INACTIVE_ACCOUNT, "Conta de destino inativa");

            var debito = await _client.Debitar(request.Id, contaOrigem.Numero, request.Valor);

            if (!debito.IsSuccess)
                return debito;

            var credito = await _client.Creditar(request.Id, request.NumeroContaDestino, request.Valor);

            if (!credito.IsSuccess)
            {
                await _client.Creditar(request.Id, contaOrigem.Numero, request.Valor);

                return Response.Error(ErrorType.INVALID_DOCUMENT, "Erro ao creditar conta destino");
            }

            var transferencia = new Domain.Entities.Transferencia(contaOrigem.Id, contaDestino.Id, request.Valor);

            await _transferenciaRepository.Inserir(transferencia);

            await _idempotenciaRepository.Salvar(request.Id, "transferencia", "OK");

            return Response.Success();
        }
    }
}

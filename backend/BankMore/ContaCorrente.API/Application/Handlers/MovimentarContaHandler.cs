using BankMore.Application.Models;
using BankMore.Domain.Enums;
using ContaCorrente.API.Application.Commands;
using ContaCorrente.API.Domain.Entities;
using ContaCorrente.API.Domain.Interfaces;
using MediatR;
using System.Text.Json;

namespace ContaCorrente.API.Application.Handlers
{
    public class MovimentarContaHandler : IRequestHandler<MovimentarContaCommand, Response>
    {
        private readonly IContaCorrenteRepository _contaRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IIdempotenciaRepository _idempotenciaRepository;
        private readonly IHttpContextAccessor _httpContext;

        public MovimentarContaHandler(
            IContaCorrenteRepository contaRepository,
            IMovimentoRepository movimentoRepository,
            IIdempotenciaRepository idempotenciaRepository,
            IHttpContextAccessor httpContext)
        {
            _contaRepository = contaRepository;
            _movimentoRepository = movimentoRepository;
            _idempotenciaRepository = idempotenciaRepository;
            _httpContext = httpContext;
        }

        public async Task<Response> Handle(MovimentarContaCommand request, CancellationToken cancellationToken)
        {
            var accountId = _httpContext.HttpContext?.User.FindFirst("accountId")?.Value;

            if (string.IsNullOrEmpty(accountId))
                return Response.Error(ErrorType.USER_UNAUTHORIZED, "Token inválido");

            if (string.IsNullOrEmpty(request.Id))
                return Response.Error(ErrorType.INVALID_DOCUMENT, "Identificador da requisição inválido");

            if (await _idempotenciaRepository.Existe(request.Id))
                return Response.Success();

            var contaLogada = await _contaRepository.ObterPorId(accountId);

            if (contaLogada == null)
                return Response.Error(ErrorType.INVALID_ACCOUNT, "Conta inválida");

            if (!contaLogada.Ativo)
                return Response.Error(ErrorType.INACTIVE_ACCOUNT, "Conta inativa");

            var contaDestino = await ObterContaCorrenteDestino(request, contaLogada);

            if (contaDestino == null)
                return Response.Error(ErrorType.INVALID_ACCOUNT, "Conta de destino inválida");

            if (!contaDestino.Ativo)
                return Response.Error(ErrorType.INACTIVE_ACCOUNT, "Conta de destino inativa");

            if (request.Valor <= 0)
                return Response.Error(ErrorType.INVALID_VALUE, "Valor inválido");

            if (request.Tipo != TipoMovimento.Credito.GetDescription() &&
                request.Tipo != TipoMovimento.Debito.GetDescription())
                return Response.Error(ErrorType.INVALID_TYPE, "Tipo de movimentação inválido");

            if (contaDestino.Id != contaLogada.Id &&
                request.Tipo != TipoMovimento.Credito.GetDescription())
                return Response.Error(ErrorType.INVALID_TYPE, "Conta de destino não pode receber esse tipo de movimentação");

            var movimento = new Movimento(
                contaDestino.Id,
                request.Tipo,
                request.Valor
            );

            await _movimentoRepository.Inserir(movimento);

            await _idempotenciaRepository.Salvar(
                request.Id,
                JsonSerializer.Serialize(request),
                "OK"
            );

            return Response.Success();
        }

        private async Task<Domain.Entities.ContaCorrente?> ObterContaCorrenteDestino(MovimentarContaCommand request, Domain.Entities.ContaCorrente contaLogada)
        {
            if (request.NumeroConta.HasValue)
                return await _contaRepository.ObterPorNumero(request.NumeroConta.Value);
            return contaLogada;
        }
    }
}

using BankMore.Application.Models;
using BankMore.Domain.Enums;
using ContaCorrente.API.Application.Queries;
using ContaCorrente.API.Domain.Interfaces;
using MediatR;

namespace ContaCorrente.API.Application.Handlers
{
    public class ObterSaldoHandler : IRequestHandler<ObterSaldoQuery, Response<SaldoResponse>>
    {
        private readonly IContaCorrenteRepository _contaRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IHttpContextAccessor _httpContext;

        public ObterSaldoHandler(
            IContaCorrenteRepository contaRepository,
            IMovimentoRepository movimentoRepository,
            IHttpContextAccessor httpContext)
        {
            _contaRepository = contaRepository;
            _movimentoRepository = movimentoRepository;
            _httpContext = httpContext;
        }

        public async Task<Response<SaldoResponse>> Handle(ObterSaldoQuery request, CancellationToken cancellationToken)
        {
            var accountId = _httpContext.HttpContext?.User.FindFirst("accountId")?.Value;

            if (string.IsNullOrEmpty(accountId))
                return Response<SaldoResponse>.Error(ErrorType.USER_UNAUTHORIZED, "Token inválido");

            var conta = await _contaRepository.ObterPorId(accountId);

            if (conta == null)
                return Response<SaldoResponse>.Error(ErrorType.INVALID_ACCOUNT, "Conta inválida");

            if (!conta.Ativo)
                return Response<SaldoResponse>.Error(ErrorType.INACTIVE_ACCOUNT, "Conta inativa");

            var saldo = await _movimentoRepository.ObterSaldo(conta.Id);

            return Response<SaldoResponse>.Success(
                new SaldoResponse
                {
                    NumeroConta = conta.Numero,
                    Nome = conta.Nome,
                    DataHoraConsulta = DateTime.Now,
                    Saldo = saldo
                });
        }
    }
}

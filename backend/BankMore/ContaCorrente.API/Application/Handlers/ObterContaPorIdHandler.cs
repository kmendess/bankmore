using BankMore.Application.Models;
using BankMore.Domain.Enums;
using ContaCorrente.API.Application.Queries;
using ContaCorrente.API.Domain.Interfaces;
using MediatR;

namespace ContaCorrente.API.Application.Handlers
{
    public class ObterContaPorIdHandler : IRequestHandler<ObterContaPorIdQuery, Response<ContaCorrenteResponse?>>
    {
        private readonly IContaCorrenteRepository _repository;
        private readonly IHttpContextAccessor _httpContext;

        public ObterContaPorIdHandler(
            IContaCorrenteRepository repository,
            IHttpContextAccessor httpContext)
        {
            _repository = repository;
            _httpContext = httpContext;
        }

        public async Task<Response<ContaCorrenteResponse?>> Handle(ObterContaPorIdQuery request, CancellationToken cancellationToken)
        {
            var accountId = _httpContext.HttpContext?.User.FindFirst("accountId")?.Value;

            if (string.IsNullOrEmpty(accountId))
                return Response<ContaCorrenteResponse?>.Error(ErrorType.USER_UNAUTHORIZED, "Token inválido");

            var conta = await _repository.ObterPorId(accountId);

            if (conta == null)
                return Response<ContaCorrenteResponse?>.Error(ErrorType.INVALID_ACCOUNT, "Conta inválida");

            return Response<ContaCorrenteResponse?>.Success(
                new ContaCorrenteResponse
                {
                    Id = conta.Id,
                    Numero = conta.Numero,
                    Nome = conta.Nome,
                    Ativo = conta.Ativo
                });
        }
    }
}

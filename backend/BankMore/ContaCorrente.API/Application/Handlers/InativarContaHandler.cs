using BankMore.Application.Models;
using BankMore.Domain.Enums;
using BankMore.Infrastructure.Auth;
using ContaCorrente.API.Application.Commands;
using ContaCorrente.API.Domain.Interfaces;
using MediatR;

namespace ContaCorrente.API.Application.Handlers
{
    public class InativarContaHandler : IRequestHandler<InativarContaCommand, Response>
    {
        private readonly IContaCorrenteRepository _repository;
        private readonly IPasswordService _passwordService;
        private readonly IHttpContextAccessor _httpContext;

        public InativarContaHandler(
            IContaCorrenteRepository repository,
            IPasswordService passwordService,
            IHttpContextAccessor httpContext)
        {
            _repository = repository;
            _passwordService = passwordService;
            _httpContext = httpContext;
        }

        public async Task<Response> Handle(InativarContaCommand request, CancellationToken cancellationToken)
        {
            var accountId = _httpContext.HttpContext?.User.FindFirst("accountId")?.Value;

            if (string.IsNullOrEmpty(accountId))
                return Response.Error(ErrorType.USER_UNAUTHORIZED, "Token inválido");

            var conta = await _repository.ObterPorId(accountId);

            if (conta == null)
                return Response.Error(ErrorType.INVALID_ACCOUNT, "Conta inválida");

            var senhaValida = _passwordService.Verify(request.Senha, conta.Senha);

            if (!senhaValida)
                return Response.Error(ErrorType.INVALID_DOCUMENT, "Senha inválida");

            await _repository.Inativar(conta.Id);

            return Response.Success();
        }
    }
}

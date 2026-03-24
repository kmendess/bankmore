using BankMore.Application.Models;
using BankMore.Domain.Enums;
using BankMore.Infrastructure.Auth;
using ContaCorrente.API.Application.Commands;
using ContaCorrente.API.Domain.Interfaces;
using MediatR;

namespace ContaCorrente.API.Application.Handlers
{
    public class LoginHandler : IRequestHandler<LoginCommand, Response<string>>
    {
        private readonly IContaCorrenteRepository _repository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;

        public LoginHandler(
            IContaCorrenteRepository repository,
            IPasswordService passwordService,
            IJwtService jwtService)
        {
            _repository = repository;
            _passwordService = passwordService;
            _jwtService = jwtService;
        }

        public async Task<Response<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.ContaCorrente conta = null;

            if (!string.IsNullOrEmpty(request.Cpf))
                conta = await _repository.ObterPorCpf(request.Cpf);
            else if (request.NumeroConta.HasValue)
                conta = await _repository.ObterPorNumero(request.NumeroConta.Value);

            if (conta == null)
                return Response<string>.Error(ErrorType.USER_UNAUTHORIZED, "Usuário não encontrado");

            var senhaValida = _passwordService.Verify(request.Senha, conta.Senha);

            if (!senhaValida)
                return Response<string>.Error(ErrorType.USER_UNAUTHORIZED, "Senha inválida");

            var token = _jwtService.GenerateToken(conta.Id);

            return Response<string>.Success(token);
        }
    }
}

using BankMore.Application.Models;
using BankMore.Domain.Enums;
using BankMore.Infrastructure.Auth;
using ContaCorrente.API.Application.Commands;
using ContaCorrente.API.Domain.Interfaces;
using MediatR;

namespace ContaCorrente.API.Application.Handlers
{
    public class LoginHandler : IRequestHandler<LoginCommand, Result<string>>
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

        public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var contaCorrente = await ObterContaCorrente(request);

            if (contaCorrente == null)
                return Result<string>.Error(ErrorType.USER_UNAUTHORIZED, "Usuário não encontrado");

            var senhaValida = _passwordService.Verify(request.Senha, contaCorrente.Senha);

            if (!senhaValida)
                return Result<string>.Error(ErrorType.USER_UNAUTHORIZED, "Senha inválida");

            var token = _jwtService.GenerateToken(contaCorrente.Id);

            return Result<string>.Success(token);
        }

        private async Task<Domain.Entities.ContaCorrente?> ObterContaCorrente(LoginCommand request)
        {
            Domain.Entities.ContaCorrente? contaCorrente = null;

            if (!string.IsNullOrEmpty(request.Cpf))
                contaCorrente = await _repository.ObterPorCpf(request.Cpf);
            else if (request.NumeroConta.HasValue)
                contaCorrente = await _repository.ObterPorNumero(request.NumeroConta.Value);

            return contaCorrente;
        }
    }
}

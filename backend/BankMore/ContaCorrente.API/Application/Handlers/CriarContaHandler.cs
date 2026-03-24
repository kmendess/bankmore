using BankMore.Application.Models;
using BankMore.Domain.Enums;
using BankMore.Infrastructure.Auth;
using ContaCorrente.API.Application.Commands;
using ContaCorrente.API.Domain.Interfaces;
using MediatR;

namespace ContaCorrente.API.Application.Handlers
{
    public class CriarContaHandler : IRequestHandler<CriarContaCommand, Response<int>>
    {
        private readonly IContaCorrenteRepository _repository;
        private readonly IPasswordService _passwordService;

        public CriarContaHandler(
            IContaCorrenteRepository repository,
            IPasswordService passwordService)
        {
            _repository = repository;
            _passwordService = passwordService;
        }

        public async Task<Response<int>> Handle(CriarContaCommand request, CancellationToken cancellationToken)
        {
            request.Cpf = new string(request.Cpf.Where(char.IsDigit).ToArray());

            if (!CpfValido(request.Cpf))
                return Response<int>.Error(ErrorType.INVALID_DOCUMENT, "CPF inválido");

            var cpfJaCadastrado = await _repository.ExistePorCpf(request.Cpf);
            if (cpfJaCadastrado)
                return Response<int>.Error(ErrorType.INVALID_DOCUMENT, "CPF já cadastrado");

            if (!SenhaValida(request.Senha))
                return Response<int>.Error(ErrorType.INVALID_DOCUMENT, "Senha inválida");

            var conta = new Domain.Entities.ContaCorrente(
                request.Cpf,
                _passwordService.Hash(request.Senha)
            );

            await _repository.Criar(conta);

            return Response<int>.Success(conta.Numero);
        }

        private bool SenhaValida(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
                return false;

            return true;
        }

        private bool CpfValido(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            if (cpf.Length != 11)
                return false;

            return true;
        }
    }
}

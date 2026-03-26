using BankMore.Application.Models;
using BankMore.Domain.Enums;
using BankMore.Infrastructure.Auth;
using ContaCorrente.API.Application.Commands;
using ContaCorrente.API.Domain.Interfaces;
using MediatR;

namespace ContaCorrente.API.Application.Handlers
{
    public class CriarContaHandler : IRequestHandler<CriarContaCommand, Result<int>>
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

        public async Task<Result<int>> Handle(CriarContaCommand request, CancellationToken cancellationToken)
        {
            request.Cpf = request.Cpf.Replace(".", "").Replace("-", "");

            var result = await ValidarCpf(request.Cpf)
                .Then(() => ValidarSenha(request.Senha))
                .Then(() => CriarConta(request));

            return result;
        }

        private async Task<Result<int>> CriarConta(CriarContaCommand request)
        {
            var conta = new Domain.Entities.ContaCorrente(
                request.Cpf,
                _passwordService.Hash(request.Senha)
            );

            await _repository.Criar(conta);

            return Result<int>.Success(conta.Numero);
        }

        private Result ValidarSenha(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
                return Result.Error(ErrorType.INVALID_DOCUMENT, "Senha inválida");

            return Result.Success();
        }

        private async Task<Result> ValidarCpf(string cpf)
        {
            if (!CpfValido(cpf))
                return Result.Error(ErrorType.INVALID_DOCUMENT, "CPF inválido");

            var cpfJaCadastrado = await _repository.ExistePorCpf(cpf);
            if (cpfJaCadastrado)
                return Result.Error(ErrorType.INVALID_DOCUMENT, "CPF já cadastrado");

            return Result.Success();
        }

        private bool CpfValido(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            // Deve ter 11 dígitos
            if (cpf.Length != 11)
                return false;

            // Apenas números
            if (!cpf.All(char.IsDigit))
                return false;

            // Bloqueia CPFs com todos os dígitos iguais
            if (cpf.Distinct().Count() == 1)
                return false;

            // Validação dos dígitos verificadores
            var numeros = cpf.Select(c => int.Parse(c.ToString())).ToArray();

            // Primeiro dígito
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += numeros[i] * (10 - i);

            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            // Segundo dígito
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += numeros[i] * (11 - i);

            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            return numeros[9] == digito1 && numeros[10] == digito2;
        }
    }
}

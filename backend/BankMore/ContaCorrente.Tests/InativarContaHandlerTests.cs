using BankMore.Infrastructure.Auth;
using ContaCorrente.API.Application.Commands;
using ContaCorrente.API.Application.Handlers;
using ContaCorrente.API.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace ContaCorrente.Tests
{
    public class InativarContaHandlerTests
    {
        private readonly Mock<IContaCorrenteRepository> _repository = new();
        private readonly Mock<IPasswordService> _passwordService = new();
        private readonly Mock<IHttpContextAccessor> _httpContext = new();

        private readonly InativarContaHandler _handler;

        public InativarContaHandlerTests()
        {
            _handler = new InativarContaHandler(
                _repository.Object,
                _passwordService.Object,
                _httpContext.Object
            );
        }

        private void MockUsuario(string accountId)
        {
            var claims = new[]
            {
                new Claim("accountId", accountId)
            };

            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            var context = new DefaultHttpContext
            {
                User = user
            };

            _httpContext.Setup(x => x.HttpContext)
                .Returns(context);
        }

        [Fact]
        public async Task Deve_Inativar_Conta_Com_Sucesso()
        {
            // Arrange
            var accountId = "123";

            MockUsuario(accountId);

            var conta = new API.Domain.Entities.ContaCorrente("cpf", "hash");

            _repository.Setup(x => x.ObterPorId(accountId))
                .ReturnsAsync(conta);

            _passwordService.Setup(x => x.Verify("senha", "hash"))
                .Returns(true);

            var request = new InativarContaCommand
            {
                Senha = "senha"
            };

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Deve_Falhar_Quando_Token_Invalido()
        {
            var request = new InativarContaCommand
            {
                Senha = "senha"
            };

            var result = await _handler.Handle(request, default);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Deve_Falhar_Quando_Conta_Nao_Existe()
        {
            var accountId = "123";

            MockUsuario(accountId);

            _repository.Setup(x => x.ObterPorId(accountId))
                .ReturnsAsync((API.Domain.Entities.ContaCorrente?)null);

            var request = new InativarContaCommand
            {
                Senha = "senha"
            };

            var result = await _handler.Handle(request, default);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Deve_Falhar_Quando_Senha_Invalida()
        {
            var accountId = "123";

            MockUsuario(accountId);

            var conta = new API.Domain.Entities.ContaCorrente("cpf", "hash");

            _repository.Setup(x => x.ObterPorId(accountId))
                .ReturnsAsync(conta);

            _passwordService.Setup(x => x.Verify("senha", "hash"))
                .Returns(false);

            var request = new InativarContaCommand
            {
                Senha = "senha"
            };

            var result = await _handler.Handle(request, default);

            result.IsSuccess.Should().BeFalse();
        }
    }
}

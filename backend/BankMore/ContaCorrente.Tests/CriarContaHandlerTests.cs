using BankMore.Infrastructure.Auth;
using ContaCorrente.API.Application.Commands;
using ContaCorrente.API.Application.Handlers;
using ContaCorrente.API.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ContaCorrente.Tests
{
    public class CriarContaHandlerTests
    {
        private readonly Mock<IContaCorrenteRepository> _repository = new();
        private readonly Mock<IPasswordService> _passwordService = new();

        private readonly CriarContaHandler _handler;

        public CriarContaHandlerTests()
        {
            _handler = new CriarContaHandler(
                _repository.Object,
                _passwordService.Object
            );
        }

        [Fact]
        public async Task Deve_Criar_Conta_Com_Sucesso()
        {
            // Arrange
            var request = new CriarContaCommand
            {
                Cpf = "12345678909",
                Senha = "123456"
            };

            _repository.Setup(x => x.ExistePorCpf(It.IsAny<string>()))
                .ReturnsAsync(false);

            _passwordService.Setup(x => x.Hash(It.IsAny<string>()))
                .Returns("senha-hash");

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeGreaterThan(0);

            _repository.Verify(x => x.Criar(It.IsAny<API.Domain.Entities.ContaCorrente>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Falhar_Quando_Cpf_Invalido()
        {
            var request = new CriarContaCommand
            {
                Cpf = "123",
                Senha = "123456"
            };

            var result = await _handler.Handle(request, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Deve_Falhar_Quando_Cpf_Ja_Cadastrado()
        {
            var request = new CriarContaCommand
            {
                Cpf = "12345678909",
                Senha = "123456"
            };

            _repository.Setup(x => x.ExistePorCpf(It.IsAny<string>()))
                .ReturnsAsync(true);

            var result = await _handler.Handle(request, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Deve_Falhar_Quando_Senha_Invalida()
        {
            var request = new CriarContaCommand
            {
                Cpf = "12345678909",
                Senha = ""
            };

            _repository.Setup(x => x.ExistePorCpf(It.IsAny<string>()))
                .ReturnsAsync(false);

            var result = await _handler.Handle(request, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
        }
    }
}

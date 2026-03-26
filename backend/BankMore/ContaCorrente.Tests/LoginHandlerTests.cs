using BankMore.Domain.Enums;
using BankMore.Infrastructure.Auth;
using ContaCorrente.API.Application.Commands;
using ContaCorrente.API.Application.Handlers;
using ContaCorrente.API.Domain.Interfaces;
using Moq;

namespace ContaCorrente.Tests
{
    public class LoginHandlerTests
    {
        private readonly Mock<IContaCorrenteRepository> _repositoryMock;
        private readonly Mock<IPasswordService> _passwordServiceMock;
        private readonly Mock<IJwtService> _jwtServiceMock;

        private readonly LoginHandler _handler;

        public LoginHandlerTests()
        {
            _repositoryMock = new Mock<IContaCorrenteRepository>();
            _passwordServiceMock = new Mock<IPasswordService>();
            _jwtServiceMock = new Mock<IJwtService>();

            _handler = new LoginHandler(
                _repositoryMock.Object,
                _passwordServiceMock.Object,
                _jwtServiceMock.Object);
        }

        [Fact]
        public async Task Deve_Retornar_Erro_Quando_Usuario_Nao_Encontrado()
        {
            var command = new LoginCommand { Cpf = "12345678900", Senha = "123" };

            _repositoryMock
                .Setup(r => r.ObterPorCpf(command.Cpf))
                .ReturnsAsync((API.Domain.Entities.ContaCorrente)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.USER_UNAUTHORIZED.ToString(), result.ErrorType);
        }

        [Fact]
        public async Task Deve_Retornar_Erro_Quando_Senha_Invalida()
        {
            var conta = new API.Domain.Entities.ContaCorrente("12345678900", "hash");

            var command = new LoginCommand { Cpf = "12345678900", Senha = "errada" };

            _repositoryMock
                .Setup(r => r.ObterPorCpf(command.Cpf))
                .ReturnsAsync(conta);

            _passwordServiceMock
                .Setup(p => p.Verify(command.Senha, conta.Senha))
                .Returns(false);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.USER_UNAUTHORIZED.ToString(), result.ErrorType);
        }

        [Fact]
        public async Task Deve_Retornar_Token_Quando_Login_Valido()
        {
            var conta = new API.Domain.Entities.ContaCorrente("12345678900", "hash");

            var token = "jwt-token";

            var command = new LoginCommand { Cpf = "12345678900", Senha = "123" };

            _repositoryMock
                .Setup(r => r.ObterPorCpf(command.Cpf))
                .ReturnsAsync(conta);

            _passwordServiceMock
                .Setup(p => p.Verify(command.Senha, conta.Senha))
                .Returns(true);

            _jwtServiceMock
                .Setup(j => j.GenerateToken(conta.Id))
                .Returns(token);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(token, result.Data);
        }
    }
}

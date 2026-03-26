using BankMore.Domain.Enums;
using ContaCorrente.API.Application.Handlers;
using ContaCorrente.API.Application.Queries;
using ContaCorrente.API.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace ContaCorrente.Tests
{
    public class ObterContaPorIdHandlerTests
    {
        private readonly Mock<IContaCorrenteRepository> _repositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextMock;

        private readonly ObterContaPorIdHandler _handler;

        public ObterContaPorIdHandlerTests()
        {
            _repositoryMock = new Mock<IContaCorrenteRepository>();
            _httpContextMock = new Mock<IHttpContextAccessor>();

            _handler = new ObterContaPorIdHandler(
                _repositoryMock.Object,
                _httpContextMock.Object);
        }

        [Fact]
        public async Task Deve_Retornar_Erro_Quando_Token_Invalido()
        {
            _httpContextMock
                .Setup(x => x.HttpContext)
                .Returns(new DefaultHttpContext());

            var query = new ObterContaPorIdQuery();

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.USER_UNAUTHORIZED.ToString(), result.ErrorType);
        }

        [Fact]
        public async Task Deve_Retornar_Erro_Quando_Conta_Nao_Encontrada()
        {
            var accountId = "123";

            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("accountId", accountId)
            }));

            _httpContextMock
                .Setup(x => x.HttpContext)
                .Returns(context);

            _repositoryMock
                .Setup(r => r.ObterPorId(accountId))
                .ReturnsAsync((API.Domain.Entities.ContaCorrente?)null);

            var result = await _handler.Handle(new ObterContaPorIdQuery(), CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.INVALID_ACCOUNT.ToString(), result.ErrorType);
        }

        [Fact]
        public async Task Deve_Retornar_Conta_Quando_Valida()
        {
            var accountId = "123";

            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("accountId", accountId)
            }));

            _httpContextMock
                .Setup(x => x.HttpContext)
                .Returns(context);

            var conta = new API.Domain.Entities.ContaCorrente("12345678909", "senha");

            _repositoryMock
                .Setup(r => r.ObterPorId(accountId))
                .ReturnsAsync(conta);

            var result = await _handler.Handle(new ObterContaPorIdQuery(), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }
    }
}

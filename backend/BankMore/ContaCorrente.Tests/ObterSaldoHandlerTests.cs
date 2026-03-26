using BankMore.Domain.Enums;
using ContaCorrente.API.Application.Handlers;
using ContaCorrente.API.Application.Queries;
using ContaCorrente.API.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace ContaCorrente.Tests
{
    public class ObterSaldoHandlerTests
    {
        private readonly Mock<IContaCorrenteRepository> _contaRepositoryMock;
        private readonly Mock<IMovimentoRepository> _movimentoRepositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextMock;

        private readonly ObterSaldoHandler _handler;

        public ObterSaldoHandlerTests()
        {
            _contaRepositoryMock = new Mock<IContaCorrenteRepository>();
            _movimentoRepositoryMock = new Mock<IMovimentoRepository>();
            _httpContextMock = new Mock<IHttpContextAccessor>();

            _handler = new ObterSaldoHandler(
                _contaRepositoryMock.Object,
                _movimentoRepositoryMock.Object,
                _httpContextMock.Object);
        }

        [Fact]
        public async Task Deve_Retornar_Erro_Quando_Token_Invalido()
        {
            _httpContextMock
                .Setup(x => x.HttpContext)
                .Returns(new DefaultHttpContext());

            var result = await _handler.Handle(new ObterSaldoQuery(), CancellationToken.None);

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

            _contaRepositoryMock
                .Setup(r => r.ObterPorId(accountId))
                .ReturnsAsync((API.Domain.Entities.ContaCorrente?)null);

            var result = await _handler.Handle(new ObterSaldoQuery(), CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.INVALID_ACCOUNT.ToString(), result.ErrorType);
        }

        [Fact]
        public async Task Deve_Retornar_Erro_Quando_Conta_Inativa()
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
            
            typeof(API.Domain.Entities.ContaCorrente)
                .GetProperty("Ativo")?
                .SetValue(conta, false);

            _contaRepositoryMock
                .Setup(r => r.ObterPorId(accountId))
                .ReturnsAsync(conta);

            var result = await _handler.Handle(new ObterSaldoQuery(), CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.INACTIVE_ACCOUNT.ToString(), result.ErrorType);
        }

        [Fact]
        public async Task Deve_Retornar_Saldo_Quando_Conta_Valida()
        {
            var accountId = "123";
            var saldo = 250m;

            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("accountId", accountId)
            }));

            _httpContextMock
                .Setup(x => x.HttpContext)
                .Returns(context);

            var conta = new API.Domain.Entities.ContaCorrente("12345678909", "senha");

            _contaRepositoryMock
                .Setup(r => r.ObterPorId(accountId))
                .ReturnsAsync(conta);

            _movimentoRepositoryMock
                .Setup(m => m.ObterSaldo(conta.Id))
                .ReturnsAsync(saldo);

            var result = await _handler.Handle(new ObterSaldoQuery(), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(saldo, result.Data.Saldo);
        }
    }
}

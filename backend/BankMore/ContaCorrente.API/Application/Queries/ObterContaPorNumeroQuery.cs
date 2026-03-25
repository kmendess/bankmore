using BankMore.Application.Models;
using MediatR;

namespace ContaCorrente.API.Application.Queries
{
    public class ObterContaPorNumeroQuery : IRequest<Response<ContaCorrenteResponse?>>
    {
        public int NumeroConta { get; private set; }

        public ObterContaPorNumeroQuery(int numeroConta)
        {
            NumeroConta = numeroConta;
        }
    }
}

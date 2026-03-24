using BankMore.Application.Models;
using MediatR;

namespace ContaCorrente.API.Application.Queries
{
    public class ObterSaldoQuery : IRequest<Response<SaldoResponse>>
    {
    }
}

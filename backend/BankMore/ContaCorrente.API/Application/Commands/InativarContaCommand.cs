using BankMore.Application.Models;
using MediatR;

namespace ContaCorrente.API.Application.Commands
{
    public class InativarContaCommand : IRequest<Response>
    {
        public string Senha { get; set; }
    }
}

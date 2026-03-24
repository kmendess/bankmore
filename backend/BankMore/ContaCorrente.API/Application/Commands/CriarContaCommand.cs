using BankMore.Application.Models;
using MediatR;

namespace ContaCorrente.API.Application.Commands
{
    public class CriarContaCommand : IRequest<Response<int>>
    {
        public string Cpf { get; set; }
        public string Senha { get; set; }
    }
}

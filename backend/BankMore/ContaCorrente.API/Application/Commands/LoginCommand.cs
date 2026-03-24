using BankMore.Application.Models;
using MediatR;

namespace ContaCorrente.API.Application.Commands
{
    public class LoginCommand : IRequest<Response<string>>
    {
        public string Cpf { get; set; }
        public int? NumeroConta { get; set; }
        public string Senha { get; set; }
    }
}

using BankMore.Application.Models;
using MediatR;

namespace ContaCorrente.API.Application.Commands
{
    public class LoginCommand : IRequest<Result<string>>
    {
        public string Cpf { get; set; }
        public int? NumeroConta { get; set; }
        public string Senha { get; set; }
    }
}

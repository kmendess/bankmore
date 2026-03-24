using BankMore.Application.Models;
using MediatR;

namespace ContaCorrente.API.Application.Commands
{
    public class MovimentarContaCommand : IRequest<Response>
    {
        public string Id { get; set; }
        public int? NumeroConta { get; set; }
        public decimal Valor { get; set; }
        public string Tipo { get; set; }
    }
}

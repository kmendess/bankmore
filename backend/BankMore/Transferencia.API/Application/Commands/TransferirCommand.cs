using BankMore.Application.Models;
using MediatR;

namespace Transferencia.API.Application.Commands
{
    public class TransferirCommand : IRequest<Response>
    {
        public string Id { get; set; }
        public int NumeroContaDestino { get; set; }
        public decimal Valor { get; set; }
    }
}

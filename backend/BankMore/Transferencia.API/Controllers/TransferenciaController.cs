using BankMore.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Transferencia.API.Application.Commands;

namespace Transferencia.API.Controllers
{
    [ApiController]
    [Route("api/transferencia")]
    public class TransferenciaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransferenciaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Transferir([FromBody] TransferirCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                if (result.ErrorType == ErrorType.USER_UNAUTHORIZED.ToString())
                    return Forbid();

                return BadRequest(new { result.Message, result.ErrorType });
            }

            return NoContent();
        }
    }
}

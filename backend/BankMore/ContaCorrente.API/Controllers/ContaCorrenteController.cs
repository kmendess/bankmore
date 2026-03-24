using ContaCorrente.API.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContaCorrente.API.Controllers
{
    [ApiController]
    [Route("api/conta")]
    public class ContaCorrenteController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContaCorrenteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarContaCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { result.Message, result.ErrorType });

            return Ok(new { numeroConta = result.Data });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return Unauthorized(new { result.Message, result.ErrorType });

            return Ok(new { token = result.Data });
        }
    }
}

using BankMore.Domain.Enums;
using ContaCorrente.API.Application.Commands;
using ContaCorrente.API.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPut("inativar")]
        [Authorize]
        public async Task<IActionResult> Inativar([FromBody] InativarContaCommand command)
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

        [HttpPost("movimentar")]
        [Authorize]
        public async Task<IActionResult> Movimentar([FromBody] MovimentarContaCommand command)
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

        [HttpGet("saldo")]
        [Authorize]
        public async Task<IActionResult> ObterSaldo()
        {
            var result = await _mediator.Send(new ObterSaldoQuery());

            if (!result.IsSuccess)
            {
                if (result.ErrorType == ErrorType.USER_UNAUTHORIZED.ToString())
                    return Forbid();

                return BadRequest(new { result.Message, result.ErrorType });
            }

            return Ok(result.Data);
        }
    }
}

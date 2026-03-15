using CRM.Application.Common.Models;
using CRM.Application.DTOs.Auth;
using CRM.Application.Features.Auth.Commands.ForgotPassword;
using CRM.Application.Features.Auth.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.WebAPI.Controllers;

/// <summary>
/// Controlador de autenticación y gestión de sesiones.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Auth")]
[Produces("application/json")]
public class AuthController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Autentica un usuario y retorna tokens de acceso.
    /// </summary>
    /// <param name="request">Credenciales del usuario (email y contraseña).</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Token de acceso, refresh token y datos del usuario.</returns>
    /// <response code="200">Autenticación exitosa.</response>
    /// <response code="401">Credenciales inválidas o cuenta inactiva.</response>
    /// <response code="400">Datos de entrada inválidos.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken ct)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await sender.Send(command, ct);

        if (!result.IsSuccess)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Authentication failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Solicita un restablecimiento de contraseña enviando instrucciones al correo.
    /// </summary>
    /// <param name="request">Correo electrónico del usuario.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Confirmación de que las instrucciones fueron enviadas.</returns>
    /// <response code="200">Instrucciones de recuperación enviadas (si el correo existe).</response>
    /// <response code="400">Datos de entrada inválidos.</response>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request, CancellationToken ct)
    {
        var command = new ForgotPasswordCommand(request.Email);
        var result = await sender.Send(command, ct);
        return Ok(result);
    }
}

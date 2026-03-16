using CRM.Application.Common.Models;
using CRM.Application.DTOs.Users;
using CRM.Application.Features.Users.Commands.ChangeUserRole;
using CRM.Application.Features.Users.Commands.CreateUser;
using CRM.Application.Features.Users.Commands.ToggleUserStatus;
using CRM.Application.Features.Users.Commands.UpdateUser;
using CRM.Application.Features.Users.Queries.GetUserById;
using CRM.Application.Features.Users.Queries.GetUsers;
using CRM.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.WebAPI.Controllers;

/// <summary>
/// Controlador de gestión de usuarios. Requiere autenticación y permisos específicos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Users")]
[Produces("application/json")]
[Authorize]
public class UsersController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Obtiene la lista paginada de usuarios.
    /// </summary>
    /// <param name="page">Número de página (default: 1).</param>
    /// <param name="pageSize">Tamaño de página (default: 10).</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Lista paginada de usuarios.</returns>
    /// <response code="200">Lista de usuarios obtenida exitosamente.</response>
    /// <response code="401">No autenticado.</response>
    /// <response code="403">No autorizado.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserManagementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "users", "read" })]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);
        var result = await sender.Send(new GetUsersQuery(page, pageSize), ct);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene un usuario por su identificador.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Datos del usuario.</returns>
    /// <response code="200">Usuario encontrado.</response>
    /// <response code="404">Usuario no encontrado.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserManagementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "users", "read" })]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetUserByIdQuery(id), ct);

        if (!result.IsSuccess)
        {
            return NotFound(new ProblemDetails
            {
                Title = "User not found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Crea un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="request">Datos del nuevo usuario.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Identificador del usuario creado.</returns>
    /// <response code="201">Usuario creado exitosamente.</response>
    /// <response code="400">Datos inválidos o email duplicado.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "users", "create" })]
    public async Task<IActionResult> Create([FromBody] CreateUserRequestDto request, CancellationToken ct)
    {
        var command = new CreateUserCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            request.RoleId
        );

        var result = await sender.Send(command, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Create user failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Actualiza los datos de un usuario existente.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="request">Datos actualizados del usuario.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Confirmación de actualización.</returns>
    /// <response code="204">Usuario actualizado exitosamente.</response>
    /// <response code="400">Datos inválidos o restricción de negocio.</response>
    /// <response code="404">Usuario no encontrado.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "users", "update" })]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequestDto request, CancellationToken ct)
    {
        var command = new UpdateUserCommand(
            id,
            request.FirstName,
            request.LastName,
            request.Email,
            request.RoleId,
            request.IsActive
        );

        var result = await sender.Send(command, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Update user failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Activa o desactiva un usuario.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="isActive">Nuevo estado del usuario.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Confirmación del cambio de estado.</returns>
    /// <response code="204">Estado actualizado exitosamente.</response>
    /// <response code="400">Restricción de negocio (ej: último admin).</response>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "users", "update" })]
    public async Task<IActionResult> ToggleStatus(Guid id, [FromBody] bool isActive, CancellationToken ct)
    {
        var result = await sender.Send(new ToggleUserStatusCommand(id, isActive), ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Toggle status failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Cambia el rol de un usuario.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="roleId">Identificador del nuevo rol.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Confirmación del cambio de rol.</returns>
    /// <response code="204">Rol actualizado exitosamente.</response>
    /// <response code="400">Rol inválido o restricción de negocio.</response>
    [HttpPatch("{id:guid}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "users", "update" })]
    public async Task<IActionResult> ChangeRole(Guid id, [FromBody] Guid roleId, CancellationToken ct)
    {
        var result = await sender.Send(new ChangeUserRoleCommand(id, roleId), ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Change role failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return NoContent();
    }
}

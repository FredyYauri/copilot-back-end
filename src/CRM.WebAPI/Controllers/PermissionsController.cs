using CRM.Application.Common.Models;
using CRM.Application.DTOs.Permissions;
using CRM.Application.DTOs.Roles;
using CRM.Application.Features.Permissions.Commands.CreatePermission;
using CRM.Application.Features.Permissions.Commands.TogglePermissionStatus;
using CRM.Application.Features.Permissions.Commands.UpdatePermission;
using CRM.Application.Features.Permissions.Queries.GetPermissionsPaged;
using CRM.Application.Features.Roles.Queries.GetPermissions;
using CRM.Application.Features.Roles.Queries.GetUserPermissions;
using CRM.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.WebAPI.Controllers;

/// <summary>
/// Controlador de permisos. Permite consultar, crear, actualizar y gestionar permisos del sistema.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Permissions")]
[Produces("application/json")]
[Authorize]
public class PermissionsController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Obtiene todos los permisos del sistema agrupados por recurso.
    /// </summary>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Permisos agrupados por recurso.</returns>
    /// <response code="200">Lista de permisos obtenida exitosamente.</response>
    [HttpGet("grouped")]
    [ProducesResponseType(typeof(IEnumerable<PermissionGroupDto>), StatusCodes.Status200OK)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "roles", "read" })]
    public async Task<IActionResult> GetAllGrouped(CancellationToken ct)
    {
        var result = await sender.Send(new GetPermissionsQuery(), ct);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene los permisos paginados para gestión.
    /// </summary>
    /// <param name="page">Número de página.</param>
    /// <param name="pageSize">Tamaño de página.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Permisos paginados.</returns>
    /// <response code="200">Lista paginada de permisos obtenida exitosamente.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PermissionManagementDto>), StatusCodes.Status200OK)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "permissions", "read" })]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);
        var result = await sender.Send(new GetPermissionsPagedQuery(page, pageSize), ct);
        return Ok(result);
    }

    /// <summary>
    /// Crea un nuevo permiso en el sistema.
    /// </summary>
    /// <param name="request">Datos del nuevo permiso.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Identificador del permiso creado.</returns>
    /// <response code="201">Permiso creado exitosamente.</response>
    /// <response code="400">Datos inválidos o permiso duplicado.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "permissions", "create" })]
    public async Task<IActionResult> Create([FromBody] CreatePermissionRequestDto request, CancellationToken ct)
    {
        var command = new CreatePermissionCommand(request.Resource, request.Action, request.Description, request.Type);
        var result = await sender.Send(command, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Create permission failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return Created(string.Empty, result.Value);
    }

    /// <summary>
    /// Actualiza un permiso existente.
    /// </summary>
    /// <param name="id">Identificador del permiso.</param>
    /// <param name="request">Datos actualizados del permiso.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Confirmación de actualización.</returns>
    /// <response code="204">Permiso actualizado exitosamente.</response>
    /// <response code="400">Datos inválidos o permiso duplicado.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "permissions", "update" })]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePermissionRequestDto request, CancellationToken ct)
    {
        var command = new UpdatePermissionCommand(id, request.Resource, request.Action, request.Description, request.Type);
        var result = await sender.Send(command, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Update permission failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Activa o desactiva un permiso.
    /// </summary>
    /// <param name="id">Identificador del permiso.</param>
    /// <param name="isActive">Estado deseado.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Confirmación de cambio de estado.</returns>
    /// <response code="204">Estado del permiso actualizado exitosamente.</response>
    /// <response code="400">Permiso no encontrado.</response>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "permissions", "update" })]
    public async Task<IActionResult> ToggleStatus(Guid id, [FromQuery] bool isActive, CancellationToken ct)
    {
        var command = new TogglePermissionStatusCommand(id, isActive);
        var result = await sender.Send(command, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Toggle permission status failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Obtiene los permisos de un usuario basado en su rol.
    /// </summary>
    /// <param name="userId">Identificador del usuario.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Permisos del usuario.</returns>
    /// <response code="200">Permisos del usuario obtenidos exitosamente.</response>
    /// <response code="404">Usuario no encontrado.</response>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(UserPermissionsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "users", "read" })]
    public async Task<IActionResult> GetUserPermissions(Guid userId, CancellationToken ct)
    {
        var result = await sender.Send(new GetUserPermissionsQuery(userId), ct);

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
}

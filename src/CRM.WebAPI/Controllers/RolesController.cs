using CRM.Application.Common.Models;
using CRM.Application.DTOs.Roles;
using CRM.Application.Features.Roles.Commands.AssignPermissions;
using CRM.Application.Features.Roles.Commands.CreateRole;
using CRM.Application.Features.Roles.Commands.UpdateRole;
using CRM.Application.Features.Roles.Queries.GetRoleById;
using CRM.Application.Features.Roles.Queries.GetRoles;
using CRM.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.WebAPI.Controllers;

/// <summary>
/// Controlador de gestión de roles. Requiere autenticación y permisos específicos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Roles")]
[Produces("application/json")]
[Authorize]
public class RolesController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Obtiene la lista paginada de roles.
    /// </summary>
    /// <param name="page">Número de página (default: 1).</param>
    /// <param name="pageSize">Tamaño de página (default: 10).</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Lista paginada de roles.</returns>
    /// <response code="200">Lista de roles obtenida exitosamente.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "roles", "read" })]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetRolesQuery(page, pageSize), ct);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene un rol por su identificador, incluyendo sus permisos.
    /// </summary>
    /// <param name="id">Identificador del rol.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Detalle del rol con permisos.</returns>
    /// <response code="200">Rol encontrado.</response>
    /// <response code="404">Rol no encontrado.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RoleDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "roles", "read" })]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetRoleByIdQuery(id), ct);

        if (!result.IsSuccess)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Role not found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Crea un nuevo rol en el sistema.
    /// </summary>
    /// <param name="request">Datos del nuevo rol.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Identificador del rol creado.</returns>
    /// <response code="201">Rol creado exitosamente.</response>
    /// <response code="400">Datos inválidos o nombre duplicado.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "roles", "create" })]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequestDto request, CancellationToken ct)
    {
        var command = new CreateRoleCommand(request.Name, request.Description);
        var result = await sender.Send(command, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Create role failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Actualiza los datos de un rol existente.
    /// </summary>
    /// <param name="id">Identificador del rol.</param>
    /// <param name="request">Datos actualizados del rol.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Confirmación de actualización.</returns>
    /// <response code="204">Rol actualizado exitosamente.</response>
    /// <response code="400">Datos inválidos o restricción de negocio.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "roles", "update" })]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleRequestDto request, CancellationToken ct)
    {
        var command = new UpdateRoleCommand(id, request.Name, request.Description, request.IsActive);
        var result = await sender.Send(command, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Update role failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Asigna permisos a un rol, reemplazando los existentes.
    /// </summary>
    /// <param name="id">Identificador del rol.</param>
    /// <param name="request">Lista de identificadores de permisos.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Confirmación de asignación.</returns>
    /// <response code="204">Permisos asignados exitosamente.</response>
    /// <response code="400">Datos inválidos.</response>
    [HttpPut("{id:guid}/permissions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [TypeFilter(typeof(PermissionAuthorizationFilter), Arguments = new object[] { "roles", "assign" })]
    public async Task<IActionResult> AssignPermissions(Guid id, [FromBody] AssignPermissionsRequestDto request, CancellationToken ct)
    {
        var permissionGuids = request.PermissionIds.Select(Guid.Parse);
        var command = new AssignPermissionsCommand(id, permissionGuids);
        var result = await sender.Send(command, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Assign permissions failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return NoContent();
    }
}

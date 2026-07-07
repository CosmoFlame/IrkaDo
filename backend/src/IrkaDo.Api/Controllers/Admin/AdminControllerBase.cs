using IrkaDo.Api.Auth;
using Microsoft.AspNetCore.Mvc;

namespace IrkaDo.Api.Controllers.Admin;

/// <summary>
/// Base for every authenticated admin content controller. Applying the auth filter here means
/// each concrete controller only declares its route and actions.
/// </summary>
[ApiController]
[AdminAuthorize]
public abstract class AdminControllerBase : ControllerBase
{
}

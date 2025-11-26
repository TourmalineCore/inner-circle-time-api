using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Name;

[Authorize]
[ApiController]
[Route("api/name")]
public class NameController : ControllerBase
{

}

using AiSqlAssistant.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiSqlAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueryController(AiSqlService aiService) : ControllerBase
{
    public record RequestDto(string Question);

    [HttpPost]
    public async Task<IActionResult> Ask([FromBody] RequestDto request)
    {
        var result = await aiService.AskDatabaseAsync(request.Question);
        return Ok(result);
    }
}
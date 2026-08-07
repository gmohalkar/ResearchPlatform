using Hangfire;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("run-job")]
public IActionResult RunJob()
{
    BackgroundJob.Enqueue<CleanupJob>(
        x => x.Execute());

    return Ok(
        "Job Queued");
}
}
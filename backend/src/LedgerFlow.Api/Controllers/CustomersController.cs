using Microsoft.AspNetCore.Mvc;

namespace LedgerFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(new[]
        {
            new
            {
                id = Guid.NewGuid(),
                companyName = "Acme Consulting LLC",
                contactName = "John Smith",
                email = "john@acme.com",
                phone = "512-555-0100",
                city = "Austin",
                state = "TX",
                country = "US",
                isActive = true,
                createdAtUtc = DateTime.UtcNow
            }
        });
    }
}
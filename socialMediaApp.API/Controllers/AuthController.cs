using System.Threading.Tasks;
using datingApp.API.Data;
using Microsoft.AspNetCore.Mvc;

namespace datingApp.API.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class AuthController
  {
    private readonly IAuthRepository repo;
    public AuthController(IAuthRepository _repo)
    {
     _repo = repo;

    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(string username, string password)
    {

//validate Req
    }
  }
}
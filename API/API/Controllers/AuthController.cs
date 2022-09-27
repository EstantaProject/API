using API.Database;
using API.Dto;
using API.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Paseto;
using Paseto.Builder;
using BC = BCrypt.Net.BCrypt;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : Controller
{
    private readonly PasetoProvider _pasetoProvider;

    public AuthController(PasetoProvider pasetoProvider)
    {
        _pasetoProvider = pasetoProvider;
    }
    
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        User? user;
        await using (var db = new Context())
        {
            user = await db.Users.FirstOrDefaultAsync(w =>
                w.UserName == model.Username);

            if (user == null)
                return Unauthorized();

            if (!BC.Verify(model.Password, user.PasswordHash))
                return Unauthorized();

            var existingSessions = await db.Sessions
                .Where(w => w.UserId == user.Id)
                .ToListAsync();
            db.Sessions.RemoveRange(existingSessions);

            var key = new PasetoBuilder()
                .Use("v1", Purpose.Local)
                .GenerateSymmetricKey();

            var newSession = new Session
            {
                Key = key.Key.ToArray(),
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddHours(1)
            };
            await db.Sessions.AddAsync(newSession);
            await db.SaveChangesAsync();
            
            return Ok(_pasetoProvider.GenerateAccessToken(user, key));
        }
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        var hashedPassword = BC.HashPassword(model.Password);
        
        var newUser = new User
        {
            UserName = model.Username,
            PasswordHash = hashedPassword
        };

        await using (var db = new Context())
        {
            db.Users.Add(newUser);
            await db.SaveChangesAsync();
        }
        
        return Ok();
    }
}
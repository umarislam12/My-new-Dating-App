using System;
using System.Threading.Tasks;
using datingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace datingApp.API.Data
{
  public class AuthRepository : IAuthRepository
  {
    private readonly DataContext _context;
    public AuthRepository(DataContext context)
    {
      _context = context;

    }
    public async Task<User> Login(string username, string password)
    {
      //firstOrDefaul returns null in case it doesnt find value
      var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
      if (user == null)
        return null;

      if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        return null;
      return user;
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
// adding Salt so that it can be compared with the sored password
      using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
      {

        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        for (int i = 0; i < computedHash.Length; i++)
        {
          if (computedHash[i] != passwordHash[i])
            return false;
        }
      }
      return true;
    }

    public async Task<User> Register(User user, string password)
    {
      //defining variables of typee byte arrays
      byte[] passwordHash, passwordSalt;

      CreatePasswordHash(password, out passwordHash, out passwordSalt);
      user.PasswordHash = passwordHash;
      user.PasswordSalt = passwordSalt;
      await _context.Users.AddAsync(user);
      await _context.SaveChangesAsync();
      return user;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
      //creating instance and "using " is used to use to enable dispose method
      //we need to dispose everything inside curly braces after using
      using (var hmac = new System.Security.Cryptography.HMACSHA512())
      {
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
      }
    }

    public async Task<bool> UserExist(string username)
    {
      if (await _context.Users.AnyAsync(x => x.Username == username))
        return true;
      return false;


    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthJwt.Models;
using BikeVille.CriptingDecripting;
using Microsoft.AspNetCore.Authorization;

namespace AuthJwt.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AdventureWorksLt2019usersInfoContext _context;

        public UsersController(AdventureWorksLt2019usersInfoContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "ADMIN")]
        // GET: api/Users
        [HttpGet("Index")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
        [Authorize(Roles = "USER")]
        // GET: api/Users/5
        [HttpGet("Details/{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }


        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register(UserDto userDto)
        {
            KeyValuePair<string, string> passHashSalt = SaltEncrypt.SaltEncryptPass(userDto.Password);
            
            var user = new User()
            {
                FirstName = userDto.FirstName,
                MiddleName = userDto.MiddleName,
                LastName = userDto.LastName,
                Suffix = userDto.Suffix,
                EmailAddress = userDto.EmailAddress,
                Phone = userDto.Phone,
                PasswordHash = passHashSalt.Key,
                PasswordSalt = passHashSalt.Value,
                Role = userDto.Role,
                Rowguid = Guid.NewGuid(),

            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

      

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}

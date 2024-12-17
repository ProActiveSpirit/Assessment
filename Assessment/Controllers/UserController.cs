using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Assessment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager; 
        }

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <returns>List of users.</returns>
        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<IdentityUser>>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        /// <summary>
        /// Get a user by ID.
        /// </summary>
        /// <param name="email">The ID of the user.</param>
        /// <returns>The user details.</returns>
        [HttpGet("{email}")]
        public async Task<ActionResult<IdentityUser>> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"User with ID '{email}' not found.");
            }

            return Ok(user);
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="user">The user details.</param>
        /// <returns>Result of user creation.</returns>
        [HttpPost("add")]
        public async Task<ActionResult> CreateUser([FromBody] IdentityUser user)
        {
            var result = await _userManager.CreateAsync(user, "DefaultPassword123!"); // Replace with your logic for password

            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(GetUserByEmail), new { Email = user.Email }, user);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing user.
        /// </summary>
        /// <param name="Email">The ID of the user to update.</param>
        /// <param name="updatedUser">The updated user details.</param>
        /// <returns>Result of the update operation.</returns>
        [HttpPut("update")]
        public async Task<ActionResult> UpdateUser(string Email, [FromBody] IdentityUser updatedUser)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return NotFound($"User with Email '{Email}' not found.");
            }

            user.Email = updatedUser.Email;
            user.UserName = updatedUser.UserName;
            user.PhoneNumber = updatedUser.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return NoContent();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Delete a user by Email.
        /// </summary>
        /// <param name="Email">The Email of the user to delete.</param>
        /// <returns>Result of the delete operation.</returns>
        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteUser(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return NotFound($"User with Email ' {Email}' not found.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return NoContent();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }
    }
}
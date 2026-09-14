using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilter;
using NZWalks.API.Models.DTO;
using NZWalks.API.Respositries;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;
        private readonly ILogger<AuthController> logger;
        private readonly IWebHostEnvironment environment;

        public AuthController(UserManager<IdentityUser> userManager,
            ITokenRepository tokenRepository,
            ILogger<AuthController> logger,
            IWebHostEnvironment environment)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
            this.logger = logger;
            this.environment = environment;
        }


        //Post: /api/Auth/Register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            if (registerRequestDto.Roles == null || !registerRequestDto.Roles.Any())
            {
                return BadRequest("At least one role must be supplied.");
            }

            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.Email,
                Email = registerRequestDto.Email
            };

            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);

            if (!identityResult.Succeeded)
            {
                logger.LogWarning("Registration failed for {Email}: {Errors}",
                    registerRequestDto.Email,
                    string.Join("; ", identityResult.Errors.Select(e => e.Description)));

                return BadRequest(identityResult.Errors.Select(e => e.Description));
            }

            var roleResult = await userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);

            if (!roleResult.Succeeded)
            {
                // The user row already exists at this point - remove it so we do not
                // leave behind an account with no roles, which can never log in.
                await userManager.DeleteAsync(identityUser);

                logger.LogWarning("Role assignment failed for {Email}: {Errors}",
                    registerRequestDto.Email,
                    string.Join("; ", roleResult.Errors.Select(e => e.Description)));

                return BadRequest(roleResult.Errors.Select(e => e.Description));
            }

            return Ok("User was registered successfully.");
        }

        //Post: /api/Auth/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LogInRequestDto logInRequestDto)
        {
            var user = await userManager.FindByEmailAsync(logInRequestDto.Email);

            if (user == null)
            {
                logger.LogWarning("Login failed: no user found for {Email}", logInRequestDto.Email);
                return BadRequest("Username or password incorrect!");
            }

            var checkPasswordResult = await userManager.CheckPasswordAsync(user, logInRequestDto.Password);

            if (!checkPasswordResult)
            {
                logger.LogWarning("Login failed: wrong password for {Email}", logInRequestDto.Email);
                return BadRequest("Username or password incorrect!");
            }

            var roles = await userManager.GetRolesAsync(user);

            if (roles == null || !roles.Any())
            {
                logger.LogWarning("Login failed: {Email} has no roles assigned", logInRequestDto.Email);
                return BadRequest("This user has no roles assigned.");
            }

            var jwtToken = tokenRepository.CreateToken(user, roles.ToList());

            return Ok(new LoginResponseDto
            {
                JwtToken = jwtToken
            });
        }

        //Post: /api/Auth/ForgotPassword
        [HttpPost]
        [Route("ForgotPassword")]
        [ValidateModel]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto forgotPasswordRequestDto)
        {
            // Always the same answer, whether or not the email exists. Returning
            // "no such user" here would let anyone probe which emails have accounts.
            const string genericResponse =
                "If that email is registered, a password reset token has been issued.";

            var user = await userManager.FindByEmailAsync(forgotPasswordRequestDto.Email);

            if (user == null)
            {
                logger.LogWarning("Password reset requested for unknown email {Email}",
                    forgotPasswordRequestDto.Email);

                return Ok(genericResponse);
            }

            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

            logger.LogInformation("Password reset token issued for {Email}",
                forgotPasswordRequestDto.Email);

            // DEVELOPMENT ONLY. In production this token must be emailed to the user,
            // never returned in the response - anyone who can call this endpoint could
            // otherwise reset any account they know the email address of.
            if (environment.IsDevelopment())
            {
                return Ok(new
                {
                    Message = genericResponse,
                    ResetToken = resetToken
                });
            }

            return Ok(genericResponse);
        }

        //Post: /api/Auth/ResetPassword
        [HttpPost]
        [Route("ResetPassword")]
        [ValidateModel]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
        {
            var user = await userManager.FindByEmailAsync(resetPasswordRequestDto.Email);

            if (user == null)
            {
                logger.LogWarning("Password reset attempted for unknown email {Email}",
                    resetPasswordRequestDto.Email);

                return BadRequest("Password reset failed.");
            }

            var resetResult = await userManager.ResetPasswordAsync(
                user,
                resetPasswordRequestDto.Token,
                resetPasswordRequestDto.NewPassword);

            if (!resetResult.Succeeded)
            {
                logger.LogWarning("Password reset failed for {Email}: {Errors}",
                    resetPasswordRequestDto.Email,
                    string.Join("; ", resetResult.Errors.Select(e => e.Description)));

                return BadRequest(resetResult.Errors.Select(e => e.Description));
            }

            logger.LogInformation("Password reset succeeded for {Email}",
                resetPasswordRequestDto.Email);

            return Ok("Password has been reset. You can now log in with the new password.");
        }
    }
}

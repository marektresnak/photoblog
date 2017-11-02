using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Photoblog.Utils.Extensions;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Photoblog.Controllers {
    public class TokenController : Controller {

        readonly Settings _settings;
        readonly ILogger<TokenController> _logger;

        public TokenController(IOptions<Settings> optionsAccessor, ILogger<TokenController> logger) {
            _settings = optionsAccessor.Value;
            _logger = logger;
        }

        [HttpPost("/api/token")]
        [AllowAnonymous]
        public IActionResult GetToken(string userName, string password) {
            var user = GetAuthenticatedUser(userName, password);

            if (user == null) {
                _logger.LogInformation("Unauthorized attempt to generate access token with username [{username}]", userName);
                return Unauthorized();
            }

            return new ObjectResult(new {
                Token = GenerateToken(user)
            });
        }

        AppUser GetAuthenticatedUser(string userName, string password) {
            var user = _settings.Users.FirstOrDefault(u => u.UserName == userName);
            var passwordHasher = new PasswordHasher<AppUser>();

            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, password);
            if (verificationResult == PasswordVerificationResult.Success || verificationResult == PasswordVerificationResult.SuccessRehashNeeded) {
                if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded) {
                    // It's unlikely that the Identity lib referenced in this project will ever be updated with a new hashing method
                    // But let's just log it and surprise myself
                    _logger.LogInformation("Passsword for user [{username}] needs to be re-hashed", userName);
                }

                return user;
            }

            return null;
        }

        string GenerateToken(AppUser user) {
            var claims = new Claim[] {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToUnixTimestamp().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddDays(30).ToUnixTimestamp().ToString()),
            };

            var token = new JwtSecurityToken(
                new JwtHeader(new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_settings.AppSecret)), SecurityAlgorithms.HmacSha256)),
                new JwtPayload(claims));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}

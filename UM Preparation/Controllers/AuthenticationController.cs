using Application.DTO.User;
using Application.Features.Users.Commands;
using Application.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UM_Preparation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO registerDTO)
        {
            var result = await _mediator.Send(new RegisterUserCommand(registerDTO));

            if (result.IsFailure)
            {
                return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
            }

            return CreatedAtAction(nameof(UserController.GetUserById), "User", new { id = result.Value }, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromQuery] RefreshTokenDTO? refreshTokenDTO, [FromBody] LoginUserDTO loginDTO)
        {
            var refreshToken = Request.Cookies["refresh_token"] ?? (refreshTokenDTO?.RefreshToken);

            var result = await _mediator.Send(new LoginUserCommand(loginDTO, refreshToken));

            if (result.IsSuccess && result.Value != null)
            {
                SetAuthCookies(result.Value.AccessToken, result.Value.RefreshToken);
            }

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromQuery] RefreshTokenDTO? refreshTokenDTO)
        {
            var refreshToken = Request.Cookies["refresh_token"] ?? (refreshTokenDTO?.RefreshToken);

            if (string.IsNullOrEmpty(refreshToken))
            {
                var missingTokenResult = Result<LoginResultDTO>.Failure("Refresh token is missing.", HttpStatusCode.Unauthorized);

                return StatusCode(missingTokenResult.StatusCode!.Value.GetHashCode(), missingTokenResult);
            }

            var result = await _mediator.Send(new RefreshTokenCommand(refreshToken));

            if (result.IsFailure)
            {
                return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
            }

            SetAuthCookies(result.Value!.AccessToken, result.Value.RefreshToken);

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromQuery] RefreshTokenDTO? refreshTokenDTO)
        {
            var refreshToken = Request.Cookies["refresh_token"] ?? (refreshTokenDTO?.RefreshToken);

            if (string.IsNullOrEmpty(refreshToken))
            {
                var missingTokenResult = Result<object>.Failure("Refresh token is missing.", HttpStatusCode.Unauthorized);

                return StatusCode(missingTokenResult.StatusCode!.Value.GetHashCode(), missingTokenResult);
            }

            var result = await _mediator.Send(new LogoutUserCommand(refreshToken));

            var cookieOptions = new CookieOptions
            {
                SameSite = SameSiteMode.None,
                Secure = true
            };


            Response.Cookies.Delete("access_token", cookieOptions);
            Response.Cookies.Delete("refresh_token", cookieOptions);

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        private void SetAuthCookies(string accessToken, string refreshToken)
        {
            Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            });

            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
        }
    }
}

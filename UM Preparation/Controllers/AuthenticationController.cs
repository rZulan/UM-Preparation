using Application.DTO.User;
using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using Application.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UM_Preparation.Extensions;

namespace UM_Preparation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IMediator mediator, IConfiguration configuration) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IMediator _mediator = mediator;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            Result<object> result = await _mediator.Send(new RegisterUserCommand(registerDto));

            if (result.IsFailure)
            {
                return StatusCode((int)result.StatusCode!.Value, result);
            }

            return CreatedAtAction(nameof(UserController.GetUserById), "User", new { id = result.Value }, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromQuery] RefreshTokenDto? refreshTokenDto,
            [FromBody] LoginUserDto loginDto)
        {
            string? refreshToken = Request.Cookies["refresh_token"] ?? refreshTokenDto?.RefreshToken;

            Result<LoginResultDto> result = await _mediator.Send(new LoginUserCommand(loginDto, refreshToken));

            if (result.IsSuccess && result.Value != null)
            {
                SetAuthCookies(result.Value.AccessToken, result.Value.RefreshToken);
            }

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromQuery] RefreshTokenDto? refreshTokenDto)
        {
            string? refreshToken = Request.Cookies["refresh_token"] ?? refreshTokenDto?.RefreshToken;

            if (string.IsNullOrEmpty(refreshToken))
            {
                Result<LoginResultDto> missingTokenResult =
                    Result<LoginResultDto>.Failure("Refresh token is missing.", HttpStatusCode.Unauthorized);

                return StatusCode((int)missingTokenResult.StatusCode!.Value, missingTokenResult);
            }

            Result<RefreshResultDto> result = await _mediator.Send(new RefreshTokenCommand(refreshToken));

            if (result.IsFailure)
            {
                return StatusCode((int)result.StatusCode!.Value, result);
            }

            SetAuthCookies(result.Value!.AccessToken, null);

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            int? userId = this.GetCurrentUserId();

            Result<MeResultDto> result = await _mediator.Send(new MeQuery(userId));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromQuery] RefreshTokenDto? refreshTokenDto)
        {
            string? refreshToken = Request.Cookies["refresh_token"] ?? refreshTokenDto?.RefreshToken;

            if (string.IsNullOrEmpty(refreshToken))
            {
                Result<object> missingTokenResult =
                    Result<object>.Failure("Refresh token is missing.", HttpStatusCode.Unauthorized);

                return StatusCode((int)missingTokenResult.StatusCode!.Value, missingTokenResult);
            }

            Result<object> result = await _mediator.Send(new LogoutUserCommand(refreshToken));

            CookieOptions cookieOptions = new() { SameSite = SameSiteMode.None, Secure = true };

            Response.Cookies.Delete("access_token", cookieOptions);
            Response.Cookies.Delete("refresh_token", cookieOptions);

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        private void SetAuthCookies(string accessToken, string? refreshToken)
        {
            IConfigurationSection jwtSettings = _configuration.GetSection("JwtSettings");

            Response.Cookies.Append("access_token", accessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(jwtSettings.GetValue<double>("ExpiryMinutes"))
                });

            if (refreshToken != null)
            {
                Response.Cookies.Append("refresh_token", refreshToken,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });
            }
        }
    }
}
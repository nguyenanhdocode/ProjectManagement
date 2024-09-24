using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Application.Common;
using Application.Exceptions;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Application.Models.User;
using Application.Models;
using Application.Common.Email;
using System.Runtime.InteropServices;

namespace API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private JwtConfiguration _jwtConfiguration;
        private IUserService _userService;
        private IEmailService _emailService;
        private ITemplateService _templateService;

        public UserController(IOptions<JwtConfiguration> jwtConfiguration
            , IUserService userService
            , IEmailService emailService
            , ITemplateService templateService)
        {
            _jwtConfiguration = jwtConfiguration.Value;
            _userService = userService;
            _emailService = emailService;
            _templateService = templateService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody]CreateUserModel model)
        {
            return Ok(ApiResult<CreateUserResponseModel>.Success(await _userService.Create(model)));
        }

        [HttpPost]
        [Route("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginModel model)
        {
            return Ok(ApiResult<LoginResponseModel>.Success(await _userService.Login(model)));
        }

        [HttpPost]
        [Route("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailModel model)
        {
            await _userService.ConfirmEmail(model);
            return Ok(ApiResult<object?>.Success(null));
        }

        [HttpGet]
        [Route("profile")]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            return Ok(ApiResult<OwnProfileResponseModel>.Success(await _userService.GetProfile()));
        }

        [HttpGet]
        [Route("{id}/profile")]
        public async Task<IActionResult> Profile(string id)
        {
            return Ok(ApiResult<ViewProfileModel>.Success(await _userService.GetProfile(id)));
        }

        [HttpPut]
        [Route("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            await _userService.ChangePassword(model);
            return Ok(ApiResult<object?>.Success(null));
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            await _userService.ForgotPassword(model);
            return Ok(ApiResult<object?>.Success(null));
        }

        [HttpPut]
        [Route("rest-password")]
        public async Task<IActionResult> ResetPasword(ResetPasswordModel model)
        {
            await _userService.ResetPassword(model);
            return Ok(ApiResult<object?>.Success(null));
        }

        [HttpPost]
        [Route("change-email-token")]
        [Authorize]
        public async Task<IActionResult> CreateChangeEmailToken(CreateChangeEmailTokenModel model)
        {
            var result = await _userService.CreateChangeEmailToken(model);
            return Ok(ApiResult<CreateChangeEmailTokenResponseModel>.Success(result));
        }

        [HttpPut]
        [Route("email")]
        public async Task<IActionResult> ChangeEmail(ChangeEmailModel model)
        {
            await _userService.ChangeEmail(model);
            return Ok(ApiResult<object?>.Success(null));
        }

        [HttpPost]
        [Route("refresh-token/authenticate")]
        public async Task<IActionResult> LoginWithRefreshToken(RefreshTokenLoginModel model)
        {
            return Ok(ApiResult<RefreshTokenLoginResponseModel>
                .Success(await _userService.LoginWithRefreshToken(model)));
        }
    }
}

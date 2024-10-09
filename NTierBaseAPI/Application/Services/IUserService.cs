using Application.MappingProfiles;
using Application.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<CreateUserResponseModel> Create(CreateUserModel model);

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<LoginResponseModel> Login(LoginModel model);

        /// <summary>
        /// Confirm email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task ConfirmEmail(ConfirmEmailModel model);

        /// <summary>
        /// Get own profile
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<OwnProfileResponseModel> GetProfile();

        /// <summary>
        /// Get profile by user id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ViewProfileModel> GetProfile(string id);

        /// <summary>
        /// Check password is valid for change password funtional
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> CheckPassword(CheckPasswordModel model);

        /// <summary>
        /// Change password model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task ChangePassword(ChangePasswordModel model);

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task ForgotPassword(ForgotPasswordModel model);

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task ResetPassword(ResetPasswordModel model);

        /// <summary>
        /// Change email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task ChangeEmail(ChangeEmailModel model);

        /// <summary>
        /// Create change email pass
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<CreateChangeEmailTokenResponseModel> CreateChangeEmailToken(CreateChangeEmailTokenModel model);

        /// <summary>
        /// Login with refresh token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<RefreshTokenLoginResponseModel> LoginWithRefreshToken(RefreshTokenLoginModel model);

        /// <summary>
        /// Find user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<List<ViewProfileModel>> FindUser(FindUserModel model);
    }
}

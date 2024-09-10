using Application.Common;
using Application.Common.Email;
using Application.Exceptions;
using Application.Helpers;
using Application.Models.User;
using Application.Validators.User;
using AutoMapper;
using Core.Entities;
using DataAccess.UnifOfWork;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Data;
using IsolationLevel = System.Transactions.IsolationLevel;
using Microsoft.Extensions.Options;

namespace Application.Services.Impl
{
    public class UserService : IUserService
    {
        private IMapper _mapper;
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private IConfiguration _configuration;
        private IEmailService _emailService;
        private ITemplateService _templateService;
        private IClaimService _claimService;
        private IUnitOfWork _uow;
        private JwtConfiguration _jwtConfiguration;

        public UserService(IMapper mapper
            , UserManager<AppUser> userManager
            , SignInManager<AppUser> signInManager
            , IConfiguration configuration
            , IEmailService emailService
            , ITemplateService templateService
            , IClaimService claimService
            , IUnitOfWork unitOfWork
            , IOptions<JwtConfiguration> jwtConfiguration)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
            _templateService = templateService;
            _claimService = claimService;
            _uow = unitOfWork;
            _jwtConfiguration = jwtConfiguration.Value;
        }

        public async Task ChangeEmail(ChangeEmailModel model)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeOption.Required
                    , new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }
                    , TransactionScopeAsyncFlowOption.Enabled))
                {
                    AppUser user = await _userManager.FindByIdAsync(model.UserId);
                    var result = await _userManager.ChangeEmailAsync(user, model.NewEmail, model.Token);
                    if (!result.Succeeded)
                        throw new UnprocessableEntityException(result.Errors.FirstOrDefault()?.Description);

                    user.UserName = model.NewEmail;
                    await _userManager.UpdateAsync(user);

                    tran.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task ChangePassword(ChangePasswordModel model)
        {
            string userId = _claimService.GetUserId();
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new BadRequestException("User is not found!");

            bool passwordCheck = await _userManager.CheckPasswordAsync(user, model.OldPassword);
            await _uow.SaveChangesAsync();
            if (!passwordCheck)
                throw new UnauthorizeException("Old password is incorect");

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
                throw new UnprocessableEntityException(result.Errors.FirstOrDefault()?.Description);

        }

        public async Task<bool> CheckPassword(CheckPasswordModel model)
        {
            string userId = _claimService.GetUserId();
            AppUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            return await _userManager.CheckPasswordAsync(user, model.Password);
        }

        public async Task ConfirmEmail(ConfirmEmailModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null || user.EmailConfirmed)
                throw new UnprocessableEntityException("Your verification link is incorrect");

            var result = await _userManager.ConfirmEmailAsync(user, model.Token);
            if (!result.Succeeded)
                throw new UnprocessableEntityException("Your verification link is incorrect");
        }

        public async Task<CreateUserResponseModel> Create(CreateUserModel model)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeOption.Required
                    , new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }
                    , TransactionScopeAsyncFlowOption.Enabled))
                {
                    var user = _mapper.Map<AppUser>(model);
                    user.UserName = model.Email;
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (!result.Succeeded)
                        throw new UnprocessableEntityException(result.Errors.FirstOrDefault()?.Description);

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    string content = await _templateService.GetTemplateAsync("confirm_email.html");
                    string body = _templateService.ReplaceInTemplate(content, new Dictionary<string, string>()
                        {
                            { "{firstName}", model.FirstName },
                            { "{lastName}", model.LastName},
                            { "{activateLink}", token }
                        });

                    await _emailService.SendEmailAsync(new EmailMessage(model.Email, body, "Activate account"));

                    await _uow.SaveChangesAsync();

                    string newId = (await _userManager.FindByEmailAsync(model.Email)).Id;

                    tran.Complete();

                    return new CreateUserResponseModel
                    {
                        Id = Guid.Parse(newId),
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<CreateChangeEmailTokenResponseModel> CreateChangeEmailToken(CreateChangeEmailTokenModel model)
        {
            AppUser user = await _userManager.FindByIdAsync(_claimService.GetUserId());
            string result = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);

            return new CreateChangeEmailTokenResponseModel
            {
                NewEmail = model.NewEmail,
                Token = result,
                UserId = user.Id.ToString()
            };
        }

        public async Task ForgotPassword(ForgotPasswordModel model)
        {
            AppUser user = await _userManager.FindByEmailAsync(model.Email);
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            string template = await _templateService.GetTemplateAsync("reset_password.html");
            string body = _templateService.ReplaceInTemplate(template
                , new Dictionary<string, string>
            {
                    { "{firstName}", user.FirstName },
                    { "{lastName}", user.LastName },
                    { "{resetLink}", token }
            });

            await _emailService.SendEmailAsync(new EmailMessage(model.Email, body, "Reset password"));
        }

        public async Task<OwnProfileResponseModel> GetProfile()
        {
            string userId = _claimService.GetUserId();
            AppUser? user = await _userManager.Users.SingleOrDefaultAsync(p => p.Id.Equals(userId));

            return _mapper.Map<OwnProfileResponseModel>(user);
        }

        public async Task<ProfileResponseModel> GetProfile(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);

            return _mapper.Map<ProfileResponseModel>(user);
        }

        public async Task<LoginResponseModel> Login(LoginModel model)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName.Equals(model.Username));

            if (user == null)
                throw new UnauthorizeException("Username or password is incorrect");

            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (!signInResult.Succeeded)
                throw new UnauthorizeException("Username or password is incorrect");

            var token = JwtHelper.GenerateToken(user, _configuration);

            user.RefreshToken = Guid.NewGuid().ToString();
            user.RefreshTokenExpires = DateTime.UtcNow.AddHours(_jwtConfiguration.RefreshTokenExpireHours);
            await _userManager.UpdateAsync(user);

            return new LoginResponseModel
            {
                RefreshToken = user.RefreshToken,
                AccessToken = token
            };
        }

        public async Task<RefreshTokenLoginResponseModel> LoginWithRefreshToken(RefreshTokenLoginModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            bool canSignIn = await _signInManager.CanSignInAsync(user);
            if (!canSignIn)
                throw new UnauthorizeException("Sign-in failed");

            string token = JwtHelper.GenerateToken(user, _configuration);

            return new RefreshTokenLoginResponseModel
            {
                AccessToken = token
            };
        }

        public async Task ResetPassword(ResetPasswordModel model)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeOption.Required
                    , new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }
                    , TransactionScopeAsyncFlowOption.Enabled))
                {
                    AppUser user = await _userManager.FindByIdAsync(model.UserId);
                    if (user == null)
                        throw new BadRequestException("User is not found");

                    string newPassword = RandomPassword.GeneratePassword();
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, newPassword);
                    if (!result.Succeeded)
                        throw new UnprocessableEntityException(result.Errors.FirstOrDefault()?.Description);

                    string template = await _templateService.GetTemplateAsync("reset_password_success.html");
                    string body = _templateService.ReplaceInTemplate(template
                        , new Dictionary<string, string>
                    {
                        { "{firstName}", user.FirstName },
                        { "{lastName}", user.LastName },
                        { "{newPassword}", newPassword }
                    });

                    await _emailService.SendEmailAsync(new EmailMessage(user.Email, body
                        , "Reset password successful"));

                    tran.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

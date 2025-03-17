using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Identity;
using eCommerceApp.Application.Services.Interfaces.Authentication;
using eCommerceApp.Application.Services.Interfaces.Logging;
using eCommerceApp.Application.Validations;
using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Interfaces.Authentication;
using FluentValidation;

namespace eCommerceApp.Application.Services.Implementations
{
    public class AuthenticationService
        (ITokenManagement tokenManagement , IUserManagement userManagement
        , IRoleManagement roleManagement ,IAppLogger<AuthenticationService> appLogger
        , IMapper mapper , IValidator<CreateUser> createUserValidater
        , IValidator<LoginUser> loginUserValidater ,IValidationService validationService) : IAuthenticationService
    {
        private readonly ITokenManagement _tokenManagement = tokenManagement;
        private readonly IUserManagement _userManagement = userManagement;
        private readonly IRoleManagement _roleManagement = roleManagement;
        private readonly IAppLogger<AuthenticationService> _appLogger = appLogger;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreateUser> _createUserValidater = createUserValidater;
        private readonly IValidator<LoginUser> _loginUserValidater = loginUserValidater;
        private readonly IValidationService _validationService = validationService;

        public async Task<ServiceResponse> CreateUser(CreateUser user)
        {
            var validationResult = await _validationService.ValidateAsync(user , _createUserValidater);
            if(!validationResult.Success)
                return validationResult;

            var mappedModel = _mapper.Map<AppUser>(user);
            mappedModel.UserName = user.Email;
            mappedModel.PasswordHash = user.Password;

            var result = await _userManagement.CreateUser(mappedModel);
            if (!result)
                return new ServiceResponse { Meassage = "Email Address Might be Already" +
                    "In Use Or Unknow Error Occurred" };

            var _user = await _userManagement.GetUserByEmail(user.Email);
            var users = await _userManagement.GetAllUsers();

            bool assignedResult = await _roleManagement.AddUserToRole(_user!, users!.Count() > 1 ? "User" : "Admin");

            if (!assignedResult)
            {
                int removeUserResult = await _userManagement.RemoveUserByEmail(user.Email);
                if(removeUserResult <= 0)
                {
                    _appLogger.LogError(
                        new Exception($"User with Email as {_user.Email} failed to be remove as a result role assigning issue"),
                        "User could nto be assigned Role");
                    return new ServiceResponse { Meassage = "Error occurred in create account" };
                }
            }

            return new ServiceResponse { Success = true, Meassage = "Account Created!" };
            // verify Email

        }

        public async Task<LoginResponse> LoginUser(LoginUser user)
        {
            var validationResult = await _validationService.ValidateAsync(user , _loginUserValidater);

            if(!validationResult.Success)
                return new LoginResponse(Message: validationResult.Meassage);

            var mappedModel = _mapper.Map<AppUser>(user);
            mappedModel.PasswordHash = user.Password;

            bool loginResult = await _userManagement.LoginUser(mappedModel);
            if (!loginResult)
                return new LoginResponse(Message: "Email not found or invalid credentials");

            var _user = await _userManagement.GetUserByEmail(user.Email);
            var claims = await _userManagement.GetUserClaims(_user!.Email!);

            string jwtToken = _tokenManagement.GenerateToken(claims);
            string refreshToken = _tokenManagement.GetRefreshToken();

            int saveTokenResult = await _tokenManagement.AddRefreshToken(_user.Id, refreshToken);

            return saveTokenResult <= 0 
                ? new LoginResponse(Message: "Internal error occurred while authentication")
                : new LoginResponse(Success:true , Token: jwtToken ,RefreshToken:refreshToken);
        }

        public async Task<LoginResponse> ReviveToken(string refreshToken)
        {
            bool validateTokenResult = await _tokenManagement.ValidDateRefreshToken(refreshToken);
            if(!validateTokenResult)
                return new LoginResponse(Message:"Invalid token");

            string userId = await _tokenManagement.GetUserIdByRefreshToken(refreshToken);
            AppUser? user = await _userManagement.GetUserById(userId);

            var claims = await _userManagement.GetUserClaims(user!.Email!);
            string newJwtToken = _tokenManagement.GenerateToken(claims);
            string newRefreshToken = _tokenManagement.GetRefreshToken();
            await _tokenManagement.UpdateRefreshToken(userId, newRefreshToken);

            return new LoginResponse(Success:true , Token: newJwtToken ,RefreshToken:newRefreshToken);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface IProfileService
{
    Task<Response<User>?> CurrentUserProfile(UserProfileModel model, HttpRequest request);

    Task<Response<User>> CurrentUserProfileUpdate(UserProfileModel model, HttpRequest request);

    Task<Response<bool>> CurrentUserChangePassword(ChangePasswordModel model, HttpRequest request);
    Task<Response<UserProfileModel>?> CurrentProfileImages(HttpRequest request);
    Task<Response<GraphDataVM?>> GetGraphDataAsync(DateOnly? fromDate, DateOnly? toDate);
    Task<Response<DashboardVM?>> GetDashboardDataAsync(DateOnly? fromDate, DateOnly? toDate);

}

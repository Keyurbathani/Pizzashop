using System.Security.Claims;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Implementations;

public class ProfileService : IProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly IOrdersRepository _orderRepository;
    private readonly IOrderedItemRepository _orderItemRepository;
    private readonly ITokenEmailService _tokenEmailService;
    private readonly ICustomerRepository _customerRepository;
    private readonly IWaitingTokensRepository _waitingTokensRepository;

    public ProfileService(IUserRepository userRepository, ITokenEmailService tokenEmailService , IOrdersRepository orderRepository , IOrderedItemRepository orderedItemRepository , ICustomerRepository customerRepository , IWaitingTokensRepository waitingTokensRepository)
    {
        _userRepository = userRepository;
        _tokenEmailService = tokenEmailService;
        _orderRepository = orderRepository;
        _orderItemRepository = orderedItemRepository;
        _customerRepository = customerRepository;
        _waitingTokensRepository = waitingTokensRepository;
    }



    public async Task<Response<User>?> CurrentUserProfile(UserProfileModel model, HttpRequest request)
    {

        var token = request.Cookies["AuthToken"];

        var Id = _tokenEmailService.GetIdFromToken(token!);

        if (Id == null)
        {
            return Response<User>.FailureResponse("ID is null");
        }

        var user = await _userRepository.GetUserById(Id);

        if (user == null)
        {
            return Response<User>.FailureResponse("User Does not exist");
        }

        model.Id = user.Id;
        model.FisrtName = user.FisrtName;
        model.Email = user.Email;
        model.Role = user.Role.Name;
        model.LastName = user.LastName;
        model.Username = user.Username;
        model.Phone = (long)user.Phone!;
        model.Address = user.Address;
        model.Zipcode = user.Zipcode;
        model.CountryId = user.CountryId;
        model.StateId = user.StateId;
        model.CityId = user.CityId;
        model.Imageurl = user.ProfileImage;

        return Response<User>.SuccessResponse(user, "Profile Fetched Successfully.");

    }


    public async Task<Response<UserProfileModel>?> CurrentProfileImages(HttpRequest request)
    {
        var token = request.Cookies["AuthToken"];
        var Id = _tokenEmailService.GetIdFromToken(token!);

        if (Id == null)
        {
            return Response<UserProfileModel>.FailureResponse("Id is not exists");
        }

        var user = await _userRepository.GetUserById(Id);
        if (user == null)
        {
            return Response<UserProfileModel>.FailureResponse("User Does not exist");
        }

        var model = new UserProfileModel()
        {
            Imageurl = user.ProfileImage,
            Username = user.Username
        };

        return Response<UserProfileModel>.SuccessResponse(model, "Profile Fetched Successfully.");

    }


    public async Task<Response<User>> CurrentUserProfileUpdate(UserProfileModel model, HttpRequest request)
    {
        var token = request.Cookies["AuthToken"];
        var Id = _tokenEmailService.GetIdFromToken(token!);

        if (Id == null)
        {
            return Response<User>.FailureResponse("Email is null");
        }

        var user = await _userRepository.GetUserById(Id);
        if (user == null)
        {
            return Response<User>.FailureResponse("User Does not exist");
        }

        var IsExist = await _userRepository.IsExistsAsync(u => u.Username.ToLower().Trim() == model.Username.ToLower().Trim() && u.Id != model.Id && !(bool)u.IsDeleted!);
        if (IsExist)
        {
            return Response<User>.FailureResponse("Username Already Exists");
        }

        IsExist = await _userRepository.IsExistsAsync(u => u.Phone.ToString().Trim() == model.Phone.ToString().Trim() && u.Id != model.Id && !(bool)u.IsDeleted!);
        if (IsExist)
        {
            return Response<User>.FailureResponse("Phone Already Exists");
        }



        if (model.ProfileImage != null)
        {
            var fileName = Path.GetFileNameWithoutExtension(model.ProfileImage.FileName);
            var extension = Path.GetExtension(model.ProfileImage.FileName);
            var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            var path = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                model.ProfileImage.CopyTo(fileStream);
            }

            // Save the relative path to the model property
            model.Imageurl = $"uploads/{uniqueFileName}";
        }
        user.FisrtName = model.FisrtName;
        user.LastName = model.LastName;
        user.Phone = model.Phone;
        user.Address = model.Address;
        user.Zipcode = model.Zipcode;
        user.Username = model.Username;
        user.CountryId = model.CountryId;
        user.StateId = model.StateId;
        user.CityId = model.CityId;
        user.ProfileImage = model.Imageurl;

        await _userRepository.UpdateUser(user);
        return Response<User>.SuccessResponse(user, "Profile Updated Successfully!");
    }


    public async Task<Response<bool>> CurrentUserChangePassword(ChangePasswordModel model, HttpRequest request)
    {
        var token = request.Cookies["AuthToken"];
        var Id = _tokenEmailService.GetIdFromToken(token!);

        var user = await _userRepository.GetUserById(Id);
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password))
        {
            return Response<bool>.FailureResponse("Invalid Current Password!");
        }

        if (user.IsFirstLogin.GetValueOrDefault())
        {
            user.IsFirstLogin = false;
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        await _userRepository.UpdateUser(user);
        return Response<bool>.SuccessResponse(true, "Password Changed Successfully!");

    }


    public async Task<Response<DashboardVM?>> GetDashboardDataAsync(DateOnly? fromDate, DateOnly? toDate)
    {
        
            OrderSummaryDTO orderSummary = await _orderRepository.GetOrderSummaryAsync(fromDate, toDate);
            (IEnumerable<ItemSummaryDTO> topSellingItems, IEnumerable<ItemSummaryDTO> leastSellingItems) = await _orderItemRepository.GetItemsSummary(fromDate, toDate);
            int newCustomersCount = await _customerRepository.GetNewCustomerCountAsync(fromDate, toDate);
            int waitingCustomerCount = await _waitingTokensRepository.GetWaitingCustomerCountAsync();

            DashboardVM dashboardVM = new()
            {
                TotalOrder = orderSummary.TotalOrders,
                TotalSales = orderSummary.TotalSales,
                AvgOrderValue = orderSummary.AvgOrderValue,
                NewCustomerCount = newCustomersCount,
                WaitingListCount = waitingCustomerCount,
                AvgWaitingTime = orderSummary.AvgWaitingTime,

                LeastSellingItems = leastSellingItems.Select(i => new ItemSummaryVM()
                {
                    Id = i.Id,
                    Name = i.Name,
                    Image = i.Image,
                    OrderCount = i.OrderCount
                }).ToList(),

                TopSellingItems = topSellingItems.Select(i => new ItemSummaryVM()
                {
                    Id = i.Id,
                    Name = i.Name,
                    Image = i.Image,
                    OrderCount = i.OrderCount
                }).ToList(),
            };

            return Response<DashboardVM?>.SuccessResponse(dashboardVM, "Dashboard Data fetched");
        
    }

    public async Task<Response<GraphDataVM?>> GetGraphDataAsync(DateOnly? fromDate, DateOnly? toDate)
    {

        (IEnumerable<GraphDataDTO> revenueData, IEnumerable<GraphDataDTO> customerGrowthData) = await _orderRepository.GetGraphDataAsync(fromDate, toDate);

        GraphDataVM graphData = new()
        {
            RevenueData = revenueData.Select(r => new GraphPointVM()
            {
                Date = r.Date,
                Value = r.Value,
            }).ToList(),
            CustomerGrowthData = customerGrowthData.Select(cg => new GraphPointVM()
            {
                Date = cg.Date,
                Value = cg.Value
            }).ToList()
        };

        return Response<GraphDataVM?>.SuccessResponse(graphData , "graph data fetched");

    }
}

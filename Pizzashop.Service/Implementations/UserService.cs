using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Helper;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Implementations;

public class UserService : IUserService
{

    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public UserService(IUserRepository userRepository, IEmailService emailService)
    {
        _userRepository = userRepository;
        _emailService = emailService;

    }
   
    public async Task<Response<PagedResult<UserNewListVM>>> GetPagedUsersAsync(string searchString, int page, int pagesize, bool isASC ,string sortColumn)
    {

        (IEnumerable<User> items, int totalRecords) = await _userRepository.GetPagedUsersAsync(searchString,page,pagesize,isASC,sortColumn);

        IEnumerable<UserNewListVM> itemlist = items.Select(u => new UserNewListVM()
        {
            Id = u.Id,
            Name = u.FisrtName + " " + u.LastName,
            Email = u.Email,
            Phone = u.Phone,
            Role = u.Role.Name,
            IsActive = u.IsActive.GetValueOrDefault(),
            Imageurl = u.ProfileImage,
            
        }).ToList();

        PagedResult<UserNewListVM> pagedResult = new()
        {
            PagedList = itemlist
        };

        pagedResult.Pagination.SetPagination(totalRecords, pagesize, page);

        return Response<PagedResult<UserNewListVM>>.SuccessResponse(pagedResult, "User list fetched successfully!");

    }



    public async Task<Response<User>> CreateUser(AddNewUserModel model)
    {

        var IsExist = await _userRepository.IsExistsAsync(u => u.Username.ToLower().Trim() == model.Username.ToLower().Trim() && !(bool)u.IsDeleted!);
        if (IsExist)
        {
            return Response<User>.FailureResponse("Username Already Exists");
        }


        IsExist = await _userRepository.IsExistsAsync(u => u.Phone.ToString().Trim() == model.Phone.ToString().Trim() && !(bool)u.IsDeleted!);
        if (IsExist)
        {
            return Response<User>.FailureResponse("Phone Number Already Exists");
        }

        IsExist = await _userRepository.IsExistsAsync(u => u.Email.ToLower().Trim() == model.Email.ToLower().Trim() && !(bool)u.IsDeleted!);
        if (IsExist)
        {
            return Response<User>.FailureResponse("Email Already Exists");
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

        var NewUser = new User()
        {
            FisrtName = model.FisrtName,
            LastName = model.LastName,
            Username = model.Username,
            RoleId = (int)model.RoleId,
            Email = model.Email!,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Zipcode = model.Zipcode,
            Address = model.Address,
            Phone = model.Phone,
            CountryId = model.CountryId,
            StateId = model.StateId,
            CityId = model.CityId,
            ProfileImage = model.Imageurl

        };


        string subject = "User Credentials";


        string body = $@"<div style='font-family: sans-serif;'>
                        <div style='background-color: #0066A7; padding: 10px; display: flex; justify-content: center; align-items: center; gap: 2rem; '>
                            <img src='http://localhost:5155/images/pizzashop_logo.png' alt=''  style='width:60px; background-color:white; border-radius:50%;'>
                            <h1 style='color: white; '>PIZZASHOP</h1>
                        </div>
                            <p>
                                Welcome to Pizza Shop, <br><br>
                                Please find the details below for login your account. <br>
                                <div style='border: 1px solid black; padding: 0.5rem; font-weight: bold;'>
                                    <h3>Login Details:</h3>
                                    Username : {model.Email} <br>
                                    Temporary Password : {model.Password}
                                </div><br>
                                If you encounter any issues or have any questions, please do not hesitate to contact our support team. <br><br>
                            </p>
                    </div>";


        await _userRepository.Adduser(NewUser);

        try
        {
            _emailService.SendEmailAsync(model.Email!, body, subject);
        }
        catch
        {
            return Response<User>.SuccessResponse(NewUser, "User Added Successfully! but Email not sent");
        }

        return Response<User>.SuccessResponse(NewUser, "User Added Succesfully!");

    }

    public async Task<Response<User>> EditUserView(AddNewUserModel model)
    {

        var user = await _userRepository.GetUserById(model.Id);
        if (user == null)
        {
            return Response<User>.FailureResponse("User does not exist!");
        }

        model.FisrtName = user.FisrtName;
        model.Email = user.Email;
        model.RoleId = user.RoleId;
        model.LastName = user.LastName;
        model.isActive = user.IsActive;
        model.Phone = (long)user.Phone;
        model.Address = user.Address;
        model.Zipcode = user.Zipcode;
        model.Username = user.Username;
        model.CountryId = user.CountryId;
        model.StateId = user.StateId;
        model.CityId = user.CityId;
        model.Imageurl = user.ProfileImage;

        return Response<User>.SuccessResponse(user, "User fetched Successfully!");

    }
    public async Task<Response<User>> EditUserById(AddNewUserModel model)
    {

        var IsExist = await _userRepository.IsExistsAsync(u => u.Username.ToLower().Trim() == model.Username.ToLower().Trim() && u.Id != model.Id && !(bool)u.IsDeleted!);
        if (IsExist)
        {
            return Response<User>.FailureResponse("Username Already Exists");
        }


        IsExist = await _userRepository.IsExistsAsync(u => u.Phone.ToString().Trim() == model.Phone.ToString().Trim() && u.Id != model.Id && !(bool)u.IsDeleted!);
        if (IsExist)
        {
            return Response<User>.FailureResponse("Phone Number Already Exists");
        }

        IsExist = await _userRepository.IsExistsAsync(u => u.Email.ToLower().Trim() == model.Email.ToLower().Trim() && u.Id != model.Id && !(bool)u.IsDeleted!);
        if (IsExist)
        {
            return Response<User>.FailureResponse("Email Already Exists");
        }

        var user = await _userRepository.GetUserById(model.Id);
        if (user == null)
        {
            return Response<User>.FailureResponse("User does not exist!");
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
        user.Email = model.Email;
        user.IsActive = model.isActive;
        user.LastName = model.LastName;
        user.RoleId = (int)model.RoleId;
        user.Phone = model.Phone;
        user.Address = model.Address;
        user.Zipcode = model.Zipcode;
        user.Username = model.Username;
        user.CountryId = model.CountryId;
        user.StateId = model.StateId;
        user.CityId = model.CityId;
        user.ProfileImage = model.Imageurl;

        await _userRepository.UpdateUser(user);
        return Response<User>.SuccessResponse(user, "User Updated Successfully!");

    }

    public async Task<Response<bool>> DeleteUser(int id)
    {

        var user = await _userRepository.GetUserById(id);
        if (user == null)
        {
            return Response<bool>.FailureResponse("User does not exist!");
        }

        user.IsDeleted = true;
        await _userRepository.UpdateUser(user);

        return Response<bool>.SuccessResponse(true, "User Deleted Successfully!");

    }

   



}


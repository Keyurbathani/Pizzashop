using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IProfileService _profileService;


    private readonly IRoleService _roleService;
    private readonly ICountryService _countryService;
    private readonly IRoleRepository _roleRepository;

    private readonly IUserRepository _userRepository;

    private readonly ITokenEmailService _tokenEmailService;


    public DashboardController(IProfileService profileService, IUserService userService, ICountryService countryService, IRoleService roleService, IRoleRepository roleRepository, ITokenEmailService tokenEmailService)
    {
        _profileService = profileService;
        _countryService = countryService;
        _roleService = roleService;
        _roleRepository = roleRepository;
        _tokenEmailService = tokenEmailService;
    }
     public IActionResult Index()
    {
        return View();
    }
 
    public async Task<IActionResult> GetDashboardData(DateOnly? fromDate, DateOnly? toDate)
    {
        Response<DashboardVM?> response = await _profileService.GetDashboardDataAsync(fromDate, toDate);
 
        if (!response.Success)
        {
            return PartialView("_Dashboard", new DashboardVM());
        }
 
        return PartialView("_Dashboard", response.Data);
    }
 
    public async Task<JsonResult> GetGraphData(DateOnly? fromDate, DateOnly? toDate)
    {
        Response<GraphDataVM?> response = await _profileService.GetGraphDataAsync(fromDate, toDate);
        return Json(response);
    }



    public async Task<IActionResult> Profile(int countryId, int stateId, int CityId, string from)
    {

        var model = new UserProfileModel()
        {
            CityId = CityId,
            CountryId = countryId,
            StateId = stateId
        };

        var response = await _profileService.CurrentUserProfile(model, Request);

        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return RedirectToAction("Index", "Dashboard");
        }

        var allCountries = await _countryService.GetAllCountries();
        var allStates = await _countryService.GetStatesByCountryId(model.CountryId);
        var allCities = await _countryService.GetCitiesByStateId(model.StateId);

        ViewBag.Countries = new SelectList(allCountries, "Id", "Name", model.CountryId);
        ViewBag.States = new SelectList(allStates, "Id", "Name", model.StateId);
        ViewBag.Cities = new SelectList(allCities, "Id", "Name", model.CityId);

        TempData["success"] = response.Message;

        if (from == "order")
        {
            ViewBag.orderLayout = true;
        }
        else
        {
            ViewBag.orderLayout = false;
        }

        return View(model);
    }

    public async Task<JsonResult> CurrentProfileImages()
    {

        var response = await _profileService.CurrentProfileImages(Request);

        if (!response.Success)
        {
            TempData["error"] = response.Message;
        }
        return Json(new { success = true, response });
    }

    public async Task<IActionResult> GetStates(int countryId)
    {
        var states = await _countryService.GetStatesByCountryId(countryId);
        return Json(states);
    }


    public async Task<IActionResult> GetCities(int stateId)
    {
        var cities = await _countryService.GetCitiesByStateId(stateId);
        return Json(cities);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UserProfileModel model)
    {
        if (ModelState.IsValid)
        {

            // Proceed with updating the user profile
            var response = await _profileService.CurrentUserProfileUpdate(model, Request);

            if (response.Success)
            {
                TempData["success"] = response.Message;
                return RedirectToAction("Index");

            }
            else
            {
                TempData["error"] = response.Message;
            }


        }

        // If validation fails, repopulate the dropdown lists
        var allCountries = await _countryService.GetAllCountries();
        var allStates = await _countryService.GetStatesByCountryId(model.CountryId);
        var allCities = await _countryService.GetCitiesByStateId(model.StateId);

        ViewBag.Countries = new SelectList(allCountries, "Id", "Name", model.CountryId);
        ViewBag.States = new SelectList(allStates, "Id", "Name", model.StateId);
        ViewBag.Cities = new SelectList(allCities, "Id", "Name", model.CityId);

        return View("Profile", model);
    }

    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.NewPassword == model.CurrentPassword)
            {
                TempData["error"] = "New Password and current Password should be different";
                return View(model);
            }

            var response = await _profileService.CurrentUserChangePassword(model, Request);
            if (response.Success)
            {
                TempData["success"] = response.Message;
                Response.Cookies.Delete("AuthToken");

                return RedirectToAction("Login", "Auth");
            }
            else
            {
                TempData["error"] = response.Message;
            }
        }

        return View(model);

    }

    public IActionResult Logout()
    {
        Response.Cookies.Delete("AuthToken");
        return RedirectToAction("Login", "Auth");
    }


}
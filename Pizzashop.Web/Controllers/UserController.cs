using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Implementations;
using Pizzashop.Service.Utils;
using Pizzashop.Entity.Constants;
using Pizzashop.Web.Attributes;

namespace Pizzashop.Web.Controllers;

[Authorize]
[PermissionAuthorize(Constants.CanView)]
public class UserController : Controller
{

    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly ICountryService _countryService;

    private readonly IUserRepository _userRepository;

    public UserController(IUserService userService, ICountryService countryService, IRoleService roleService, IUserRepository userRepository)
    {
        _userService = userService;
        _countryService = countryService;
        _roleService = roleService;
        _userRepository = userRepository;

    }

    public IActionResult Index()
    {

        string? canEdit = HttpContext.Items[Constants.CanEdit]?.ToString();
        string? canDelete = HttpContext.Items[Constants.CanDelete]?.ToString();
        HttpContext.Session.SetString(Constants.CanEdit, canEdit ?? string.Empty);
        HttpContext.Session.SetString(Constants.CanDelete, canDelete ?? string.Empty);

        return View();
    }

    public async Task<IActionResult> GetPagedUsers(string searchString = "", int page = 1, int pagesize = 5, bool isASC = true, string sortColumn = "")
    {

        Response<PagedResult<UserNewListVM>> response = await _userService.GetPagedUsersAsync(searchString, page, pagesize, isASC, sortColumn);
        if (!response.Success)
        {
            return PartialView("_UserNewList");
        }

        PagedResult<UserNewListVM> model = response.Data!;
        return PartialView("_UserNewList", model);

    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> AddNewUser(int CountryId, int StateId, int RoleId, int CityId)
    {
        AddNewUserModel model = new()
        {
            CountryId = CountryId,
            StateId = StateId,
            CityId = CityId,
            RoleId = RoleId
        };
        var allCountries = await _countryService.GetAllCountries();
        var allRoles = await _countryService.GetRoles();
        var allStates = await _countryService.GetStatesByCountryId(CountryId);
        var allCities = await _countryService.GetCitiesByStateId(StateId);

        ViewBag.Roles = new SelectList(allRoles, "Id", "Name", RoleId);
        ViewBag.Countries = new SelectList(allCountries, "Id", "Name", CountryId);
        ViewBag.States = new SelectList(allStates, "Id", "Name", StateId);
        ViewBag.Cities = new SelectList(allCities, "Id", "Name", CityId);

        return View(model);
    }

    [PermissionAuthorize(Constants.CanEdit)]
    [HttpPost]
    public async Task<IActionResult> CreateUser(AddNewUserModel model)
    {
        if (ModelState.IsValid)
        {

            var response = await _userService.CreateUser(model);

            if (!response.Success)
            {
                TempData["error"] = response.Message;
                var allCountries = await _countryService.GetAllCountries();
                var allRoles = await _countryService.GetRoles();
                var allStates = await _countryService.GetStatesByCountryId(model.CountryId);
                var allCities = await _countryService.GetCitiesByStateId(model.StateId);

                ViewBag.Roles = new SelectList(allRoles, "Id", "Name", model.RoleId);
                ViewBag.Countries = new SelectList(allCountries, "Id", "Name", model.CountryId);
                ViewBag.States = new SelectList(allStates, "Id", "Name", model.StateId);
                ViewBag.Cities = new SelectList(allCities, "Id", "Name", model.CityId);
                return View("AddNewUser", model);
            }

            TempData["success"] = response.Message;
            return RedirectToAction("Index");

        }


        return View("AddNewUser", model);
    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> EditUser(int Id)
    {

        var model = new AddNewUserModel()
        {
            Id = Id,
        };

        var response = await _userService.EditUserView(model);

        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return RedirectToAction("Index");
        }

        var user = response.Data;

        var allCountries = await _countryService.GetAllCountries();
        var allRoles = await _countryService.GetRoles();
        var allStates = await _countryService.GetStatesByCountryId(user!.CountryId);
        var allCities = await _countryService.GetCitiesByStateId(user.StateId);

        ViewBag.Roles = new SelectList(allRoles, "Id", "Name", user.RoleId);
        ViewBag.Countries = new SelectList(allCountries, "Id", "Name", user.CountryId);
        ViewBag.States = new SelectList(allStates, "Id", "Name", user.StateId);
        ViewBag.Cities = new SelectList(allCities, "Id", "Name", user.CityId);

        return View(model);

    }

    [HttpPost]
    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> EditUserNew(AddNewUserModel model)
    {
        ModelState.Remove("Password");
        var allCountries = await _countryService.GetAllCountries();
        var allRoles = await _countryService.GetRoles();
        var allStates = await _countryService.GetStatesByCountryId(model.CountryId);
        var allCities = await _countryService.GetCitiesByStateId(model.StateId);

        if (ModelState.IsValid)
        {
            var response = await _userService.EditUserById(model);

            if (!response.Success)
            {
                TempData["error"] = response.Message;
                ViewBag.Roles = new SelectList(allRoles, "Id", "Name", model.RoleId);
                ViewBag.Countries = new SelectList(allCountries, "Id", "Name", model.CountryId);
                ViewBag.States = new SelectList(allStates, "Id", "Name", model.StateId);
                ViewBag.Cities = new SelectList(allCities, "Id", "Name", model.CityId);
                return View("EditUser", model);
            }

            TempData["success"] = response.Message;
            return RedirectToAction("Index");


        }

        ViewBag.Roles = new SelectList(allRoles, "Id", "Name", model.RoleId);
        ViewBag.Countries = new SelectList(allCountries, "Id", "Name", model.CountryId);
        ViewBag.States = new SelectList(allStates, "Id", "Name", model.StateId);
        ViewBag.Cities = new SelectList(allCities, "Id", "Name", model.CityId);

        return View("EditUser", model);
    }


    [HttpPost]
    [PermissionAuthorize(Constants.CanDelete)]
    public async Task<IActionResult> DeleteUser(int Id)
    {

        var response = await _userService.DeleteUser(Id);
        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return Json(new { success = false, message = response.Message });
        }

        TempData["success"] = response.Message;
        return Json(new { success = true, message = response.Message });


    }
}

using iText.Html2pdf;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Web.Controllers;

[Authorize]
public class OrdersController : Controller
{

    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {

        _orderService = orderService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetPagedOrdersAsync(DateOnly? fromDate, DateOnly? toDate, string searchString = "", int page = 1, int pagesize = 5, bool isASC = true, string sortColumn = "", string status = "")
    {

        Response<PagedResult<OrderListVM>> response = await _orderService.GetPagedOrdersAsync(fromDate, toDate, searchString, page, pagesize, isASC, sortColumn, status);
        if (!response.Success)
        {
            return PartialView("_OrderList");
        }

        PagedResult<OrderListVM> model = response.Data!;
        return PartialView("_OrderList", model);

    }


    public async Task<IActionResult> ExportOrders(string pastDays = "month", string status = "", string searchString = "")
    {
        DateOnly? fromDate = null, toDate = null;
        DateTime today = DateTime.Today;

        if (pastDays.Equals("month"))
        {
            fromDate = new DateOnly(today.Year, today.Month, 1);
            toDate = DateOnly.FromDateTime(today);
        }
        else if (!string.IsNullOrEmpty(pastDays) && !pastDays.Equals("all"))
        {
            toDate = DateOnly.FromDateTime(today);
            fromDate = DateOnly.FromDateTime(today.AddDays(-int.Parse(pastDays)));
        }

        Response<MemoryStream> fileStream = await _orderService.ExportOrdersAsync(searchString, status, fromDate, toDate);

        if (fileStream.Success)
        {
            return File(fileStream.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
        }
        
        TempData["error"] = fileStream.Message;
        return View("Index");
        


    }

    public async Task<IActionResult> OrderDetails(int Id)
    {
        var response = await _orderService.GetOrderByIdAsync(Id);
        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return RedirectToAction("Index");
        }

        return View(response.Data);

    }

    public async Task<IActionResult> OrderPrint(int id)
    {
        var orderDetails = await _orderService.GetOrderByIdAsync(id);
        string htmlContent = await RenderViewToString("OrderPdf", orderDetails.Data!);

        // Convert HTML string to PDF using iText7
        using (MemoryStream stream = new MemoryStream())
        {
            PdfWriter writer = new PdfWriter(stream);
            PdfDocument pdf = new PdfDocument(writer);

            pdf.AddNewPage().SetMediaBox(iText.Kernel.Geom.PageSize.A4.Rotate());
            HtmlConverter.ConvertToPdf(htmlContent, stream);

            return File(stream.ToArray(), "application/pdf", $"Order_{id}.pdf");
        }
    }

    private async Task<string> RenderViewToString(string viewName, OrderDetailsVM model)
    {
        using (var sw = new StringWriter())
        {
            var viewEngine = HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

            var viewResult = viewEngine.FindView(ControllerContext, viewName, false);

            if (viewResult.View == null)
            {
                throw new ArgumentNullException($"View {viewName} not found");
            }

            var viewContext = new ViewContext(
                ControllerContext,
                viewResult.View,
                new ViewDataDictionary<object>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                },
                TempData,
                sw,
                new HtmlHelperOptions()
            );

            viewResult.View.RenderAsync(viewContext);
            return sw.ToString();

        }
    }

}

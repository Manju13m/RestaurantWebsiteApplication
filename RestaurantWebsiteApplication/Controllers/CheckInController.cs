using Microsoft.AspNetCore.Mvc;
using RestaurantWebsiteApplication.Models.ViewModels;
using RestaurantWebsiteApplication.Models;
using RestaurantWebsiteApplication.Data;
using Microsoft.EntityFrameworkCore;

namespace RestaurantWebsiteApplication.Controllers
{
    public class CheckInController : Controller
    {
        private readonly RestaurantDbContext restrauntDbContext;
        public CheckInController(RestaurantDbContext restrauntDbContext)
        {
            this.restrauntDbContext = restrauntDbContext;
        }


        [HttpGet]
        public IActionResult CheckIn()
        {
            return View(new CheckInViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CheckIn(CheckInViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Save the check-in details to the database
                
                var checkIn = new CheckIn
                {
                    CustomerId = model.CustomerId,
                    CheckInDate = model.CheckInDate,
                    CheckInTime = model.CheckInTime
                };

                restrauntDbContext.CheckIns.Add(checkIn);
                await restrauntDbContext.SaveChangesAsync();

                return RedirectToAction("CheckInConfirmation");
            }

            return View(model);
        }

        public IActionResult CheckInConfirmation()
        {
            return View();
        }


    }
}

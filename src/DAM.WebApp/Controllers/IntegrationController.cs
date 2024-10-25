using DAM.Core.Abstractions.Identity;
using DAM.Domain.Entities;
using DAM.Domain.Ïdentity;
using DAM.WebApp.Filters.Action;
using DAM.WebApp.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAM.WebApp.Controllers
{
    [RequireHttps]
    [Authorize(AuthenticationSchemes = "Discord")]
    [TypeFilter(typeof(CheckAlliianceRegistration), Order = 2)]
    [TypeFilter(typeof(AllianceDataFilter), Order = 1)]
    [Route("{controller}/{allianceid}/{action}")]
    public class IntegrationController : Controller
    {

        private readonly IAppUserIdentityManagerAsync AppIdentityManager;


        public IntegrationController(IAppUserIdentityManagerAsync appIdentityManager)
        {
            this.AppIdentityManager = appIdentityManager;

        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AppUsers()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddAppUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddAppUser(AppUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var alliance = ViewBag.allianceContext as Alliance;
            await this.AppIdentityManager.RegisterAsync(model.UserName, alliance.Id);


            return RedirectToAction("AppUsers");
        }
        [HttpGet]
        [Route("{Id}")]
        public async Task<IActionResult> EditAppUser(Guid Id)
        {
            var alliance = ViewBag.allianceContext as Alliance;

            var founduser = await this.AppIdentityManager.GetUser(Id);
            if (founduser == null || founduser.AllianceId != alliance.Id)
                return NotFound();

            var vm = new AppUserViewModel
            {
                Id = Id,
                UserName = founduser.Username
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> EditAppUser(AppUserViewModel model)
        {

            var alliance = ViewBag.allianceContext as Alliance;
            var founduser = await this.AppIdentityManager.GetUser(model.Id);
            if (founduser == null)
                return NotFound();
            if (founduser.AllianceId != alliance.Id)
            {
                return NotFound();
            }
            var user = new AppUser
            {
                Id = model.Id,
                Username = model.UserName
            };


            await this.AppIdentityManager.UpdateAsync(user);
            return RedirectToAction("AppUsers");
        }
    }
}

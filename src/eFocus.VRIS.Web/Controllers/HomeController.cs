using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using eFocus.VRIS.Core.Models;
using eFocus.VRIS.Core.Repositories;

namespace eFocus.VRIS.Web.Controllers
{
    public class HomeController : Controller
    {
        private AuthToken _token;
        private readonly ICalenderRepository _calenderRepository;

        public HomeController(ICalenderRepository calenderRepository)
        {
            _calenderRepository = calenderRepository;
        }

        public async Task<ActionResult> Index()
        {
            var accessToken = Session["AccessToken"];
            if (accessToken != null)
            {
                ViewBag.Token = Session["AccessToken"];
                ViewBag.UserInfo = await _calenderRepository.GetUserInfoAsync(accessToken as string);
            }

            return View();
        }

        public async Task<ActionResult> Login()
        {
            var loginRedirectUri = new Uri(Url.Action(nameof(Authorize), "Home", null, Request.Url.Scheme));
            var redirectUrl = await _calenderRepository.Login(loginRedirectUri);
            Session["RedirectUrl"] = redirectUrl;
            Session["LoginUri"] = loginRedirectUri;
            return Redirect(redirectUrl.AbsoluteUri);
        }

        public async Task<ActionResult> Authorize()
        {
            var redirect = Session["RedirectUrl"];
            var login = Session["LoginUri"] as Uri;
            _token =  await _calenderRepository.Authorize(Request.Params["code"], login);
            Session["AccessToken"] = _token.AccessToken;
            return RedirectToAction(nameof(Index));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using eFocus.VRIS.Core.Repositories;

namespace eFocus.VRIS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICalenderRepository _calenderRepository;

        public HomeController(ICalenderRepository calenderRepository)
        {
            _calenderRepository = calenderRepository;
        }

        public async Task<ActionResult> Index()
        {
            ViewBag.Token = await _calenderRepository.Authorize();
            return View();
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
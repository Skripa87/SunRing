using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UfaSmartCity.Models;

namespace UfaSmartCity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ContextManager _manager;
        private static string _selectedModuleTypeId;
        private static List<ModuleType> _moduleTypes;
        private static List<ContentType> _contentTypes;
        private static StationViewModel Station { get; set; }
        private static OptionsAndPreviewViewModel OptionsAndPreviewViewModel { get; set; }
        private static OptionsViewModel OptionsViewModel { get; set; }
        private static ContentAddViewModel ContentAddViewModel { get; set; }
        private static InformationTableViewModel InformationTableViewModel { get; set; }
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            try
            {
                _manager = new ContextManager();
                _moduleTypes = _manager.GetModuleTypes();
                _contentTypes = Enum.GetValues(typeof(ContentType))
                    .Cast<ContentType>()
                    .ToList();
            }
            catch (Exception)
            {
                RedirectToAction("Index", "Offline");
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

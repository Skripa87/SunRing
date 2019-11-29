using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult Index()
        {
            //var manager = new PredictorManager(); //var stations = manager.GetStations();        //var db = new CityStationsDBContext();            //foreach(var s in stations)            //{            //    db.Stations.Add(new StationModel(s));            //}           //db.SaveChanges();
            ViewData["MyAcc"] = User?.Identity
                                    ?.Name == "skripinalexey1987@gmail.com";
            return View();
        }

        [Authorize]
        public ActionResult IndexAuthtorize()
        {
            ViewData["MyAcc"] = User?.Identity
                                    ?.Name == "skripinalexey1987@gmail.com";
            return View();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public PartialViewResult SearchBlockPart(string searchBoxText)
        {
            _manager = new ContextManager();
            var stations = _manager.GetStations(User.Identity.Name);
            if (stations == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(searchBoxText))
            {
                return PartialView(stations);
            }
            List<StationModel> resultSearch;
            if (string.Equals(searchBoxText, "onlyActivateStationAndNothingMore"))
            {
                resultSearch = stations.FindAll(s => s.Active);
            }
            else
            {
                resultSearch = stations.FindAll(s => s.Name.Length >= searchBoxText.Length
                                                      && s.Name
                                                          .Substring(0, searchBoxText.Length)
                                                          .Trim()
                                                          .ToLower()
                                                          .Contains(searchBoxText.Trim().ToLower()));
            }
            var searcStations = resultSearch.Count > 100
                          ? resultSearch.GetRange(0, 100)
                                        .ToList()
                          : resultSearch.GetRange(0, resultSearch.Count)
                                        .ToList();
            return PartialView(searcStations);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public PartialViewResult SelectStation(string stationId)
        {
            _manager = new ContextManager();
            var city = _manager.GetUserRoleName(User.Identity.Name) ?? "ufa";
            var station = _manager.GetStation(stationId);
            if (station == null)
            {
                return PartialView();
            }
            ContentAddViewModel = new ContentAddViewModel(station.Id, new Content()
            {
                ContentType = ContentType.TEXT,
                InnerContent = "",
                TimeOut = 0,
                Id = Guid.NewGuid()
                         .ToString()
            }, _contentTypes);
            _moduleTypes = _manager.GetModuleTypes();
            _selectedModuleTypeId = station.InformationTable
                                          ?.ModuleType
                                          ?.Id ?? "0";
            Station = new StationViewModel(station, _moduleTypes, _contentTypes, _selectedModuleTypeId, city);
            InformationTableViewModel = Station?.OptionsAndPreviewModel?.InformationTablePreview;
            ViewData["timeOutNextContent"] = GetTimeOutNextContent(Station, ContainerClassType.STATION, out var cssClass, out var centralPosition);
            ViewData["CssClass"] = cssClass;
            ViewData["CentralPosition"] = centralPosition;
            ViewData["stationId"] = station.Id;
            ViewData["informationTableId"] = station.InformationTable?.Id;
            return PartialView(Station);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult SaveChangeOptions(string stationId, string informationTableId, int widthWithModules = 0, int heightWithModules = 0,
                                                   int rowCount = 0, int timeOutPredictShow = 0)
        {
            var contextManager = new ContextManager();
            var city = _manager.GetUserRoleName(User.Identity.Name);
            contextManager.ChangeInformationTable(informationTableId, new InformationTable()
            {
                HeightWithModule = heightWithModules,
                RowCount = rowCount,
                WidthWithModule = widthWithModules
            });
            var station = contextManager.GetStation(stationId);
            _selectedModuleTypeId = station?.InformationTable
                                           ?.ModuleType
                                           ?.Id ?? "0";
            OptionsAndPreviewViewModel = new OptionsAndPreviewViewModel(station, _moduleTypes,
                                                                        _contentTypes, _selectedModuleTypeId, city);
            InformationTableViewModel = OptionsAndPreviewViewModel?.InformationTablePreview;
            OptionsViewModel = OptionsAndPreviewViewModel?.Options;
            ViewData["timeOutNextContent"] = GetTimeOutNextContent(OptionsAndPreviewViewModel, ContainerClassType.OPTION, out var cssClass, out var centralPosition);
            ViewData["CssClass"] = cssClass;
            ViewData["CentralPosition"] = centralPosition;
            ViewData["stationId"] = station?.Id;
            ViewData["informationTableId"] = station?.InformationTable?.Id;
            return PartialView("PreviewAndOptionsBlock", OptionsAndPreviewViewModel);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public PartialViewResult SelectModuleType(string parametr)
        {
            var parametrList = parametr.Split(';');
            _selectedModuleTypeId = parametrList[0];
            var informationTableId = parametrList[2];
            var stationId = parametrList[3];
            var contextManager = new ContextManager();
            var city = _manager.GetUserRoleName(User.Identity.Name);
            contextManager.SetModuleType(informationTableId, _selectedModuleTypeId);
            var station = contextManager.GetStation(stationId);
            OptionsAndPreviewViewModel = new OptionsAndPreviewViewModel(station, _moduleTypes,
                                                                        _contentTypes, _selectedModuleTypeId, city);
            ViewData["timeOutNextContent"] = GetTimeOutNextContent(OptionsAndPreviewViewModel, ContainerClassType.OPTION, out var cssClass, out var centralPosition);
            ViewData["CssClass"] = cssClass;
            ViewData["CentralPosition"] = centralPosition;
            return PartialView("PreviewAndOptionsBlock", OptionsAndPreviewViewModel);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public PartialViewResult SelectContentType(string parametr)
        {
            var index = parametr.Split(';')[0];
            ContentAddViewModel.SelectContentType(index);
            return PartialView(ContentAddViewModel);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ActivateInformationTable(string stationId, string isActive)
        {
            _manager = new ContextManager();
            var city = _manager.GetUserRoleName(User.Identity.Name);
            var station = _manager.ActivateInformationTable(stationId);
            _moduleTypes = _manager.GetModuleTypes();
            Station = new StationViewModel(station, _moduleTypes, _contentTypes, (_moduleTypes?.FirstOrDefault()?.Id ?? ""), city);
            InformationTableViewModel = Station?.OptionsAndPreviewModel
                                               ?.InformationTablePreview;
            return View("SelectStation", Station);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DeactivateInformationTable(string stationId)
        {
            _manager = new ContextManager();
            var city = _manager.GetUserRoleName(User.Identity.Name);
            var station = _manager.DeactivateInformationTable(stationId);
            Station = new StationViewModel(station, _moduleTypes, _contentTypes, _selectedModuleTypeId, city);
            return View("SelectStation", Station);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public PartialViewResult CreateAdditionalContent(string stationId, ContentAddViewModel model)
        {
            _manager = new ContextManager();
            var city = _manager.GetUserRoleName(User.Identity.Name);
            var contentType = _contentTypes.Find(c => string.Equals(((int)c).ToString(), model.SelectedContentType));
            var content = new Content()
            {
                ContentType = contentType,
                Id = model.ContentId,
                InnerContent = model.InnerContent,
                TimeOut = model.TimeOut
            };
            _manager.CreateContent(stationId, content);
            var station = _manager.GetStation(stationId);
            var sModuleType = station?.InformationTable
                                     ?.ModuleType?.Id ?? "";
            OptionsViewModel = new OptionsViewModel(station, _moduleTypes, _contentTypes, sModuleType, city);
            InformationTableViewModel = new InformationTableViewModel(station?.InformationTable, station?.Id ?? "", station?.InformationTable
                                                                                                                          ?.RowCount ?? 0, city);
            return PartialView("InformationTableOptions", OptionsViewModel);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ContentAdd(string stationId)
        {
            var content = new Content()
            {
                ContentType = ContentType.TEXT,
                Id = Guid.NewGuid()
                         .ToString(),
                InnerContent = "",
                TimeOut = 0
            };
            ContentAddViewModel = new ContentAddViewModel(stationId, content, _contentTypes);
            return View("ContentAdd", ContentAddViewModel);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public PartialViewResult ContentOption(string contentId, string stationId, string contentType, string innerContent, int contentTimeout)
        {
            var newContent = new Content()
            {
                InnerContent = contentType == "0"
                    ? stationId
                    : innerContent,
                TimeOut = contentTimeout,
                ContentType = contentType == "0"
                            ? ContentType.FORECAST
                            : _contentTypes.Find(f => string.Equals(((int)f).ToString(), contentType))
            };
            _manager = new ContextManager();
            var city = _manager.GetUserRoleName(User.Identity.Name);
            _manager.ChangeContent(contentId, newContent);
            var content = _manager.GetContent(contentId);
            var station = _manager.GetStation(stationId);
            var informationTableId = station?.InformationTable
                                            ?.Id ?? "-1";
            var contentOption = new ContentOption(content, _contentTypes, ((int)content.ContentType).ToString(), stationId, informationTableId);
            OptionsAndPreviewViewModel = new OptionsAndPreviewViewModel(station, _moduleTypes, _contentTypes, _selectedModuleTypeId, city);
            OptionsViewModel = OptionsAndPreviewViewModel?.Options;
            InformationTableViewModel = OptionsAndPreviewViewModel?.InformationTablePreview;
            return PartialView(contentOption);
        }

        //[Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public PartialViewResult CurrentContentView(int index, string informationTableId, string stationId)
        {
            _manager = new ContextManager();
            var informationTable = _manager.GetInformationTable(informationTableId);
            var rowcount = informationTable.RowCount;
            var informationTableViewModel = new InformationTableViewModel(informationTable, stationId, rowcount, "ufa");
            var contents = informationTableViewModel.Contents;
            index++;
            if (contents != null && index >= contents.Count)
            {
                index = 0;
            }
            var modelContent = informationTableViewModel.Contents
                                                         .ElementAt(index);
            var indexNext = index + 1;
            if (contents != null && indexNext >= contents.Count)
            {
                indexNext = 0;
            }
            var nextContent = informationTableViewModel.Contents
                                                        .ElementAt(indexNext);
            ViewData["timeOutNextContent"] = nextContent?.TimeOut ?? 0;
            ViewData["stationId"] = informationTableViewModel.StationId;
            ViewData["informationTableId"] = informationTableViewModel.InformationTableId;
            ViewData["CssClass"] = informationTableViewModel.CssClass ?? "";
            ViewData["CentralPosition"] = ((informationTableViewModel.Height) / 2 - 10) + "px";
            ViewData["WidthTablo"] = informationTableViewModel.Width;
            ViewData["HeightTablo"] = informationTableViewModel.Height;
            return PartialView(modelContent);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public PartialViewResult RemoveContent(string contentId, string stationId)
        {
            _manager = new ContextManager();
            _manager.RemoveContent(stationId, contentId);
            var station = _manager.GetStation(stationId);
            var city = _manager.GetUserRoleName(User.Identity.Name);
            Station = new StationViewModel(station, _moduleTypes, _contentTypes, _selectedModuleTypeId, city);
            OptionsAndPreviewViewModel = Station?.OptionsAndPreviewModel;
            OptionsViewModel = Station?.OptionsAndPreviewModel?.Options;
            InformationTableViewModel = Station?.OptionsAndPreviewModel?.InformationTablePreview;
            ViewData["timeOutNextContent"] = GetTimeOutNextContent(Station, ContainerClassType.STATION, out var cssClass, out var centralPosition);
            ViewData["CssClass"] = cssClass;
            ViewData["CentralPosition"] = centralPosition;
            return PartialView("InformationTableOptions", OptionsViewModel);
        }

        public FileResult CreateConfigFile(string stationId)
        {
            _manager = new ContextManager();
            var city = _manager.GetUserRoleName(User.Identity.Name) ?? "ufa";
            var station = _manager.GetStation(stationId);
            string accessCode;
            accessCode = string.IsNullOrEmpty(station.AccessCode)
                       ? _manager.SetAccessCode(station.Id)
                       : station.AccessCode;
            var text = "http://92.50.187.210/" + this.HttpContext.Request.ApplicationPath + String.Format("/Home/DisplayInformationTable?stationId={0}&accessCode={1}&city={2}", stationId, accessCode, city);
            var file = System.IO.File.Create("C:/CityStations/cromium.desktop");
            file.Close();
            var streamWriter = new StreamWriter(file.Name);
            streamWriter.WriteLine("[Desktop Entry]");
            streamWriter.WriteLine("Encoding=UTF-8");
            streamWriter.WriteLine("Name=Connect");
            streamWriter.WriteLine("Comment=Checks internet connectivity");
            streamWriter.WriteLine("Exec=/usr/bin/chromium-browser -incognito --noerrdialogs --kiosk " + text);
            streamWriter.Close();
            file = System.IO.File.Open("C:/CityStations/cromium.desktop", FileMode.Open);
            byte[] buffer = new byte[1024];
            file.Read(buffer, 0, 1024);
            file.Close();
            return File(buffer, "text/txt", "C:/CityStations/cromium.desktop");
        }

        public ActionResult DisplayInformationTable(string stationId, string accessCode, string city = "ufa")
        {
            _manager = new ContextManager();
            _moduleTypes = _manager.GetModuleTypes();
            _contentTypes = Enum.GetValues(typeof(ContentType))
                                .Cast<ContentType>()
                                .ToList();
            var station = _manager.GetStation(stationId);
            if (accessCode != station.AccessCode || station.Active == false) return View("Error");
            Station = new StationViewModel(station, _moduleTypes, _contentTypes, _selectedModuleTypeId, city);
            OptionsAndPreviewViewModel = Station?.OptionsAndPreviewModel;
            InformationTableViewModel = OptionsAndPreviewViewModel?.InformationTablePreview;
            OptionsViewModel = OptionsAndPreviewViewModel?.Options;
            ViewData["timeOutNextContent"] = GetTimeOutNextContent(Station, ContainerClassType.STATION, out _, out _);
            ViewData["stationId"] = Station?.StationId;
            ViewData["InformationTable"] = Station?.OptionsAndPreviewModel?.InformationTablePreview;
            ViewData["informationTableId"] = Station?.OptionsAndPreviewModel?.InformationTablePreview?.InformationTableId ?? "-1";
            ViewData["CssClass"] = Station?.OptionsAndPreviewModel?.InformationTablePreview?.CssClass;
            ViewData["CentralPosition"] = (((Station?.OptionsAndPreviewModel?.InformationTablePreview?.Height ?? 0) / 2) - 10) + "px";
            ViewData["WidthTablo"] = InformationTableViewModel?.Width ?? 0;
            ViewData["HeightTablo"] = InformationTableViewModel?.Height ?? 0;
            return View();
        }

        [HttpPost]
        public PartialViewResult Upload(HttpPostedFileBase upload)
        {
            if (upload != null)
            {
                string fileName = Path.GetFileName(upload.FileName);
                upload.SaveAs(Server.MapPath("~/Files/" + fileName));
            }
            return PartialView("FileLoad");
        }

        private int GetTimeOutNextContent(object container, ContainerClassType key, out string cssClass, out string centralPosition)
        {
            int timeOutNextContent;
            cssClass = "";
            centralPosition = "";
            IContent currentContent = null;
            int contentsCount = -1;
            if (key == ContainerClassType.OPTION)
            {
                currentContent = ((OptionsAndPreviewViewModel)container)?.InformationTablePreview
                                                                         ?.CurrentContent;
                cssClass = ((OptionsAndPreviewViewModel)container)?.InformationTablePreview
                                                                   .CssClass ?? "";
                centralPosition = ((((OptionsAndPreviewViewModel)container)?.InformationTablePreview
                                    ?.Height ?? 0) / 2 - 10) + "px";
                contentsCount = ((OptionsAndPreviewViewModel)container)?.InformationTablePreview
                                                                       ?.Contents
                                                                       ?.Count ?? 0;
            }
            else if (key == ContainerClassType.STATION)
            {
                currentContent = ((StationViewModel)container)?.OptionsAndPreviewModel
                                                              ?.InformationTablePreview
                                                              ?.CurrentContent;
                cssClass = ((StationViewModel)container)?.OptionsAndPreviewModel
                                                        ?.InformationTablePreview
                                                        ?.CssClass ?? "";
                centralPosition = ((((StationViewModel)container)?.OptionsAndPreviewModel
                                    ?.InformationTablePreview
                                    ?.Height ?? 0) / 2 - 10) + "px";
                contentsCount = ((StationViewModel)container)?.OptionsAndPreviewModel
                                                             ?.InformationTablePreview
                                                             ?.Contents
                                                             ?.Count ?? 0;
            }
            var indexNextContent = (currentContent?.IndexInContent + 1 ?? 0) >= contentsCount
                                 ? 0
                                 : currentContent?.IndexInContent + 1 ?? 0;
            if (contentsCount > 0)
            {

                if (key == ContainerClassType.OPTION)
                {
                    timeOutNextContent = ((OptionsAndPreviewViewModel)container)?.InformationTablePreview
                                                                                ?.Contents
                                                                                ?.ElementAt(indexNextContent)
                                                                                ?.TimeOut ?? 0;
                }
                else
                {
                    timeOutNextContent = ((StationViewModel)container)?.OptionsAndPreviewModel
                                                                      ?.InformationTablePreview
                                                                      ?.Contents
                                                                      ?.ElementAt(indexNextContent)
                                                                      ?.TimeOut ?? 0;
                }
            }
            else
            {
                timeOutNextContent = 0;
            }
            return timeOutNextContent;
        }

        public FileStreamResult GetAudioPredictFree(string stationId)
        {
            IPredictManager manager;
            var streamAudio = new MemoryStream();
            List<StationForecast> predictNotObject;
            if (stationId.Contains("s"))
            {
                manager = new SterlitamakPredictorManager();
                predictNotObject = ((SterlitamakPredictorManager)manager).GetStationForecast(stationId).ToList();
            }
            else
            {
                manager = new UfaPredictorManager();
                predictNotObject = ((UfaPredictorManager)manager).GetStationForecast(stationId).ToList();
            }
            predictNotObject = predictNotObject.Count() > 4
                             ? predictNotObject.GetRange(0, 4)
                             : predictNotObject;
            var text = "Уважаемые пасажиры!" + '\n';
            foreach (var item in predictNotObject)
            {
                var time = item.Arrt != null
                    ? (((int)item.Arrt / 60) == 0 ? "1" : ((int)item.Arrt / 60).ToString())
                    : "";
                var resultTime = "";
                switch (time)
                {
                    case "1":
                        resultTime = "одну минуту";
                        break;
                    case "2":
                        resultTime = "две минуты";
                        break;
                    case "3":
                        resultTime = "три минуты";
                        break;
                    case "4":
                        resultTime = "четыре минуты";
                        break;
                    case "5":
                        resultTime = "пять минут";
                        break;
                    case "6":
                        resultTime = "шесть минут";
                        break;
                    case "7":
                        resultTime = "семь минут";
                        break;
                    case "8":
                        resultTime = "восемь минут";
                        break;
                    case "9":
                        resultTime = "девять минут";
                        break;
                    case "10":
                        resultTime = "десять минут";
                        break;
                    case "11":
                        resultTime = "одинадцать минут";
                        break;
                    case "12":
                        resultTime = "двенадцать минут";
                        break;
                    case "13":
                        resultTime = "тринадцать минут";
                        break;
                    case "14":
                        resultTime = "четырнадцать минут";
                        break;
                    case "15":
                        resultTime = "пятнадцать минут";
                        break;
                    case "16":
                        resultTime = "шестнадцать минут";
                        break;
                    case "17":
                        resultTime = "семнадцать минут";
                        break;
                    case "18":
                        resultTime = "восемнадцать минут";
                        break;
                    case "19":
                        resultTime = "девятнадцать минут";
                        break;
                    case "20":
                        resultTime = "двадцать минут";
                        break;
                }
                text += $"Через {resultTime} , ожидаем прибытие маршрута номер {item.Rnum}.";
                text += '\n';
            }
            using (SpeechSynthesizer synth = new SpeechSynthesizer())
            {
                //var mSoundPlayer = new SoundPlayer();
                synth.SetOutputToWaveStream(streamAudio);
                synth.Speak(text);
                streamAudio.Position = 0;
                synth.SetOutputToNull();
            }
            return new FileStreamResult(streamAudio, "audio/wav");
        }

        private char IsHaveCharacter(string input, out string str)
        {
            char result = ' ';
            str = "";
            foreach (var item in input)
            {
                if (!('0' < item && item < '9'))
                {
                    result = item;
                    break;
                }
            }
            input.ToList()
                 .RemoveAll(c => !(c < '9' && c > '0'));
            foreach (var ch in input)
            {
                str += ch;
            }
            return result;
        }

        public async Task<FileStreamResult> GetAudioPredictYa(string stationId)
        {
            const string iamKey = "AQVNxZGa5ljWbloj1cO4uRGJCMrLtYGigczIIjpO";
            IPredictManager manager;
            List<StationForecast> predictNotObject;
            if (stationId.Contains("s"))
            {
                manager = new SterlitamakPredictorManager();
                predictNotObject = ((SterlitamakPredictorManager)manager).GetStationForecast(stationId).ToList();
            }
            else
            {
                manager = new UfaPredictorManager();
                predictNotObject = ((UfaPredictorManager)manager).GetStationForecast(stationId).ToList();
            }
            predictNotObject = predictNotObject.Count() > 4
                             ? predictNotObject.GetRange(0, 4)
                             : predictNotObject;
            var text = "Уважаемые пасажиры!" + '\n';
            foreach (var item in predictNotObject)
            {
                var time = item.Arrt != null
                    ? (((int)item.Arrt / 60) == 0 ? "1" : ((int)item.Arrt / 60).ToString())
                    : "";
                var resultTime = "";
                switch (time)
                {
                    case "1":
                        resultTime = "одну минуту";
                        break;
                    case "2":
                        resultTime = "две минуты";
                        break;
                    case "3":
                        resultTime = "три минуты";
                        break;
                    case "4":
                        resultTime = "четыре минуты";
                        break;
                    case "5":
                        resultTime = "пять минут";
                        break;
                    case "6":
                        resultTime = "шесть минут";
                        break;
                    case "7":
                        resultTime = "семь минут";
                        break;
                    case "8":
                        resultTime = "восемь минут";
                        break;
                    case "9":
                        resultTime = "девять минут";
                        break;
                    case "10":
                        resultTime = "десять минут";
                        break;
                    case "11":
                        resultTime = "одинадцать минут";
                        break;
                    case "12":
                        resultTime = "двенадцать минут";
                        break;
                    case "13":
                        resultTime = "тринадцать минут";
                        break;
                    case "14":
                        resultTime = "четырнадцать минут";
                        break;
                    case "15":
                        resultTime = "пятнадцать минут";
                        break;
                    case "16":
                        resultTime = "шестнадцать минут";
                        break;
                    case "17":
                        resultTime = "семнадцать минут";
                        break;
                    case "18":
                        resultTime = "восемнадцать минут";
                        break;
                    case "19":
                        resultTime = "девятнадцать минут";
                        break;
                    case "20":
                        resultTime = "двадцать минут";
                        break;
                }
                /*732*/
                var charx = IsHaveCharacter(item.Rnum, out var str);
                text += $"Через {resultTime} , ожидаем прибытие маршрута номер {str} {charx}.";
                text += '\n';
            }
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Api-Key " + iamKey);
            var values = new Dictionary<string, string>
            {
                {"text",text },
                {"lang","ru-RU" },
            };
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("https://tts.api.cloud.yandex.net/speech/v1/tts:synthesize", content);
            var resultstream = (MemoryStream)await response.Content.ReadAsStreamAsync();
            return new FileStreamResult(resultstream, "application / ogg");
        }
    }
}

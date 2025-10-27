using Microsoft.AspNetCore.Mvc;
using SurveyWeb.Data.Models;
using SurveyWeb.Services.Interfaces;
using QRCoder;

namespace SurveyWeb.Controllers
{
    public class SurveyController : Controller
    {
        private readonly ISurveyService _surveyService;

        public SurveyController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        // GET /Survey
        public async Task<IActionResult> Index()
        {
            var surveys = await _surveyService.GetAllAsync();
            return View(surveys);
        }

        // GET /Survey/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST /Survey/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(survey model)
        {
            // Remove validation cho navigation properties
            ModelState.Remove("owner");
            
            if (!ModelState.IsValid)
            {
                // Log errors để debug
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
                return View(model);
            }

            try
            {
                model._id = Guid.NewGuid();
                model.createdAt = DateTime.UtcNow;
                
                if (string.IsNullOrEmpty(model.status))
                {
                    model.status = "draft";
                }

                await _surveyService.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log chi tiết exception
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                ModelState.AddModelError("", "Lỗi khi tạo khảo sát: " + ex.Message);
                if (ex.InnerException != null)
                {
                    ModelState.AddModelError("", "Chi tiết: " + ex.InnerException.Message);
                }
                return View(model);
            }
        }

        public sealed record ShareViewModel(Guid Id, string Title, string Link);

        // GET /Survey/Share/{id}
        [HttpGet]
        public async Task<IActionResult> Share(Guid id)
        {
            var survey = await _surveyService.GetAsync(id);
            if (survey == null) return NotFound("Survey not found");

            // Absolute link to public "Take" endpoint
            var link = Url.Action(
                action: nameof(Take),
                controller: "Survey",
                values: new { id },
                protocol: Request.Scheme
            ) ?? string.Empty;

            var vm = new ShareViewModel(id, survey.title ?? "(untitled)", link);
            return View(vm);
        }

        // GET /Survey/Qr/{id} -> returns a PNG QR for the public link
        [HttpGet]
        public IActionResult Qr(Guid id)
        {
            var link = Url.Action(
                action: nameof(Take),
                controller: "Survey",
                values: new { id },
                protocol: Request.Scheme
            ) ?? string.Empty;

            var generator = new QRCodeGenerator();
            var data = generator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
            var png = new PngByteQRCode(data).GetGraphic(pixelsPerModule: 8);
            return File(png, "image/png");
        }

        // Public landing page for respondents (stub – customize to render your fill UI)
        // GET /Survey/Take/{id}
        [HttpGet]
        public async Task<IActionResult> Take(Guid id)
        {
            var survey = await _surveyService.GetAsync(id);
            if (survey == null) return NotFound("Survey not found");

            // Optional: gate by status
            // if (!string.Equals(survey.status, "active", StringComparison.OrdinalIgnoreCase))
            //     return BadRequest("Survey is not active.");

            return View(survey);
        }
    }
}

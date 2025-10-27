using Microsoft.AspNetCore.Mvc;
using SurveyWeb.Data.Models;
using SurveyWeb.Services.Interfaces;

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
    }
}

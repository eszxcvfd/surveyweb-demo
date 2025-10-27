using Microsoft.AspNetCore.Mvc;
using SurveyWeb.Data.Models;
using SurveyWeb.Services.Interfaces;
using System.Diagnostics;

namespace SurveyWeb.Controllers
{
    public class QuestionController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly ISurveyService _surveyService;

        public QuestionController(IQuestionService questionService, ISurveyService surveyService)
        {
            _questionService = questionService;
            _surveyService = surveyService;
        }

        // GET /Question/List/{surveyId}
        [HttpGet]
        public async Task<IActionResult> List(Guid surveyId)
        {
            var survey = await _surveyService.GetAsync(surveyId);
            if (survey == null)
            {
                return NotFound("Survey not found");
            }

            var questions = await _questionService.GetSurveyQuestionsAsync(surveyId);

            var vm = new QuestionListViewModel
            {
                SurveyId = surveyId,
                SurveyTitle = survey.title,
                Questions = questions.ToList()
            };

            return View(vm);
        }

        // GET /Question/Create/{surveyId}
        [HttpGet]
        public async Task<IActionResult> Create(Guid surveyId)
        {
            var survey = await _surveyService.GetAsync(surveyId);
            
            if (survey == null)
            {
                return NotFound($"Survey not found with ID: {surveyId}");
            }

            var questionTypes = await _questionService.GetQuestionTypesAsync();

            var vm = new QuestionCreateViewModel
            {
                SurveyId = surveyId,
                SurveyTitle = survey.title,
                AvailableTypes = questionTypes
                    .Select(qt => new QuestionTypeOption
                    {
                        Id = qt.questionTypeId,
                        Name = qt.displayName,
                        Code = qt.internalCode,
                        SupportsOptions = qt.supportsOptions
                    })
                    .ToList(),
                IsRequired = false,
                Options = new List<QuestionOptionItem>
                {
                    new QuestionOptionItem { OrderNo = 1, Text = "" },
                    new QuestionOptionItem { OrderNo = 2, Text = "" }
                }
            };

            return View(vm);
        }

        // POST /Question/Create/{surveyId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid surveyId, QuestionCreatePostModel form)
        {
            if (!ModelState.IsValid)
            {
                var survey = await _surveyService.GetAsync(surveyId);
                var questionTypes = await _questionService.GetQuestionTypesAsync();

                var vmRetry = new QuestionCreateViewModel
                {
                    SurveyId = surveyId,
                    SurveyTitle = survey?.title ?? "",
                    Text = form.Text,
                    OrderNo = form.OrderNo,
                    IsRequired = form.IsRequired,
                    QuestionTypeId = form.QuestionTypeId,
                    AvailableTypes = questionTypes.Select(qt => new QuestionTypeOption
                    {
                        Id = qt.questionTypeId,
                        Name = qt.displayName,
                        Code = qt.internalCode,
                        SupportsOptions = qt.supportsOptions
                    }).ToList(),
                    Options = (form.Options ?? new List<string>())
                        .Select((o, i) => new QuestionOptionItem { OrderNo = i + 1, Text = o ?? "" })
                        .ToList(),
                    MinValue = form.MinValue,
                    MaxValue = form.MaxValue,
                    Step = form.Step,
                    MaxChars = form.MaxChars
                };

                return View(vmRetry);
            }

            // In Create POST, just after ModelState.IsValid check passes and before creating entity:
            var all = await _questionService.GetSurveyQuestionsAsync(surveyId);
            if (all.Any(x => x.orderNo == form.OrderNo))
            {
                ModelState.AddModelError("OrderNo", "Thứ tự này đã được sử dụng. Vui lòng chọn số khác.");
                var survey = await _surveyService.GetAsync(surveyId);
                var questionTypes = await _questionService.GetQuestionTypesAsync();
                var vmRetry = new QuestionCreateViewModel
                {
                    SurveyId = surveyId,
                    SurveyTitle = survey?.title ?? "",
                    Text = form.Text,
                    OrderNo = form.OrderNo,
                    IsRequired = form.IsRequired,
                    QuestionTypeId = form.QuestionTypeId,
                    AvailableTypes = questionTypes.Select(qt => new QuestionTypeOption
                    {
                        Id = qt.questionTypeId,
                        Name = qt.displayName,
                        Code = qt.internalCode,
                        SupportsOptions = qt.supportsOptions
                    }).ToList(),
                    Options = (form.Options ?? new List<string>()).Select((o, i) => new QuestionOptionItem { OrderNo = i + 1, Text = o ?? "" }).ToList(),
                    MinValue = form.MinValue,
                    MaxValue = form.MaxValue,
                    Step = form.Step,
                    MaxChars = form.MaxChars
                };
                return View(vmRetry);
            }

            try
            {
                var entity = new question
                {
                    _id = Guid.Empty,
                    text = form.Text ?? string.Empty,
                    orderNo = form.OrderNo,
                    isRequired = form.IsRequired,
                    questionTypeId = form.QuestionTypeId,
                    type = form.QuestionTypeCode ?? "text",
                    minValue = form.MinValue,
                    maxValue = form.MaxValue,
                    step = form.Step,
                    maxChars = form.MaxChars,
                    // new fields
                    allowedMime = form.AllowedMime,
                    maxFiles = form.MaxFiles,
                    maxFileSizeMB = form.MaxFileSizeMB,
                    matrixRowsJson = form.MatrixRowsJson,
                    matrixColsJson = form.MatrixColsJson,
                    scaleLabelsJson = form.ScaleLabelsJson,
                    aiProbeEnabled = form.AiProbeEnabled
                };

                var createdQuestion = await _questionService.CreateQuestionAsync(surveyId, entity);

                if (form.Options != null && form.Options.Any())
                {
                    await _questionService.AddQuestionOptionsAsync(createdQuestion._id, form.Options);
                }

                TempData["SuccessMessage"] = "Đã thêm câu hỏi thành công!";
                return RedirectToAction("List", new { surveyId = surveyId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi tạo câu hỏi: " + ex.Message);

                var survey = await _surveyService.GetAsync(surveyId);
                var questionTypes = await _questionService.GetQuestionTypesAsync();

                var vmError = new QuestionCreateViewModel
                {
                    SurveyId = surveyId,
                    SurveyTitle = survey?.title ?? "",
                    Text = form.Text,
                    OrderNo = form.OrderNo,
                    IsRequired = form.IsRequired,
                    QuestionTypeId = form.QuestionTypeId,
                    AvailableTypes = questionTypes.Select(qt => new QuestionTypeOption
                    {
                        Id = qt.questionTypeId,
                        Name = qt.displayName,
                        Code = qt.internalCode,
                        SupportsOptions = qt.supportsOptions
                    }).ToList(),
                    Options = (form.Options ?? new List<string>())
                        .Select((o, i) => new QuestionOptionItem { OrderNo = i + 1, Text = o ?? "" })
                        .ToList(),
                    MinValue = form.MinValue,
                    MaxValue = form.MaxValue,
                    Step = form.Step,
                    MaxChars = form.MaxChars
                };

                return View(vmError);
            }
        }

        // GET /Question/Edit/{surveyId}/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid surveyId, Guid id)
        {
            var survey = await _surveyService.GetAsync(surveyId);
            if (survey == null) return NotFound("Survey not found");

            var q = await _questionService.GetQuestionByIdAsync(id);
            if (q == null) return NotFound("Question not found");

            var questionTypes = await _questionService.GetQuestionTypesAsync();
            var options = await _questionService.GetQuestionOptionsAsync(id); // trả về List<string> hoặc list model

            var vm = new QuestionEditViewModel
            {
                QuestionId = q._id,
                SurveyId = surveyId,
                SurveyTitle = survey.title,
                Text = q.text,
                OrderNo = q.orderNo,
                IsRequired = q.isRequired,
                QuestionTypeId = q.questionTypeId,
                AvailableTypes = questionTypes.Select(t => new QuestionTypeOption
                {
                    Id = t.questionTypeId,
                    Name = t.displayName,
                    Code = t.internalCode,
                    SupportsOptions = t.supportsOptions
                }).ToList(),
                MinValue = q.minValue,
                MaxValue = q.maxValue,
                Step = q.step,
                MaxChars = q.maxChars,
                AllowedMime = q.allowedMime,
                MaxFiles = q.maxFiles,
                MaxFileSizeMB = q.maxFileSizeMB,
                MatrixRowsJson = q.matrixRowsJson,
                MatrixColsJson = q.matrixColsJson,
                ScaleLabelsJson = q.scaleLabelsJson,
                AiProbeEnabled = q.aiProbeEnabled,
                Options = (options ?? new List<string>())
                    .Select((o, i) => new QuestionOptionItem { OrderNo = i + 1, Text = o })
                    .ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid surveyId, Guid id, QuestionEditPostModel form)
        {
            if (!ModelState.IsValid)
            {
                var survey = await _surveyService.GetAsync(surveyId);
                var questionTypes = await _questionService.GetQuestionTypesAsync();

                var vmRetry = new QuestionEditViewModel
                {
                    QuestionId = id,
                    SurveyId = surveyId,
                    SurveyTitle = survey?.title ?? "",
                    Text = form.Text,
                    OrderNo = form.OrderNo,
                    IsRequired = form.IsRequired,
                    QuestionTypeId = form.QuestionTypeId,
                    AvailableTypes = questionTypes.Select(qt => new QuestionTypeOption
                    {
                        Id = qt.questionTypeId,
                        Name = qt.displayName,
                        Code = qt.internalCode,
                        SupportsOptions = qt.supportsOptions
                    }).ToList(),
                    MinValue = form.MinValue,
                    MaxValue = form.MaxValue,
                    Step = form.Step,
                    MaxChars = form.MaxChars,
                    AllowedMime = form.AllowedMime,
                    MaxFiles = form.MaxFiles,
                    MaxFileSizeMB = form.MaxFileSizeMB,
                    MatrixRowsJson = form.MatrixRowsJson,
                    MatrixColsJson = form.MatrixColsJson,
                    ScaleLabelsJson = form.ScaleLabelsJson,
                    AiProbeEnabled = form.AiProbeEnabled,
                    Options = (form.Options ?? new List<string>())
                        .Select((o, i) => new QuestionOptionItem { OrderNo = i + 1, Text = o ?? "" })
                        .ToList()
                };

                return View(vmRetry);
            }

            // In Edit POST, BEFORE calling service: validate unique OrderNo
            var allInSurvey = await _questionService.GetSurveyQuestionsAsync(surveyId);
            if (allInSurvey.Any(x => x.orderNo == form.OrderNo && x._id != id))
            {
                ModelState.AddModelError("OrderNo", "Thứ tự này đã được sử dụng. Vui lòng chọn số khác.");
                var survey = await _surveyService.GetAsync(surveyId);
                var questionTypes = await _questionService.GetQuestionTypesAsync();
                var vmRetry = new QuestionEditViewModel
                {
                    QuestionId = id,
                    SurveyId = surveyId,
                    SurveyTitle = survey?.title ?? "",
                    Text = form.Text,
                    OrderNo = form.OrderNo,
                    IsRequired = form.IsRequired,
                    QuestionTypeId = form.QuestionTypeId,
                    AvailableTypes = questionTypes.Select(qt => new QuestionTypeOption
                    {
                        Id = qt.questionTypeId,
                        Name = qt.displayName,
                        Code = qt.internalCode,
                        SupportsOptions = qt.supportsOptions
                    }).ToList(),
                    MinValue = form.MinValue,
                    MaxValue = form.MaxValue,
                    Step = form.Step,
                    MaxChars = form.MaxChars,
                    AllowedMime = form.AllowedMime,
                    MaxFiles = form.MaxFiles,
                    MaxFileSizeMB = form.MaxFileSizeMB,
                    MatrixRowsJson = form.MatrixRowsJson,
                    MatrixColsJson = form.MatrixColsJson,
                    ScaleLabelsJson = form.ScaleLabelsJson,
                    AiProbeEnabled = form.AiProbeEnabled,
                    Options = (form.Options ?? new List<string>()).Select((o, i) => new QuestionOptionItem { OrderNo = i + 1, Text = o ?? "" }).ToList()
                };
                return View(vmRetry);
            }

            try
            {
                // Cập nhật phần thông tin câu hỏi chính (bao gồm type + cấu hình)
                // THÊM CÁC THAM SỐ MỚI VÀO ĐÂY
                    var updated = await _questionService.UpdateQuestionAsync(
                    id,
                    form.Text,
                    form.OrderNo,
                    form.IsRequired,
                    form.QuestionTypeId,
                    form.QuestionTypeCode,
                    form.MinValue,
                    form.MaxValue,
                    form.Step,
                    form.MaxChars,
                    form.AllowedMime,        // ← Thêm
                    form.MaxFiles,           // ← Thêm
                    form.MaxFileSizeMB,      // ← Thêm
                    form.MatrixRowsJson,     // ← Thêm
                    form.MatrixColsJson,     // ← Thêm
                    form.ScaleLabelsJson,    // ← Thêm
                    form.AiProbeEnabled      // ← Thêm
                );

                if (!updated)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy câu hỏi để cập nhật.";
                    return RedirectToAction("List", new { surveyId });
                }

                // Nếu loại hỗ trợ options: thay thế danh sách lựa chọn
                if (form.Options != null)
                {
                    await _questionService.ReplaceQuestionOptionsAsync(id, form.Options);
                }

                TempData["SuccessMessage"] = "Đã cập nhật câu hỏi thành công!";
                return RedirectToAction("List", new { surveyId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật câu hỏi: " + ex.Message);

                var survey = await _surveyService.GetAsync(surveyId);
                var questionTypes = await _questionService.GetQuestionTypesAsync();

                var vmError = new QuestionEditViewModel
                {
                    QuestionId = id,
                    SurveyId = surveyId,
                    SurveyTitle = survey?.title ?? "",
                    Text = form.Text,
                    OrderNo = form.OrderNo,
                    IsRequired = form.IsRequired,
                    QuestionTypeId = form.QuestionTypeId,
                    AvailableTypes = questionTypes.Select(qt => new QuestionTypeOption
                    {
                        Id = qt.questionTypeId,
                        Name = qt.displayName,
                        Code = qt.internalCode,
                        SupportsOptions = qt.supportsOptions
                    }).ToList(),
                    MinValue = form.MinValue,
                    MaxValue = form.MaxValue,
                    Step = form.Step,
                    MaxChars = form.MaxChars,
                    AllowedMime = form.AllowedMime,
                    MaxFiles = form.MaxFiles,
                    MaxFileSizeMB = form.MaxFileSizeMB,
                    MatrixRowsJson = form.MatrixRowsJson,
                    MatrixColsJson = form.MatrixColsJson,
                    ScaleLabelsJson = form.ScaleLabelsJson,
                    AiProbeEnabled = form.AiProbeEnabled,
                    Options = (form.Options ?? new List<string>())
                        .Select((o, i) => new QuestionOptionItem { OrderNo = i + 1, Text = o ?? "" })
                        .ToList()
                };

                return View(vmError);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid surveyId, Guid id)
        {
            try
            {
                var success = await _questionService.DeleteQuestionAsync(id);
                
                if (!success)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy câu hỏi để xóa.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Đã xóa câu hỏi thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa câu hỏi: {ex.Message}";
            }

            return RedirectToAction("List", new { surveyId = surveyId });
        }
    }

    // ViewModels
    public class QuestionListViewModel
    {
        public Guid SurveyId { get; set; }
        public string? SurveyTitle { get; set; }
        public List<question> Questions { get; set; } = new();
    }

    public class QuestionTypeOption
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public bool SupportsOptions { get; set; }
    }

    public class QuestionOptionItem
    {
        public string? Text { get; set; }
        public int OrderNo { get; set; }
        public string? Value { get; set; }
        public double? Score { get; set; }
    }

    public class QuestionCreateViewModel
    {
        public Guid SurveyId { get; set; }
        public string? SurveyTitle { get; set; }
        public string? Text { get; set; }
        public int OrderNo { get; set; }
        public bool IsRequired { get; set; }
        public int? QuestionTypeId { get; set; }
        public List<QuestionTypeOption> AvailableTypes { get; set; } = new();
        public List<QuestionOptionItem> Options { get; set; } = new();
        
        // Rating/Slider specific
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
        public double? Step { get; set; }
        
        // Text specific
        public int? MaxChars { get; set; }
    }

    public class QuestionCreatePostModel
    {
        public string? Text { get; set; }
        public int OrderNo { get; set; }
        public bool IsRequired { get; set; }
        public int? QuestionTypeId { get; set; }
        public string? QuestionTypeCode { get; set; }
        public List<string>? Options { get; set; }
        
        // Additional fields
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
        public double? Step { get; set; }
        public int? MaxChars { get; set; }

        // New: Upload / Matrix / AI
        public string? AllowedMime { get; set; }
        public int? MaxFiles { get; set; }
        public int? MaxFileSizeMB { get; set; }
        public string? MatrixRowsJson { get; set; }
        public string? MatrixColsJson { get; set; }
        public string? ScaleLabelsJson { get; set; }
        public bool AiProbeEnabled { get; set; }
    }

    public class QuestionEditViewModel
    {
        public Guid QuestionId { get; set; }
        public Guid SurveyId { get; set; }
        public string? SurveyTitle { get; set; }
        public string? Text { get; set; }
        public int OrderNo { get; set; }
        public bool IsRequired { get; set; }
        public int? QuestionTypeId { get; set; }
        public List<QuestionTypeOption> AvailableTypes { get; set; } = new();

        // Thêm các trường để UI Edit giống Create
        public List<QuestionOptionItem> Options { get; set; } = new();
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
        public double? Step { get; set; }
        public int? MaxChars { get; set; }

        // New fields for Upload / Matrix / AI
        public string? AllowedMime { get; set; }
        public int? MaxFiles { get; set; }
        public int? MaxFileSizeMB { get; set; }
        public string? MatrixRowsJson { get; set; }
        public string? MatrixColsJson { get; set; }
        public string? ScaleLabelsJson { get; set; }
        public bool AiProbeEnabled { get; set; }
    }

    public class QuestionEditPostModel
    {
        public string? Text { get; set; }
        public int OrderNo { get; set; }
        public bool IsRequired { get; set; }
        public int? QuestionTypeId { get; set; }

        // Thêm các trường cần post về (giống Create)
        public string? QuestionTypeCode { get; set; }
        public List<string>? Options { get; set; }
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
        public double? Step { get; set; }
        public int? MaxChars { get; set; }

        // New fields
        public string? AllowedMime { get; set; }
        public int? MaxFiles { get; set; }
        public int? MaxFileSizeMB { get; set; }
        public string? MatrixRowsJson { get; set; }
        public string? MatrixColsJson { get; set; }
        public string? ScaleLabelsJson { get; set; }
        public bool AiProbeEnabled { get; set; }
    }
}
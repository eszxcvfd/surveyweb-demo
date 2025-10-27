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
                    }).ToList()
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
                    maxChars = form.MaxChars
                };

                var createdQuestion = await _questionService.CreateQuestionAsync(surveyId, entity);
                
                // Thêm options nếu có
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
                    }).ToList()
                };

                return View(vmError);
            }
        }

        // GET /Question/Edit/{surveyId}/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid surveyId, Guid id)
        {
            var survey = await _surveyService.GetAsync(surveyId);
            if (survey == null)
            {
                return NotFound("Survey not found");
            }

            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound("Question not found");
            }

            var questionTypes = await _questionService.GetQuestionTypesAsync();

            var vm = new QuestionEditViewModel
            {
                QuestionId = question._id,
                SurveyId = surveyId,
                SurveyTitle = survey.title,
                Text = question.text,
                OrderNo = question.orderNo,
                IsRequired = question.isRequired,
                QuestionTypeId = question.questionTypeId,
                AvailableTypes = questionTypes
                    .Select(qt => new QuestionTypeOption
                    {
                        Id = qt.questionTypeId,
                        Name = qt.displayName,
                        Code = qt.internalCode,
                        SupportsOptions = qt.supportsOptions
                    })
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
                        Name = qt.displayName
                    }).ToList()
                };

                return View(vmRetry);
            }

            try
            {
                var success = await _questionService.UpdateQuestionAsync(id, form.Text, form.OrderNo, form.IsRequired, form.QuestionTypeId);
                
                if (!success)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy câu hỏi để cập nhật.";
                    return RedirectToAction("List", new { surveyId = surveyId });
                }

                TempData["SuccessMessage"] = "Đã cập nhật câu hỏi thành công!";
                return RedirectToAction("List", new { surveyId = surveyId });
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
                        Name = qt.displayName
                    }).ToList()
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
    }

    public class QuestionEditPostModel
    {
        public string? Text { get; set; }
        public int OrderNo { get; set; }
        public bool IsRequired { get; set; }
        public int? QuestionTypeId { get; set; }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var options = await _questionService.GetQuestionOptionsAsync(id); // strings of current question options

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

            // Câu hỏi đích: chỉ các câu có orderNo lớn hơn câu hiện tại
            var allQuestions = await _questionService.GetSurveyQuestionsAsync(surveyId);
            ViewBag.TargetQuestions = allQuestions
                .Where(x => x.orderNo > q.orderNo)
                .OrderBy(x => x.orderNo)
                .ToList();

            var optionPairs = await _questionService.GetQuestionOptionsWithIdsAsync(id);
            ViewBag.CurrentOptions = optionPairs;

            // Lựa chọn của câu hiện tại (để condition “chọn đáp án cụ thể”)
            ViewBag.CurrentOptionTexts = options?.ToList() ?? new List<string>();

            // Logic
            var (rule, cond) = await _questionService.GetLogicForQuestionWithConditionAsync(id);
            ViewBag.SelectedLogicType = rule?.logicType; // "display" | "skip" | "display_option"
            ViewBag.SelectedTargetQuestionId = rule?.targetId; // với skip/display
            ViewBag.SelectedConditionOperator = cond?._operator; // "answered" | "option_equals" | ...
            ViewBag.SelectedRightOptionId = cond?.optionId;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid surveyId, Guid id, QuestionEditPostModel form)
        {
            if (!ModelState.IsValid)
            {
                var questionTypes = await _questionService.GetQuestionTypesAsync();
                var vmRetry = new QuestionEditViewModel
                {
                    QuestionId = id,
                    SurveyId = surveyId,
                    SurveyTitle = (await _surveyService.GetAsync(surveyId))?.title ?? "",
                    Text = form.Text,
                    OrderNo = form.OrderNo,
                    IsRequired = form.IsRequired,
                    QuestionTypeId = form.QuestionTypeId,
                    AvailableTypes = questionTypes.Select(qt => new QuestionTypeOption
                    {
                        Id = qt.questionTypeId, Name = qt.displayName, Code = qt.internalCode, SupportsOptions = qt.supportsOptions
                    }).ToList(),
                    MinValue = form.MinValue, MaxValue = form.MaxValue, Step = form.Step, MaxChars = form.MaxChars,
                    AllowedMime = form.AllowedMime, MaxFiles = form.MaxFiles, MaxFileSizeMB = form.MaxFileSizeMB,
                    MatrixRowsJson = form.MatrixRowsJson, MatrixColsJson = form.MatrixColsJson, ScaleLabelsJson = form.ScaleLabelsJson,
                    AiProbeEnabled = form.AiProbeEnabled,
                    Options = (form.Options ?? new List<string>()).Select((o, i) => new QuestionOptionItem { OrderNo = i + 1, Text = o ?? "" }).ToList()
                };

                // NẠP LẠI VIEWBAG để form render đầy đủ
                var currentQForList = await _questionService.GetQuestionByIdAsync(id);
                var allQsForList = await _questionService.GetSurveyQuestionsAsync(surveyId);
                ViewBag.TargetQuestions = allQsForList
                    .Where(x => currentQForList == null || x.orderNo > currentQForList.orderNo)
                    .OrderBy(x => x.orderNo)
                    .ToList();
                ViewBag.CurrentOptions = await _questionService.GetQuestionOptionsWithIdsAsync(id);

                return View(vmRetry);
            }

            // In Edit POST, BEFORE calling service: validate unique OrderNo
            var allInSurvey = await _questionService.GetSurveyQuestionsAsync(surveyId);
            if (allInSurvey.Any(x => x.orderNo == form.OrderNo && x._id != id))
            {
                ModelState.AddModelError("OrderNo", "Thứ tự này đã được sử dụng. Vui lòng chọn số khác.");
                var questionTypes = await _questionService.GetQuestionTypesAsync();
                var vmRetry = new QuestionEditViewModel
                {
                    QuestionId = id,
                    SurveyId = surveyId,
                    SurveyTitle = (await _surveyService.GetAsync(surveyId))?.title ?? "",
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

                // NẠP LẠI VIEWBAG
                var currentQForList = await _questionService.GetQuestionByIdAsync(id);
                var allQsForList = await _questionService.GetSurveyQuestionsAsync(surveyId);
                ViewBag.TargetQuestions = allQsForList
                    .Where(x => currentQForList == null || x.orderNo > currentQForList.orderNo)
                    .OrderBy(x => x.orderNo)
                    .ToList();
                ViewBag.CurrentOptions = await _questionService.GetQuestionOptionsWithIdsAsync(id);

                return View(vmRetry);
            }

            try
            {
                // Cập nhật phần thông tin câu hỏi chính (bao gồm type + cấu hình)
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
                    form.AllowedMime,
                    form.MaxFiles,
                    form.MaxFileSizeMB,
                    form.MatrixRowsJson,
                    form.MatrixColsJson,
                    form.ScaleLabelsJson,
                    form.AiProbeEnabled
                );

                if (!updated)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy câu hỏi để cập nhật.";
                    return RedirectToAction("List", new { surveyId });
                }

                // LẤY CÂU HỎI HIỆN TẠI ĐỂ VALIDATE LOGIC
                var currentQ = await _questionService.GetQuestionByIdAsync(id);
                if (currentQ == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy câu hỏi hiện tại.";
                    return RedirectToAction("List", new { surveyId });
                }

                // Nếu loại hỗ trợ options: thay thế danh sách lựa chọn
                var oldOptionsPairs = await _questionService.GetQuestionOptionsWithIdsAsync(id);

                // Nếu loại hỗ trợ options: thay thế danh sách lựa chọn
                if (form.Options != null)
                {
                    await _questionService.ReplaceQuestionOptionsAsync(id, form.Options);
                }

                // Map lại OptionId nếu điều kiện dựa trên đáp án
                Guid? effectiveOptionId = form.RightOptionId;
                var isOptionBased = string.Equals(form.ConditionOperator, "option_equals", StringComparison.OrdinalIgnoreCase)
                                 || string.Equals(form.ConditionOperator, "not_equals", StringComparison.OrdinalIgnoreCase);

                if (isOptionBased && form.RightOptionId.HasValue)
                {
                    // Tìm text của option cũ theo RightOptionId
                    var oldSelected = (oldOptionsPairs ?? Array.Empty<SurveyWeb.Services.Interfaces.QuestionOptionPair>())
                        .FirstOrDefault(o => o.Id == form.RightOptionId.Value);

                    // Lấy danh sách option mới sau Replace
                    var newOptionsPairs = await _questionService.GetQuestionOptionsWithIdsAsync(id);

                    // Map theo Text (không phân biệt hoa/thường)
                    var mapped = newOptionsPairs.FirstOrDefault(o =>
                        string.Equals(o.Text?.Trim(), oldSelected.Text?.Trim(), StringComparison.OrdinalIgnoreCase));

                    if (mapped != default)
                    {
                        effectiveOptionId = mapped.Id;
                    }
                    else
                    {
                        ModelState.AddModelError("RightOptionId", "Đáp án đã bị thay đổi hoặc xóa sau khi chỉnh sửa options. Vui lòng chọn lại.");
                        var questionTypes = await _questionService.GetQuestionTypesAsync();
                        var vmRetry = new QuestionEditViewModel
                        {
                            QuestionId = id,
                            SurveyId = surveyId,
                            SurveyTitle = (await _surveyService.GetAsync(surveyId))?.title ?? "",
                            Text = form.Text,
                            OrderNo = form.OrderNo,
                            IsRequired = form.IsRequired,
                            QuestionTypeId = form.QuestionTypeId,
                            AvailableTypes = questionTypes.Select(qt => new QuestionTypeOption
                            {
                                Id = qt.questionTypeId, Name = qt.displayName, Code = qt.internalCode, SupportsOptions = qt.supportsOptions
                            }).ToList(),
                            MinValue = form.MinValue, MaxValue = form.MaxValue, Step = form.Step, MaxChars = form.MaxChars,
                            AllowedMime = form.AllowedMime, MaxFiles = form.MaxFiles, MaxFileSizeMB = form.MaxFileSizeMB,
                            MatrixRowsJson = form.MatrixRowsJson, MatrixColsJson = form.MatrixColsJson, ScaleLabelsJson = form.ScaleLabelsJson,
                            AiProbeEnabled = form.AiProbeEnabled,
                            Options = (form.Options ?? new List<string>()).Select((o, i) => new QuestionOptionItem { OrderNo = i + 1, Text = o ?? "" }).ToList()
                        };
                        var allQuestions = await _questionService.GetSurveyQuestionsAsync(surveyId);
                        ViewBag.TargetQuestions = allQuestions.Where(x => x.orderNo > currentQ.orderNo).OrderBy(x => x.orderNo).ToList();
                        ViewBag.CurrentOptions = await _questionService.GetQuestionOptionsWithIdsAsync(id);
                        return View(vmRetry);
                    }
                }

                // Logic processing
                switch ((form.LogicType ?? "").Trim().ToLowerInvariant())
                {
                    case "display":
                    {
                        if (form.LogicTargetQuestionId.HasValue)
                        {
                            // Target phải nằm sau
                            var target = await _questionService.GetQuestionByIdAsync(form.LogicTargetQuestionId.Value);
                            if (target == null || target.orderNo <= currentQ.orderNo)
                            {
                                ModelState.AddModelError("LogicTargetQuestionId", "Câu hỏi đích phải có thứ tự lớn hơn câu hiện tại.");
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
                                        Id = qt.questionTypeId, Name = qt.displayName, Code = qt.internalCode, SupportsOptions = qt.supportsOptions
                                    }).ToList(),
                                    MinValue = form.MinValue, MaxValue = form.MaxValue, Step = form.Step, MaxChars = form.MaxChars,
                                    AllowedMime = form.AllowedMime, MaxFiles = form.MaxFiles, MaxFileSizeMB = form.MaxFileSizeMB,
                                    MatrixRowsJson = form.MatrixRowsJson,
                                    MatrixColsJson = form.MatrixColsJson,
                                    ScaleLabelsJson = form.ScaleLabelsJson,
                                    AiProbeEnabled = form.AiProbeEnabled,
                                    Options = (form.Options ?? new List<string>()).Select((o, i) => new QuestionOptionItem { OrderNo = i + 1, Text = o ?? "" }).ToList()
                                };
                                // Nạp lại dữ liệu ViewBag để render
                                var allQuestions = await _questionService.GetSurveyQuestionsAsync(surveyId);
                                ViewBag.TargetQuestions = allQuestions.Where(x => x.orderNo > currentQ.orderNo).OrderBy(x => x.orderNo).ToList();
                                ViewBag.CurrentOptions = await _questionService.GetQuestionOptionsWithIdsAsync(id);
                                return View(vmRetry);
                            }

                            // Điều kiện: dùng operator (answered | option_equals)
                            await _questionService.UpsertDisplayLogicAsync(
                                surveyId,
                                form.LogicTargetQuestionId.Value,
                                id,
                                form.ConditionOperator ?? "answered",
                                null, null, null,
                                effectiveOptionId // dùng OptionId đã map
                            );
                        }
                        else
                        {
                            await _questionService.DeleteLogicForQuestionAsync(id);
                        }
                        break;
                    }

                    case "skip":
                    {
                        if (form.LogicTargetQuestionId.HasValue)
                        {
                            var target = await _questionService.GetQuestionByIdAsync(form.LogicTargetQuestionId.Value);
                            if (target == null || target.orderNo <= currentQ.orderNo)
                            {
                                ModelState.AddModelError("LogicTargetQuestionId", "Câu hỏi đích phải có thứ tự lớn hơn câu hiện tại.");
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
                                        Id = qt.questionTypeId, Name = qt.displayName, Code = qt.internalCode, SupportsOptions = qt.supportsOptions
                                    }).ToList(),
                                    MinValue = form.MinValue, MaxValue = form.MaxValue, Step = form.Step, MaxChars = form.MaxChars,
                                    AllowedMime = form.AllowedMime, MaxFiles = form.MaxFiles, MaxFileSizeMB = form.MaxFileSizeMB,
                                    MatrixRowsJson = form.MatrixRowsJson,
                                    MatrixColsJson = form.MatrixColsJson,
                                    ScaleLabelsJson = form.ScaleLabelsJson,
                                    AiProbeEnabled = form.AiProbeEnabled,
                                    Options = (form.Options ?? new List<string>()).Select((o, i) => new QuestionOptionItem { OrderNo = i + 1, Text = o ?? "" }).ToList()
                                };
                                var allQuestions = await _questionService.GetSurveyQuestionsAsync(surveyId);
                                ViewBag.TargetQuestions = allQuestions.Where(x => x.orderNo > currentQ.orderNo).OrderBy(x => x.orderNo).ToList();
                                ViewBag.CurrentOptions = await _questionService.GetQuestionOptionsWithIdsAsync(id);
                                return View(vmRetry);
                            }

                            await _questionService.UpsertSkipLogicAsync(
                                surveyId,
                                id,
                                form.LogicTargetQuestionId.Value,
                                form.ConditionOperator ?? "answered",
                                null, null, null,
                                effectiveOptionId // dùng OptionId đã map
                            );
                        }
                        else
                        {
                            await _questionService.DeleteLogicForQuestionAsync(id);
                        }
                        break;
                    }

                    case "display_option":
                        // Giữ nguyên nếu bạn dùng, hoặc có thể tạm bỏ nếu chưa cần
                        break;

                    default:
                        await _questionService.DeleteLogicForQuestionAsync(id);
                        break;
                }

                TempData["SuccessMessage"] = "Đã cập nhật câu hỏi thành công!";
                return RedirectToAction("List", new { surveyId });
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", "Lỗi khi cập nhật câu hỏi: " + inner);

                var questionTypes = await _questionService.GetQuestionTypesAsync();
                var vmError = new QuestionEditViewModel
                {
                    QuestionId = id,
                    SurveyId = surveyId,
                    SurveyTitle = (await _surveyService.GetAsync(surveyId))?.title ?? "",
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

                // Nạp lại ViewBag để render form đầy đủ
                var currentQForList = await _questionService.GetQuestionByIdAsync(id);
                var allQsForList = await _questionService.GetSurveyQuestionsAsync(surveyId);
                ViewBag.TargetQuestions = allQsForList
                    .Where(x => currentQForList == null || x.orderNo > currentQForList.orderNo)
                    .OrderBy(x => x.orderNo)
                    .ToList();
                ViewBag.CurrentOptions = await _questionService.GetQuestionOptionsWithIdsAsync(id);

                return View(vmError);
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", "Lỗi khi cập nhật câu hỏi: " + inner);

                var questionTypes = await _questionService.GetQuestionTypesAsync();
                var vmError = new QuestionEditViewModel
                {
                    QuestionId = id,
                    SurveyId = surveyId,
                    SurveyTitle = (await _surveyService.GetAsync(surveyId))?.title ?? "",
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

                // Nạp lại ViewBag để render form đầy đủ
                var currentQForList = await _questionService.GetQuestionByIdAsync(id);
                var allQsForList = await _questionService.GetSurveyQuestionsAsync(surveyId);
                ViewBag.TargetQuestions = allQsForList
                    .Where(x => currentQForList == null || x.orderNo > currentQForList.orderNo)
                    .OrderBy(x => x.orderNo)
                    .ToList();
                ViewBag.CurrentOptions = await _questionService.GetQuestionOptionsWithIdsAsync(id);

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

        public string? QuestionTypeCode { get; set; }
        public List<string>? Options { get; set; }
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
        public double? Step { get; set; }
        public int? MaxChars { get; set; }

        // Upload / Matrix / AI
        public string? AllowedMime { get; set; }
        public int? MaxFiles { get; set; }
        public int? MaxFileSizeMB { get; set; }
        public string? MatrixRowsJson { get; set; }
        public string? MatrixColsJson { get; set; }
        public string? ScaleLabelsJson { get; set; }
        public bool AiProbeEnabled { get; set; }

        // Logic
        public string? LogicType { get; set; } // "", "display", "skip", "display_option"
        public Guid? LogicSourceQuestionId { get; set; } // parent/condition question
        public Guid? LogicTargetQuestionId { get; set; } // target jump for skip
        public string? ConditionOperator { get; set; }   // "answered","equals","gt","lt","contains","not_equals"
        public string? RightValueText { get; set; }
        public double? RightValueNumber { get; set; }
        public DateTime? RightValueDate { get; set; }
        public Guid? RightOptionId { get; set; }
        public List<Guid>? OptionIdsToShow { get; set; } // for display_option
    }
}
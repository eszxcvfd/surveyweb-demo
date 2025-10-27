using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class question
{
    public Guid _id { get; set; }

    public Guid surveyId { get; set; }

    public Guid? sectionId { get; set; }

    public string text { get; set; } = null!;

    public string type { get; set; } = null!;

    public bool isRequired { get; set; }

    public int orderNo { get; set; }

    public string? validationRule { get; set; }

    public string? subtype { get; set; }

    public bool hasOtherOption { get; set; }

    public double? minValue { get; set; }

    public double? maxValue { get; set; }

    public double? step { get; set; }

    public string? scaleLabelsJson { get; set; }

    public string? matrixRowsJson { get; set; }

    public string? matrixColsJson { get; set; }

    public bool allowMultiple { get; set; }

    public string? allowedMime { get; set; }

    public int? maxFiles { get; set; }

    public int? maxFileSizeMB { get; set; }

    public string? regexPattern { get; set; }

    public int? maxChars { get; set; }

    public bool aiProbeEnabled { get; set; }

    public string? aggregationFunction { get; set; }

    public string? chartSuggestion { get; set; }

    public string? resultVisibility { get; set; }

    public string? optionOrderInResults { get; set; }

    public string? defaultOptionCoding { get; set; }

    public int? questionTypeId { get; set; }

    public virtual ICollection<answer> answers { get; set; } = new List<answer>();

    public virtual ICollection<logicRuleCondition> logicRuleConditions { get; set; } = new List<logicRuleCondition>();

    public virtual ICollection<questionOption> questionOptions { get; set; } = new List<questionOption>();

    public virtual questionType? questionType { get; set; }

    public virtual section? section { get; set; }

    public virtual survey survey { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class answer
{
    public Guid _id { get; set; }

    public Guid responseId { get; set; }

    public Guid questionId { get; set; }

    public Guid? selectedOptionId { get; set; }

    public string? textValue { get; set; }

    public double? numericValue { get; set; }

    public DateTime? dateValue { get; set; }

    public string? selectedOptionIdsJson { get; set; }

    public string? matrixValuesJson { get; set; }

    public string? fileUrlsJson { get; set; }

    public double? confidence { get; set; }

    public long? durationMs { get; set; }

    public string? exportLabel { get; set; }

    public int? exportOrder { get; set; }

    public string? exportValueText { get; set; }

    public string? exportValueCode { get; set; }

    public double? exportValueScore { get; set; }

    public bool? isValid { get; set; }

    public string? normalizedValue { get; set; }

    public string? bucketLabel { get; set; }

    public virtual question question { get; set; } = null!;

    public virtual response response { get; set; } = null!;

    public virtual questionOption? selectedOption { get; set; }
}

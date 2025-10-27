using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class response
{
    public Guid _id { get; set; }

    public Guid surveyId { get; set; }

    public Guid? respondentId { get; set; }

    public Guid? distributionChannelId { get; set; }

    public DateTime submittedAt { get; set; }

    public string? clientIP { get; set; }

    public string? userAgent { get; set; }

    public string? oneTimeToken { get; set; }

    public double? recaptchaScore { get; set; }

    public string? clientLocale { get; set; }

    public string? deviceType { get; set; }

    public long? randomSeed { get; set; }

    public long? durationMs { get; set; }

    public int? timeSpentSec { get; set; }

    public string? collectorType { get; set; }

    public string? collectorDetails { get; set; }

    public string? ipGeo { get; set; }

    public bool markedFlag { get; set; }

    public DateTime? deletedAt { get; set; }

    public bool isComplete { get; set; }

    public bool isValid { get; set; }

    public string? validationReason { get; set; }

    public virtual ICollection<answer> answers { get; set; } = new List<answer>();

    public virtual distributionChannel? distributionChannel { get; set; }

    public virtual user? respondent { get; set; }

    public virtual survey survey { get; set; } = null!;
}

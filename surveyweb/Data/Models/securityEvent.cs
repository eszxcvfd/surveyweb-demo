using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class securityEvent
{
    public Guid _id { get; set; }

    public Guid? userId { get; set; }

    public Guid? surveyId { get; set; }

    public string type { get; set; } = null!;

    public string? metaJson { get; set; }

    public DateTime createdAt { get; set; }

    public string? reason { get; set; }

    public double? recaptchaScore { get; set; }

    public virtual survey? survey { get; set; }

    public virtual user? user { get; set; }
}

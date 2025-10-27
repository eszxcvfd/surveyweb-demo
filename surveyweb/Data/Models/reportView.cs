using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class reportView
{
    public Guid _id { get; set; }

    public Guid surveyId { get; set; }

    public Guid createdBy { get; set; }

    public string name { get; set; } = null!;

    public string criteriaJson { get; set; } = null!;

    public DateTime createdAt { get; set; }

    public string? visualizationConfig { get; set; }

    public string? metrics { get; set; }

    public string? filters { get; set; }

    public string? groupBy { get; set; }

    public string? crosstabRows { get; set; }

    public string? crosstabCols { get; set; }

    public string? compareBy { get; set; }

    public string? sortBy { get; set; }

    public string? chartType { get; set; }

    public string? colorPalette { get; set; }

    public bool? includeValidCountPerParticipant { get; set; }

    public bool? aiInsightsEnabled { get; set; }

    public string? aiInsightsText { get; set; }

    public string? filterSnapshotName { get; set; }

    public virtual user createdByNavigation { get; set; } = null!;

    public virtual survey survey { get; set; } = null!;
}

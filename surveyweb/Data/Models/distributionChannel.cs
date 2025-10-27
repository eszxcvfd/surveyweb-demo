using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class distributionChannel
{
    public Guid _id { get; set; }

    public Guid surveyId { get; set; }

    public string type { get; set; } = null!;

    public string? publicUrl { get; set; }

    public string? embedCode { get; set; }

    public bool isActive { get; set; }

    public DateTime createdAt { get; set; }

    public string channelType { get; set; } = null!;

    public string? platform { get; set; }

    public string? integrationName { get; set; }

    public bool isTrackingEnabled { get; set; }

    public bool hiddenFieldSupport { get; set; }

    public string? customLinkTemplate { get; set; }

    public bool audienceTargetingEnabled { get; set; }

    public string? audienceProvider { get; set; }

    public string? qrImagePath { get; set; }

    public string? collectorType { get; set; }

    public string? collectorDetails { get; set; }

    public virtual ICollection<response> responses { get; set; } = new List<response>();

    public virtual survey survey { get; set; } = null!;
}

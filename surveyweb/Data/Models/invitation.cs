using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class invitation
{
    public Guid _id { get; set; }

    public Guid surveyId { get; set; }

    public string recipient { get; set; } = null!;

    public string? token { get; set; }

    public string sendMethod { get; set; } = null!;

    public string messageTemplate { get; set; } = null!;

    public string? senderInfo { get; set; }

    public bool trackingEnabled { get; set; }

    public DateTime? openedAt { get; set; }

    public DateTime? clickedAt { get; set; }

    public string? deliveryStatus { get; set; }

    public string? audienceSegment { get; set; }

    public string? customParams { get; set; }

    public string? integrationId { get; set; }

    public string status { get; set; } = null!;

    public virtual survey survey { get; set; } = null!;
}

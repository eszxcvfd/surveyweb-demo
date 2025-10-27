using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class exportJob
{
    public Guid _id { get; set; }

    public Guid surveyId { get; set; }

    public Guid requestedBy { get; set; }

    public string format { get; set; } = null!;

    public string? filterJson { get; set; }

    public DateTime? requestedAt { get; set; }

    public string status { get; set; } = null!;

    public string? filePath { get; set; }

    public bool? includeCharts { get; set; }

    public bool? includeTables { get; set; }

    public string? colorPalette { get; set; }

    public string? locale { get; set; }

    public string? anonymizeMode { get; set; }

    public string? filterSnapshot { get; set; }

    public string? chartTypeOverride { get; set; }

    public DateTime? scheduleTime { get; set; }

    public string? shareDestination { get; set; }

    public string? scope { get; set; }

    public string? selectedResponseIds { get; set; }

    public string? optionCoding { get; set; }

    public string? deliveryMethod { get; set; }

    public string? emailTo { get; set; }

    public string? statusUrl { get; set; }

    public bool? includeHeader { get; set; }

    public bool? includeIndex { get; set; }

    public bool? includeOptionText { get; set; }

    public bool? includeOptionNumber { get; set; }

    public bool? includeOptionScore { get; set; }

    public bool? includeIpAddress { get; set; }

    public bool? includeCollector { get; set; }

    public bool? includeTimeSpent { get; set; }

    public bool? includeDeletedResponses { get; set; }

    public bool? includeGeoLocation { get; set; }

    public string? sheetName { get; set; }

    public int? rowCount { get; set; }

    public long? fileSize { get; set; }

    public DateTime? exportAt { get; set; }

    public virtual user requestedByNavigation { get; set; } = null!;

    public virtual survey survey { get; set; } = null!;
}

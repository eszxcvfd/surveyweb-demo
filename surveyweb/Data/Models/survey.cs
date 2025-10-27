using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class survey
{
    public Guid _id { get; set; }

    public Guid ownerId { get; set; }

    public string title { get; set; } = null!;

    public string? description { get; set; }

    public bool isAnonymous { get; set; }

    public bool preventDuplicate { get; set; }

    public int? maxResponses { get; set; }

    public DateTime? openAt { get; set; }

    public DateTime? closeAt { get; set; }

    public string status { get; set; } = null!;

    public DateTime createdAt { get; set; }

    public DateTime? publishedAt { get; set; }

    public string? pagingMode { get; set; }

    public string? randomizationMode { get; set; }

    public string? localeDefault { get; set; }

    public string? defaultLanguage { get; set; }

    public bool autoTranslateEnabled { get; set; }

    public DateTime? startDate { get; set; }

    public DateTime? cutOffDate { get; set; }

    public string? dailyTimeSlot { get; set; }

    public int? maxAnswerTime { get; set; }

    public bool requirePassword { get; set; }

    public bool oneQuestionPerPage { get; set; }

    public string? thankYouMessage { get; set; }

    public string? redirectUrlAfterSubmit { get; set; }

    public bool conditionalProcessingEnabled { get; set; }

    public bool emailNotificationEnabled { get; set; }

    public bool deviceRestrictionEnabled { get; set; }

    public bool ipRestrictionEnabled { get; set; }

    public string? platformRestrictionList { get; set; }

    public bool allowBreakpointContinuation { get; set; }

    public bool allowOfflineResponses { get; set; }

    public bool allowPreviewBeforeSubmit { get; set; }

    public int? limitResponseCount { get; set; }

    public bool storeIpAddress { get; set; }

    public bool allowViewAfterSubmit { get; set; }

    public bool copyRestrictionEnabled { get; set; }

    public bool searchEngineRestrictionEnabled { get; set; }

    public bool responseInquiryEnabled { get; set; }

    public string? surveyStyle { get; set; }

    public bool autoDataSyncEnabled { get; set; }

    public bool resultsSharingEnabled { get; set; }

    public string? resultsPublicUrl { get; set; }

    public string? resultsPassword { get; set; }

    public string? defaultChartType { get; set; }

    public string? defaultColorPalette { get; set; }

    public bool aiReportEnabled { get; set; }

    public string? defaultOptionCoding { get; set; }

    public virtual ICollection<distributionChannel> distributionChannels { get; set; } = new List<distributionChannel>();

    public virtual ICollection<exportJob> exportJobs { get; set; } = new List<exportJob>();

    public virtual ICollection<invitation> invitations { get; set; } = new List<invitation>();

    public virtual ICollection<logicRule> logicRules { get; set; } = new List<logicRule>();

    public virtual user owner { get; set; } = null!;

    public virtual ICollection<question> questions { get; set; } = new List<question>();

    public virtual ICollection<reportView> reportViews { get; set; } = new List<reportView>();

    public virtual ICollection<response> responses { get; set; } = new List<response>();

    public virtual ICollection<section> sections { get; set; } = new List<section>();

    public virtual ICollection<securityEvent> securityEvents { get; set; } = new List<securityEvent>();

    public virtual ICollection<surveyPermission> surveyPermissions { get; set; } = new List<surveyPermission>();
}

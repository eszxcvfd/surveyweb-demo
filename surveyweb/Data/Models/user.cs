using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class user
{
    public Guid _id { get; set; }

    public string? fullName { get; set; }

    public string email { get; set; } = null!;

    public byte[] passwordHash { get; set; } = null!;

    public string role { get; set; } = null!;

    public string status { get; set; } = null!;

    public DateTime createdAt { get; set; }

    public DateTime? lastLogin { get; set; }

    public virtual ICollection<exportJob> exportJobs { get; set; } = new List<exportJob>();

    public virtual ICollection<otpToken> otpTokens { get; set; } = new List<otpToken>();

    public virtual ICollection<reportView> reportViews { get; set; } = new List<reportView>();

    public virtual ICollection<response> responses { get; set; } = new List<response>();

    public virtual ICollection<securityEvent> securityEvents { get; set; } = new List<securityEvent>();

    public virtual ICollection<surveyPermission> surveyPermissions { get; set; } = new List<surveyPermission>();

    public virtual ICollection<survey> surveys { get; set; } = new List<survey>();
}

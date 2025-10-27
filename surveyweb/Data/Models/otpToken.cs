using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class otpToken
{
    public Guid _id { get; set; }

    public Guid userId { get; set; }

    public string code { get; set; } = null!;

    public DateTime expiresAt { get; set; }

    public DateTime? usedAt { get; set; }

    public virtual user user { get; set; } = null!;
}

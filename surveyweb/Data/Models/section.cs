using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class section
{
    public Guid _id { get; set; }

    public Guid surveyId { get; set; }

    public string? title { get; set; }

    public int orderNo { get; set; }

    public bool hasTimer { get; set; }

    public int? timeLimitSec { get; set; }

    public virtual ICollection<question> questions { get; set; } = new List<question>();

    public virtual survey survey { get; set; } = null!;
}

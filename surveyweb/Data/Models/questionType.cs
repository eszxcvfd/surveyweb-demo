using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class questionType
{
    public int questionTypeId { get; set; }

    public string displayName { get; set; } = null!;

    public string internalCode { get; set; } = null!;

    public int categoryId { get; set; }

    public bool isMatrix { get; set; }

    public bool supportsOptions { get; set; }

    public string? description { get; set; }

    public virtual questionTypeCategory category { get; set; } = null!;

    public virtual ICollection<question> questions { get; set; } = new List<question>();
}

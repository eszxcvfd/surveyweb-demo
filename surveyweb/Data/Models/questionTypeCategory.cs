using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class questionTypeCategory
{
    public int categoryId { get; set; }

    public string categoryName { get; set; } = null!;

    public virtual ICollection<questionType> questionTypes { get; set; } = new List<questionType>();
}

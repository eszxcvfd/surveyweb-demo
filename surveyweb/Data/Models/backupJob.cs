using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class backupJob
{
    public Guid _id { get; set; }

    public DateTime createdAt { get; set; }

    public string? filePath { get; set; }

    public string status { get; set; } = null!;

    public string type { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class role
{
    public Guid _id { get; set; }

    public string roleName { get; set; } = null!;

    public string? description { get; set; }

    public virtual ICollection<surveyPermission> surveyPermissions { get; set; } = new List<surveyPermission>();
}

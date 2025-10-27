using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class surveyPermission
{
    public Guid _id { get; set; }

    public Guid surveyId { get; set; }

    public Guid userId { get; set; }

    public Guid roleId { get; set; }

    public DateTime addedAt { get; set; }

    public virtual role role { get; set; } = null!;

    public virtual survey survey { get; set; } = null!;

    public virtual user user { get; set; } = null!;
}

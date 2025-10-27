using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class logicRule
{
    public Guid _id { get; set; }

    public Guid surveyId { get; set; }

    public Guid? sourceId { get; set; }

    public Guid? targetId { get; set; }

    public string logicType { get; set; } = null!;

    public string targetType { get; set; } = null!;

    public string? conditionGroup { get; set; }

    public string? action { get; set; }

    public bool isActive { get; set; }

    public int? priority { get; set; }

    public string? description { get; set; }

    public virtual ICollection<logicRuleCondition> logicRuleConditions { get; set; } = new List<logicRuleCondition>();

    public virtual survey survey { get; set; } = null!;
}

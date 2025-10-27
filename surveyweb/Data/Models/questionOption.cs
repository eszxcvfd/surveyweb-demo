using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class questionOption
{
    public Guid _id { get; set; }

    public Guid questionId { get; set; }

    public string optionText { get; set; } = null!;

    public string? value { get; set; }

    public int orderNo { get; set; }

    public bool isExclusive { get; set; }

    public bool isOther { get; set; }

    public string? code { get; set; }

    public double? score { get; set; }

    public virtual ICollection<answer> answers { get; set; } = new List<answer>();

    public virtual ICollection<logicRuleCondition> logicRuleConditions { get; set; } = new List<logicRuleCondition>();

    public virtual question question { get; set; } = null!;
}

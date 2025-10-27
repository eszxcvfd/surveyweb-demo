using System;
using System.Collections.Generic;

namespace SurveyWeb.Data.Models;

public partial class logicRuleCondition
{
    public int conditionId { get; set; }

    public Guid ruleId { get; set; }

    public Guid? questionId { get; set; }

    public Guid? optionId { get; set; }

    public string leftOperandType { get; set; } = null!;

    public string _operator { get; set; } = null!;

    public string? rightValueText { get; set; }

    public double? rightValueNumber { get; set; }

    public DateTime? rightValueDate { get; set; }

    public string groupConnector { get; set; } = null!;

    public virtual questionOption? option { get; set; }

    public virtual question? question { get; set; }

    public virtual logicRule rule { get; set; } = null!;
}

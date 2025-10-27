using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SurveyWeb.Data.Models;

public partial class SurveyDbContext : DbContext
{
    public SurveyDbContext(DbContextOptions<SurveyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<answer> answers { get; set; }

    public virtual DbSet<backupJob> backupJobs { get; set; }

    public virtual DbSet<distributionChannel> distributionChannels { get; set; }

    public virtual DbSet<exportJob> exportJobs { get; set; }

    public virtual DbSet<invitation> invitations { get; set; }

    public virtual DbSet<logicRule> logicRules { get; set; }

    public virtual DbSet<logicRuleCondition> logicRuleConditions { get; set; }

    public virtual DbSet<otpToken> otpTokens { get; set; }

    public virtual DbSet<question> questions { get; set; }

    public virtual DbSet<questionOption> questionOptions { get; set; }

    public virtual DbSet<questionType> questionTypes { get; set; }

    public virtual DbSet<questionTypeCategory> questionTypeCategories { get; set; }

    public virtual DbSet<reportView> reportViews { get; set; }

    public virtual DbSet<response> responses { get; set; }

    public virtual DbSet<role> roles { get; set; }

    public virtual DbSet<section> sections { get; set; }

    public virtual DbSet<securityEvent> securityEvents { get; set; }

    public virtual DbSet<survey> surveys { get; set; }

    public virtual DbSet<surveyPermission> surveyPermissions { get; set; }

    public virtual DbSet<user> users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<answer>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__answers__DED88B1CAB769729");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.bucketLabel).HasMaxLength(200);
            entity.Property(e => e.exportLabel).HasMaxLength(200);

            entity.HasOne(d => d.question).WithMany(p => p.answers)
                .HasForeignKey(d => d.questionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_answers_question");

            entity.HasOne(d => d.response).WithMany(p => p.answers)
                .HasForeignKey(d => d.responseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_answers_response");

            entity.HasOne(d => d.selectedOption).WithMany(p => p.answers)
                .HasForeignKey(d => d.selectedOptionId)
                .HasConstraintName("FK_answers_option");
        });

        modelBuilder.Entity<backupJob>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__backupJo__DED88B1C6CE45912");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.createdAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.type).HasMaxLength(50);
        });

        modelBuilder.Entity<distributionChannel>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__distribu__DED88B1CB1BDC513");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.audienceProvider).HasMaxLength(200);
            entity.Property(e => e.channelType).HasMaxLength(100);
            entity.Property(e => e.collectorType).HasMaxLength(100);
            entity.Property(e => e.createdAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.integrationName).HasMaxLength(200);
            entity.Property(e => e.isActive).HasDefaultValue(true);
            entity.Property(e => e.platform).HasMaxLength(200);
            entity.Property(e => e.type).HasMaxLength(100);

            entity.HasOne(d => d.survey).WithMany(p => p.distributionChannels)
                .HasForeignKey(d => d.surveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_distributionChannels_survey");
        });

        modelBuilder.Entity<exportJob>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__exportJo__DED88B1CC45E8373");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.anonymizeMode).HasMaxLength(50);
            entity.Property(e => e.chartTypeOverride).HasMaxLength(100);
            entity.Property(e => e.colorPalette).HasMaxLength(100);
            entity.Property(e => e.deliveryMethod).HasMaxLength(50);
            entity.Property(e => e.emailTo).HasMaxLength(255);
            entity.Property(e => e.format).HasMaxLength(20);
            entity.Property(e => e.locale).HasMaxLength(20);
            entity.Property(e => e.optionCoding).HasMaxLength(50);
            entity.Property(e => e.scope).HasMaxLength(50);
            entity.Property(e => e.sheetName).HasMaxLength(255);
            entity.Property(e => e.status).HasMaxLength(50);

            entity.HasOne(d => d.requestedByNavigation).WithMany(p => p.exportJobs)
                .HasForeignKey(d => d.requestedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_exportJobs_user");

            entity.HasOne(d => d.survey).WithMany(p => p.exportJobs)
                .HasForeignKey(d => d.surveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_exportJobs_survey");
        });

        modelBuilder.Entity<invitation>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__invitati__DED88B1C1A1E11EF");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.audienceSegment).HasMaxLength(200);
            entity.Property(e => e.deliveryStatus).HasMaxLength(50);
            entity.Property(e => e.integrationId).HasMaxLength(200);
            entity.Property(e => e.recipient).HasMaxLength(255);
            entity.Property(e => e.sendMethod).HasMaxLength(50);
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.token).HasMaxLength(200);

            entity.HasOne(d => d.survey).WithMany(p => p.invitations)
                .HasForeignKey(d => d.surveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_invitations_survey");
        });

        modelBuilder.Entity<logicRule>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__logicRul__DED88B1CDF233461");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.action).HasMaxLength(50);
            entity.Property(e => e.isActive).HasDefaultValue(true);
            entity.Property(e => e.logicType).HasMaxLength(50);
            entity.Property(e => e.targetType).HasMaxLength(50);

            entity.HasOne(d => d.survey).WithMany(p => p.logicRules)
                .HasForeignKey(d => d.surveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_logicRules_survey");
        });

        modelBuilder.Entity<logicRuleCondition>(entity =>
        {
            entity.HasKey(e => e.conditionId).HasName("PK__logicRul__A29757BC93DE753E");

            entity.Property(e => e._operator)
                .HasMaxLength(50)
                .HasColumnName("operator");
            entity.Property(e => e.groupConnector)
                .HasMaxLength(10)
                .HasDefaultValue("AND");
            entity.Property(e => e.leftOperandType).HasMaxLength(50);

            entity.HasOne(d => d.option).WithMany(p => p.logicRuleConditions)
                .HasForeignKey(d => d.optionId)
                .HasConstraintName("FK_logicRuleConditions_option");

            entity.HasOne(d => d.question).WithMany(p => p.logicRuleConditions)
                .HasForeignKey(d => d.questionId)
                .HasConstraintName("FK_logicRuleConditions_question");

            entity.HasOne(d => d.rule).WithMany(p => p.logicRuleConditions)
                .HasForeignKey(d => d.ruleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_logicRuleConditions_rule");
        });

        modelBuilder.Entity<otpToken>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__otpToken__DED88B1CA8F734B5");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.code).HasMaxLength(50);

            entity.HasOne(d => d.user).WithMany(p => p.otpTokens)
                .HasForeignKey(d => d.userId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_otpTokens_user");
        });

        modelBuilder.Entity<question>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__question__DED88B1C4800D71D");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.aggregationFunction).HasMaxLength(100);
            entity.Property(e => e.chartSuggestion).HasMaxLength(100);
            entity.Property(e => e.defaultOptionCoding).HasMaxLength(100);
            entity.Property(e => e.optionOrderInResults).HasMaxLength(100);
            entity.Property(e => e.resultVisibility).HasMaxLength(100);
            entity.Property(e => e.subtype).HasMaxLength(100);
            entity.Property(e => e.type).HasMaxLength(50);

            entity.HasOne(d => d.questionType).WithMany(p => p.questions)
                .HasForeignKey(d => d.questionTypeId)
                .HasConstraintName("FK_questions_questionTypes");

            entity.HasOne(d => d.section).WithMany(p => p.questions)
                .HasForeignKey(d => d.sectionId)
                .HasConstraintName("FK_questions_section");

            entity.HasOne(d => d.survey).WithMany(p => p.questions)
                .HasForeignKey(d => d.surveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_questions_survey");
        });

        modelBuilder.Entity<questionOption>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__question__DED88B1C3B02A61F");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.code).HasMaxLength(100);
            entity.Property(e => e.value).HasMaxLength(200);

            entity.HasOne(d => d.question).WithMany(p => p.questionOptions)
                .HasForeignKey(d => d.questionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_questionOptions_question");
        });

        modelBuilder.Entity<questionType>(entity =>
        {
            entity.HasKey(e => e.questionTypeId).HasName("PK__question__F5758B73E9E2A1B3");

            entity.HasIndex(e => e.internalCode, "UQ__question__A71407B1F4EE7F4C").IsUnique();

            entity.Property(e => e.displayName).HasMaxLength(200);
            entity.Property(e => e.internalCode).HasMaxLength(100);

            entity.HasOne(d => d.category).WithMany(p => p.questionTypes)
                .HasForeignKey(d => d.categoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_questionTypes_category");
        });

        modelBuilder.Entity<questionTypeCategory>(entity =>
        {
            entity.HasKey(e => e.categoryId).HasName("PK__question__23CAF1D8400C25E1");

            entity.Property(e => e.categoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<reportView>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__reportVi__DED88B1CEBCC84BD");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.chartType).HasMaxLength(100);
            entity.Property(e => e.colorPalette).HasMaxLength(100);
            entity.Property(e => e.createdAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.name).HasMaxLength(255);

            entity.HasOne(d => d.createdByNavigation).WithMany(p => p.reportViews)
                .HasForeignKey(d => d.createdBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_reportViews_user");

            entity.HasOne(d => d.survey).WithMany(p => p.reportViews)
                .HasForeignKey(d => d.surveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_reportViews_survey");
        });

        modelBuilder.Entity<response>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__response__DED88B1C11DF88DE");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.clientIP).HasMaxLength(100);
            entity.Property(e => e.clientLocale).HasMaxLength(20);
            entity.Property(e => e.collectorType).HasMaxLength(100);
            entity.Property(e => e.deviceType).HasMaxLength(100);
            entity.Property(e => e.isComplete).HasDefaultValue(true);
            entity.Property(e => e.isValid).HasDefaultValue(true);
            entity.Property(e => e.oneTimeToken).HasMaxLength(200);

            entity.HasOne(d => d.distributionChannel).WithMany(p => p.responses)
                .HasForeignKey(d => d.distributionChannelId)
                .HasConstraintName("FK_responses_distribution");

            entity.HasOne(d => d.respondent).WithMany(p => p.responses)
                .HasForeignKey(d => d.respondentId)
                .HasConstraintName("FK_responses_user");

            entity.HasOne(d => d.survey).WithMany(p => p.responses)
                .HasForeignKey(d => d.surveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_responses_survey");
        });

        modelBuilder.Entity<role>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__roles__DED88B1C81C3EEDB");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.roleName).HasMaxLength(100);
        });

        modelBuilder.Entity<section>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__sections__DED88B1CB7B0DF86");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.title).HasMaxLength(500);

            entity.HasOne(d => d.survey).WithMany(p => p.sections)
                .HasForeignKey(d => d.surveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_sections_survey");
        });

        modelBuilder.Entity<securityEvent>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__security__DED88B1C89DC7271");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.createdAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.type).HasMaxLength(100);

            entity.HasOne(d => d.survey).WithMany(p => p.securityEvents)
                .HasForeignKey(d => d.surveyId)
                .HasConstraintName("FK_securityEvents_survey");

            entity.HasOne(d => d.user).WithMany(p => p.securityEvents)
                .HasForeignKey(d => d.userId)
                .HasConstraintName("FK_securityEvents_user");
        });

        modelBuilder.Entity<survey>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__surveys__DED88B1C08E897BA");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.createdAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.dailyTimeSlot).HasMaxLength(100);
            entity.Property(e => e.defaultChartType).HasMaxLength(50);
            entity.Property(e => e.defaultColorPalette).HasMaxLength(100);
            entity.Property(e => e.defaultLanguage).HasMaxLength(20);
            entity.Property(e => e.defaultOptionCoding).HasMaxLength(50);
            entity.Property(e => e.localeDefault).HasMaxLength(20);
            entity.Property(e => e.pagingMode).HasMaxLength(50);
            entity.Property(e => e.randomizationMode).HasMaxLength(50);
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.storeIpAddress).HasDefaultValue(true);
            entity.Property(e => e.title).HasMaxLength(500);

            entity.HasOne(d => d.owner).WithMany(p => p.surveys)
                .HasForeignKey(d => d.ownerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_surveys_users_owner");
        });

        modelBuilder.Entity<surveyPermission>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__surveyPe__DED88B1C1A9C3948");

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.addedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.role).WithMany(p => p.surveyPermissions)
                .HasForeignKey(d => d.roleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_surveyPermissions_role");

            entity.HasOne(d => d.survey).WithMany(p => p.surveyPermissions)
                .HasForeignKey(d => d.surveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_surveyPermissions_survey");

            entity.HasOne(d => d.user).WithMany(p => p.surveyPermissions)
                .HasForeignKey(d => d.userId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_surveyPermissions_user");
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e._id).HasName("PK__users__DED88B1C3DFFD5D4");

            entity.HasIndex(e => e.email, "UQ__users__AB6E61644D5B1416").IsUnique();

            entity.Property(e => e._id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.createdAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.fullName).HasMaxLength(200);
            entity.Property(e => e.role).HasMaxLength(50);
            entity.Property(e => e.status).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

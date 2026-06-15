namespace TrainingAssistant.Infrastructure.Helpers;

/// <summary>
/// 7-дневные периоды от даты регистрации пользователя
/// </summary>
public static class WeekPeriodHelper
{
    public static DateOnly GetAnchorDate(DateTime createdAtUtc) => DateOnly.FromDateTime(createdAtUtc);

    /// <summary>
    /// Начало текущего 7-дневного периода относительно якоря (даты регистрации)
    /// </summary>
    public static DateOnly GetCurrentPeriodStart(DateTime createdAtUtc, DateTime utcNow)
    {
        var anchor = GetAnchorDate(createdAtUtc);
        var today = DateOnly.FromDateTime(utcNow);
        if (today < anchor)
            return anchor;

        var daysSince = today.DayNumber - anchor.DayNumber;
        var periodIndex = daysSince / 7;
        return anchor.AddDays(periodIndex * 7);
    }

    public static bool IsPeriodExpired(DateOnly weekStart, DateTime utcNow)
        => weekStart.AddDays(7) <= DateOnly.FromDateTime(utcNow);
}

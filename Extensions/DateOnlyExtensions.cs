namespace Skii.Extensions;

public static class DateOnlyExtensions
{
    public static bool IsWeekend(this DateOnly date)
    {
        return date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }
}
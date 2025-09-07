using System.Collections.Immutable;
using Skii.Enums;

namespace Skii.Models;

public readonly struct DateRange(DateOnly start, DateOnly end)
{
    public DateOnly StartDate { get; } = start;
    public DateOnly EndDate { get; } = end;

    public bool Contains(DateOnly date) => date >= StartDate && date <= EndDate;
    public static DateRange[] GetAvailableRanges(SortedDictionary<DateOnly, DateChoices> selection)
    {
        List<DateRange> res = new List<DateRange>((selection.Count/2)+1);

        DateOnly? rangeStart = null;
        DateOnly? rangeEnd = null;

        foreach (var (date, choice) in selection)
        {
            if (choice is DateChoices.Yes or DateChoices.Maybe)
            {
                rangeStart ??= date;
                rangeEnd = date;
            }
            else
            {
                if (!rangeStart.HasValue || !rangeEnd.HasValue) continue;
                res.Add(new DateRange(rangeStart.Value, rangeEnd.Value));
                rangeStart = null;
                rangeEnd = null;
            }
        }

     
        if (rangeStart.HasValue && rangeEnd.HasValue)
            res.Add(new DateRange(rangeStart.Value, rangeEnd.Value));

        return res.ToArray();
    }
}
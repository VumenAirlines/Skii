using Skii.Constants;

namespace Skii.Models;

public class UserAvailabilities
{
    public Guid UserId { get; init; }
    public DateRange[] Ranges { get; init; } =  [];
    
    public static bool IsUserConsistent(DateOnly start, DateOnly end, ReadOnlySpan<DateRange> ranges)
    {
        int left = 0;
        int right = ranges.Length - 1;
        while (left<=right)
        {
            int mid = left + (right - left) / 2;
            DateRange midRange = ranges[mid];
            if (start < midRange.StartDate)
                right = mid - 1;
            else if (start > midRange.EndDate)
                left = mid + 1;
            else return end <= midRange.EndDate;

        }

        return false;
    }
    public static Guid[] GetUsersInRange(DateOnly start, DateOnly end, UserAvailabilities[] userAvailabilities)
    {
        
        Span<Guid> users = userAvailabilities.Length <= AnswerConstants.StackMax 
            ? stackalloc Guid[userAvailabilities.Length]
            : new Guid[userAvailabilities.Length];
        int cnt = 0;
        foreach (UserAvailabilities user in userAvailabilities)
        {
            if(IsUserConsistent(start,end,user.Ranges.AsSpan()))
                users[cnt++]  = user.UserId;
        }

        return users[..cnt].ToArray();

    }
}

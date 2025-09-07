using System.Collections.Immutable;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skii.Binders;
using Skii.Constants;
using Skii.Data;
using Skii.DTOs;
using Skii.Enums;
using Skii.Extensions;
using Skii.Models;

namespace Skii.Services;

public class AnswerService(AppDbContext dbContext) : IAnswerService
{
    public async Task<AnswerDto?> GetAnswerByIdAsync(Guid id,Guid userId, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Answers.AsNoTracking().Include(a => a.AvailableDates)
                .FirstOrDefaultAsync(a =>a.UserId==userId &&a.Id == id, cancellationToken) is { } answer)
            return new AnswerDto(answer.Id, answer.MinLength,answer.MaxLength,  answer.SkiiOnFirstDay,answer.PreferWeekends, answer.AvailableDates);
        return null;
    }

    public async Task<AnswerDto> CreateAnswerAsync( Answer answer , CancellationToken cancellationToken=default)
    {
        
        await dbContext.Answers.AddAsync(answer, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new AnswerDto(answer.Id, answer.MinLength,answer.MaxLength,  answer.SkiiOnFirstDay,answer.PreferWeekends, answer.AvailableDates);
    }

    public async Task<bool> UpdateAnswerAsync(Guid id,UpdateAnswerParsedDto updateAnswerDto,Guid userId, CancellationToken cancellationToken=default)
    {
        if (await dbContext.Answers.Include(a => a.AvailableDates)
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Id == id, cancellationToken) is not
            { } answer)
            return false;
        if (updateAnswerDto.AvailableDates is not null)
        {
            answer.AvailableDates.Clear();
            answer.AvailableDates.AddRange(updateAnswerDto.AvailableDates);
        }
        if (updateAnswerDto.MinLength.HasValue)
            answer.MinLength = updateAnswerDto.MinLength.Value;
        if (updateAnswerDto.MaxLength.HasValue)
            answer.MaxLength = updateAnswerDto.MaxLength.Value;
        if (updateAnswerDto.SkiiOnFirstDay.HasValue)
            answer.SkiiOnFirstDay = updateAnswerDto.SkiiOnFirstDay.Value;
        if (updateAnswerDto.PreferWeekends.HasValue)
            answer.PreferWeekends = updateAnswerDto.PreferWeekends.Value;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
        
    }

    public async Task<Dictionary<int, List<DateResult>>> MakeAnalytics()
    {
        Dictionary<int, List<DateResult>> res;
        DateOnly startDate = DateOnly.Parse(AnswerConstants.StartDate);
        DateOnly endDate = DateOnly.Parse(AnswerConstants.EndDate);

        var dates = await dbContext.DateSelections
            .AsNoTracking()
            .Include(x => x.Answer).Select(ds=>new
            {
                UserId = ds.Answer.UserId, 
                Date = ds.Date, 
                Choice = ds.Choice!.Value,
                MinLength = ds.Answer.MinLength,
                MaxLength = ds.Answer.MaxLength,
                PreferWeekends = ds.Answer.PreferWeekends
            }).ToArrayAsync();
        if (dates.Length == 0) throw new InvalidDataException("Query is empty");
        
        int minLength = dates[0].MinLength;
        int maxLength = dates[0].MaxLength;
        int weekendPreference = 0;
        foreach (var date in dates)
        {
            minLength = int.Min(minLength, date.MinLength);
            maxLength = int.Max(maxLength, date.MaxLength);
            weekendPreference += date.PreferWeekends ? 1 : -1;
        }

        
        bool preferWeekends = weekendPreference > 0;
        
        var datesAvailableByUser = dates
            .GroupBy(ds => ds.UserId)
            .AsParallel()
            .Select(g => new UserAvailabilities
            {
                UserId = g.Key,
                Ranges = DateRange.GetAvailableRanges(
                    new SortedDictionary<DateOnly, DateChoices>(
                        g.ToDictionary(ds => ds.Date, ds => ds.Choice)
                    )
                )
            })
            .ToArray();

        var tasks = Enumerable.Range(minLength, maxLength - minLength + 1)
            .AsParallel()
            .Select(x => new
            {
                Length = x,
                Res = CalculateUsersInRange(x, startDate, endDate, datesAvailableByUser)
            }).ToArray();
        return tasks.ToDictionary(x => x.Length, x => x.Res);
    }
    private static List<DateResult> CalculateUsersInRange(
        int length, 
        DateOnly startDate, 
        DateOnly endDate, 
        UserAvailabilities[] userAvailabilities)
    {
        List<DateResult> dateResults = [];
        DateOnly finalDate = endDate.AddDays(-length + 1);
    
        for (DateOnly current = startDate; current <= finalDate; current = current.AddDays(1))
        {
            DateOnly end = current.AddDays(length - 1);
            Guid[] consistentUsers = UserAvailabilities.GetUsersInRange(current, end, userAvailabilities);
        
            dateResults.Add(new DateResult
            {
                Range = new DateRange(current, end),
                UserIds = consistentUsers
            });
        }
        
        var sortedResults = dateResults
                .OrderByDescending(d => d.UserIds.Length)
                .Take(AnswerConstants.TopAmnt)
                .ToList();
    
        return sortedResults;
    }
    
    

   
    

   
}
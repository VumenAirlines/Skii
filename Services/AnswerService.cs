using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skii.Binders;
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

    public async Task<Dictionary<int, (int MaxSum, int StartIndex)>> MakeAnalytics()
    {


        Dictionary<DateOnly, DateScore> allDates = [];
        List<DateSelection> dates = await dbContext.DateSelections.AsNoTracking().ToListAsync();
        bool preferWeekends = await dbContext.Answers
            .Select(a => a.PreferWeekends ? 1 : -1)
            .SumAsync() > 0;
        
        foreach (var date in dates)
        {
            if (!allDates.TryGetValue(date.Date, out var dateScore))
                allDates.Add(date.Date, new DateScore
                {
                    Date = date.Date,
                    UserIds = [date.AnswerId],
                    Score = date.Date.IsWeekend() && preferWeekends ? 1 : 0 + date.Choice switch
                    {
                        DateChoices.Yes => 1,
                        DateChoices.No => -1,
                        DateChoices.Maybe => 0,
                        _ => throw new Exception(),
                    },
                });
            else
            {
                dateScore.UserIds.Add(date.AnswerId);
                dateScore.Score += date.Date.IsWeekend() && preferWeekends ? 1 : 0 +  date.Choice switch
                {
                    DateChoices.Yes => 1,
                    DateChoices.No => -1,
                    DateChoices.Maybe => 0,
                    _ => throw new Exception(),
                };
            }

           
        }

        var ordered = allDates.OrderBy(x => x.Key).ToImmutableList();
        int windowSizeMin = await dbContext.Answers.AsNoTracking().Select(x => x.MinLength).MinAsync();
        int windowSizeMax = await dbContext.Answers.AsNoTracking().Select(x => x.MaxLength).MaxAsync();
        int length = ordered.Count;

        if (windowSizeMin >= windowSizeMax)
            throw new InvalidDataException("Maximum window size must be larger than minimum");
        if (length < windowSizeMin)
            throw new InvalidDataException("Window size must be less than length of array");

        int[] arr = new int [length + 1];

        Span<int> prefixSumList = new Span<int>(arr)
        {
            [0] = 0
        };
        int index = 1;
        int runningSum = 0;
        foreach (var kvp in ordered)
        {
            runningSum += kvp.Value.Score;
            prefixSumList[index++] = runningSum;
        }

        Dictionary<int, (int MaxSum, int StartIndex)> results = new(windowSizeMax - windowSizeMin + 1);

        for (int windowSize = windowSizeMin; windowSize <= windowSizeMax; windowSize++)
        {
            int bestWindowIndex = -1;
            int maxSum = int.MinValue;

            for (int j = 0; j <= length - windowSize; j++)
            {
                int windowSum = prefixSumList[j + windowSize] - prefixSumList[j];
                if (windowSum <= maxSum) continue;
                maxSum = windowSum;
                bestWindowIndex = j;
            }

            results.Add(windowSize, (maxSum, bestWindowIndex));
        }

        return results;
    }
}
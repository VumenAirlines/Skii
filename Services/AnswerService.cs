using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skii.Binders;
using Skii.Data;
using Skii.DTOs;
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

    public async Task<Object> MakeAnalytics()
    {
        Dictionary<DateOnly,DateScore> allDates = [];
        var dates = await dbContext.DateSelections.Select(x => );
        foreach (var date in dates)
        {
            if(!allDates.TryGetValue(date, out var dateScore))
                allDates.Add(new KeyValuePair<DateOnly,DateScore>(date.Date,new DateScore{
                    Date = date.Date,
                    UserIds = new HashSet<Guid>(date.AnswerId),
                    Score = date.Choice switch {
                        Yes => 1,
                        No=>-,
                        Maybe=>0,
                        _ => throw new Exception(),
                    } ;
                }))
            else 
            {
                dateScore.UserIds.Add(date.AnsweId);
                dateScore.Score += Score = date.Choice switch {
                        Yes => 1,
                        No=>-,
                        Maybe=>0,
                        _ => throw new Exception(),
                    } ;
            
            }
            
        }


    }
}
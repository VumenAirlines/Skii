
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Skii.DTOs;
using Skii.Enums;
using Skii.Models;

namespace Skii.Binders;

public class UpdateAnswerBinder: IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var body = bindingContext.HttpContext.Request.Body;
        using StreamReader reader = new StreamReader(body);
        var json = reader.ReadToEndAsync().Result;
        var dto = JsonSerializer.Deserialize<UpdateAnswerDto>(json);
        if (dto is null) bindingContext.Result = ModelBindingResult.Failed();
        //todo nullable thingiadas
        var answer = new UpdateAnswerParsedDto()
        {
            MinLength = dto?.MinLength,
            MaxLength = dto?.MaxLength,
            PreferWeekends = dto?.PreferWeekends,
            SkiiOnFirstDay = dto?.SkiiOnFirstDay,
            AvailableDates = dto?.AvailableDates?.Select(a=> new DateSelection{Id = Guid.NewGuid(),Date = a.Key, Choice = a.Value})
                .ToList()
        };

        bindingContext.Result = ModelBindingResult.Success(answer);
        return Task.CompletedTask;
    }
}
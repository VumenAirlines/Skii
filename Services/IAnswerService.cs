using Skii.DTOs;
using Skii.Models;

namespace Skii.Services;

public interface IAnswerService
{
   Task<AnswerDto?> GetAnswerByIdAsync(Guid id,Guid userId,CancellationToken cancellationToken);
   Task<AnswerDto> CreateAnswerAsync(Answer answer,CancellationToken cancellationToken);

   Task<bool> UpdateAnswerAsync(Guid id, UpdateAnswerParsedDto updateAnswerDto, Guid userId,
      CancellationToken cancellationToken = default);

   Task<Dictionary<int, List<DateResult>>> MakeAnalytics();
}
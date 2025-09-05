
using System.Security.Claims;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skii.Binders;
using Skii.DTOs;
using Skii.Models;
using Skii.Services;

namespace Skii.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Authorize]
public partial class AnswerController(IAnswerService answerService, IValidator<Answer> answerValidator, IValidator<UpdateAnswerParsedDto> updateValidator) :ControllerBase
{
    private Guid UserId => Guid.Parse( User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AnswerDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetAnswerById(Guid id, CancellationToken cancellationToken)
    {
        if (await answerService.GetAnswerByIdAsync(id, UserId, cancellationToken) is not { } answer)
            return NotFound();
        return Ok(answer);
    }
    [HttpGet]
    [ProducesResponseType(typeof(Dictionary<DateOnly,DateScore>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetAnalytics(CancellationToken cancellationToken)
    {
        if (await answerService.MakeAnalytics() is not { } analytics)
            return NotFound();
        return Ok(analytics);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(AnswerDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CreateAnswer([ModelBinder(BinderType = typeof(CreateAnswerBinder))] Answer answer, CancellationToken cancellationToken)
    {
        ValidationResult res = await answerValidator.ValidateAsync(answer, cancellationToken);
        if (!res.IsValid)
            return BadRequest();
        answer.UserId = UserId;
        if (await answerService.CreateAnswerAsync(answer, cancellationToken) is not {} createdAnswer)
            return StatusCode(StatusCodes.Status500InternalServerError);
        return CreatedAtAction(nameof(GetAnswerById),new{id=answer.Id}, createdAnswer);
    }


    [HttpPut("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateAnswer(Guid id,[ModelBinder(BinderType = typeof(UpdateAnswerBinder))]UpdateAnswerParsedDto updateAnswerDto, CancellationToken cancellationToken)
    {
        ValidationResult res = await updateValidator.ValidateAsync(updateAnswerDto, cancellationToken);
        if (!res.IsValid)
            return BadRequest();
        if (!await answerService.UpdateAnswerAsync(id,updateAnswerDto, UserId, cancellationToken))
            return NotFound();
        return NoContent();
    }
    
}
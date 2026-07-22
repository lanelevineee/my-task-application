using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.TestHelper;
using TaskManager.Application.DTOs;
using TaskManager.Application.Validators;

namespace TaskManager.Tests.Validators;

public class UpdateTaskDtoValidatorTests
{
    private readonly UpdateTaskDtoValidator _validator;

    public UpdateTaskDtoValidatorTests()
    {
        _validator = new UpdateTaskDtoValidator();
    }

    [Fact]
    public async System.Threading.Tasks.Task ShouldHaveErrorWhenTitleIsEmpty()
    {
        var model = new UpdateTaskDto("", null, null, null, null);
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public async System.Threading.Tasks.Task ShouldHaveErrorWhenTitleExceeds200Characters()
    {
        var model = new UpdateTaskDto(new string('A', 201), null, null, null, null);
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public async System.Threading.Tasks.Task ShouldHaveErrorWhenDescriptionExceeds2000Characters()
    {
        var model = new UpdateTaskDto("Title", new string('A', 2001), null, null, null);
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public async System.Threading.Tasks.Task ShouldNotHaveErrorForValidModel()
    {
        var model = new UpdateTaskDto("Valid Title", "Valid Description", Domain.Enums.TaskStatus.Completed, Domain.Enums.TaskPriority.High, null);
        var result = await _validator.TestValidateAsync(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async System.Threading.Tasks.Task ShouldHaveErrorWhenStatusIsInvalid()
    {
        var model = new UpdateTaskDto("Title", null, (Domain.Enums.TaskStatus)999, null, null);
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }
}
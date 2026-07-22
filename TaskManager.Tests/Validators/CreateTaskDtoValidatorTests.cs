using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.TestHelper;
using TaskManager.Application.DTOs;
using TaskManager.Application.Validators;

namespace TaskManager.Tests.Validators;

public class CreateTaskDtoValidatorTests
{
    private readonly CreateTaskDtoValidator _validator;

    public CreateTaskDtoValidatorTests()
    {
        _validator = new CreateTaskDtoValidator();
    }

    [Fact]
    public async System.Threading.Tasks.Task ShouldHaveErrorWhenTitleIsEmpty()
    {
        var model = new CreateTaskDto("", null);
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public async System.Threading.Tasks.Task ShouldHaveErrorWhenTitleExceeds200Characters()
    {
        var model = new CreateTaskDto(new string('A', 201), null);
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public async System.Threading.Tasks.Task ShouldHaveErrorWhenDescriptionExceeds2000Characters()
    {
        var model = new CreateTaskDto("Title", new string('A', 2001));
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public async System.Threading.Tasks.Task ShouldNotHaveErrorForValidModel()
    {
        var model = new CreateTaskDto("Valid Title", "Valid Description", Domain.Enums.TaskStatus.Pending, Domain.Enums.TaskPriority.Medium);
        var result = await _validator.TestValidateAsync(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async System.Threading.Tasks.Task ShouldHaveErrorWhenStatusIsInvalid()
    {
        var model = new CreateTaskDto("Title", null, (Domain.Enums.TaskStatus)999);
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }
}
using FluentValidation;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Validators;

public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status. Valid values: Pending, InProgress, Completed, Cancelled");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority. Valid values: Low, Medium, High, Critical");
    }
}

public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
{
    public UpdateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.Status)
            .IsInEnum().When(x => x.Status.HasValue)
            .WithMessage("Invalid status. Valid values: Pending, InProgress, Completed, Cancelled");

        RuleFor(x => x.Priority)
            .IsInEnum().When(x => x.Priority.HasValue)
            .WithMessage("Invalid priority. Valid values: Low, Medium, High, Critical");
    }
}
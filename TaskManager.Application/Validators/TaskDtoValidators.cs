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
            .Must(BeValidStatus).WithMessage("Invalid status. Valid values: Pending, InProgress, Completed, Cancelled");

        RuleFor(x => x.Priority)
            .Must(BeValidPriority).WithMessage("Invalid priority. Valid values: Low, Medium, High, Critical");
    }

    private static bool BeValidStatus(string status)
    {
        var validStatuses = new[] { "Pending", "InProgress", "Completed", "Cancelled" };
        return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeValidPriority(string priority)
    {
        var validPriorities = new[] { "Low", "Medium", "High", "Critical" };
        return validPriorities.Contains(priority, StringComparer.OrdinalIgnoreCase);
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
            .Must(BeValidStatus).When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Invalid status. Valid values: Pending, InProgress, Completed, Cancelled");

        RuleFor(x => x.Priority)
            .Must(BeValidPriority).When(x => !string.IsNullOrEmpty(x.Priority))
            .WithMessage("Invalid priority. Valid values: Low, Medium, High, Critical");
    }

    private static bool BeValidStatus(string status)
    {
        var validStatuses = new[] { "Pending", "InProgress", "Completed", "Cancelled" };
        return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeValidPriority(string priority)
    {
        var validPriorities = new[] { "Low", "Medium", "High", "Critical" };
        return validPriorities.Contains(priority, StringComparer.OrdinalIgnoreCase);
    }
}
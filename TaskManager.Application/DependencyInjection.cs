using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Handlers.Commands;
using TaskManager.Application.Handlers.Queries;
using TaskManager.Application.Validators;

namespace TaskManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        
        services.AddValidatorsFromAssemblyContaining<CreateTaskDtoValidator>();
        
        return services;
    }
}
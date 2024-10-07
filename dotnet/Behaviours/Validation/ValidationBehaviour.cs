using Ardalis.Result;
using dotnet.Features.Carts.Operations;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace dotnet.Behaviours.Validation;

public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Check if no validators exist
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Find failures
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

        if (failures.Count > 0)
        {
            // Create the Ardalis.ValidationError list
            var validationErrors = failures.Select(f => new ValidationError
            {
                Identifier = f.PropertyName,
                ErrorMessage = f.ErrorMessage,
                ErrorCode = f.ErrorCode,
            }).ToList();

            // Determine the generic type T from Result<T>
            var responseType = typeof(TResponse);
            if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var resultGenericType = responseType.GetGenericArguments()[0];

                // Use reflection to call Result.Invalid<T>(IEnumerable<ValidationError>)

                var methods = typeof(Result<>)
                    .MakeGenericType(resultGenericType)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static);

                var invalidMethod = methods.FirstOrDefault(m => m.Name == "Invalid" &&
                                         m.GetParameters().Length == 1 &&
                                         m.GetParameters()[0].ParameterType == typeof(IEnumerable<ValidationError>));

                var notFoundMethod = methods.FirstOrDefault(m => m.Name == "NotFound" &&
                                         m.GetParameters().Length == 1 );

                if (validationErrors.Any(x => x.ErrorCode == "404")) // NotFound
                {
                    // Find specific error with ErrorCode "404" and return NotFound with message
                    var error = validationErrors.First(x => x.ErrorCode == "404");

                    var notFoundResult = notFoundMethod.Invoke(null, new object[] { new string[] { error.ErrorMessage } });

                    return (TResponse)notFoundResult;
                } else
                {
                    var invalidResult = invalidMethod.Invoke(null, new object[] { validationErrors });

                    return (TResponse)invalidResult;
                }


            }
            else
            {
                // If TResponse is non-generic Result, return Invalid directly
                var invalidResult = Result.Invalid(validationErrors);
                return (TResponse)(object)invalidResult;
            }

        }

        return await next();
    }
}

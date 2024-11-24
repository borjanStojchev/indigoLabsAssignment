using System;
using IndigoLabsAssignment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IndigoLabsAssignment.Authentication;

public class ApiKeyAuthFilter : IAuthorizationFilter
{
    private const string ApiKeyHeaderName = "Authorization";
    private readonly List<string> _validApiKeys;

    public ApiKeyAuthFilter(Settings settings)
    {
        _validApiKeys = settings.ApiKeys;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValue))
        {
            context.Result = new UnauthorizedObjectResult("Api key missing");
            return;
        }

        var apiKey = apiKeyHeaderValue.FirstOrDefault()?.Split(' ').Last();

        if (apiKey == null || !_validApiKeys.Contains(apiKey))
        {
            context.Result = new UnauthorizedObjectResult("Invalid Api Key");
            return;
        }
    }
}

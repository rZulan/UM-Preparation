using Application.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace UM_Preparation.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "API-KEY";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = configuration.GetValue<string>("Authentication:ApiKey:Key");

            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                var result = Result<string>.Failure("API Key is missing", HttpStatusCode.Unauthorized);
                context.Result = new ObjectResult(result) { StatusCode = (int)result.StatusCode! };
                return;
            }

            if (!string.Equals(apiKey, extractedApiKey, StringComparison.Ordinal))
            {
                var result = Result<string>.Failure("Invalid API Key", HttpStatusCode.Unauthorized);
                context.Result = new ObjectResult(result) { StatusCode = (int)result.StatusCode! };
                return;
            }

            await next();
        }
    }
}
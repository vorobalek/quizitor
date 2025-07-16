using Microsoft.Extensions.Options;
using Quizitor.Api.Options;

namespace Quizitor.Api.Middleware;

internal sealed class SecretTokenValidatorMiddleware(IOptions<TelegramBotSecrets> options) : IMiddleware
{
    public async Task InvokeAsync(
        HttpContext context,
        RequestDelegate next)
    {
        var secretToken = context.Request.Headers
            .FirstOrDefault(x => x.Key == options.Value.HeaderName)
            .Value
            .ToString();

        if (string.IsNullOrWhiteSpace(secretToken) ||
            options.Value.Token != secretToken)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.CompleteAsync();
            return;
        }

        await next(context);
    }
}
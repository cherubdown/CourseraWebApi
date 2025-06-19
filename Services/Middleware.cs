using System.Diagnostics; 

namespace CourseraWebApi.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            await LogRequest(context.Request);
            var originalBodyStream = context.Response.Body;
            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;
            try
            {
                await _next(context);
                stopwatch.Stop();
                await LogResponse(context.Response, stopwatch.ElapsedMilliseconds);
            }
            finally
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }
        }

        private async Task LogRequest(HttpRequest request)
        {
            request.EnableBuffering();

            string requestBodyContent = string.Empty;
            using (var reader = new StreamReader(request.Body, leaveOpen: true))
            {
                requestBodyContent = await reader.ReadToEndAsync();
            }

            request.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation(
                "Request: {Method} {Path}{QueryString} from {RemoteIpAddress} - Headers: {Headers} - Body: {RequestBody}",
                request.Method,
                request.Path,
                request.QueryString,
                request.HttpContext.Connection.RemoteIpAddress,
                FormatHeaders(request.Headers),
                requestBodyContent
            );
        }

        private async Task LogResponse(HttpResponse response, long durationMs)
        {
            response.Body.Seek(0, SeekOrigin.Begin);

            string responseBodyContent = await new StreamReader(response.Body).ReadToEndAsync();

            _logger.LogInformation(
                "Response: Status {StatusCode} for {Path} in {DurationMs}ms - Headers: {Headers} - Body: {ResponseBody}",
                response.StatusCode,
                response.HttpContext.Request.Path,
                durationMs,
                FormatHeaders(response.Headers),
                responseBodyContent
            );

            response.Body.Seek(0, SeekOrigin.Begin);
        }

        private string FormatHeaders(IHeaderDictionary headers)
        {
            return string.Join(", ", headers.Select(h => $"{h.Key}: {h.Value}"));
        }
    }

    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}

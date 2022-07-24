namespace WebApiAutores.Middlewares
{
    public static class LoggingResponseMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggingResponse(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoggingResponseMiddleware>();
        }
    }
    public class LoggingResponseMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<LoggingResponseMiddleware> logger;

        public LoggingResponseMiddleware(RequestDelegate next, ILogger<LoggingResponseMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            using (var ms = new MemoryStream())
            {
                var cuerpoOriginalRespuesta = context.Response.Body;
                context.Response.Body = ms;

                await next(context);

                ms.Seek(0, SeekOrigin.Begin);
                string respuesta = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(cuerpoOriginalRespuesta);
                context.Response.Body = cuerpoOriginalRespuesta;

                logger.LogInformation(respuesta);
            }
        }
    }
}

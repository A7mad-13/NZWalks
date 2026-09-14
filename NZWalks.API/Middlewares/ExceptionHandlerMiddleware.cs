using Microsoft.AspNetCore.Http;
using Serilog.Core;
using System.Net;

namespace NZWalks.API.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly ILogger<ExceptionHandlerMiddleware> logger;
        private readonly RequestDelegate Request;

        public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, RequestDelegate request) 
        {
            this.logger = logger;
            this.Request = request;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this.Request(context);
            }
            catch(Exception ex)
            {
                var errorId = Guid.NewGuid();

                //Log the exception
                logger.LogError(ex, $"{errorId} : {ex.Message}");

                //If the response has already started, we cannot rewrite it - rethrow
                //and let the server abort the connection.
                if (context.Response.HasStarted)
                {
                    throw;
                }

                //Return custom error response
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var error = new 
                {
                    Id = errorId,
                    ErrorMessage = "Something went wrong, please contact adminstrator"
                };

                await context.Response.WriteAsJsonAsync(error);

            }
        }

    }
}

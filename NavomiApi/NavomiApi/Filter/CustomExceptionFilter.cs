using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NavomiApi.Interfaces;
using NavomiApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NavomiApi.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        private ILoginService _jwtService;
        
        private IHostingEnvironment _environment;
        private ILogService _logger;

        public CustomExceptionFilter( IHostingEnvironment environment, ILogService logger)
        {
           // _jwtService = jwtService;
            _environment = environment;
            _logger = logger;
        }

        /// <summary>
        /// Global exception handler for otherwise unhandled exceptions
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            // Note: 404-Not Found should be implemented explicitly in the controllers as a business rule, not handled as an exception type          

            _logger.LogError(context.Exception, context.Exception.Message);

            // Default to 500-Internal Server Error
            var errorCode = (int)HttpStatusCode.InternalServerError;

            if (context.Exception.GetType() == typeof(UnauthorizedAccessException))
            {
                errorCode = (int)HttpStatusCode.Unauthorized;
            }
            if (context.Exception.GetType() == typeof(NotImplementedException))
            {
                errorCode = (int)HttpStatusCode.NotImplemented;
            }

            var failedMessage = new FailedMessage()
            {
                ErrorCode = errorCode,
                Message = _environment.IsProduction() ? context.Exception.Message : context.Exception.ToString()
            };

            var tokenString = context.HttpContext.Request.Headers["Authorization"].ToString();

            // Pull the JwtToken off of the Authorization header and instantiate a JwtToken model object with it (if it exists).
            // If the exception being handled is from the RefreshTokenAsync method, don't try to refresh.
            if (!String.IsNullOrEmpty(tokenString) && !failedMessage.Message.Contains("RefreshTokenAsync"))
            {
                try
                {
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    failedMessage.Message = _environment.IsProduction() ? ex.Message : ex.ToString();
                    context.Result = new ObjectResult(failedMessage)
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
                }
            }
            else
            {
                context.Result = new ObjectResult(failedMessage)
                {
                    StatusCode = failedMessage.ErrorCode
                };
            }

            context.ExceptionHandled = true;
        }
    }
}

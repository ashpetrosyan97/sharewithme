using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace SWM.Helpers
{

    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private readonly Logger<ExceptionFilter> _logger;
        public ExceptionFilter(Logger<ExceptionFilter> logger)
        {
            _logger = logger;
        }
        public override void OnException(ExceptionContext context)
        {
            _logger.Log(context.Exception.Message);
            _logger.Log(context.Exception.StackTrace);
            context.HttpContext.Response.StatusCode = context.Exception is CoreException e ? e.Code : (int)HttpStatusCode.InternalServerError;
            context.HttpContext.Response.ContentType = "application/json";
            context.Result = new ObjectResult("")
            {
                Value = context.Exception is CoreException ex ? ex.Message : "Something went wrong try again",
                StatusCode = context.HttpContext.Response.StatusCode
            };

            base.OnException(context);
        }
    }
}

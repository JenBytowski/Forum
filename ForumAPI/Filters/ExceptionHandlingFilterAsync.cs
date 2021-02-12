using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace ForumAPI.Filters
{
    public class ExceptionHandlingFilterAsync : IAsyncExceptionFilter
    {
        private ILogger<ExceptionHandlingFilterAsync> logger;

        public ExceptionHandlingFilterAsync(ILogger<ExceptionHandlingFilterAsync> logger)
        {
            this.logger = logger;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            this.logger.LogError($"{context.Exception.GetType()} {context.Exception.Message}");
        }
    }
}
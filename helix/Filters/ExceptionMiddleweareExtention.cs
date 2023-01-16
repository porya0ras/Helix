using helix.Models.Response;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using System.Net;

namespace helix.Filters
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureBuiltInExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(apperror =>
            {
                apperror.Run(async context =>
                {
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var contextRequest = context.Features.Get<IHttpRequestFeature>();

                    if(contextFeature!=null)
                    {
                        var errorString = new ErrorResponseData()
                        {
                            Error=contextFeature.Error.Message,
                            Path= contextRequest.Path,
                            StatusCode=(int)HttpStatusCode.InternalServerError
                        }.ToString();


                        await context.Response.WriteAsync(errorString);
                    }

                });

            });
        }
    }
}

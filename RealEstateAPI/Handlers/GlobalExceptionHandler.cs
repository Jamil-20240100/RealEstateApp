﻿using Microsoft.AspNetCore.Diagnostics;
using RealEstateApp.Core.Application.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace RealEstateAPI.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            string exceptionTitle = "An unexpected error occurred";
            string details = exception.Message;

            switch (exception)
            {
                case ApiException apiException:

                    switch (apiException.StatusCode)
                    {
                        case (int)HttpStatusCode.BadRequest:
                            exceptionTitle = "Bad Request";
                            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            break;
                        case (int)HttpStatusCode.InternalServerError:
                            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            break;
                        case (int)HttpStatusCode.NotFound:
                            exceptionTitle = "Not found";
                            httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                            break;
                        default:
                            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            break;
                    }
                    break;
                case KeyNotFoundException:
                    exceptionTitle = "Not found";
                    httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case ArgumentException or ValidationException:
                    exceptionTitle = "Bad Request";
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                default:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var problemDetails = new
            {
                Title = exceptionTitle,
                Status = httpContext.Response.StatusCode,
                Detail = details,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

            return true;
        }
    }
}

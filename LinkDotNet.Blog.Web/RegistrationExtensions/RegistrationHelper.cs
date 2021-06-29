using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web.RegistrationExtensions
{
    public static class RegistrationHelper
    {
        public static void AssertNotAlreadyRegistered<T>(this IServiceCollection services)
        {
            var repoExists = services.Any(s => s.ServiceType == typeof(T));
            if (repoExists)
            {
                throw new NotSupportedException(
                    $"Can't have multiple implementations registered of type {nameof(T)}");
            }
        }
    }
}
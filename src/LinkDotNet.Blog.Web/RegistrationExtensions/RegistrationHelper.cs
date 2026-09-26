using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.Web.RegistrationExtensions;

public static class RegistrationHelper
{
    public static void AssertNotAlreadyRegistered(this IServiceCollection services, Type typeToCheck)
    {
        ArgumentNullException.ThrowIfNull(typeToCheck);
        var repoExists = services.Any(s => s.ServiceType == typeToCheck);
        if (repoExists)
        {
            throw new NotSupportedException(
                $"Can't have multiple implementations registered of type {typeToCheck.Name}");
        }
    }
}

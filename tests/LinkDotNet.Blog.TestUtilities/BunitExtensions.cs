using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace LinkDotNet.Blog.TestUtilities;

public static class BunitExtensions
{
    public static IRenderedComponent<TComponent> WaitForComponent<TComponent>(this IRenderedFragment component) 
        where TComponent : IComponent
    {
        component.WaitForState(component.HasComponent<TComponent>);
        return component.FindComponent<TComponent>();
    }
    
    public static IReadOnlyList<IRenderedComponent<TComponent>> WaitForComponents<TComponent>(this IRenderedFragment component) 
        where TComponent : IComponent
    {
        component.WaitForState(component.HasComponent<TComponent>);
        return component.FindComponents<TComponent>();
    }
}

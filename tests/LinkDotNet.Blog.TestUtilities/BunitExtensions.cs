using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace LinkDotNet.Blog.TestUtilities;

public static class BunitExtensions
{
    public static RenderedComponent<TComponent> WaitForComponent<TComponent>(this RenderedFragment component) 
        where TComponent : IComponent
    {
        component.WaitForState(component.HasComponent<TComponent>);
        return component.FindComponent<TComponent>();
    }
    
    public static IReadOnlyList<RenderedComponent<TComponent>> WaitForComponents<TComponent>(this RenderedFragment component) 
        where TComponent : IComponent
    {
        component.WaitForState(component.HasComponent<TComponent>);
        return component.FindComponents<TComponent>();
    }
}

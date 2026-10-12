using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace LinkDotNet.Blog.TestUtilities;

public static class BunitExtensions
{
    public static IRenderedComponent<TComponent> WaitForComponent<TComponent>(this IRenderedComponent<IComponent> component) 
        where TComponent : IComponent
    {
        component.WaitForState(component.HasComponent<TComponent>);
        return component.FindComponent<TComponent>();
    }
    
    public static IReadOnlyList<IRenderedComponent<TComponent>> WaitForComponents<TComponent>(this IRenderedComponent<IComponent> component) 
        where TComponent : IComponent
    {
        component.WaitForState(component.HasComponent<TComponent>);
        return component.FindComponents<TComponent>();
    }

    public static BunitJSModuleInterop SetupNativeDialog(this BunitJSInterop jsInterop)
    {
        var module = jsInterop.SetupModule("./Features/Components/NativeDialog.razor.js");
        module.Mode = JSRuntimeMode.Loose;
        return module;
    }
}

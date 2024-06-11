using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace LinkDotNet.Blog.TestUtilities.Fakes;

public sealed class MarkdownFake : ComponentBase
{
    [Parameter]
    public string Value { get; set; }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public string Class { get; set; }

    [Parameter]
    public string Id { get; set; }

    [Parameter]
    public int Rows { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "textarea");
        builder.AddAttribute(1, "id", Id);
        builder.AddAttribute(2, "class", Class);
        builder.AddAttribute(3, "rows", Rows);
        builder.AddAttribute(4, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, async args =>
        {
            Value = args.Value.ToString();
            await ValueChanged.InvokeAsync(Value);
        }));
        builder.AddContent(5, Value);
        builder.CloseElement();
    }
}
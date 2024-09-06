using LinkDotNet.Blog.Web.Features.Components;

namespace LinkDotNet.Blog.IntegrationTests.Web.Shared;

public class ConfirmDialogTests
{
    [Fact]
    public void ShouldInvokeEventOnOkClick()
    {
        var okWasClicked = false;
        using var ctx = new BunitContext();
        var cut = ctx.Render<ConfirmDialog>(
            b => b
                .Add(p => p.OnYesPressed, _ => okWasClicked = true));
        cut.Instance.Open();

        cut.Find("#ok").Click();

        okWasClicked.ShouldBeTrue();
    }
}

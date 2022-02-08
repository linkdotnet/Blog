using Bunit;

namespace LinkDotNet.Blog.IntegrationTests.Web.Shared;

public class ConfirmDialogTests
{
    [Fact]
    public void ShouldInvokeEventOnOkClick()
    {
        var okWasClicked = false;
        using var ctx = new TestContext();
        var cut = ctx.RenderComponent<ConfirmDialog>(
            b => b
                .Add(p => p.OnYesPressed, _ => okWasClicked = true));
        cut.Instance.Open();

        cut.Find("#ok").Click();

        okWasClicked.Should().BeTrue();
    }
}

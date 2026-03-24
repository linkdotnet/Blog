using System.Linq;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Bunit.TestDoubles;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.Web.Features.Services.FileUpload;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Components;

public class MarkdownTextAreaTests : BunitContext
{
    private void SetupServices()
    {
        Services.AddScoped(_ => Substitute.For<IBlobUploadService>());
        Services.AddScoped(_ => Substitute.For<IToastService>());
        JSInterop.Setup<bool>("markdownEditor.isMac", _ => true).SetResult(false);
        JSInterop.SetupVoid("markdownEditor.setupKeyboardShortcuts", _ => true).SetVoidResult();
        JSInterop.SetupVoid("markdownEditor.setupKeyboardShortcuts", invocation => invocation.Arguments.Count == 2).SetVoidResult();
        JSInterop.SetupVoid("markdownEditor.highlightCodeBlocks");
    }

    [Fact]
    public void ShouldRenderTextArea()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "Test content")
            .Add(c => c.Rows, 10));

        var textarea = cut.Find("textarea");
        textarea.ShouldNotBeNull();
        textarea.ClassList.ShouldContain("markdown-textarea");
    }

    [Fact]
    public void ShouldRenderToolbar()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10));

        var toolbar = cut.Find(".markdown-toolbar");
        toolbar.ShouldNotBeNull();

        cut.FindAll(".toolbar-btn").Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void ShouldHaveUndoRedoButtons()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10));

        var undoButton = cut.Find("button[title*='Undo']");
        var redoButton = cut.Find("button[title*='Redo']");

        undoButton.ShouldNotBeNull();
        redoButton.ShouldNotBeNull();
    }

    [Fact]
    public void ShouldHaveBoldItalicButtons()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10));

        var boldButton = cut.Find("button[title*='Bold']");
        var italicButton = cut.Find("button[title*='Italic']");

        boldButton.ShouldNotBeNull();
        italicButton.ShouldNotBeNull();
    }

    [Fact]
    public void ShouldHaveHeadingButtons()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10));

        var h1Button = cut.Find("button[title='Heading 1']");
        var h2Button = cut.Find("button[title='Heading 2']");
        var h3Button = cut.Find("button[title='Heading 3']");

        h1Button.ShouldNotBeNull();
        h2Button.ShouldNotBeNull();
        h3Button.ShouldNotBeNull();
    }

    [Fact]
    public void ShouldHaveCodeButtons()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10));

        var codeButton = cut.Find("button[title*='Code']");
        var codeBlockButton = cut.Find("button[title='Code Block']");

        codeButton.ShouldNotBeNull();
        codeBlockButton.ShouldNotBeNull();
    }

    [Fact]
    public void ShouldHaveLinkAndImageButtons()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10));

        var linkButton = cut.Find("button[title*='Link']");
        var imageButton = cut.Find("button[title='Image']");

        linkButton.ShouldNotBeNull();
        imageButton.ShouldNotBeNull();
    }

    [Fact]
    public void ShouldHaveListButtons()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10));

        var ulButton = cut.Find("button[title='Unordered List']");
        var olButton = cut.Find("button[title='Ordered List']");

        ulButton.ShouldNotBeNull();
        olButton.ShouldNotBeNull();
    }

    [Fact]
    public void ShouldHavePreviewButton()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10));

        var previewButton = cut.Find("button[title*='Preview']");

        previewButton.ShouldNotBeNull();
        previewButton.QuerySelector("i.bi-eye").ShouldNotBeNull();
    }

    [Fact]
    public async Task ShouldTogglePreviewMode()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "# Test\n\nSome content")
            .Add(c => c.Rows, 10));

        var previewButton = cut.Find("button[title*='Preview']");

        await previewButton.ClickAsync();

        var preview = cut.Find(".markdown-preview");
        preview.ShouldNotBeNull();

        cut.FindAll("textarea").ShouldBeEmpty();
    }

    [Fact]
    public async Task ShouldRenderMarkdownInPreview()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "# Heading\n\nParagraph")
            .Add(c => c.Rows, 10));

        var previewButton = cut.Find("button[title*='Preview']");
        await previewButton.ClickAsync();

        var preview = cut.Find(".markdown-preview");
        preview.InnerHtml.ShouldContain("<h1");
        preview.InnerHtml.ShouldContain("Heading");
    }

    [Fact]
    public async Task ShouldToggleBackToEditor()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "# Test")
            .Add(c => c.Rows, 10));

        var previewButton = cut.Find("button[title*='Preview']");
        await previewButton.ClickAsync();
        await previewButton.ClickAsync();

        var textarea = cut.Find("textarea");
        textarea.ShouldNotBeNull();

        cut.FindAll(".markdown-preview").ShouldBeEmpty();
    }

    [Fact]
    public void ShouldSetPlaceholder()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10)
            .Add(c => c.Placeholder, "Enter markdown here"));

        var textarea = cut.Find("textarea");
        textarea.GetAttribute("placeholder").ShouldBe("Enter markdown here");
    }

    [Fact]
    public void ShouldSetCustomClass()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10)
            .Add(c => c.Class, "custom-class"));

        var container = cut.Find(".markdown-editor-container");
        container.ClassList.ShouldContain("custom-class");
    }

    [Fact]
    public void ShouldSetCustomId()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10)
            .Add(c => c.Id, "custom-id"));

        var container = cut.Find("#custom-id");
        container.ShouldNotBeNull();
    }

    [Fact]
    public void ShouldCalculateHeightFromRows()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 20));

        var editorContent = cut.Find(".editor-content");
        editorContent.ShouldNotBeNull();
        var attribute = editorContent.GetAttribute("style");
        attribute.ShouldNotBeNull();
        attribute.ShouldContain("500px");
    }

    [Fact]
    public async Task ShouldInvokeValueChangedCallback()
    {
        SetupServices();

        var valueChanged = false;
        var newValue = string.Empty;

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10)
            .Add(c => c.ValueChanged, v => { valueChanged = true; newValue = v; }));

        var textarea = cut.Find("textarea");
        await textarea.InputAsync("New content");

        valueChanged.ShouldBeTrue();
        newValue.ShouldBe("New content");
    }

    [Fact]
    public void ShouldHaveInputFileComponent()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10));

        var inputFile = cut.Find("input[type='file']");
        inputFile.ShouldNotBeNull();
        inputFile.GetAttribute("accept").ShouldBe("image/*");
        var attribute = inputFile.GetAttribute("style");
        attribute.ShouldNotBeNull();
        attribute.ShouldContain("display: none");
    }

    [Fact]
    public async Task ShouldCallSetupKeyboardShortcutsOnFirstRender()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10));

        await cut.InvokeAsync(() => Task.CompletedTask);

        JSInterop.Invocations.ShouldContain(i => i.Identifier == "markdownEditor.setupKeyboardShortcuts");
    }

    [Fact]
    public void ShouldHaveStrikethroughButton()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10));

        var button = cut.Find("button[title='Strikethrough']");
        button.ShouldNotBeNull();
        button.QuerySelector("i.bi-type-strikethrough").ShouldNotBeNull();
    }

    [Fact]
    public void ShouldHaveSuperscriptAndSubscriptButtons()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10));

        cut.Find("button[title='Superscript']").ShouldNotBeNull();
        cut.Find("button[title='Subscript']").ShouldNotBeNull();
    }

    [Fact]
    public void ShouldHaveTableButton()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10));

        var tableButton = cut.Find("button[title='Table']");
        tableButton.ShouldNotBeNull();
        tableButton.QuerySelector("i.bi-table").ShouldNotBeNull();
    }

    [Fact]
    public async Task ShouldShowTablePickerWhenTableButtonClicked()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10));

        cut.FindAll(".table-popover").ShouldBeEmpty();

        await cut.Find("button[title='Table']").ClickAsync();

        cut.Find(".table-popover").ShouldNotBeNull();
    }

    [Fact]
    public async Task ShouldHideTablePickerWhenTableButtonClickedAgain()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10));

        var tableButton = cut.Find("button[title='Table']");
        await tableButton.ClickAsync();
        await cut.Find("button[title='Table']").ClickAsync();

        cut.FindAll(".table-popover").ShouldBeEmpty();
    }

    [Fact]
    public async Task ShouldInsertCorrectTableMarkdown()
    {
        SetupServices();
        JSInterop.Setup<object>("markdownEditor.insertText", _ => true).SetException(new JSException("no JS"));

        var capturedValue = string.Empty;
        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10)
            .Add(c => c.ValueChanged, v => capturedValue = v));

        await cut.Find("button[title='Table']").ClickAsync();

        var insertButton = cut.Find(".table-popover .btn-primary");
        await insertButton.ClickAsync();

        capturedValue.ShouldContain("|  |  |");
        capturedValue.ShouldContain("| --- | --- |");
        cut.FindAll(".table-popover").ShouldBeEmpty();
    }

    [Fact]
    public void ShouldHaveTaskListButton()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10));

        var button = cut.Find("button[title='Task List']");
        button.ShouldNotBeNull();
        button.QuerySelector("i.bi-list-check").ShouldNotBeNull();
    }

    [Fact]
    public void ShouldHaveHorizontalRuleButton()
    {
        SetupServices();

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10));

        var button = cut.Find("button[title*='Horizontal Rule']");
        button.ShouldNotBeNull();
        button.QuerySelector("i.bi-hr").ShouldNotBeNull();
    }

    [Fact]
    public async Task ShouldCallInsertLinePrefixesForBlockQuote()
    {
        SetupServices();
        JSInterop.SetupVoid("markdownEditor.insertLinePrefixes", _ => true);

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10));

        await cut.Find("button[title*='Quote']").ClickAsync();

        var invocation = JSInterop.Invocations
            .FirstOrDefault(i => i.Identifier == "markdownEditor.insertLinePrefixes");
        invocation.Identifier.ShouldBe("markdownEditor.insertLinePrefixes");
        invocation.Arguments[1].ShouldBe("> ");
    }

    [Fact]
    public async Task ShouldInsertHorizontalRuleViaFallback()
    {
        SetupServices();
        JSInterop.Setup<object>("markdownEditor.insertHorizontalRule", _ => true)
            .SetException(new JSException("no JS"));

        var capturedValue = string.Empty;
        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10)
            .Add(c => c.ValueChanged, v => capturedValue = v));

        await cut.Find("button[title*='Horizontal Rule']").ClickAsync();

        capturedValue.ShouldContain("---");
    }

    [Fact]
    public async Task ShouldShowErrorWhenFileExceedsMaxSize()
    {
        var toastService = Substitute.For<IToastService>();
        Services.AddScoped(_ => Substitute.For<IBlobUploadService>());
        Services.AddScoped(_ => toastService);
        JSInterop.Setup<bool>("markdownEditor.isMac", _ => true).SetResult(false);
        JSInterop.SetupVoid("markdownEditor.setupKeyboardShortcuts", _ => true).SetVoidResult();
        JSInterop.SetupVoid("markdownEditor.highlightCodeBlocks");

        var cut = Render<MarkdownTextArea>(p => p
            .Add(c => c.Value, "")
            .Add(c => c.Rows, 10));

        var mockFile = Substitute.For<IBrowserFile>();
        mockFile.Size.Returns(600 * 1024L); // 600 KB > 512 KB
        mockFile.Name.Returns("large-image.jpg");

        var inputFile = cut.FindComponent<InputFile>();
        await cut.InvokeAsync(() => inputFile.Instance.OnChange.InvokeAsync(
            new InputFileChangeEventArgs(new[] { mockFile })));

        toastService.Received(1).ShowError(
            Arg.Is<string>(msg => msg.Contains("large-image.jpg") && msg.Contains("512 KB")));
    }
}

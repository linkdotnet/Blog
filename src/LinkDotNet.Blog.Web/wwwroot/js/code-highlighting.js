window.highlightCodeBlocks = function () {
    // highlight.js is loaded async and invokes this again once it is available
    if (typeof hljs === 'undefined') {
        return;
    }

    document.querySelectorAll('pre code:not([data-highlighted])').forEach((block) => {
        hljs.highlightElement(block);
    });
};

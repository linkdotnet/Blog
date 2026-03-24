window.markdownEditor = {
    isMac: function() {
        return navigator.platform.toUpperCase().indexOf('MAC') >= 0 ||
               navigator.userAgent.toUpperCase().indexOf('MAC') >= 0;
    },

    insertLinePrefixes: function(textareaElement, prefix) {
        const textarea = textareaElement;
        textarea.focus();

        const start = textarea.selectionStart;
        const end = textarea.selectionEnd;
        const selectedText = textarea.value.substring(start, end);

        if (selectedText) {
            const prefixed = selectedText.split('\n').map(line => prefix + line).join('\n');
            document.execCommand('insertText', false, prefixed);
            textarea.setSelectionRange(start, start + prefixed.length);
        } else {
            document.execCommand('insertText', false, prefix);
            textarea.setSelectionRange(start + prefix.length, start + prefix.length);
        }

        textarea.dispatchEvent(new Event('input', { bubbles: true }));
    },

    insertHorizontalRule: function(textareaElement) {
        const textarea = textareaElement;
        textarea.focus();

        const pos = textarea.selectionStart;
        const value = textarea.value;

        const needsLeadingNewline = pos > 0 && value[pos - 1] !== '\n';
        const needsTrailingNewline = pos < value.length && value[pos] !== '\n';

        const rule = (needsLeadingNewline ? '\n' : '') + '---' + (needsTrailingNewline ? '\n' : '');
        document.execCommand('insertText', false, rule);
        textarea.dispatchEvent(new Event('input', { bubbles: true }));
    },

    insertText: function(textareaElement, prefix, suffix) {
        const textarea = textareaElement;
        textarea.focus();

        const start = textarea.selectionStart;
        const end = textarea.selectionEnd;
        const selectedText = textarea.value.substring(start, end);

        if (selectedText) {
            document.execCommand('insertText', false, prefix + selectedText + suffix);
            const newStart = start + prefix.length;
            const newEnd = newStart + selectedText.length;
            textarea.setSelectionRange(newEnd, newEnd);
        } else {
            document.execCommand('insertText', false, prefix + suffix);
            const cursorPos = start + prefix.length;
            textarea.setSelectionRange(cursorPos, cursorPos);
        }

        textarea.dispatchEvent(new Event('input', { bubbles: true }));
    },

    clickInputFile: function(fileInputElement) {
        fileInputElement.click();
    },

    undo: function(textareaElement) {
        if (textareaElement && typeof textareaElement.focus === 'function') {
            textareaElement.focus();
            document.execCommand('undo');
        } else {
            document.execCommand('undo');
        }
    },

    redo: function(textareaElement) {
        if (textareaElement && typeof textareaElement.focus === 'function') {
            textareaElement.focus();
            document.execCommand('redo');
        } else {
            document.execCommand('redo');
        }
    },

    getUndoRedoState: function(textareaElement) {
        return {
            canUndo: textareaElement.value && textareaElement.value.length > 0,
            canRedo: false
        };
    },

    highlightCodeBlocks: function() {
        if (typeof hljs !== 'undefined') {
            document.querySelectorAll('.markdown-preview pre code').forEach((block) => {
                hljs.highlightElement(block);
            });
        }
    },

    getFiles: function(fileInput) {
        if (!fileInput.files || fileInput.files.length === 0) {
            return [];
        }
        const fileNames = [];
        for (let i = 0; i < fileInput.files.length; i++) {
            fileNames.push(fileInput.files[i].name);
        }
        return fileNames;
    },

    readFile: function(fileInput, fileName) {
        return new Promise((resolve, reject) => {
            const file = Array.from(fileInput.files).find(f => f.name === fileName);
            if (!file) {
                reject(new Error('File not found'));
                return;
            }

            const reader = new FileReader();
            reader.onload = function(e) {
                const arrayBuffer = e.target.result;
                const uint8Array = new Uint8Array(arrayBuffer);
                resolve(Array.from(uint8Array));
            };
            reader.onerror = function(e) {
                reject(new Error('Failed to read file'));
            };
            reader.readAsArrayBuffer(file);
        });
    },

    setupKeyboardShortcuts: function(textareaElement, dotNetHelper) {
        if (textareaElement._keyboardListenerAttached) {
            return;
        }

        const textarea = textareaElement;
        const isMac = navigator.platform.toUpperCase().indexOf('MAC') >= 0;

        textarea.addEventListener('keydown', function(e) {
            const cmdOrCtrl = isMac ? e.metaKey : e.ctrlKey;

            if (cmdOrCtrl && !e.shiftKey && !e.altKey) {
                switch(e.key.toLowerCase()) {
                    case 'z':
                    case 'y':
                        return;
                    case 'b':
                        e.preventDefault();
                        dotNetHelper.invokeMethodAsync('HandleKeyboardShortcut', 'bold');
                        break;
                    case 'i':
                        e.preventDefault();
                        dotNetHelper.invokeMethodAsync('HandleKeyboardShortcut', 'italic');
                        break;
                    case 'k':
                        e.preventDefault();
                        dotNetHelper.invokeMethodAsync('HandleKeyboardShortcut', 'link');
                        break;
                    case 'e':
                        e.preventDefault();
                        dotNetHelper.invokeMethodAsync('HandleKeyboardShortcut', 'code');
                        break;
                }
            } else if (cmdOrCtrl && e.shiftKey && !e.altKey) {
                switch(e.key.toLowerCase()) {
                    case 'z':
                        return;
                    case 'p':
                        e.preventDefault();
                        dotNetHelper.invokeMethodAsync('HandleKeyboardShortcut', 'preview');
                        break;
                    case '.':
                        e.preventDefault();
                        dotNetHelper.invokeMethodAsync('HandleKeyboardShortcut', 'quote');
                        break;
                    case 'h':
                        e.preventDefault();
                        dotNetHelper.invokeMethodAsync('HandleKeyboardShortcut', 'hr');
                        break;
                }
            }
        });

        textarea._keyboardListenerAttached = true;
    }
};

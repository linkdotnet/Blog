export async function show(dialog, dotNetHelper) {
    dialog.onclose = () => {
        // Bootstrap's own Dialog API toggles this class to stop the page from scrolling behind the modal
        if (!document.querySelector('dialog[open]:modal')) {
            document.documentElement.classList.remove('dialog-open');
        }
        dotNetHelper.invokeMethodAsync('OnClosedAsync');
    };
    dialog.showModal();
    document.documentElement.classList.add('dialog-open');
    // Bootstrap fades .dialog in from visibility: hidden, so the browser's own initial focus finds nothing
    await Promise.all(dialog.getAnimations().map(animation => animation.finished));
    dialog.focus();
}

export function close(dialog) {
    dialog.close();
}

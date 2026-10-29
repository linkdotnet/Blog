export async function show(dialog, dotNetHelper) {
    dialog.onclose = () => {
        unlockScroll();
        dotNetHelper.invokeMethodAsync('OnClosedAsync');
    };
    dialog.showModal();
    // Bootstrap's own Dialog API toggles this class to stop the page from scrolling behind the modal
    document.documentElement.classList.add('dialog-open');
    // Bootstrap fades .dialog in from visibility: hidden, so the browser's own initial focus finds nothing.
    // Closing mid fade cancels the animations, which rejects their finished promises.
    await Promise.allSettled(dialog.getAnimations().map(animation => animation.finished));
    if (dialog.open) {
        dialog.focus();
    }
}

export function close(dialog) {
    dialog.close();
}

// Leaving the page removes an open dialog without firing close, which would keep the page scroll-locked
export function dispose(dialog) {
    dialog.onclose = null;
    if (dialog.isConnected && dialog.open) {
        dialog.close();
    }
    unlockScroll();
}

function unlockScroll() {
    if (!document.querySelector('dialog[open]:modal')) {
        document.documentElement.classList.remove('dialog-open');
    }
}

let cleanup = null;

export function initReadingProgress(containerSelector, progressContainer, progressBar) {
    // Re-init (e.g. Blazor re-render) tears down any previous listeners first.
    cleanup?.();

    let hideTimeout;
    let ticking = false;

    const update = () => {
        ticking = false;
        const container = document.querySelector(containerSelector);
        if (!container) {
            return;
        }

        const contentHeight = container.getBoundingClientRect().height;
        const windowHeight = document.documentElement.clientHeight;
        const scrolled = document.documentElement.scrollTop;
        const maxScroll = contentHeight - windowHeight;
        const progress = maxScroll <= 0 ? 1 : Math.min(1, Math.max(0, scrolled / maxScroll));

        progressBar.style.transform = `scaleX(${progress})`;

        progressContainer.classList.add('visible');
        clearTimeout(hideTimeout);
        hideTimeout = setTimeout(() => progressContainer.classList.remove('visible'), 2000);
    };

    const onScroll = () => {
        if (!ticking) {
            ticking = true;
            requestAnimationFrame(update);
        }
    };

    window.addEventListener('scroll', onScroll, { passive: true });
    window.addEventListener('resize', onScroll, { passive: true });

    cleanup = () => {
        window.removeEventListener('scroll', onScroll);
        window.removeEventListener('resize', onScroll);
        clearTimeout(hideTimeout);
    };
}

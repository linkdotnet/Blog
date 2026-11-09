let progressTimeout;
let rafId;
let contentSelector;
let progressContainer;

function getContentHeight(className) {
    const content = document.querySelector(className);
    if (!content) {
        return 0;
    }
    const contentRect = content.getBoundingClientRect();
    return contentRect.height;
}

function showProgressIndicator(container) {
    container.classList.add("visible");
    container.style.animation = 'none';
}

function hideProgressIndicator(container) {
    container.style.animation = 'fadeOut 0.5s forwards';
    setTimeout(() => {
        container.classList.remove('visible');
    }, 500);
}

function updateProgress() {
    rafId = null;
    if (!progressContainer?.isConnected) {
        return;
    }

    clearTimeout(progressTimeout);

    const contentHeight = getContentHeight(contentSelector);
    const windowHeight = document.documentElement.clientHeight;
    const scrollAmount = document.documentElement.scrollTop;
    const maxScrollAmount = contentHeight - windowHeight;
    const progress = Math.max(0, Math.min(100, (scrollAmount / maxScrollAmount) * 100));
    progressContainer.querySelector('.progress-bar').style.strokeDashoffset = 100 - progress;

    showProgressIndicator(progressContainer);

    const container = progressContainer;
    progressTimeout = setTimeout(() => {
        hideProgressIndicator(container);
    }, 2000);
}

function onScroll() {
    rafId ??= requestAnimationFrame(updateProgress);
}

window.initCircularReadingProgress = (parentContainer, container) => {
    // Blazor navigation keeps the module alive, so a later post only swaps the tracked elements
    contentSelector = parentContainer;
    progressContainer = container;
    if (!window.circularProgressScrollListenerAdded) {
        window.circularProgressScrollListenerAdded = true;
        window.addEventListener('scroll', onScroll, { passive: true });
    }
};

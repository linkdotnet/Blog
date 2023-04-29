let progressTimeout;
let rafId;

function getContentHeight(className) {
    const content = document.querySelector(className);
    if (!content) {
        return 0;
    }
    const contentRect = content.getBoundingClientRect();
    return contentRect.height;
}

function showProgressIndicator(progressContainer) {
    progressContainer.classList.add("visible");
    progressContainer.style.animation = 'none';
}

function hideProgressIndicator(progressContainer) {
    progressContainer.style.animation = 'fadeOut 0.5s forwards';
    setTimeout(() => {
        progressContainer.classList.remove('visible');
    }, 500);
}

function onScroll(onScroll) {
    if (!rafId) {
        rafId = requestAnimationFrame(onScroll);
    }
}

window.initCircularReadingProgress = (parentContainer, progressContainer) => {
    const progressBar = document.getElementById('progressBar');

    const onScroll = () => {
        clearTimeout(progressTimeout);

        const contentHeight = getContentHeight(parentContainer);
        const windowHeight = document.documentElement.clientHeight;
        const scrollAmount = document.documentElement.scrollTop;
        const maxScrollAmount = contentHeight - windowHeight;
        const progress = Math.max(0, Math.min(100, (scrollAmount / maxScrollAmount) * 100));
        progressBar.style.strokeDashoffset = 100 - progress;

        showProgressIndicator(progressContainer);

        progressTimeout = setTimeout(() => {
            hideProgressIndicator(progressContainer);
        }, 2000);

        rafId = null;
    };

    window.addEventListener('scroll', onScroll) ;
};

window.destroyCircularReadingProgress = () => {
    window.removeEventListener('scroll', onScroll);
    clearTimeout(progressTimeout);
    cancelAnimationFrame(rafId);
}
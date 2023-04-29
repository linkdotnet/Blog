let progressTimeout;

function getContentHeight() {
    const content = document.querySelector(".blog-inner-content");
    if (!content) {
        return 0;
    }
    const contentRect = content.getBoundingClientRect();
    return contentRect.height;
}

function showProgressIndicator(progressContainer) {
    progressContainer.classList.add("visible");
    progressContainer.style.animation = "none";
}

function hideProgressIndicator(progressContainer) {
    progressContainer.style.animation = "fadeOut 0.5s forwards";
}

window.initCircularReadingProgress = () => {
    const progressBar = document.getElementById("progressBar");
    const progressContainer = progressBar.closest(".progress-container");

    window.addEventListener("scroll", () => {
        clearTimeout(progressTimeout);

        const contentHeight = getContentHeight();
        const windowHeight = document.documentElement.clientHeight;
        const scrollAmount = document.documentElement.scrollTop;
        const maxScrollAmount = contentHeight - windowHeight;
        const progress = Math.max(0, Math.min(100, (scrollAmount / maxScrollAmount) * 100));
        progressBar.style.strokeDashoffset = 100 - progress;

        showProgressIndicator(progressContainer);

        progressTimeout = setTimeout(() => {
            hideProgressIndicator(progressContainer);
        }, 2000);
    });
};
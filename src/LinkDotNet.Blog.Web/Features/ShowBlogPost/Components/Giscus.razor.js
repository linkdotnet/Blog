window.initGiscus = (divClass, giscus) => {
    const script = document.createElement('script');
    script.src = 'https://giscus.app/client.js'
    script.setAttribute('data-repo', giscus.repository)
    script.setAttribute('data-repo-id', giscus.repositoryId)
    script.setAttribute('data-category', giscus.category)
    script.setAttribute('data-category-id', giscus.categoryId)
    script.setAttribute('data-mapping', 'title')
    script.setAttribute('data-reactions-enabled', '0')
    script.setAttribute('data-emit-metadata', '0')
    script.setAttribute('data-theme', 'dark_dimmed')
    script.crossOrigin = 'anonymous'

    const elementToAppend = document.getElementsByClassName(divClass)[0]
    if (elementToAppend) {
        elementToAppend.appendChild(script)
    }
}
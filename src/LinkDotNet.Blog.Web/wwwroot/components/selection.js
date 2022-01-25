window.getSelectionFromElement = function (id) {
    const elem = document.getElementById(id)
    const start = elem.selectionStart
    const end = elem.selectionEnd
    return { start, end }
}
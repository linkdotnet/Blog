window.getSelectionFromElement = function (id) {
    const elem = document.getElementById(id)
    const start = elem.selectionStart
    const end = elem.selectionEnd
    elem.selectionStart = elem.selectionEnd = 0
    return { start, end }
}
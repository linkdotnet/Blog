window.getSelectionFromElement = function (id) {
    const elem = document.getElementById(id)
    const start = elem.selectionStart
    const end = elem.selectionEnd
    return { start, end }
}

window.setSelectionFromElement = function (id, cursor, newText) {
    const elem = document.getElementById(id)
    document.execCommand("insertText", false, newText)
    elem.selectionStart = cursor
    elem.selectionEnd = cursor
}
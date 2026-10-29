window.setTheme = (theme, remember) => {
	const resolved = theme === 'auto' ? window.getCurrentSystemPreference() : theme
	document.documentElement.setAttribute('data-bs-theme', resolved)
	if (remember) {
		localStorage.setItem('theme', resolved)
	} else {
		localStorage.removeItem('theme')
	}
}

window.getCurrentSystemPreference = () => {
	return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'
}

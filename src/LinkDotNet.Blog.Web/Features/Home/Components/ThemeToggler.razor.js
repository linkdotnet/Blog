window.setTheme = (theme) => {
	if (theme === 'auto' && window.matchMedia('(prefers-color-scheme: dark)').matches) {
		document.documentElement.setAttribute('data-bs-theme', 'dark')
	} else {
		document.documentElement.setAttribute('data-bs-theme', theme)
	}
}

window.getCurrentSystemPreference = () => {
	return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'
}

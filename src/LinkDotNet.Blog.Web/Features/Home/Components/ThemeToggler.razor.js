window.toggleTheme = () => {
	const theme = document.documentElement.dataset.bsTheme === 'dark' ? 'light' : 'dark'
	document.documentElement.dataset.bsTheme = theme
	localStorage.setItem('theme', theme)
}

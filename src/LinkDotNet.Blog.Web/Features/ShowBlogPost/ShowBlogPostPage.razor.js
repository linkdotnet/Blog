import { ScrollSpy } from 'https://cdn.jsdelivr.net/npm/bootstrap@6.0.0-alpha.1/dist/js/bootstrap.bundle.min.js';

let spy;

window.spyToc = () => {
	spy?.dispose();
	spy = undefined;
	const toc = document.querySelector('.toc-slot nav');
	if (toc) {
		// Spying on body: ScrollSpy marks the last link active once its element's end is visible,
		// so a narrower element would jump to the last heading long before the page bottom
		spy = ScrollSpy.getOrCreateInstance(document.body, { target: toc, topMargin: '96px' });
	}
}

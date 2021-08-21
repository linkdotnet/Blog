const templateSlideshowContainer = document.createElement('template')
templateSlideshowContainer.innerHTML = `
<style>
.slideshow-container {
  max-width: 1000px;
  position: relative;
  margin: auto;
}

.mySlides {
  display: none;
}

.prev, .next {
  cursor: pointer;
  position: absolute;
  top: 50%;
  width: auto;
  margin-top: -22px;
  padding: 16px;
  color: black;
  font-weight: bold;
  font-size: 18px;
  transition: 0.6s ease;
  border-radius: 0 3px 3px 0;
  user-select: none;
  background-color: rgba(0,0,0,0.4);
}

.next {
  right: 0;
  border-radius: 3px 0 0 3px;
}

.prev:hover, .next:hover {
  background-color: rgba(0,0,0,0.8);
}

.text {
  color: var(--white);p
  font-size: 15px;
  padding: 8px 12px;
  position: absolute;
  bottom: 8px;
  width: 100%;
  text-align: center;
}

.numbertext {
  color: var(--white);
  font-size: 12px;
  padding: 8px 12px;
  position: absolute;
  top: 0;
}

.dot {
  cursor: pointer;
  height: 15px;
  width: 15px;
  margin: 0 2px;
  background-color: #bbb;
  border-radius: 50%;
  display: inline-block;
  transition: background-color 0.6s ease;
}

.active, .dot:hover {
  background-color: #717171;
}
</style>

<div class="slideshow-container">
  <a class="prev" onclick="this.getRootNode().host.plusSlides(-1)">&#10094;</a>
  <a class="next" onclick="this.getRootNode().host.plusSlides(1)">&#10095;</a>
</div>
`

class Slideshow extends HTMLElement {
    slideIndex = 1

    constructor() {
        super();

        this.attachShadow({ mode: 'open' })

        const elements = this.getSlideshowElements()
        const slideShowContainer = templateSlideshowContainer.content.cloneNode(true)
        const innerContainer = slideShowContainer.querySelector(".slideshow-container")
        const dotContainer = document.createElement("div")
        dotContainer.style = 'text-align: center'
        for (let i = 0; i < elements.length; i++) {
            const container = document.createElement("div")
            container.classList.add('mySlides')

            const numberText = document.createElement("div")
            numberText.classList.add('numbertext')
            numberText.innerHTML = `${i + 1} / ${elements.length}`
            container.appendChild(numberText)

            const image = document.createElement("img")
            image.src = elements[i].url
            image.style = 'width:100%'
            container.appendChild(image)

            const text = document.createElement("div")
            text.classList.add("text")
            text.innerHTML = elements[i].title
            text.style.color = elements[i].color ?? 'black'
            container.appendChild(text)

            innerContainer.appendChild(container)

            const dot = document.createElement("span")
            dot.classList.add('dot')
            dot.addEventListener('click', () => this.showSlides(i + 1))
            dotContainer.appendChild(dot)
        }

        slideShowContainer.appendChild(dotContainer)
        this.shadowRoot.appendChild(slideShowContainer)
        this.showSlides(1)
    }

    getSlideshowElements() {
        const nodes = []

        this.childNodes.forEach(node => {
            if (node.tagName !== 'SLIDE-SHOW-IMAGE') {
                return
            }

            const imgUrl = node.getAttribute('src')
            const title = node.getAttribute('title')
            const color = node.getAttribute('color')

            if (!imgUrl || !title) {
                return
            }

            nodes.push({url: imgUrl, title: title, color: color})
        })

        return nodes
    }

    plusSlides(step) {
        this.showSlides(this.slideIndex + step);
    }

    showSlides(newSlideIndex) {
        let i;
        const slides = this.shadowRoot.querySelectorAll(".mySlides");
        const dots = this.shadowRoot.querySelectorAll(".dot");
        if (newSlideIndex > slides.length) {
            this.slideIndex = 1
        }
        else if (newSlideIndex < 1) {
            this.slideIndex = slides.length
        }
        else {
            this.slideIndex = newSlideIndex
        }


        for (i = 0; i < slides.length; i++) {
            slides[i].style.display = "none";
        }

        for (i = 0; i < dots.length; i++) {
            dots[i].className = dots[i].className.replace(" active", "");
        }
        slides[this.slideIndex - 1].style.display = "block";
        dots[this.slideIndex - 1].className += " active";
    }
}

class SlideshowImage extends HTMLElement {
    constructor() {
        super();
    }
}

customElements.define("slide-show", Slideshow)
customElements.define("slide-show-image", SlideshowImage)
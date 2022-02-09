window.initDisqus = (disqus) => {
    var d = document, s = d.createElement('script');

    s.src = `https://${disqus.shortname}.disqus.com/embed.js`;

    s.setAttribute('data-timestamp', +new Date());
    d.body.appendChild(s);
}
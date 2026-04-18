const canvas = document.getElementById("sparksCanvas");

if (canvas) {
    const ctx = canvas.getContext("2d");

    function resize() {
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
    }

    resize();
    window.addEventListener("resize", resize);

    let sparks = [];
    let scrollY = 0;

    function createSpark() {
        sparks.push({
            x: Math.random() * canvas.width,
            y: canvas.height,
            size: Math.random() * 3 + 1,
            speedY: Math.random() * 2 + 0.5,
            speedX: (Math.random() - 0.5) * 0.5,
            alpha: 1
        });
    }

    window.addEventListener("scroll", () => {
        scrollY = window.scrollY;
    });

    function update() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        for (let i = sparks.length - 1; i >= 0; i--) {
            let s = sparks[i];

            s.x += s.speedX;
            s.y -= s.speedY;
            s.alpha -= 0.01;

            ctx.beginPath();
            ctx.arc(s.x, s.y, s.size, 0, Math.PI * 2);
            ctx.fillStyle = `rgba(255,140,0,${s.alpha})`;
            ctx.fill();

            if (s.alpha <= 0) {
                sparks.splice(i, 1);
            }
        }

        requestAnimationFrame(update);
    }

    setInterval(createSpark, 120);
    update();
}


window.addEventListener("load", () => {
    document.body.classList.add("loaded");
});


document.querySelectorAll("a").forEach(link => {

    const href = link.getAttribute("href");

  
    if (
        !href ||
        href === "#" ||
        link.hasAttribute("data-bs-toggle") ||
        link.classList.contains("dropdown-toggle") ||
        link.classList.contains("btn") && link.getAttribute("type") === "submit"
    ) return;

   
    if (link.closest("form")) return;

  
    if (link.hostname === window.location.hostname) {

        link.addEventListener("click", function (e) {

         
            if (e.ctrlKey || e.metaKey) return;

            e.preventDefault();

            const loader = document.getElementById("pageLoader");
            if (loader) loader.classList.add("active");

            setTimeout(() => {
                window.location.href = this.href;
            }, 180);
        });
    }
});
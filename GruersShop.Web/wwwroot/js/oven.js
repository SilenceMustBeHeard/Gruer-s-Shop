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
    let embers = [];
    let flames = [];
    let smoke = [];
    let scrollY = 0;
    let mouseX = canvas.width / 2;
    let mouseY = canvas.height;
    let furnaceIntensity = 1;

    // Track mouse position for interactive sparks
    window.addEventListener("mousemove", (e) => {
        mouseX = e.clientX;
        mouseY = e.clientY;
    });

    // Create a spark (bright, fast, rises up)
    function createSpark(x, y, intensity = 1) {
        sparks.push({
            x: x || Math.random() * canvas.width,
            y: y || canvas.height - Math.random() * 150,
            size: Math.random() * 5 + 2,
            speedY: Math.random() * 4 + 2 * intensity,
            speedX: (Math.random() - 0.5) * 1.2,
            alpha: 0.9,
            color: `hsl(${35 + Math.random() * 20}, 100%, ${55 + Math.random() * 25}%)`,
            rotation: Math.random() * Math.PI * 2,
            rotationSpeed: (Math.random() - 0.5) * 0.1,
            trail: []
        });
    }

    // Create an ember (slower, warmer, flickers)
    function createEmber(x, y) {
        embers.push({
            x: x || Math.random() * canvas.width,
            y: y || canvas.height - 50,
            size: Math.random() * 4 + 1.5,
            speedY: Math.random() * 2 + 0.8,
            speedX: (Math.random() - 0.5) * 0.4,
            alpha: 0.8,
            flicker: Math.random() * 0.5,
            color: `hsl(${30 + Math.random() * 25}, 95%, 48%)`,
            pulse: 0
        });
    }

    // Create flame (stays near bottom, dances)
    function createFlame(x, y) {
        flames.push({
            x: x || Math.random() * canvas.width,
            y: y || canvas.height - 30 + Math.random() * 60,
            size: Math.random() * 12 + 6,
            life: 1,
            decay: 0.01 + Math.random() * 0.02,
            color: `hsl(${45 + Math.random() * 15}, 100%, ${50 + Math.random() * 30}%)`,
            dance: Math.random() * Math.PI * 2,
            danceSpeed: 0.05 + Math.random() * 0.05
        });
    }

    // Create smoke (rises slowly, fades)
    function createSmoke(x, y) {
        smoke.push({
            x: x || Math.random() * canvas.width,
            y: y || canvas.height - 20,
            size: Math.random() * 15 + 8,
            speedY: Math.random() * 0.8 + 0.3,
            speedX: (Math.random() - 0.5) * 0.2,
            alpha: 0.3,
            color: `rgba(80, 60, 40, ${0.1 + Math.random() * 0.2})`
        });
    }

    window.addEventListener("scroll", () => {
        scrollY = window.scrollY;
        // Increase furnace intensity when scrolling
        furnaceIntensity = Math.min(1.5, 1 + scrollY / 500);
    });

    function update() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        // Draw warm furnace glow at the bottom
        const gradient = ctx.createLinearGradient(0, canvas.height - 250, 0, canvas.height);
        gradient.addColorStop(0, 'rgba(255, 80, 20, 0)');
        gradient.addColorStop(0.3, 'rgba(255, 70, 15, 0.05)');
        gradient.addColorStop(0.6, 'rgba(255, 60, 10, 0.12)');
        gradient.addColorStop(1, 'rgba(255, 50, 5, 0.25)');
        ctx.fillStyle = gradient;
        ctx.fillRect(0, 0, canvas.width, canvas.height);

        // Draw flickering fire glow at mouse position
        if (mouseY > canvas.height - 200) {
            const mouseGradient = ctx.createRadialGradient(mouseX, mouseY, 0, mouseX, mouseY, 80);
            mouseGradient.addColorStop(0, 'rgba(255, 100, 30, 0.15)');
            mouseGradient.addColorStop(1, 'rgba(255, 50, 10, 0)');
            ctx.fillStyle = mouseGradient;
            ctx.fillRect(0, 0, canvas.width, canvas.height);
        }

        // Update and draw flames (dancing fire)
        for (let i = flames.length - 1; i >= 0; i--) {
            let f = flames[i];

            f.life -= f.decay;
            f.dance += f.danceSpeed;

            // Dancing flame movement
            f.x += Math.sin(f.dance) * 0.5;
            f.y += Math.cos(f.dance * 0.7) * 0.3 - 0.5;

            const currentSize = f.size * f.life;
            const opacity = f.life * 0.6;

            // Outer flame glow
            ctx.shadowBlur = 20;
            ctx.shadowColor = `rgba(255, 80, 0, ${opacity * 0.8})`;

            // Main flame
            ctx.beginPath();
            ctx.moveTo(f.x, f.y - currentSize);
            ctx.quadraticCurveTo(f.x + currentSize * 0.5, f.y - currentSize * 0.3, f.x, f.y);
            ctx.quadraticCurveTo(f.x - currentSize * 0.5, f.y - currentSize * 0.3, f.x, f.y - currentSize);
            ctx.fillStyle = f.color;
            ctx.fill();

            // Inner flame (hotter)
            ctx.beginPath();
            ctx.moveTo(f.x, f.y - currentSize * 0.7);
            ctx.quadraticCurveTo(f.x + currentSize * 0.3, f.y - currentSize * 0.2, f.x, f.y);
            ctx.quadraticCurveTo(f.x - currentSize * 0.3, f.y - currentSize * 0.2, f.x, f.y - currentSize * 0.7);
            ctx.fillStyle = `rgba(255, 220, 100, ${opacity * 0.9})`;
            ctx.fill();

            ctx.shadowBlur = 0;

            if (f.life <= 0) {
                flames.splice(i, 1);
            }
        }

        // Update and draw sparks with trails
        for (let i = sparks.length - 1; i >= 0; i--) {
            let s = sparks[i];

            // Store previous position for trail
            s.trail.unshift({ x: s.x, y: s.y });
            if (s.trail.length > 5) s.trail.pop();

            // Movement with wind effect
            s.x += s.speedX + Math.sin(Date.now() * 0.003 + i) * 0.15;
            s.y -= s.speedY;
            s.alpha -= 0.012;
            s.rotation += s.rotationSpeed;

            // Draw trail
            for (let t = 0; t < s.trail.length; t++) {
                const trailAlpha = s.alpha * (1 - t / s.trail.length) * 0.5;
                ctx.beginPath();
                ctx.arc(s.trail[t].x, s.trail[t].y, s.size * (1 - t / s.trail.length), 0, Math.PI * 2);
                ctx.fillStyle = `rgba(255, 120, 20, ${trailAlpha})`;
                ctx.fill();
            }

            // Draw spark with glow
            ctx.shadowBlur = 15;
            ctx.shadowColor = `rgba(255, 100, 0, ${s.alpha})`;

            ctx.save();
            ctx.translate(s.x, s.y);
            ctx.rotate(s.rotation);
            ctx.beginPath();
            ctx.moveTo(0, -s.size);
            ctx.lineTo(s.size * 0.5, s.size * 0.5);
            ctx.lineTo(0, s.size * 0.3);
            ctx.lineTo(-s.size * 0.5, s.size * 0.5);
            ctx.closePath();
            ctx.fillStyle = s.color;
            ctx.fill();
            ctx.restore();

            ctx.shadowBlur = 0;

            // Add small smoke trail occasionally
            if (Math.random() < 0.05 && s.alpha > 0.5) {
                createSmoke(s.x, s.y);
            }

            if (s.y < -50 || s.alpha <= 0) {
                sparks.splice(i, 1);
            }
        }

        // Update and draw embers with pulse effect
        for (let i = embers.length - 1; i >= 0; i--) {
            let e = embers[i];

            e.x += e.speedX;
            e.y -= e.speedY;
            e.alpha -= 0.007;
            e.pulse = (e.pulse + 0.05) % (Math.PI * 2);
            e.flicker = 0.6 + Math.sin(Date.now() * 0.01 + i) * 0.3;

            const pulseSize = e.size * (0.8 + Math.sin(e.pulse) * 0.2);

            ctx.beginPath();
            ctx.arc(e.x, e.y, pulseSize * e.flicker, 0, Math.PI * 2);
            ctx.fillStyle = `rgba(255, 100, 30, ${e.alpha * 0.5})`;
            ctx.fill();

            ctx.beginPath();
            ctx.arc(e.x, e.y, pulseSize * 0.6 * e.flicker, 0, Math.PI * 2);
            ctx.fillStyle = `rgba(255, 180, 70, ${e.alpha})`;
            ctx.fill();

            if (e.y < -30 || e.alpha <= 0) {
                embers.splice(i, 1);
            }
        }

        // Update and draw smoke
        for (let i = smoke.length - 1; i >= 0; i--) {
            let s = smoke[i];

            s.x += s.speedX;
            s.y -= s.speedY;
            s.size += 0.2;
            s.alpha -= 0.005;

            ctx.beginPath();
            ctx.arc(s.x, s.y, s.size, 0, Math.PI * 2);
            ctx.fillStyle = `rgba(60, 45, 30, ${s.alpha * 0.3})`;
            ctx.fill();

            if (s.y < -100 || s.alpha <= 0 || s.size > 60) {
                smoke.splice(i, 1);
            }
        }

        requestAnimationFrame(update);
    }

    // Create regular particles with furnace intensity
    setInterval(() => {
        const intensity = furnaceIntensity;
        createSpark(null, null, intensity);
        if (Math.random() < 0.3) createEmber();
        if (Math.random() < 0.2) createFlame();
    }, 60);

    // Create flames more often
    setInterval(() => {
        for (let i = 0; i < 2 + Math.floor(Math.random() * 3); i++) {
            createFlame();
        }
    }, 800);

    // Burst of sparks on scroll
    let lastScroll = 0;
    window.addEventListener("scroll", () => {
        const currentScroll = window.scrollY;
        if (Math.abs(currentScroll - lastScroll) > 50) {
            for (let i = 0; i < 12; i++) {
                setTimeout(() => createSpark(null, null, 1.2), i * 15);
            }
            lastScroll = currentScroll;
        }
    });

    // Mouse interaction - create sparks when moving mouse near bottom
    window.addEventListener("mousemove", (e) => {
        if (e.clientY > canvas.height - 150) {
            if (Math.random() < 0.1) {
                createSpark(e.clientX, e.clientY, 0.8);
            }
        }
    });

    update();
}

// Page loader
window.addEventListener("load", () => {
    document.body.classList.add("loaded");
});

// Smooth navigation with furnace sparks burst
document.querySelectorAll("a").forEach(link => {
    const href = link.getAttribute("href");

    if (
        !href ||
        href === "#" ||
        href === "" ||
        link.hasAttribute("data-bs-toggle") ||
        link.classList.contains("dropdown-toggle") ||
        link.classList.contains("dropdown-item")
    ) return;

    if (href.includes("/Admin/") || link.href?.includes("/Admin/")) {
        return;
    }

    if (link.closest("form")) return;

    if (link.hostname === window.location.hostname) {
        link.addEventListener("click", function (e) {
            if (e.ctrlKey || e.metaKey) return;
            e.preventDefault();

            // Massive spark burst on click
            if (canvas) {
                for (let i = 0; i < 40; i++) {
                    setTimeout(() => {
                        if (typeof createSpark === 'function') {
                            createSpark(null, null, 1.5);
                        }
                    }, i * 8);
                }
                for (let i = 0; i < 5; i++) {
                    setTimeout(() => createFlame(), i * 50);
                }
            }

            const loader = document.getElementById("pageLoader");
            if (loader) loader.classList.add("active");

            setTimeout(() => {
                window.location.href = this.href;
            }, 200);
        });
    }
});
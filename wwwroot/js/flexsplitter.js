// wwwroot/js/flexsplitter.js

// صبر کن تا روت واقعاً اندازه بگیرد (عرض > 0)
async function waitForLayout(el) {
    // دو فریم صبر + تا 10 تلاش اگر width=0 بود
    const raf = () => new Promise(r => requestAnimationFrame(r));
    await raf(); await raf();

    let tries = 0;
    while (tries < 10) {
        const w = el.getBoundingClientRect().width;
        if (w > 0) return;
        tries++;
        await raf();
    }
}

export async function initSplitter(root, left, right, gutter, opts) {
    if (!root || !left || !right || !gutter) return;

    // اگر قبلاً اینیت شده بود، اول پاکسازی
    cleanup(root);

    // اطمینان از آماده بودن لِی‌اوت (مشکل صفحه‌ی اول)
    await waitForLayout(root);

    const state = {
        dragging: false,
        startX: 0,
        startLeft: 0,
        minLeft: opts?.minLeft ?? 200,
        minRight: opts?.minRight ?? 200,
        gutter: gutter,
        left: left,
        right: right,
        root: root
    };

    const onMouseDown = (e) => {
        state.dragging = true;
        state.startX = e.clientX ?? (e.touches?.[0]?.clientX ?? 0);
        state.startLeft = left.getBoundingClientRect().width;
        document.addEventListener('mousemove', onMouseMove);
        document.addEventListener('mouseup', onMouseUp, { once: true });
        // موبایل
        document.addEventListener('touchmove', onMouseMove, { passive: false });
        document.addEventListener('touchend', onMouseUp, { once: true });
        e.preventDefault();
    };

    const onMouseMove = (e) => {
        if (!state.dragging) return;
        const cx = e.clientX ?? (e.touches?.[0]?.clientX ?? 0);
        const dx = cx - state.startX;
        let newLeft = state.startLeft + dx;

        const total = state.root.getBoundingClientRect().width;
        const gutterW = state.gutter.getBoundingClientRect().width;

        const maxLeft = total - gutterW - state.minRight;
        if (newLeft < state.minLeft) newLeft = state.minLeft;
        if (newLeft > maxLeft) newLeft = Math.max(state.minLeft, maxLeft);

        state.left.style.flexBasis = `${newLeft}px`;

        if (opts?.persistKey) {
            try { localStorage.setItem(opts.persistKey, String(newLeft)); } catch { }
        }

        e.preventDefault?.();
    };

    const onMouseUp = () => {
        state.dragging = false;
        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('touchmove', onMouseMove);
    };

    gutter.addEventListener('mousedown', onMouseDown);
    gutter.addEventListener('touchstart', onMouseDown, { passive: false });
    gutter.addEventListener('dragstart', e => e.preventDefault());

    // اگر persisted مقدار داشت، بعد از آماده شدن لِی‌اوت اعمال شود
    if (opts?.persistKey) {
        try {
            const saved = parseFloat(localStorage.getItem(opts.persistKey));
            if (!isNaN(saved) && saved > 0) {
                const total = root.getBoundingClientRect().width;
                const gutterW = gutter.getBoundingClientRect().width;
                const maxLeft = total - gutterW - state.minRight;
                const clamped = Math.max(state.minLeft, Math.min(saved, maxLeft));
                left.style.flexBasis = `${clamped}px`;
            }
        } catch { }
    }

    // برای cleanup در دیسپوز
    root.__fsCleanup = () => {
        gutter.removeEventListener('mousedown', onMouseDown);
        gutter.removeEventListener('touchstart', onMouseDown);
        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('touchmove', onMouseMove);
    };
}

export function cleanup(root) {
    if (root && typeof root.__fsCleanup === 'function') {
        try { root.__fsCleanup(); } catch { }
        root.__fsCleanup = null;
    }
}

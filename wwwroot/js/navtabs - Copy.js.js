// wwwroot/js/navtabs.js

export function watchTabsOverflow(host) {
    if (!host) return;

    const getWrapper = () => host.querySelector('.mud-tabs-scroll-wrapper');

    // --- تشخیص overflow برای نمایش فلش‌ها (اگر CSS‌ داری بر اساس .has-overflow)
    const update = () => {
        const w = getWrapper();
        if (!w) return;
        const has = Math.ceil(w.scrollWidth) > Math.ceil(w.clientWidth) + 1;
        host.classList.toggle('has-overflow', has);
    };

    const w = getWrapper();
    const ro = new ResizeObserver(update);
    if (w) {
        ro.observe(w);
        w.addEventListener('scroll', update, { passive: true });
    }
    window.addEventListener('resize', update);
    requestAnimationFrame(update);

    // --- فریز اسکرول: نذار MudTabs روی کلیک تب‌ها خودکار اسکرول کند
    const restoreAfterMud = () => {
        const wrap = getWrapper();
        if (!wrap) return;
        const keep = lastManualScrollLeft = wrap.scrollLeft;
        let tries = 0;
        const step = () => {
            // چند فریم پشت سر هم به مقدار قبلی برگردان تا اسکرول برنامه‌ای خنثی شود
            wrap.scrollLeft = keep;
            if (tries++ < 8) requestAnimationFrame(step);
        };
        requestAnimationFrame(step);
    };

    let lastManualScrollLeft = w ? w.scrollLeft : 0;

    const onClick = (e) => {
        // اگر روی دکمه‌های فلش اسکرول کلیک شد، اجازه بده کار کنند
        if (e.target.closest('.mud-tabs-scroll-button')) return;
        // فقط وقتی روی خود تب‌ها کلیک می‌شود اسکرول را فریز کن
        if (e.target.closest('.mud-tab')) {
            restoreAfterMud();
        }
    };

    host.addEventListener('click', onClick, true);
    host.addEventListener('mousedown', onClick, true);

    host.__tabsCleanup = () => {
        try { if (w) w.removeEventListener('scroll', update); } catch { }
        try { ro.disconnect(); } catch { }
        window.removeEventListener('resize', update);
        host.removeEventListener('click', onClick, true);
        host.removeEventListener('mousedown', onClick, true);
    };
}

export function cleanupTabs(host) {
    if (host && typeof host.__tabsCleanup === 'function') {
        try { host.__tabsCleanup(); } catch { }
        host.__tabsCleanup = null;
    }
}

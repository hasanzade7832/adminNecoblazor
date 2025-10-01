// wwwroot/js/auth.js
(function () {
    function getCookie(name) {
        if (typeof document === "undefined") return null;
        const nameEQ = name + "=";
        const ca = document.cookie.split(";");
        for (let i = 0; i < ca.length; i++) {
            let c = ca[i];
            while (c.charAt(0) === " ") c = c.substring(1);
            if (c.indexOf(nameEQ) === 0) return decodeURIComponent(c.substring(nameEQ.length));
        }
        return null;
    }

    function getFromStorage(keys) {
        try {
            for (const k of keys) {
                const v = localStorage.getItem(k) || sessionStorage.getItem(k);
                if (v && v.trim()) return v.trim();
            }
        } catch { }
        return null;
    }

    function getAnyToken() {
        // اول از استوریج، بعد کوکی (مشابه پیاده‌سازی React شما)
        return (
            getFromStorage(["app.authToken", "token", "TTKK", "authToken", "access_token"]) ||
            getCookie("token") ||
            getCookie("TTKK") ||
            getCookie("authToken") ||
            getCookie("access_token") ||
            null
        );
    }

    function getAnyUsername() {
        return (
            getFromStorage(["app.username", "username", "user"]) ||
            getCookie("username") ||
            getCookie("user") ||
            null
        );
    }

    window.AdminNecoAuth = { getAnyToken, getAnyUsername };
})();

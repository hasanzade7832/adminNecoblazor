// wwwroot/js/apiBridge.js

function apiBase() {
    const meta = document.querySelector('meta[name="api-base"]');
    return (meta && meta.content) ? meta.content : "";
}

function buildAuthHeader() {
    const has = window.AdminNecoAuth;
    const token = has && has.getAnyToken ? has.getAnyToken() : null;
    const user = has && has.getAnyUsername ? has.getAnyUsername() : null;
    if (!token) return null;
    // ★★ هدر مطابق نیاز شما: Bearer <token>:<username>
    return "Bearer " + token + (user ? (":" + user) : "");
}

async function apiFetch(method, path, body) {
    const base = apiBase();
    const full = base ? (base.endsWith("/") ? base + path : base + "/" + path) : path;

    const headers = { "Content-Type": "application/json" };
    const auth = buildAuthHeader();
    if (auth) headers["Authorization"] = auth;

    const res = await fetch(full, {
        method,
        mode: "cors",
        credentials: "include",
        headers,
        body: body != null ? JSON.stringify(body) : undefined
    });

    if (!res.ok) {
        const t = await res.text().catch(() => "");
        throw new Error(`HTTP ${res.status} ${res.statusText}${t ? `: ${t}` : ""}`);
    }

    const ct = res.headers.get("content-type") || "";
    return ct.includes("application/json") ? await res.json() : null;
}

export const postJson = (path, body) => apiFetch("POST", path, body);
export const getJson = (path) => apiFetch("GET", path, null);
export { apiFetch, buildAuthHeader };

import { API_BASE } from "./config";

async function http(url, opts = {}) {
  const res = await fetch(API_BASE + url, {
    headers: { "Content-Type": "application/json", ...(opts.headers || {}) },
    ...opts,
  });
  if (!res.ok) {
    const txt = await res.text();
    throw new Error(txt || `HTTP ${res.status}`);
  }
  const ct = res.headers.get("content-type") || "";
  return ct.includes("application/json") ? res.json() : res.text();
}

export const Api = {
  listCategories() {
    return http("/api/categories");
  },
  listProducts(params) {
    const q = new URLSearchParams(params);
    return http(`/api/products?${q.toString()}`);
  },
  getProduct(idOrSlug) {
    return http(`/api/products/${encodeURIComponent(idOrSlug)}`);
  },
  createOrder(payload) {
    return http(`/api/orders`, {
      method: "POST",
      body: JSON.stringify(payload),
    });
  },
};

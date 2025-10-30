import { API_BASE } from "./config.js";

async function http(path, opts = {}) {
  const res = await fetch(API_BASE + path, {
    headers: { "Content-Type": "application/json", ...(opts.headers || {}) },
    ...opts,
  });
  if (!res.ok) {
    const msg = await res.text().catch(() => `HTTP ${res.status}`);
    throw new Error(msg || `HTTP ${res.status}`);
  }
  const ct = res.headers.get("content-type") || "";
  return ct.includes("application/json") ? res.json() : res.text();
}

export const Api = {
  listCategories() {
    return http("/api/categories");
  },
  listProducts(params) {
    const sp = new URLSearchParams();
    if (params) {
      Object.entries(params).forEach(([k, v]) => {
        if (Array.isArray(v)) v.forEach((x) => sp.append(k, x));
        else if (v !== undefined && v !== null && v !== "") sp.append(k, v);
      });
    }
    return http(`/api/products?${sp.toString()}`);
  },
  getProduct(idOrSlug) {
    return http(`/api/products/product/${encodeURIComponent(idOrSlug)}`);
  },
  createOrder(payload) {
    return http(`/api/orders`, {
      method: "POST",
      body: JSON.stringify(payload),
    });
  },
  sayHello() {
    return http(`/api/products/say-hello`);
  },
};

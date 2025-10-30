import { initHeader } from "./header.js";
import { renderCatalog } from "./views/catalog-view.js";
import { renderProduct } from "./views/product-view.js";
import { renderCart } from "./views/cart-view.js";

initHeader();

const routes = [
  { match: /^#\/?$/, action: () => renderCatalog() },
  { match: /^#\/product\/([^/]+)$/, action: (id) => renderProduct(id) },
  { match: /^#\/cart$/, action: () => renderCart() },
];

function onRoute() {
  const hash = location.hash || "#/";
  for (const r of routes) {
    const m = hash.match(r.match);
    if (m) return r.action(...m.slice(1));
  }
  renderCatalog();
}

window.addEventListener("hashchange", onRoute);
onRoute();

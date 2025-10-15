import { renderCatalog } from "./pages/catalog";
import { renderProduct } from "./pages/product";
import { renderCart } from "./pages/cart";

const app = document.getElementById("app");

function navigate(pathname) {
  console.log(navigate);
  history.pushState({}, "", pathname);
  route();
}

function route() {
  const url = new URL(window.location.href);
  const parts = url.pathname.split("/").filter(Boolean);
  const path = parts[0] || "catalog";

  switch (path) {
    case "catalog":
      renderCatalog(app, url);
      break;
    case "product":
      if (parts[1]) {
        renderProduct(app, parts[1]);
      } else {
        app.innerHTML = `<h1>Incorrect item URL</h1>`;
      }
      break;
    case "cart":
      renderCart(app);
      break;
    default:
      app.innerHTML = `<h1>404 - Page not found</h1>`;
      break;
  }
}

document.addEventListener("click", (e) => {
  const a = e.target.closest("a[data-link]");
  const btn = e.target.closest(".cart-btn");
  if (a) {
    e.preventDefault();
    navigate(a.getAttribute("href"));
  } else if (btn) {
    e.preventDefault();
    navigate(btn.getAttribute("href"));
  }
});

window.addEventListener("popstate", route);

route();

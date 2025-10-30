import { Api } from "../api.js";
import { Cart } from "../cart.js";
import { toast } from "../toast.js";
import { fmt, qs, spinner } from "../utils.js";

export function renderProduct(idOrSlug) {
  const app = qs("#app");
  app.innerHTML = "";
  app.appendChild(spinner(420));

  Api.getProduct(idOrSlug)
    .then((p) => {
      app.innerHTML = `
      <a class="btn link" href="#/">← Back to catalog</a>
      <div class="product" style="margin-top:12px">
        <div class="gallery">
          <div class="gallery-main"><img id="mainImg" src="${
            p.images[0]?.url || ""
          }" alt=""></div>
          <div class="gallery-thumbs" id="thumbs"></div>
        </div>
        <div>
          <h1 style="margin:0 0 6px">${p.name}</h1>
          <div class="muted" style="margin-bottom:8px">SKU: ${p.sku} · ID: ${
        p.id
      }</div>
          <div class="row" style="margin-bottom:8px">
            <div class="price">${fmt.price(p.price)}</div>
            <div class="muted">${p.inStock ? "In stock" : "Out of stock"}</div>
          </div>
          <p class="muted" style="line-height:1.6">${p.description || "—"}</p>
          <div class="muted" style="margin:8px 0">Categories: ${
            p.categories.join(", ") || "—"
          }</div>
          <button id="add" class="btn primary" ${
            p.inStock ? "" : "disabled"
          }>Add to cart</button>
        </div>
      </div>`;

      const thumbs = qs("#thumbs", app);
      const mainImg = qs("#mainImg", app);
      p.images.forEach((img, i) => {
        const t = document.createElement("img");
        t.src = img.url;
        t.alt = img.alt || "";
        if (i === 0) t.classList.add("active");
        t.addEventListener("click", () => {
          [...thumbs.children].forEach((c) => c.classList.remove("active"));
          t.classList.add("active");
          mainImg.src = img.url;
        });
        thumbs.appendChild(t);
      });

      qs("#add", app)?.addEventListener("click", () => {
        Cart.upsert({
          productId: p.id,
          qty: 1,
          name: p.name,
          price: p.price,
          thumbnail: p.images[0]?.url,
        });
        toast("Added to cart");
      });
    })
    .catch(() => {
      app.innerHTML = `<div class="empty">Product not found. <a class="btn link" href="#/">Back to catalog</a></div>`;
    });
}

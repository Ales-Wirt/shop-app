import { Api } from "../js/api";
import { Cart } from "../js/cart";
import { toast } from "../js/toast";
import { fmt, qs, spinner } from "../js/utils";
import { initHeader } from "../js/header";

initHeader();

const content = qs("#content");
const params = new URLSearchParams(location.search);
const id = params.get("id") || params.get("slug") || "";

if (!id) {
  content.innerHTML = `<div class="empty">Product not found. <a class="btn link" href="./index.html">Back to catalog</a></div>`;
} else {
  content.appendChild(spinner(420));
  Api.getProduct(id)
    .then((p) => {
      content.innerHTML = `
      <a class="btn link" href="./index.html">← Back to catalog</a>
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

      const thumbs = qs("#thumbs");
      const mainImg = qs("#mainImg");
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

      qs("#add")?.addEventListener("click", () => {
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
      content.innerHTML = `<div class="empty">Product not found. <a class="btn link" href="./index.html">Back to catalog</a></div>`;
    });
}

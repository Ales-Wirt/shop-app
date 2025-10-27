import { Cart } from "./cart";
import { fmt } from "./utils";

export function initHeader() {
  const header = document.querySelector(".header");
  if (!header) return;

  const badge = header.querySelector(".badge");
  const updateCount = () => {
    badge.textContent = Cart.count();
  };
  updateCount();
  addEventListener("cart:change", updateCount);

  const btn = header.querySelector(".cart-btn");
  const popup = header.querySelector(".minicart");
  btn.addEventListener("click", () => popup.classList.toggle("open"));
  document.addEventListener("click", (e) => {
    if (!popup.contains(e.target) && !btn.contains(e.target))
      popup.classList.remove("open");
  });

  const renderMini = () => {
    const list = popup.querySelector(".list");
    list.innerHTML = "";
    const s = Cart.get();
    if (s.items.length === 0) {
      list.innerHTML = `<div class="empty">Cart is empty</div>`;
      popup.querySelector(".goto").disabled = true;
      return;
    }
    popup.querySelector(".goto").disabled = false;
    s.items.forEach((it) => {
      const row = document.createElement("div");
      row.className = "item";
      row.innerHTML = `
        <img src="${it.thumbnail || ""}" alt="">
        <div style="flex:1">
          <div style="font-weight:600">${it.name}</div>
          <div class="muted">${fmt.price(it.price)} · each</div>
          <div class="qty" style="margin-top:6px;display:flex;gap:8px">
            <button class="btn" data-a="dec">−</button>
            <span>${it.qty}</span>
            <button class="btn" data-a="inc">+</button>
            <button class="btn" data-a="del" style="margin-left:auto">Remove</button>
          </div>
        </div>`;
      row.addEventListener("click", (e) => {
        const a = e.target.closest("[data-a]")?.dataset.a;
        if (!a) return;
        if (a === "inc")
          Cart.upsert({
            productId: it.productId,
            qty: 1,
            name: it.name,
            price: it.price,
            thumbnail: it.thumbnail,
          });
        if (a === "dec") Cart.setQty(it.productId, it.qty - 1);
        if (a === "del") Cart.remove(it.productId);
      });
      list.appendChild(row);
    });
  };
  renderMini();
  addEventListener("cart:change", renderMini);
}

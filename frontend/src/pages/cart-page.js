import { Cart } from "../js/cart";
import { Api } from "../js/api";
import { fmt, qs, qsa } from "../js/utils";
import { toast } from "../js/toast";
import { initHeader } from "../js/header";

initHeader();

const cartArea = qs("#cartArea");
const form = qs("#orderForm");
const submitBtn = qs("#submitBtn");

function compute() {
  const s = Cart.get();
  const total = s.items.reduce((a, i) => a + i.price * i.qty, 0);
  return { total };
}

function renderCart() {
  const s = Cart.get();
  if (s.items.length === 0) {
    cartArea.innerHTML = `<div class="empty">Cart is empty. <a class="btn link" href="./index.html">Go to catalog</a></div>`;
    submitBtn.disabled = true;
    return;
  }
  submitBtn.disabled = false;

  const { total } = compute();

  const table = document.createElement("div");
  table.innerHTML = `
    <table class="table">
      <thead><tr><th>Product</th><th>Price</th><th>Qty</th><th>Subtotal</th><th></th></tr></thead>
      <tbody>${s.items
        .map(
          (i) => `
        <tr data-id="${i.productId}">
          <td style="display:flex;gap:10px;align-items:center">
            <img src="${
              i.thumbnail || ""
            }" alt="" style="width:54px;height:54px;object-fit:cover;border-radius:8px">
            ${i.name}
          </td>
          <td>${fmt.price(i.price)}</td>
          <td class="qty">
            <button class="btn" data-a="dec">−</button>
            <input class="input" type="number" min="1" value="${i.qty}" />
            <button class="btn" data-a="inc">+</button>
          </td>
          <td class="line">${fmt.price(i.price * i.qty)}</td>
          <td><button class="btn" data-a="del">Remove</button></td>
        </tr>`
        )
        .join("")}
      </tbody>
    </table>
    <div class="summary">
      <div class="line total"><span>Total</span><span>${fmt.price(
        total
      )}</span></div></div>
    </div>
  `;

  cartArea.innerHTML = "";
  cartArea.appendChild(table);

  qsa("tbody tr", table).forEach((tr) => {
    const id = tr.dataset.id;
    const input = tr.querySelector("input");
    tr.addEventListener("click", (e) => {
      const a = e.target.closest("[data-a]")?.dataset.a;
      if (!a) return;
      const current = parseInt(input.value, 10) || 1;
      if (a === "inc") Cart.setQty(id, current + 1);
      if (a === "dec") Cart.setQty(id, Math.max(1, current - 1));
      if (a === "del") Cart.remove(id);
    });
    input.addEventListener("change", () => {
      const v = Math.max(1, parseInt(input.value, 10) || 1);
      Cart.setQty(id, v);
    });
  });
}

addEventListener("cart:change", renderCart);
renderCart();

const validators = {
  FirstName: (v) => v.trim().length > 0 || "Enter first name",
  LastName: (v) => v.trim().length > 0 || "Enter last name",
  Phone: (v) => /^[0-9]+$/.test(v) || "Digits only",
  Email: (v) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v) || "Invalid email",
};
function validateForm() {
  let ok = true;
  Object.keys(validators).forEach((name) => {
    const input = form.elements[name];
    const msg = validators[name](input.value);
    const errEl = qs(`.error-text[data-for="${name}"]`);
    if (msg !== true) {
      ok = false;
      errEl.textContent = msg;
      input.classList.add("invalid");
    } else {
      errEl.textContent = "";
      input.classList.remove("invalid");
    }
  });
  submitBtn.disabled = !ok || Cart.get().items.length === 0;
  return ok;
}
form.addEventListener("input", validateForm);
validateForm();

form.addEventListener("submit", async (e) => {
  e.preventDefault();
  if (!validateForm()) return;

  const s = Cart.get();
  const payload = {
    FirstName: form.elements.FirstName.value.trim(),
    LastName: form.elements.LastName.value.trim(),
    Phone: form.elements.Phone.value.trim(),
    Email: form.elements.Email.value.trim(),
    Items: s.items.map((i) => ({ ProductId: i.productId, Quantity: i.qty })),
  };

  submitBtn.disabled = true;
  submitBtn.textContent = "Submitting…";
  try {
    const res = await Api.createOrder(payload);
    Cart.clear();
    toast(`Order placed: #${res.orderNumber}`);
    cartArea.innerHTML = `
      <div class="empty">
        <div>Order <b>#${res.orderNumber}</b> has been placed.</div>
        <div style="margin-top:6px">
          Total: <b>${fmt.price(res.total)}</b>
        </div>
        <div style="margin-top:12px"><a class="btn" href="./index.html">Back to catalog</a></div>
      </div>`;
  } catch (err) {
    toast("Order failed");
    alert(`Couldn't place the order:\n${err.message}`);
  } finally {
    submitBtn.disabled = false;
    submitBtn.textContent = "Place order";
  }
});

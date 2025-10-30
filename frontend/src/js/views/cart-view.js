import { Cart } from "../cart.js";
import { Api } from "../api.js";
import { fmt, qs, qsa } from "../utils.js";
import { toast } from "../toast.js";

export function renderCart() {
  const app = qs("#app");
  app.innerHTML = `
    <h1>Cart</h1>
    <div id="cartArea"></div>

    <h2 style="margin-top:24px">Checkout</h2>
    <form id="orderForm" class="form" novalidate>
      <div class="field">
        <label>First name</label>
        <input class="input" name="FirstName" required />
        <div class="error-text" data-for="FirstName"></div>
      </div>
      <div class="field">
        <label>Last name</label>
        <input class="input" name="LastName" required />
        <div class="error-text" data-for="LastName"></div>
      </div>
      <div class="field">
        <label>Phone</label>
        <input class="input" name="Phone" required inputmode="numeric" pattern="^[0-9]+$" />
        <div class="error-text" data-for="Phone"></div>
      </div>
      <div class="field">
        <label>Email</label>
        <input class="input" name="Email" required type="email" />
        <div class="error-text" data-for="Email"></div>
      </div>
      <button id="submitBtn" class="btn primary" type="submit">Place order</button>
    </form>
  `;

  const cartArea = qs("#cartArea", app);
  const form = qs("#orderForm", app);
  const submitBtn = qs("#submitBtn", app);

  function total() {
    const s = Cart.get();
    return s.items.reduce((a, i) => a + i.price * i.qty, 0);
  }

  function renderTable() {
    const s = Cart.get();
    if (s.items.length === 0) {
      cartArea.innerHTML = `<div class="empty">Cart is empty. <a class="btn link" href="#/">Go to catalog</a></div>`;
      submitBtn.disabled = true;
      return;
    }
    submitBtn.disabled = false;

    const sum = total();

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
          sum
        )}</span></div>
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

  addEventListener("cart:change", renderTable);
  renderTable();

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
      const errEl = qs(`.error-text[data-for="${name}"]`, app);
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
          <div style="margin-top:12px"><a class="btn" href="#/">Back to catalog</a></div>
        </div>`;
    } catch (err) {
      toast("Order failed");
      alert(`Couldn't place the order:\n${err.message}`);
    } finally {
      submitBtn.disabled = false;
      submitBtn.textContent = "Place order";
    }
  });
}

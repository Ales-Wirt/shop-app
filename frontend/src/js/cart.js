const KEY = "cart_v1";

function read() {
  try {
    return JSON.parse(localStorage.getItem(KEY)) || { items: [] };
  } catch {
    return { items: [] };
  }
}
function write(state) {
  localStorage.setItem(KEY, JSON.stringify(state));
  dispatchEvent(new CustomEvent("cart:change"));
}

export const Cart = {
  get() {
    return read();
  },
  count() {
    return read().items.reduce((a, x) => a + x.qty, 0);
  },
  upsert(item) {
    const s = read();
    const found = s.items.find((i) => i.productId === item.productId);
    if (found) found.qty += item.qty;
    else s.items.push(item);
    s.items = s.items.filter((i) => i.qty > 0);
    write(s);
  },
  setQty(productId, qty) {
    if (qty <= 0) return Cart.remove(productId);
    const s = read();
    const it = s.items.find((i) => i.productId === productId);
    if (it) {
      it.qty = qty;
      write(s);
    }
  },
  remove(productId) {
    const s = read();
    s.items = s.items.filter((i) => i.productId !== productId);
    write(s);
  },
  clear() {
    write({ items: [] });
  },
};

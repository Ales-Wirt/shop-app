export const fmt = {
  price(v) {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
    }).format(v);
  },
};

export function qs(s, root = document) {
  return root.querySelector(s);
}
export function qsa(s, root = document) {
  return [...root.querySelectorAll(s)];
}

export function spinner(h = 140) {
  const d = document.createElement("div");
  d.className = "loader";
  d.innerHTML = `<div class="skeleton" style="height:${h}px;width:100%"></div>`;
  return d;
}

export function formData(from) {
  return Object.fromEntries(new FormData(from).entries());
}

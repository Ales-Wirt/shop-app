import { renderCart } from "../cart";

describe("renderCart", () => {
  test("renders cart heading and link back to catalog", () => {
    const root = document.createElement("div");
    renderCart(root);

    const h1 = root.querySelector("h1");
    expect(h1).not.toBeNull();
    expect(h1.textContent.toLowerCase()).toContain("cart blueprint");

    const back = root.querySelector('a[data-link][href="/catalog"]');
    expect(back).not.toBeNull();
  });
});

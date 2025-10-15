import { renderProduct } from "../product";

describe("renderProduct", () => {
  test("renders product page with given id", () => {
    const root = document.createElement("div");
    renderProduct(root, 42);

    const back = root.querySelector('a[data-link][href="/catalog"]');
    expect(back).not.toBeNull();

    const h1 = root.querySelector("h1");
    expect(h1).not.toBeNull();
    expect(h1.textContent).toContain("Item #42");
  });

  test("renders disabled add-to-cart button (stub)", () => {
    const root = document.createElement("div");
    renderProduct(root, 1);
    const btn = root.querySelector("button");
    expect(btn).not.toBeNull();
    expect(btn.disabled).toBe(true);
  });
});

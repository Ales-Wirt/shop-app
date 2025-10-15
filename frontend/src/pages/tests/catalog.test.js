import { renderCatalog } from "../catalog";

describe("renderCatalog", () => {
  test("renders heading and sample product links", () => {
    const root = document.createElement("div");
    const url = new URL("http://localhost:5173/catalog");
    renderCatalog(root, url);

    const h1 = root.querySelector("h1");
    expect(h1).not.toBeNull();
    expect(h1.textContent.toLowerCase()).toContain("catalog");

    const links = [...root.querySelectorAll("a[data-link]")];
    expect(links.length).toBeGreaterThan(0);
    expect(links[0].getAttribute("href")).toMatch(/^\/product\//);
  });
});

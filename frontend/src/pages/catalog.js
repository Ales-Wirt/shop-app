export function renderCatalog(root, url) {
  root.innerHTML = `
    <section>
        <h1>Catalog blueprint</h1>
        <p>Soon here will be a list of items and filters.</p>
        <a data-link href="/product/1">Item #1</a>
        <a data-link href="product/2">Item #2</a>
    </section>
    `;
}

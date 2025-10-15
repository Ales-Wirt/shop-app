export function renderProduct(root, id) {
  root.innerHTML = `
        <section>
            <a data-link href="/catalog">Back</a>
            <h1>Item #${id}</h1>
            <p>Here soon will be the item's info</p>
            <button disabled>To cart</button>
        </section
    `;
}

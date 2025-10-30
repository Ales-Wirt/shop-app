import { Api } from "../api.js";
import { Cart } from "../cart.js";
import { toast } from "../toast.js";
import { fmt, qs, spinner } from "../utils.js";

export function renderCatalog() {
  const app = qs("#app");
  app.innerHTML = `
    <div class="filters">
      <div class="block">
        <div class="muted" style="margin-bottom:8px">Categories</div>
        <div id="catList" class="checkbox-list"></div>
      </div>
      <div class="block" style="min-width:260px">
        <div class="muted" style="margin-bottom:8px">Price</div>
        <div class="input-inline">
          <input id="minPrice" class="input" type="number" min="0" placeholder="Min">
          <input id="maxPrice" class="input" type="number" min="0" placeholder="Max">
        </div>
      </div>
      <div class="block" style="min-width:260px">
        <div class="muted" style="margin-bottom:8px">Sort by</div>
        <select id="sort" class="select">
          <option value="NameAsc">Name A→Z</option>
          <option value="NameDesc">Name Z→A</option>
          <option value="PriceAsc">Price ↑</option>
          <option value="PriceDesc">Price ↓</option>
        </select>
      </div>
    </div>
    <div id="listArea"></div>
  `;

  const listArea = qs("#listArea", app);
  const catList = qs("#catList", app);
  const sortSel = qs("#sort", app);
  const minPrice = qs("#minPrice", app);
  const maxPrice = qs("#maxPrice", app);

  function selectedCats() {
    return [...catList.querySelectorAll("input[type=checkbox]:checked")].map(
      (x) => x.value
    );
  }

  function renderProducts(page) {
    listArea.innerHTML = "";
    const load = spinner(220);
    listArea.appendChild(load);

    const params = { Page: page || 1, PageSize: 24, Sort: sortSel.value };
    const cats = selectedCats();
    if (cats.length) params.CategorySlugs = cats;
    if (minPrice.value) params.MinPrice = minPrice.value;
    if (maxPrice.value) params.MaxPrice = maxPrice.value;

    Api.listProducts(params)
      .then((data) => {
        listArea.innerHTML = "";

        if (data.total === 0) {
          listArea.innerHTML = `
          <div class="empty">
            <div class="muted" style="margin-bottom:8px">Nothing found</div>
            <button id="reset" class="btn">Reset filters</button>
          </div>`;
          qs("#reset", listArea).addEventListener("click", () => {
            [...catList.querySelectorAll("input")].forEach(
              (i) => (i.checked = false)
            );
            minPrice.value = "";
            maxPrice.value = "";
            sortSel.value = "NameAsc";
            renderProducts(1);
          });
          return;
        }

        const grid = document.createElement("div");
        grid.className = "grid";
        data.items.forEach((p) => {
          const card = document.createElement("div");
          card.className = "card";
          card.innerHTML = `
          <a href="#/product/${encodeURIComponent(p.id)}">
            <img src="${p.thumbnailUrl || ""}" alt="${p.name}">
          </a>
          <div class="card-body">
            <a class="title" href="#/product/${encodeURIComponent(p.id)}">${
            p.name
          }</a>
            <div class="row">
              <div class="price">${fmt.price(p.price)}</div>
              <div class="muted">${
                p.inStock ? "In stock" : "Out of stock"
              }</div>
            </div>
            <button class="btn primary add" ${
              p.inStock ? "" : "disabled"
            }>Add to cart</button>
          </div>`;
          card.querySelector(".add")?.addEventListener("click", () => {
            Cart.upsert({
              productId: p.id,
              qty: 1,
              name: p.name,
              price: p.price,
              thumbnail: p.thumbnailUrl,
            });
            toast("Added to cart");
          });
          grid.appendChild(card);
        });
        listArea.appendChild(grid);
      })
      .catch((err) => {
        listArea.innerHTML = `
        <div class="error">
          Failed to load: ${err.message}
          <div><button id="retry" class="btn" style="margin-top:8px">Retry</button></div>
        </div>`;
        qs("#retry", listArea).addEventListener("click", () =>
          renderProducts(page || 1)
        );
      });
  }

  Api.listCategories().then((list) => {
    catList.innerHTML = "";
    list.forEach((c) => {
      const id = `cat-${c.slug}`;
      const w = document.createElement("label");
      w.innerHTML = `<input type="checkbox" id="${id}" value="${c.slug}"><span>${c.name}</span>`;
      catList.appendChild(w);
    });
    catList.addEventListener("change", () => renderProducts(1));
  });

  sortSel.addEventListener("change", () => renderProducts(1));
  [minPrice, maxPrice].forEach((i) =>
    i.addEventListener("input", () => renderProducts(1))
  );

  renderProducts(1);
}

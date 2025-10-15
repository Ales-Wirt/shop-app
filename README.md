# 🌸 E-Shop — Flower & Decorative Plant Store

**E-Shop** is a full-stack web application for selling flowers and decorative plants.  
The app includes a **product catalog**, a **product details page**, and a **shopping cart** with order placement.  
It is built with **Vanilla JavaScript**, **HTML**, and **CSS** on the frontend, and **.NET 8 + Entity Framework Core + SQL Server** on the backend.  
Deployment and hosting are planned on **Microsoft Azure** using **Azure App Service** and **Azure SQL**.

---

## 🪴 Features

### 🌼 Product Catalog

- Displays a list of available plants with image, name, price, and stock status.
- Supports **sorting** by price (asc/desc) and name (A–Z / Z–A).
- Includes **filtering** by category/type and price range.
- **Add to Cart** button active only for items in stock.
- **Dynamic UI updates** — filtering, sorting, and cart updates without page reload.
- Loading states and empty states (`No results found`, `Error`, `Retry`).

### 🌷 Product Page

- Shows product name, image gallery, price, description, and availability.
- “Add to Cart” button disabled if the product is out of stock.
- “Back to Catalog” link for easy navigation.
- Displays proper loading and error states.

### 🛒 Cart & Checkout

- Editable list of items (quantity ±, subtotal, delete item).
- Calculates subtotal, delivery cost, and total.
- Empty cart view (“Your cart is empty”).
- Checkout form with **validation**:
  - Required: First Name, Last Name, Phone, Email.
  - Valid formats for phone and email.
- On success: shows “Order placed” message and clears cart.
- On error: keeps cart data and shows error notification.

### 🌺 Shared Header

- Appears on all pages.
- Logo (clickable → returns to catalog).
- Cart icon with live item counter and popup summary.

---

## 🧩 Tech Stack

| Layer                  | Technology                               |
| ---------------------- | ---------------------------------------- |
| **Frontend**           | HTML5, CSS3, Vanilla JavaScript          |
| **Bundler/Dev Server** | [Parcel 2](https://parceljs.org/)        |
| **Testing (frontend)** | Jest + jsdom                             |
| **Linting**            | ESLint (Standard config)                 |
| **Backend**            | ASP.NET Core 8 (Minimal API)             |
| **ORM**                | Entity Framework Core 8                  |
| **Database**           | Microsoft SQL Server (local & Azure SQL) |
| **Testing (backend)**  | xUnit + FluentAssertions                 |
| **Cloud**              | Microsoft Azure (App Service + SQL)      |

---

## 🏗️ Project Structure

```
shop-monorepo/
├─ backend/
│  ├─ src/Shop.Web/
│  │   ├─ Program.cs
│  │   ├─ appsettings.Development.json
│  │   └─ ...
│  └─ tests/Shop.Tests/
│      └─ SmokeTests.cs
│
├─ frontend/
│  ├─ index.html
│  ├─ package.json
│  ├─ src/
│  │   ├─ main.js
│  │   ├─ pages/
│  │   │   ├─ catalog.js
│  │   │   ├─ product.js
│  │   │   └─ cart.js
│  │   └─ utils/
│  │       └─ formatPrice.js
│  ├─ .eslintrc.json
│  └─ .stylelintrc.json
│
├─ .editorconfig
├─ .gitignore
└─ README.md
```

---

## ⚙️ Local Development

### 1️⃣ Backend (.NET 8 + SQL Server)

```powershell
cd backend/src/Shop.Web
dotnet run
```

💾 Connection string (in `appsettings.Development.json`):

```json
"ConnectionStrings": {
  "SqlServer": "Server=localhost;Database=ShopDev;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
}
```

> For SQL Express use:  
> `"Server=localhost\\SQLEXPRESS;Database=ShopDev;Trusted_Connection=True;TrustServerCertificate=True;"`

---

### 2️⃣ Frontend (Parcel Dev Server)

```powershell
cd frontend
npm install
npm run dev
```

Runs development server at:  
👉 [http://localhost:5173](http://localhost:5173)

---

## 🧪 Testing

### Run all frontend tests

```powershell
npm test
```

### Run all backend tests

```powershell
cd backend
dotnet test
```

---

## 🔍 Linting

```powershell
# JavaScript
npm run lint
```

- **ESLint** ensures consistent JS code style using the `standard` configuration.
- **.editorconfig** maintains consistent indentation, newlines, and encoding across editors.

---

## ☁️ Deployment (planned)

- **Backend:** Azure App Service (ASP.NET Core runtime)
- **Database:** Azure SQL
- **Frontend:** Azure Static Web Apps or Azure Blob hosting
- **Environment variables:** managed via Azure App Configuration / Key Vault

---

## 📄 License

MIT License © 2025

---

## 💐 Author

**E-Shop Project — Full-Stack Demo**  
Developed with ❤️ using `.NET 8`, `Entity Framework Core`, `SQL Server`, `Vanilla JS`, `Parcel`, and `Azure`.

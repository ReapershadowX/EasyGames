# EasyGamesProject


## Credentials for existing Users
Admin : admin@email.com
Password : password

Moderator : mod@email.com
Password : password

User : user@email.com
Password : password

EasyGamesProject is an **ASP.NET Core MVC web application** built with **.NET 8**, **Entity Framework Core**, and **SQL Server LocalDB**.  
It provides functionality for stock management, user management, and a shopping cart system for purchasing items.

---

## 📦 Setup Summary
- Development environment with **Visual Studio 2022**, **.NET 8**, **SQL Server LocalDB**, and **Git** configured.
- Project scaffolded as an **ASP.NET Core MVC app** with Controllers, Models, Views, and wwwroot.

---

## 🗂️ Project Foundation
- Organized folders: `Controllers`, `Models`, `Views`, `wwwroot`, `Data`.
- Installed NuGet packages: `Microsoft.EntityFrameworkCore.SqlServer` and `Microsoft.EntityFrameworkCore.Tools`.
- Implemented `ApplicationDbContext.cs` with dependency injection and `appsettings.json` connection string.
- Code-first migrations completed and database schema verified.

---

## 📊 Models
- **Stock**: Represents items with validation (ID, Name, Category, Price, Quantity, Description, CreatedDate, Image)  
- **User**: Includes authentication data (ID, Name, Email, Password with hashing, Role, CreatedDate).  
- **ShoppingCart**: Tracks user carts (CartId, UserId, StockId, Quantity, DateAdded).  

---

## 🎮 Controllers
- **HomeController**: Landing page and navigation.  
- **StockController**: Full CRUD for stock management (Create, Read, Update, Delete).  
- **UserController**: Manage users, secure password hashing, and registration flow.  
- **ShoppingCartController**: Add to cart, view, remove, update, checkout with session/user association.

---

## 🖼️ Views
- **Layout**: Navigation with placeholders for Bootstrap responsiveness and role-based menus.  
- **Stock Views**: Index, Details, Create, Edit, Delete with validation.  
- **User Views**: All CRUD views plus Register.cshtml.  
- **Cart Views**: ViewCart, Checkout, with Add-to-Cart integration in stock pages.  
- **Home Page**: Awaiting enhancement for featured products and responsive design.  

---

## 🛒 Shopping Cart
- Cart functionality includes Add, View, Remove, Update, Checkout.  
- Displays total cost, integrates with user/session.  
- Pending: Cart count in navigation, final workflow testing.  

---

## ✅ Progress Overview
- Environment and database completed.  
- Models and migrations implemented.  
- CRUD operations for stock and users functional.  
- Public registration and cart system implemented.  
- Pending: UI/UX improvements, responsive design, error handling, and final testing.  

---

## 📚 References
- [ASP.NET Core MVC Documentation (Microsoft Learn)](https://learn.microsoft.com/aspnet/core/mvc)  
- [Entity Framework Core Documentation](https://learn.microsoft.com/ef/core)  
- [Bootstrap Documentation](https://getbootstrap.com)  
- [Visual Studio Community Edition](https://visualstudio.microsoft.com/vs/community)  
- [Git Documentation](https://git-scm.com/doc)  
- [Assistance and content generation supported by **ChatGPT**, OpenAI Language Model (2023-2025)](https://chatgpt.com/)


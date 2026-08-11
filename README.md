# 🥐 Gruer's Artisanal Bakery

> *"Where every bite tells a story — crafted with warmth, kissed by magic, and baked to perfection."*

## 🌐 Live Demo

**Live application:**
https://gruers-shop.onrender.com/

**GitHub repository:**
https://github.com/SilenceMustBeHeard/Gruer-s-Shop

---

## 📖 Overview

**Gruer's Artisanal Bakery** is a full-stack e-commerce platform built with **ASP.NET Core MVC** and **PostgreSQL**.

The project simulates a complete online bakery experience where customers can browse products, manage favorites, place orders, leave reviews, and communicate with the bakery.

The platform also includes dedicated **Admin** and **Manager** functionality for managing products, categories, users, orders, reviews, and system messages.

The application is containerized with **Docker** and deployed as a live application.

---

## ✨ Key Features

| Feature                               | Description                                                                   |
| ------------------------------------- | ----------------------------------------------------------------------------- |
| 🔐 **Authentication & Authorization** | Registration, login, logout and role-based access using ASP.NET Core Identity |
| 👤 **User Profiles**                  | Manage personal information and view messages                                 |
| 🍞 **Product Catalog**                | Browse products and filter them by category                                   |
| 🛒 **Orders**                         | Place and manage product orders                                               |
| ⭐ **Reviews & Ratings**               | Customers can review products and authorized users can moderate reviews       |
| ❤️ **Favorites**                      | Save products for later                                                       |
| 📬 **Contact Messages**               | Customers can communicate with the bakery                                     |
| 📢 **System Messages**                | Administrators can send announcements to users                                |
| 👑 **Admin Panel**                    | Manage users, products, categories, orders, reviews and messages              |
| 📋 **Manager Area**                   | Dedicated management functionality for operational tasks                      |
| ✨ **Animated UI**                     | Floating particles, hover effects and smooth transitions                      |
| 📧 **Password Recovery**              | Email-based password reset using SendGrid                                     |
| 🐳 **Docker Support**                 | Containerized application ready for deployment                                |

---

## 🛠️ Technology Stack

| Technology                   | Purpose                                       |
| ---------------------------- | --------------------------------------------- |
| **C# / .NET 10**             | Application development                       |
| **ASP.NET Core MVC**         | Web framework                                 |
| **Entity Framework Core 10** | ORM and data access                           |
| **PostgreSQL**               | Relational database                           |
| **Npgsql**                   | PostgreSQL provider for Entity Framework Core |
| **ASP.NET Core Identity**    | Authentication and authorization              |
| **SendGrid**                 | Password-reset email delivery                 |
| **Bootstrap 5.3**            | Responsive UI                                 |
| **Bootstrap Icons**          | UI icons                                      |
| **Razor Views**              | Server-side rendering                         |
| **Docker**                   | Containerization                              |
| **Git / GitHub**             | Version control and source management         |

---

## 🏗️ Architecture

The project follows a layered architecture separating responsibilities across multiple projects:

* **Web** — MVC controllers, Razor views and application entry point
* **Web.Infrastructure** — web-specific infrastructure and extensions
* **ViewModels** — presentation models
* **Services.Core** — business logic
* **Services.Common** — shared service contracts and abstractions
* **Services.Automapping** — object mapping configuration
* **Data** — Entity Framework Core context, repositories and database logic
* **Data.Models** — entity models
* **Data.Common** — shared data-layer abstractions
* **API** — API-related functionality
* **Tests** — unit, web and integration tests

This structure keeps the application modular and makes the business logic easier to maintain and test.

---

## 🚀 Getting Started

### Prerequisites

* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* [PostgreSQL](https://www.postgresql.org/download/)
* [Git](https://git-scm.com/)
* Visual Studio 2022 or VS Code
* Docker *(optional)*

### Clone the repository

```bash
git clone https://github.com/SilenceMustBeHeard/Gruer-s-Shop.git
cd Gruer-s-Shop
```

### Configure PostgreSQL

Create a PostgreSQL database and configure the connection string through your local configuration or environment variables.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=GruersShop;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

> **Do not commit real credentials to the repository.**

For local development, use `.env`, User Secrets, or environment variables.

### Apply migrations

```bash
dotnet ef database update \
  --project GruersShop.Data \
  --startup-project GruersShop.Web
```

The database seeder will populate the database with sample data and configured application roles.

### Configure SendGrid

Configure the required SendGrid settings using User Secrets or environment variables:

```bash
dotnet user-secrets set "SendGrid:ApiKey" "YOUR_SENDGRID_API_KEY"
dotnet user-secrets set "SendGrid:FromEmail" "YOUR_VERIFIED_EMAIL"
```

### Run the application

```bash
cd GruersShop.Web
dotnet run
```

The application will be available at the local URL shown by ASP.NET Core.

---

## 👥 User Roles & Permissions

| Permission           | 👑 Admin | 📋 Manager | 👤 Customer |
| -------------------- | :------: | :--------: | :---------: |
| Browse products      |     ✅    |      ✅     |      ✅      |
| Leave reviews        |     ✅    |      ✅     |      ✅      |
| Add favorites        |     ✅    |      ✅     |      ✅      |
| Contact bakery       |     ✅    |      ✅     |      ✅      |
| Place orders         |     ✅    |      ✅     |      ✅      |
| Manage users         |     ✅    |      ❌     |      ❌      |
| Manage categories    |     ✅    |      ❌     |      ❌      |
| Manage products      |     ✅    |      ❌     |      ❌      |
| Manage reviews       |     ✅    |      ✅     |     Own     |
| Manage orders        |     ✅    |      ✅     |     Own     |
| Send system messages |     ✅    |      ❌     |      ❌      |

---

## 🎨 Custom UI & Theme

The application uses a custom bakery-inspired visual identity featuring:

* 🪵 Warm wood and stone textures
* 🍯 Golden honey accents
* 🔥 Fire-glow effects
* ✨ Floating particle animations
* 🌓 Custom dark and light themes
* 🖱️ Smooth hover and transition effects
* 📱 Responsive layouts

The goal was to create a UI that feels more like a real branded e-commerce product rather than a generic CRUD application.

---

## 📸 Screenshots

### 🔐 Login

![Login](https://github.com/user-attachments/assets/020d6a83-abc2-4c9e-b90a-d04018f5ff56)

### 📝 Contact Messages

![Contact](https://github.com/user-attachments/assets/db014191-a0e3-4160-8c27-9f9a94c1c781)

### 🛠️ Admin Panel

![Admin Panel](https://github.com/user-attachments/assets/b5fc691c-6708-4b84-8a13-d5dfc0b1b145)

### 📝 Product Catalog

![Catalog](https://github.com/user-attachments/assets/5abc01a6-23f0-4272-94a3-522074dee2ab)

![Catalog](https://github.com/user-attachments/assets/4ae30aef-0997-4350-94be-6c3b2769ee42)

### 🛒 Product Details, Orders & Reviews

![Product Details](https://github.com/user-attachments/assets/cf095df7-f1e1-4c56-b3e9-764bf18950b5)

---

## 🐳 Docker

The project includes Docker configuration for containerized development and deployment.

Build the image:

```bash
docker build -t gruers-shop .
```

Run the container:

```bash
docker run -p 8080:8080 gruers-shop
```

The application can then be accessed through:

```text
http://localhost:8080
```

---

## 🧪 Testing

The solution contains dedicated projects for:

* Unit tests
* Web tests
* Integration tests

This allows business logic and application functionality to be tested independently from the presentation layer.

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch

```bash
git checkout -b feature/amazing-feature
```

3. Commit your changes

```bash
git commit -m "Add amazing feature"
```

4. Push the branch

```bash
git push origin feature/amazing-feature
```

5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License. See the `LICENSE` file for details.



## 👨‍💻 Author

**Konstantin Konstantinov**

C# / .NET Developer

* GitHub: https://github.com/SilenceMustBeHeard
* Live Demo: https://gruers-shop.onrender.com/

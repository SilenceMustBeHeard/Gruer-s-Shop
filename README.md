# 🥐 Gruer's Artisanal Bakery

> *"Where every bite tells a story — crafted with warmth, kissed by magic, and baked to perfection."*

## 📖 Overview

**Gruer's Artisanal Bakery** is a full-featured e-commerce platform for a magical bakery. Built with ASP.NET Core MVC, it offers a warm, rustic experience where customers can browse products, leave reviews, manage favorites, and contact the bakery. Administrators have full control over users, products, orders, and messages.

---

## ✨ Key Features

| Feature | Description |
|---------|-------------|
| 🔐 **User Authentication** | Register, login, logout with ASP.NET Core Identity |
| 👤 **User Profiles** | View and manage personal information and messages |
| 🍞 **Product Catalog** | Browse bakery products with filtering by category |
| ⭐ **Reviews & Ratings** | Leave and manage product reviews |
| ❤️ **Favorites** | Save favorite products for later |
| 📬 **Contact Messages** | Send messages to the bakery (user → admin) |
| 📢 **System Messages** | Admin → user announcements |
| 👑 **Admin Dashboard** | Full control over users, products, categories, orders, and messages |
| 🌓 **Dark/Light Theme** | Toggle between warm light and cozy dark mode |
| ✨ **Magic Animations** | Floating sparks, hover effects, and smooth transitions |

---

## 🛠️ Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| ASP.NET Core | 10.0 | Web framework |
| Entity Framework Core | 10.0 | ORM & data access |
| SQL Server | 2022 | Database |
| ASP.NET Core Identity | 10.0 | Authentication & authorization |
| SendGrid | Latest | Email service (password reset) |
| Bootstrap | 5.3 | Responsive UI |
| Bootstrap Icons | 1.11 | Icon library |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)
- [Git](https://git-scm.com/)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/SilenceMustBeHeard/Gruer-s-Shop.git
   cd Gruer-s-Shop

2. Configure the database connection

Update appsettings.json in GruersShop.Web:

json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=GruersShop;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}


3. Apply migrations and seed the database

bash
dotnet ef database update --project GruersShop.Data --startup-project GruersShop.Web
The seeder will automatically create:

Admin user: admin@gruersshop.com / Admin123!

Manager user: manager@gruersshop.com / Manager123!

Sample categories and products

4. Configure SendGrid (for password reset)

bash
dotnet user-secrets set "SendGrid:ApiKey" "YOUR_SENDGRID_API_KEY"
dotnet user-secrets set "SendGrid:FromEmail" "your-verified-email@example.com"
Run the application

bash
cd GruersShop.Web
dotnet run --launch-profile https
Open your browser and navigate to https://localhost:7021

👥 User Roles & Permissions
Permission	👑 Admin	📋 Manager	👤 Customer
Browse products	✅	✅	✅
Leave reviews	✅	✅	✅
Add to favorites	✅	✅	✅
Contact bakery	✅	✅	✅
Manage all users	✅	❌	❌
Manage categories	✅	❌	❌
Manage products	✅	❌	❌
Manage reviews	✅	✅	❌ (own only)
Manage orders	✅	✅	❌
Send system messages	✅	❌	❌


🎨 Theme
The project features a custom warm, rustic bakery theme with:

Wood and stone textures

Golden honey accents

Fire glow effects

Floating spark animations

Smooth card hover effects

Dark/light mode toggle

🤝 Contributing
Fork the repository

Create a feature branch (git checkout -b feature/amazing-feature)

Commit your changes (git commit -m 'Add amazing feature')

Push to the branch (git push origin feature/amazing-feature)

Open a Pull Request

📄 License
This project is licensed under the MIT License - see the LICENSE file for details.

🙏 Acknowledgments
Bootstrap - UI framework

Bootstrap Icons - Icon library

SendGrid - Email service

Google Fonts - Playfair Display & Inter fonts


   

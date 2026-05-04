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

SCREENSHOTS : 

🔐 Login Page
<img width="1890" height="910" alt="login" src="https://github.com/user-attachments/assets/020d6a83-abc2-4c9e-b90a-d04018f5ff56" />




📝 Contact MEssage 
<img width="1868" height="902" alt="contact" src="https://github.com/user-attachments/assets/db014191-a0e3-4160-8c27-9f9a94c1c781" />





🛠 Admin Panel
<img width="1890" height="900" alt="admin panel" src="https://github.com/user-attachments/assets/b5fc691c-6708-4b84-8a13-d5dfc0b1b145" />



📝 Catalog page 
<img width="1896" height="914" alt="catalog" src="https://github.com/user-attachments/assets/5abc01a6-23f0-4272-94a3-522074dee2ab" />
<img width="1890" height="902" alt="catalog 2" src="https://github.com/user-attachments/assets/4ae30aef-0997-4350-94be-6c3b2769ee42" />


📝 Product Details + Order + review
<img width="1874" height="914" alt="details + review" src="https://github.com/user-attachments/assets/cf095df7-f1e1-4c56-b3e9-764bf18950b5" />





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


   

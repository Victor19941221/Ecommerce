# Ecommerce Platform - ASP.NET Core & MongoDB

## Overview

This is a modern, responsive e-commerce web application built with **ASP.NET Core**, **Razor Pages**, **MongoDB**, and **Stripe** for payments. It provides a full online shopping experience with features such as:

- Product listings and details
- Shopping cart
- User authentication and role-based authorization
- Stripe-based checkout process
- Admin panel for managing products and viewing orders

This documentation is meant for **future team members or interview candidates** to quickly understand the structure and purpose of the project.

---

## Tech Stack

- **Backend**: ASP.NET Core (Razor Pages), MongoDB
- **Authentication**: ASP.NET Core Identity with MongoDB backend
- **Payments**: Stripe Checkout
- **Frontend**: Razor Pages + Bootstrap 5 + jQuery
- **Session Management**: ASP.NET Core Session

---

## Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download)
- [MongoDB](https://www.mongodb.com/try/download/community) (local or cloud)
- [Stripe account](https://dashboard.stripe.com/register) with API keys

### Setup Instructions

1. **Clone the Repository**

```bash
git clone https://github.com/your-org/ecommerce.git
cd ecommerce
```

2. **Configure `appsettings.json`**

Update the file with your MongoDB and Stripe settings:

```json
"MongoSettings": {
  "ConnectionString": "mongodb://localhost:27017",
  "DatabaseName": "EcommerceDB"
},
"Stripe": {
  "PublishableKey": "your-publishable-key",
  "SecretKey": "your-secret-key"
},
"Admin": {
  "Email": "vlarsson347@gmail.com"
}
```

> The `Admin:Email` is used to seed an admin role to this user if the email matches an existing user.

3. **Restore Dependencies and Run the App**

```bash
dotnet restore
dotnet run
```

The app will be available at `https://localhost:5001`

---

## Project Structure

- **Controllers/**: MVC Controllers for Cart, Orders, Admin, etc.
- **Models/**: Data classes like `Product`, `Order`, `CartItem`, `Review`, `ApplicationUser`, etc.
- **Services/**: Business logic (e.g., `ProductService`, `OrderService`, `ReviewService`)
- **Pages/**: Razor Pages for UI (product browsing, checkout, login, etc.)
- **Areas/Identity/**: Contains Identity Razor Pages (login, register, etc.)
- **wwwroot/**: Static files (CSS, JS, images)

---

## Authentication & Authorization

- **User Registration & Login**: ASP.NET Core Identity
- **Role-Based Access**: "admin" and "user" roles
- **Admin Seeding**: Uses `Admin:Email` from config to assign the admin role at app startup
- **Policies**: Admin-only pages use `[Authorize(Roles = "admin")]`

---

## Key Features

### 🛒 Cart & Checkout

- Cart is session-based
- AJAX Add-to-Cart using jQuery
- Cart popup using Bootstrap toast or off-canvas
- Stripe Checkout integration

### 🧾 Orders

- Orders stored in MongoDB
- Admin can view and mark orders as shipped
- Order includes user info, cart snapshot, and payment session ID

### 🧑‍💼 Admin Tools

- Product management (Create, Edit, Delete)
- Admin dashboard
- Admin-only routes

---

## Styling & Frontend

- **Bootstrap 5** for layout and responsiveness
- **jQuery** for cart interactivity
- **Custom CSS** for brand-specific polish
- **Icons** via Bootstrap Icons CDN

---

## Extending the App

To add a new feature:

1. Add a new model
2. Add a service method
3. Create a Razor Page or Controller endpoint
4. Secure it using `[Authorize]` if needed
5. Style it using Bootstrap classes
6. Write tests in a test project

---

## Demo Admin Login

Seed admin by adding email adresses for admin in appsettings.json

**Admin Email**: `example@gmail.com`

After registration, the startup logic will assign the admin role if the email matches.

---

## Contribution Guidelines

- Follow C# naming conventions
- Keep business logic in Services
- Use Razor Pages for UI
- Use `[Authorize]` to protect routes
- Keep commits small and meaningful

---



# Whisperwood Online Bookstore

## Overview

Whisperwood is an online bookstore platform built with ASP.NET Core Web API (.NET 8.0) and PostgreSQL, designed to enhance the book-buying experience for users and streamline store management for administrators. The platform offers a user-friendly interface for browsing, purchasing, and reviewing books, with robust backend services for managing inventory, promotions, and orders. Key features include JWT-based authentication, real-time order notifications via SignalR, and a responsive UI styled with Tailwind CSS.

## Features

- **Browse Books**: Filter and sort books by author, genre, price, or popularity.
- **Wishlist & Cart**: Registered users can save books to a wishlist or add them to a cart for purchase.
- **Order Management**: Place, cancel, and track orders with automated billing and email notifications.
- **Reviews**: Submit reviews for purchased books.
- **Promotions**: Apply discounts on individual books or based on order history (e.g., 5% for 5+ books, 10% loyalty discount after 10 orders).
- **Announcements**: Admins can create timed announcements for users.
- **Admin Tools**: Manage books, authors, genres, categories, and orders.
- **Real-Time Notifications**: Staff receive instant order updates via SignalR.

## Technology Stack

- **Backend**: ASP.NET Core Web API (.NET 8.0)
- **Database**: PostgreSQL
- **Frontend**: Razor Pages, Tailwind CSS, SweetAlert2
- **Libraries**:
  - Microsoft.AspNetCore.Authentication.JwtBearer (8.0.1)
  - Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.0)
  - Microsoft.AspNetCore.SignalR (1.2.0)
  - Microsoft.EntityFrameworkCore (9.0.4)
  - Microsoft.EntityFrameworkCore.Tools (9.0.4)
  - Npgsql.EntityFrameworkCore.PostgreSQL (9.0.4)
  - PdfSharpCore (1.3.67)
  - Swashbuckle.AspNetCore
- **Tools**: Git, Trello (project management)

## Prerequisites

- .NET 8.0 SDK
- PostgreSQL 14+
- Node.js (for Tailwind CSS)
- IDE (e.g., Visual Studio 2022, VS Code)
- Git

## Setup Instructions

1. **Clone the Repository**:

   ```bash
   git clone https://github.com/Rakshita-Baidya/Whisperwood.git
   ```

2. **Configure PostgreSQL**:

   - Create a database named `whisperwood`.

   - Update the connection string in `appsettings.json`:

     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=whisperwood;Username=your_username;Password=your_password"
     }
     ```

3. **Apply Database Migrations**:

   - Open NuGet Package Manager Console.

   ```bash
   add-migration InitialCreate
   update-database
   ```

4. **Configure Email Service**:

   - Update email settings in `appsettings.json` for bill notifications:

     ```json
     "EmailSettings": {
       "SmtpServer": "smtp.your-email-provider.com",
       "Port": 587,
       "Username": "your-email",
       "Password": "your-password"
     }
     ```

5. **Run**:

   - Run the program using desired IDE. (Visual Studio)
   - Access the application at `https://localhost:7018`.
   - API documentation is available at `https://localhost:7018/swagger/index.html`.

## API Endpoints

Full documentation is available via Swagger UI. Key endpoints include:

- **Books**: `/api/Book/getall`, `/api/Book/filter`, `/api/Book/add`
- **Cart**: `/api/CartItem/add`, `/api/CartItem/getbyuserid`
- **Wishlist**: `/api/WishlistItem/add`, `/api/WishlistItem/getbyuserid`
- **Orders**: `/api/Order/add`, `/api/Order/getbyuser/{userId}`
- **Reviews**: `/api/Review/add`, `/api/Review/getbybook/{bookId}`
- **Promotions**: `/api/Promotion/add`, `/api/Promotion/validate/{promoCode}`
- **Announcements**: `/api/Announcement/add`, `/api/Announcement/getall`

## Usage

1. **User Actions**:

   - Register/login at `/User/Register` or `/User/Login`.
   - Browse books at `/Book/Browse`.
   - Add books to wish list or cart, then proceed to checkout.
   - Submit reviews for purchased books at `/Review/Add`.

2. **Admin Actions**:

   - Log in with admin credentials.
   - Manage books at `/Book/ManageBook`.
   - Manage announcements at `/Announcement/ManageAnnouncement`.
   - Process orders at `/Order/ManageOrder`.
   - Manage promotion at `/Promotion/ManagePromotion`.

3. **Real-Time Notifications**:

   - Staff/admins receive order updates via SignalR on the admin dashboard.

## Future Improvements

- Add a mobile application for enhanced accessibility.
- Implement AI-driven book recommendations.
- Conduct load testing for scalability.
- Integrate secure payment gateways.
- Introduce social features like user book clubs.

## Team

- **Rakshita Baidya**: Lead developer, backend/frontend, documentation.
- **Bidur Neupane**: Backend (genres, categories, promotions), frontend (home, login, registration).
- **Prashansa Lama**: Backend (reviews), frontend (review pages), UI/UX design.

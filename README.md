# 🏨 Hotel Reservation System – Enterprise RESTful API

A scalable and production-ready Hotel Reservation REST API built using **ASP.NET Core**.

This system manages hotel rooms, reservations, payments, offers, feedback, facilities, and user roles with secure authentication and clean architecture principles.

Designed as a real-world backend system suitable for enterprise-level hotel management platforms.

---

## 📌 Project Overview

The system supports:

- Customer registration & authentication
- Role-based authorization (Admin / Staff / Customer)
- Room management & filtering
- Reservation lifecycle management
- Secure Stripe payment integration
- Offer & discount management
- Feedback system
- Facility management
- Payment history tracking

The project focuses on clean system design, maintainability, and scalability.

---

## 🏗 Architecture & Design

This API follows clean layered architecture principles:

- Presentation Layer (Controllers)
- Application Layer (Services / Business Logic)
- Infrastructure Layer (EF Core / Database / External Services)
- Domain Layer (Entities & Core Models)

### Design Decisions

- DTO-based data transfer to prevent over-posting
- Role-based authorization using JWT
- Stripe Webhook integration for secure payment confirmation
- Separation of concerns for maintainability
- Proper entity relationships using EF Core
- Validation and database constraints
- RESTful endpoint design

---

## 🛠 Technology Stack

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Authentication
- Stripe Payment Gateway
- Swagger (OpenAPI Documentation)

---

## 🔐 Authentication & Authorization

The system uses **JWT Bearer Authentication**.

### Flow:
1. User logs in
2. JWT token is generated
3. Token must be sent in Authorization header for protected endpoints

```http
Authorization: Bearer {your_token}

# FoodShare

A web-based data management system for **Buckinghamshire Community Food Bank (BCFB)**, developed as part of **COM5009 — Web Applications Development** (CW2).

**Author:** Elijah Oladunjoye (22230127)

FoodShare replaces manual spreadsheet processes with a secure, data-driven application for managing donors, recipients, inventory, donations, distributions, and collection bookings.

---

## Tech Stack

| Layer | Technology |
|-------|------------|
| Framework | ASP.NET Core (.NET 10) — Razor Pages |
| Database | SQLite via Entity Framework Core (Code First) |
| Authentication | ASP.NET Core Identity (Administrator, Volunteer, Donor roles) |
| Architecture | Three-tier — Pages / Services / Data |

---

## Features

- **Role-based login** with password hashing and account lockout (FR1)
- **Donor and recipient management** with search, validation, and GDPR notice (FR2–FR3)
- **Donation logging** with automatic inventory updates (FR4)
- **Inventory tracking** with expiry alerts, categories, and CSV export (FR5)
- **Distribution processing** with stock validation and dietary warnings (FR6)
- **Reports and dashboard** with date-range summaries and CSV export (FR7)
- **User account management** for administrators (US06)
- **Volunteer shift scheduling** (US12)
- **Collection slot booking** — admin and public-facing pages (US13)
- **Anonymised recipient export** (US15)

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (or .NET 8+ with compatible package versions)
- A modern web browser (Chrome, Edge, or Firefox)


---

## Demo Accounts

| Role | Email | Password |
|------|-------|----------|
| Administrator | `admin@foodshare.local` | `Admin123!` |
| Volunteer | `volunteer@foodshare.local` | `Volunteer123!` |
| Donor | `r.brown@email.com` | `Donor123!` |

---

## Project Structure

```
FoodShare/
├── Models/              # Domain entities (ERD)
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── DbSeeder.cs
│   └── Migrations/      # EF Core Code First migrations
├── Services/            # Business logic (donations, distributions, reports, bookings)
├── Pages/               # Razor Pages (UI)
├── ViewModels/          # Form models with validation
├── wwwroot/css/         # Custom FoodShare styling
└── Program.cs           # App configuration and DI
```



## Key Pages

| Page | Path | Access |
|------|------|--------|
| Login | `/Account/Login` | Public |
| Dashboard | `/Dashboard` | Admin, Volunteer |
| Donors | `/Donors` | Admin, Volunteer |
| Recipients | `/Recipients` | Admin, Volunteer |
| Inventory | `/Inventory` | Admin, Volunteer |
| Donations | `/Donations` | Admin, Volunteer |
| Distributions | `/Distributions` | Admin, Volunteer |
| Reports | `/Reports` | Admin |
| Users | `/Users` | Admin |
| Volunteers | `/Volunteers` | Admin |
| Bookings (admin) | `/Bookings` | Admin |
| Book a Collection | `/Bookings/Book` | Public |

---

## Design Traceability

This implementation is based on the CW1 design artefacts:

| CW1 Artefact | Implementation |
|--------------|----------------|
| ERD | `Models/` + `ApplicationDbContext` |
| UML services | `Services/` |
| DFD processes | Donation, inventory, distribution, booking flows |
| Wireframes | Razor Pages + `wwwroot/css/foodshare.css` |
| Functional requirements | FR1–FR7 |
| User stories | US01–US15 (MoSCoW prioritisation) |


---

## Coursework

- **Module:** COM5009 — Web Applications Development
- **Assignment:** CW2 — Team Project Web Application
- **Institution:** Buckinghamshire New University

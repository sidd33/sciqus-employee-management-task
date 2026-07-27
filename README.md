# Sciqus Employee Management System

This repository contains the source code for the Sciqus group project, which consists of a robust enterprise backend API, a cross-platform mobile frontend application, and a web frontend.

## 🚀 Project Architecture (Backend)

The backend has been completely refactored into a modern **N-Tier Architecture** utilizing the **Repository Pattern** to ensure clean code, separation of concerns, and high scalability:

1. **`EmployeeManagement.WebAPI` (Presentation Layer)**: Handles HTTP requests, Controllers, Swagger, and JWT Authentication configuration.
2. **`EmployeeManagement.BUSINESS` (Business Logic Layer)**: Contains Core Services (`TokenService`, `EmployeeService`), DTOs, Business Models, and AutoMapper configurations.
3. **`EmployeeManagement.DATA` (Data Access Layer)**: Contains Entity Framework Core, the `AppDbContext`, Generic Repositories, Migrations, and Database Seeding logic.
4. **`EmployeeManagement.COMMON` (Shared Layer)**: Contains shared enums and global utilities.
5. **`Mobile-Frontend`**: The mobile client application built with React Native. It provides a user-friendly interface for interacting with the management system on iOS and Android devices.
6. **`Web-Frontend`**: The web application client built with React and Vite, providing a fast and modern web interface.

---

## 🛠️ Technologies Used

### Backend (`/Backend`)
- **Framework**: .NET 8 / ASP.NET Core Web API
- **Language**: C#
- **Database**: Microsoft SQL Server (LocalDB)
- **ORM**: Entity Framework Core
- **Authentication**: JWT Bearer Tokens
- **Security**: BCrypt Password Hashing
- **Mapping**: AutoMapper

### Mobile Frontend (`/Mobile-Frontend`)
- **Framework**: React Native
- **Language**: TypeScript / JavaScript
- **Platform**: iOS & Android

### Web Frontend (`/Web-Frontend`)
- **Framework**: React
- **Build Tool**: Vite (with HMR and ESLint rules)

---

## 💻 Getting Started

### Prerequisites
- [Node.js](https://nodejs.org/)
- [React Native CLI environment setup](https://reactnative.dev/docs/environment-setup)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server Express LocalDB (Installs automatically with Visual Studio on Windows)

### 1. Backend Setup

1. Open a terminal and navigate to the API directory:
   ```bash
   cd Backend
   ```
2. Restore packages and build the project:
   ```bash
   dotnet build
   ```
3. Update the database. (This will automatically connect to your Windows LocalDB and build the tables):
   ```bash
   dotnet ef database update --project EmployeeManagement.DATA --startup-project EmployeeManagement.WebAPI
   ```
4. Run the API:
   ```bash
   cd EmployeeManagement.WebAPI
   dotnet run
   ```

### 🔑 Testing Authentication (Seeded Data)
The system uses **JWT Authentication**. When you run the application, the database is automatically seeded with a Super Admin. 

You can test the Login endpoint (`/api/Auth/login`) using these credentials:
- **Email:** `admin@company.com`
- **Password:** `Admin@123`

Copy the `Token` from the response and paste it into the **Authorize** lock button at the top of the Swagger UI!

---

### 2. Mobile Frontend Setup

1. Open a new terminal and navigate to the mobile frontend directory:
   ```bash
   cd Mobile-Frontend
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the application:
   - **For Android:** `npm run android`
   - **For iOS:** `npm run ios`

### 3. Web Frontend Setup

1. Navigate to the web frontend directory:
   ```bash
   cd Web-Frontend
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the Vite development server:
   ```bash
   npm run dev
   ```

---

## 🤝 Contributing & Git Workflow

When contributing to this project, please ensure you avoid merge conflicts by following this workflow:
1. Ensure your local `main` branch is up to date: `git pull origin main`
2. Create a new branch for your feature: `git checkout -b feature/your-feature-name`
3. Commit your changes with clear and descriptive messages.
4. Push to your branch and open a Pull Request against the `main` branch.

# Sciqus Group Project

This repository contains the source code for the Sciqus group project, which consists of a robust backend management API, a cross-platform mobile frontend application, and a web frontend.

## 🚀 Project Structure

The repository is structured as a monorepo containing three main components:

1. **`managment.Api`**: The backend RESTful API built with .NET 8 (ASP.NET Core). It handles data management, business logic, and serves as the core backend for the client applications.
2. **`Mobile-Frontend`**: The mobile client application built with React Native. It provides a user-friendly interface for interacting with the management system on iOS and Android devices.
3. **`Web-Frontend`**: The web application client built with React and Vite, providing a fast and modern web interface.

---

## 🛠️ Technologies Used

### Backend (`managment.Api`)
- **Framework**: .NET 8 / ASP.NET Core Web API
- **Language**: C#
- **ORM**: Entity Framework Core (EF Core)

### Mobile Frontend (`Mobile-Frontend`)
- **Framework**: React Native
- **Language**: TypeScript / JavaScript
- **Platform**: iOS & Android

### Web Frontend (`Web-Frontend`)
- **Framework**: React
- **Build Tool**: Vite (with HMR and ESLint rules)
- **Language**: JavaScript / TypeScript

---

## 💻 Getting Started

### Prerequisites
Before you begin, ensure you have the following installed:
- [Node.js](https://nodejs.org/)
- [React Native CLI environment setup](https://reactnative.dev/docs/environment-setup)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A suitable IDE (e.g., Visual Studio, VS Code, or Cursor)

### 1. Backend Setup (`managment.Api`)

1. Open a terminal and navigate to the API directory:
   ```bash
   cd managment.Api
   ```
2. Restore packages and update the database:
   ```bash
   dotnet restore
   dotnet ef database update
   ```
3. Run the API:
   ```bash
   dotnet run
   ```

### 2. Mobile Frontend Setup (`Mobile-Frontend`)

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

### 3. Web Frontend Setup (`Web-Frontend`)

*(Note: Ensure your web frontend folder is placed here in the root folder alongside the others)*

1. Navigate to the web frontend directory:
   ```bash
   cd Web-Frontend
   ```
2. Install the necessary Node dependencies:
   ```bash
   npm install
   ```
3. Start the Vite development server:
   ```bash
   npm run dev
   ```

#### About the Web Frontend Template
This web template provides a minimal setup to get React working in Vite with HMR and some ESLint rules. Two official plugins are available:
- `@vitejs/plugin-react` uses Oxc
- `@vitejs/plugin-react-swc` uses SWC

*Note: The React Compiler is not enabled on this template because of its impact on dev & build performances. To add it, see [this documentation](https://react.dev/learn/react-compiler/installation).*

---

## 🤝 Contributing

When contributing to this project, please ensure you:
1. Create a new branch for your feature or bugfix (`git checkout -b feature/your-feature-name`).
2. Commit your changes with clear and descriptive messages.
3. Push to your branch and open a Pull Request against the `main` branch.

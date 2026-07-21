# Sciqus Group Project

This repository contains the source code for the Sciqus group project, which consists of a robust backend management API and a cross-platform mobile frontend application.

## 🚀 Project Structure

The repository is divided into two main components:

1. **`managment.Api`**: The backend RESTful API built with .NET 8 (ASP.NET Core). It handles data management, business logic, and serves as the core backend for the mobile application.
2. **`Mobile-Frontend`**: The mobile client application built with React Native. It provides a user-friendly interface for interacting with the management system on both iOS and Android devices.

---

## 🛠️ Technologies Used

### Backend (`managment.Api`)
- **Framework**: .NET 8 / ASP.NET Core Web API
- **Language**: C#
- **ORM**: Entity Framework Core (EF Core)
- **Architecture**: N-Tier Architecture (Controllers, Services, DTOs, Entities, Data access)

### Frontend (`Mobile-Frontend`)
- **Framework**: React Native
- **Language**: TypeScript / JavaScript
- **Platform**: iOS & Android

---

## 💻 Getting Started

### Prerequisites
Before you begin, ensure you have the following installed:
- [Node.js](https://nodejs.org/) (for React Native)
- [React Native CLI environment setup](https://reactnative.dev/docs/environment-setup)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (for the backend API)
- A suitable IDE (e.g., Visual Studio, VS Code, or Cursor)
- A local or remote database server (check `appsettings.json` for the connection string)

### 1. Backend Setup (`managment.Api`)

1. Open a terminal and navigate to the API directory:
   ```bash
   cd managment.Api
   ```
2. Restore the required .NET packages:
   ```bash
   dotnet restore
   ```
3. Update the database using Entity Framework Core migrations (ensure your database connection string in `appsettings.json` is correct):
   ```bash
   dotnet ef database update
   ```
4. Run the API:
   ```bash
   dotnet run
   ```
   *The API should now be running locally. You can typically access the Swagger UI at `https://localhost:<port>/swagger`.*

### 2. Frontend Setup (`Mobile-Frontend`)

1. Open a new terminal and navigate to the mobile frontend directory:
   ```bash
   cd Mobile-Frontend
   ```
2. Install the necessary Node dependencies:
   ```bash
   npm install
   # or
   yarn install
   ```
3. **For iOS development (macOS only):**
   ```bash
   cd ios
   pod install
   cd ..
   ```
4. Start the React Native Metro bundler:
   ```bash
   npm start
   # or
   yarn start
   ```
5. In a separate terminal (or by pressing `i` or `a` in the Metro console), start the application on your emulator/device:
   - **For Android:** `npm run android`
   - **For iOS:** `npm run ios`

---

## 📝 Configuration

- **Backend Configuration**: All backend configuration like database connection strings and environment variables can be found in `managment.Api/appsettings.json` and `appsettings.Development.json`.
- **Frontend Configuration**: Make sure the frontend is pointing to your locally running API URL. Look for a `.env` file or update the base URL in the frontend's API service (`src/services/api.ts`).

---

## 🤝 Contributing

When contributing to this project, please ensure you:
1. Create a new branch for your feature or bugfix (`git checkout -b feature/your-feature-name`).
2. Commit your changes with clear and descriptive messages.
3. Push to your branch and open a Pull Request against the `main` branch.

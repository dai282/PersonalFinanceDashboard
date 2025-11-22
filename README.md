# Personal Finance Dashboard

A full-stack web application for tracking personal finances, budgets, and expenses.

## 🚀 Features

- Track income and expenses
- Budget management
- Visual reports and analytics
- Category-based tracking

## 🛠️ Tech Stack

- **Backend:** ASP.NET Core Web API, Entity Framework Core, SQL Server
- **Frontend:** React, TypeScript, Tailwind CSS
- **Deployment:** Azure, Vercel

## 📦 Getting Started

### Prerequisites

- .NET 8.0 SDK
- Node.js 18+
- SQL Server or LocalDB

### Backend Setup

```bash
cd backend
dotnet restore
dotnet ef database update --project PersonalFinance.Infrastructure --startup-project PersonalFinance.API
cd PersonalFinance.API
dotnet run
```

Backend runs on: `https://localhost:5151`

### Frontend Setup

```bash
cd frontend
npm install
npm start
```

Frontend runs on: `http://localhost:3000`

## 🔌 API Endpoints

### Authentication

| Method | Endpoint             | Description       | Auth Required |
| ------ | -------------------- | ----------------- | ------------- |
| POST   | `/api/auth/register` | Register new user | No            |
| POST   | `/api/auth/login`    | Login user        | No            |

**Register Request:**

```json
{
  "email": "user@example.com",
  "password": "Password123",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Login Request:**

```json
{
  "email": "user@example.com",
  "password": "Password123"
}
```

**Response:**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe"
}
```

### Categories

| Method | Endpoint          | Description        | Auth Required |
| ------ | ----------------- | ------------------ | ------------- |
| GET    | `/api/categories` | Get all categories | Yes           |
| POST   | `/api/categories` | Create category    | Yes           |

### Transactions

| Method | Endpoint                 | Description           | Auth Required |
| ------ | ------------------------ | --------------------- | ------------- |
| GET    | `/api/transactions`      | Get all transactions  | Yes           |
| GET    | `/api/transactions/{id}` | Get transaction by ID | Yes           |
| POST   | `/api/transactions`      | Create transaction    | Yes           |
| PUT    | `/api/transactions/{id}` | Update transaction    | Yes           |
| DELETE | `/api/transactions/{id}` | Delete transaction    | Yes           |

### Budgets

| Method | Endpoint            | Description     | Auth Required |
| ------ | ------------------- | --------------- | ------------- |
| GET    | `/api/budgets`      | Get all budgets | Yes           |
| POST   | `/api/budgets`      | Create budget   | Yes           |
| PUT    | `/api/budgets/{id}` | Update budget   | Yes           |
| DELETE | `/api/budgets/{id}` | Delete budget   | Yes           |

### Reports

| Method | Endpoint                            | Description              | Auth Required |
| ------ | ----------------------------------- | ------------------------ | ------------- |
| GET    | `/api/reports/monthly-summary`      | Get monthly summary      | Yes           |
| GET    | `/api/reports/spending-by-category` | Get spending by category | Yes           |

**Note:** Include JWT token in Authorization header: `Bearer {token}`

## 🎯 Project Status

✅ Authentication  
✅ Categories API
✅ Transactions API  
✅ Budgets API
✅ Reports API
✅ Frontend Development  
🚧 Testing (In Progress)
⬜ Deployment

## 📸 Screenshots

<img width="2559" height="1277" alt="image" src="https://github.com/user-attachments/assets/0e027726-92f2-4afb-95f0-29d3358fb360" />

<img width="2557" height="1278" alt="image" src="https://github.com/user-attachments/assets/ef3d4ff5-f934-4fde-8e21-af4888bc1492" />

<img width="2559" height="1276" alt="image" src="https://github.com/user-attachments/assets/4b049978-cebd-473f-bc04-236500515457" />

<img width="2559" height="1277" alt="image" src="https://github.com/user-attachments/assets/7328baee-fbe0-4559-bed8-333416e2e072" />





## 🧪 Testing

```bash
cd backend
dotnet test
```

## 📝 License

MIT License

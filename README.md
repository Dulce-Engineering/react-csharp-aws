# React + C# AWS Lambda Application

A simple, clean full-stack application featuring a **React (Vite)** frontend communicating with **C# (.NET 8)** AWS Lambda functions.

---

## Architecture Overview

```
react-csharp-aws/
├── frontend/                     # React Single Page Application (Vite)
│   ├── src/
│   │   ├── components/
│   │   │   ├── Header.jsx        # App header & API Gateway URL configuration
│   │   │   ├── GreetingCard.jsx  # Component invoking GreetingFunction Lambda
│   │   │   ├── CalculatorCard.jsx# Component invoking CalculatorFunction Lambda
│   │   │   └── ArchitectureInfo.jsx # Live architecture & deployment guide
│   │   ├── api.js                # API client with endpoint dispatchers
│   │   ├── App.jsx               # Main React application layout
│   │   ├── App.css               # Clean, simple styling
│   │   └── main.jsx
│   ├── vite.config.js            # Configured with proxy to local backend (/api -> localhost:5000)
│   └── package.json
│
├── backend/                      # C# (.NET 8) AWS Lambda Functions & Local Harness
│   ├── GreetingFunction/         # Lambda 1: Localized greetings handler
│   │   ├── Function.cs           # APIGatewayProxyRequest handler + CORS
│   │   ├── Models.cs             # GreetingRequest & GreetingResponse models
│   │   ├── GreetingFunction.csproj
│   │   └── aws-lambda-tools-defaults.json
│   ├── CalculatorFunction/       # Lambda 2: Math computation handler
│   │   ├── Function.cs           # APIGatewayProxyRequest handler + CORS
│   │   ├── Models.cs             # CalculatorRequest & CalculatorResponse models
│   │   ├── CalculatorFunction.csproj
│   │   └── aws-lambda-tools-defaults.json
│   ├── LocalDevServer/           # ASP.NET Core dev server to run & test Lambdas locally
│   │   ├── Program.cs            # Routes /api/greeting and /api/calculator to Lambda handlers
│   │   └── LocalDevServer.csproj
│   ├── serverless.template       # AWS CloudFormation / SAM template for AWS deployment
│   └── Backend.sln               # .NET Solution containing all backend projects
│
└── README.md
```

---

## Included Lambda Functions

### 1. `GreetingFunction`
- **Purpose**: Accepts a name and optional language code (`en`, `es`, `fr`, `de`, `ja`) and returns a personalized greeting, server timestamp, and runtime environment.
- **Handler**: `GreetingFunction::GreetingFunction.Function::FunctionHandler`
- **Route**: `POST /api/greeting`
- **Payload**:
  ```json
  {
    "name": "Alice",
    "language": "fr"
  }
  ```
- **Response**:
  ```json
  {
    "greeting": "Bonjour, Alice! Greetings from AWS Lambda (.NET 8)!",
    "name": "Alice",
    "language": "fr",
    "timestamp": "2026-10-12T04:47:45Z",
    "environment": "AWS Lambda (.NET 8 on Amazon Linux 2023)"
  }
  ```

### 2. `CalculatorFunction`
- **Purpose**: Computes mathematical operations (`add`, `subtract`, `multiply`, `divide`, `power`) on numbers `a` and `b`.
- **Handler**: `CalculatorFunction::CalculatorFunction.Function::FunctionHandler`
- **Route**: `POST /api/calculator`
- **Payload**:
  ```json
  {
    "a": 50,
    "b": 10,
    "operation": "divide"
  }
  ```
- **Response**:
  ```json
  {
    "a": 50,
    "b": 10,
    "operation": "divide",
    "result": 5,
    "expression": "50 ÷ 10 = 5",
    "error": null,
    "timestamp": "2026-10-12T04:47:45Z",
    "environment": "AWS Lambda (.NET 8 on Amazon Linux 2023)"
  }
  ```

---

## How to Run Locally

### Step 1: Start the Local C# Backend Server
The `LocalDevServer` invokes the exact same Lambda function code paths and enables CORS for local testing.

From the root directory:
```powershell
dotnet run --project backend/LocalDevServer
```
*The backend will start on `http://localhost:5000`.*

### Step 2: Start the React Frontend
In a separate terminal:
```powershell
cd frontend
npm.cmd run dev
```
*Open `http://localhost:5173` in your browser.*

The React dev server is preconfigured to proxy `/api` requests to `http://localhost:5000`.

---

## Deploying to AWS

### Option A: Using AWS Serverless Application Model (SAM) / CloudFormation
From the `backend` folder:
```powershell
cd backend
sam build -t serverless.template
sam deploy --guided
```

### Option B: Using the `dotnet lambda` CLI tool
```powershell
cd backend
dotnet lambda deploy-serverless --template serverless.template --s3-bucket <your-s3-deployment-bucket> --stack-name react-csharp-lambda-stack
```

Once deployed, copy the **`ApiURL`** output from AWS and paste it directly into the "Backend / API Gateway URL" input at the top of the React app (or set `VITE_API_BASE_URL` in `frontend/.env`).

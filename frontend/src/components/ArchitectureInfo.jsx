import React from 'react'

export function ArchitectureInfo() {
  return (
    <div className="card full-width">
      <div className="card-header">
        <span className="badge info">Architecture</span>
        <h2>How It Works</h2>
      </div>

      <div className="arch-grid">
        <div className="arch-step">
          <div className="step-num">1</div>
          <h3>React Frontend</h3>
          <p>
            Vite React app with separate components (<code>GreetingCard</code> and <code>CalculatorCard</code>) making HTTP POST requests to API endpoints.
          </p>
        </div>

        <div className="arch-step">
          <div className="step-num">2</div>
          <h3>API Gateway / Local Dev Server</h3>
          <p>
            In local development, <code>backend/LocalDevServer</code> serves requests directly. In AWS, <strong>Amazon API Gateway</strong> routes requests to the Lambda functions.
          </p>
        </div>

        <div className="arch-step">
          <div className="step-num">3</div>
          <h3>C# (.NET 8) AWS Lambda</h3>
          <p>
            Compiled serverless C# handlers (<code>GreetingFunction</code> and <code>CalculatorFunction</code>) processing incoming events and returning CORS-ready JSON responses.
          </p>
        </div>
      </div>

      <div className="terminal-commands">
        <div className="command-block">
          <strong>Start Local C# Backend:</strong>
          <code>dotnet run --project backend/LocalDevServer</code>
        </div>
        <div className="command-block">
          <strong>Start React Frontend:</strong>
          <code>npm run dev</code> (inside <code>frontend/</code>)
        </div>
        <div className="command-block">
          <strong>Deploy to AWS:</strong>
          <code>cd backend &amp;&amp; dotnet lambda deploy-serverless</code>
        </div>
      </div>
    </div>
  )
}

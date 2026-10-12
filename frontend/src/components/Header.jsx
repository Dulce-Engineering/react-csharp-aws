import React from 'react'

export function Header({ apiUrl, onApiUrlChange, backendStatus }) {
  return (
    <header className="app-header">
      <div className="header-content">
        <div>
          <h1>React + C# AWS Lambda</h1>
          <p className="subtitle">
            Simple demonstration of a React frontend invoking C# (.NET 8) AWS Lambda functions
          </p>
        </div>

        <div className="status-badge-container">
          <span className={`status-dot ${backendStatus.isOnline ? 'online' : 'offline'}`} />
          <span className="status-text">
            {backendStatus.isOnline ? 'Backend Connected' : 'Backend Offline'}
          </span>
        </div>
      </div>

      <div className="api-config-row">
        <label htmlFor="api-url-input">Backend / API Gateway URL:</label>
        <input
          id="api-url-input"
          type="text"
          value={apiUrl}
          onChange={(e) => onApiUrlChange(e.target.value)}
          placeholder="Leave blank for local proxy (http://localhost:5000), or paste AWS API Gateway URL"
        />
        {apiUrl && (
          <button
            type="button"
            className="secondary-btn"
            onClick={() => onApiUrlChange('')}
          >
            Reset to Local
          </button>
        )}
      </div>
    </header>
  )
}

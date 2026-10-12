import React, { useState } from 'react'
import { fetchGreeting } from '../api'

export function GreetingCard({ apiUrl }) {
  const [name, setName] = useState('Alice')
  const [language, setLanguage] = useState('en')
  const [loading, setLoading] = useState(false)
  const [response, setResponse] = useState(null)
  const [error, setError] = useState(null)

  const handleSubmit = async (e) => {
    e.preventDefault()
    setLoading(true)
    setError(null)
    setResponse(null)

    try {
      const data = await fetchGreeting(name, language, apiUrl)
      setResponse(data)
    } catch (err) {
      setError(err.message || 'Failed to call Greeting Lambda.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="card">
      <div className="card-header">
        <span className="badge">Lambda 1</span>
        <h2>Greeting Function</h2>
      </div>
      <p className="card-desc">
        Invokes <code>GreetingFunction</code> (C# .NET 8) to generate a localized greeting.
      </p>

      <form onSubmit={handleSubmit} className="form-group">
        <label htmlFor="greeting-name">Your Name</label>
        <input
          id="greeting-name"
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          placeholder="e.g. Alice"
          required
        />

        <label htmlFor="greeting-language">Language</label>
        <select
          id="greeting-language"
          value={language}
          onChange={(e) => setLanguage(e.target.value)}
        >
          <option value="en">English (Hello)</option>
          <option value="es">Spanish (¡Hola)</option>
          <option value="fr">French (Bonjour)</option>
          <option value="de">German (Hallo)</option>
          <option value="ja">Japanese (こんにちは)</option>
        </select>

        <button type="submit" disabled={loading} className="primary-btn">
          {loading ? 'Invoking Lambda...' : 'Call Greeting Lambda'}
        </button>
      </form>

      {error && (
        <div className="error-box">
          <strong>Error:</strong> {error}
          <div className="hint">
            Make sure the C# backend is running: <code>dotnet run --project backend/LocalDevServer</code>
          </div>
        </div>
      )}

      {response && (
        <div className="result-box">
          <h3>Lambda Response:</h3>
          <p className="greeting-text">{response.greeting}</p>
          <div className="meta-info">
            <div><strong>Recipient:</strong> {response.name}</div>
            <div><strong>Environment:</strong> {response.environment}</div>
            <div><strong>Timestamp:</strong> {new Date(response.timestamp).toLocaleTimeString()}</div>
          </div>
        </div>
      )}
    </div>
  )
}

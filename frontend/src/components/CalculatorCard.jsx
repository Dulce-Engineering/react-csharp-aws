import React, { useState } from 'react'
import { calculate } from '../api'

export function CalculatorCard({ apiUrl }) {
  const [a, setA] = useState(12)
  const [b, setB] = useState(4)
  const [operation, setOperation] = useState('add')
  const [loading, setLoading] = useState(false)
  const [response, setResponse] = useState(null)
  const [error, setError] = useState(null)

  const handleSubmit = async (e) => {
    e.preventDefault()
    setLoading(true)
    setError(null)
    setResponse(null)

    try {
      const data = await calculate(a, b, operation, apiUrl)
      setResponse(data)
    } catch (err) {
      setError(err.message || 'Failed to call Calculator Lambda.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="card">
      <div className="card-header">
        <span className="badge">Lambda 2</span>
        <h2>Calculator Function</h2>
      </div>
      <p className="card-desc">
        Invokes <code>CalculatorFunction</code> (C# .NET 8) to perform mathematical operations.
      </p>

      <form onSubmit={handleSubmit} className="form-group">
        <div className="grid-two">
          <div>
            <label htmlFor="calc-num-a">Number A</label>
            <input
              id="calc-num-a"
              type="number"
              step="any"
              value={a}
              onChange={(e) => setA(e.target.value)}
              required
            />
          </div>
          <div>
            <label htmlFor="calc-num-b">Number B</label>
            <input
              id="calc-num-b"
              type="number"
              step="any"
              value={b}
              onChange={(e) => setB(e.target.value)}
              required
            />
          </div>
        </div>

        <label htmlFor="calc-operation">Operation</label>
        <select
          id="calc-operation"
          value={operation}
          onChange={(e) => setOperation(e.target.value)}
        >
          <option value="add">Addition (+)</option>
          <option value="subtract">Subtraction (-)</option>
          <option value="multiply">Multiplication (×)</option>
          <option value="divide">Division (÷)</option>
          <option value="power">Power (^)</option>
        </select>

        <button type="submit" disabled={loading} className="primary-btn">
          {loading ? 'Calculating in Lambda...' : 'Execute Calculation'}
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
          <div className="calc-result-display">
            <span className="calc-expression">{response.expression}</span>
            <span className="calc-result-number">{response.result}</span>
          </div>
          <div className="meta-info">
            <div><strong>Operation:</strong> {response.operation}</div>
            <div><strong>Environment:</strong> {response.environment}</div>
            <div><strong>Timestamp:</strong> {new Date(response.timestamp).toLocaleTimeString()}</div>
          </div>
        </div>
      )}
    </div>
  )
}

import React, { useState, useEffect } from 'react'
import { Header } from './components/Header'
import { GreetingCard } from './components/GreetingCard'
import { CalculatorCard } from './components/CalculatorCard'
import { ArchitectureInfo } from './components/ArchitectureInfo'
import { checkBackendHealth } from './api'
import './App.css'

export default function App() {
  const [apiUrl, setApiUrl] = useState('')
  const [backendStatus, setBackendStatus] = useState({ isOnline: false, checked: false })

  useEffect(() => {
    let isMounted = true

    const verifyBackend = async () => {
      const result = await checkBackendHealth(apiUrl)
      if (isMounted) {
        setBackendStatus({ isOnline: result.ok, checked: true })
      }
    }

    verifyBackend()
    const timer = setInterval(verifyBackend, 5000)

    return () => {
      isMounted = false
      clearInterval(timer)
    }
  }, [apiUrl])

  return (
    <div className="app-container">
      <Header
        apiUrl={apiUrl}
        onApiUrlChange={setApiUrl}
        backendStatus={backendStatus}
      />

      <main className="cards-grid">
        <GreetingCard apiUrl={apiUrl} />
        <CalculatorCard apiUrl={apiUrl} />
      </main>

      <section className="architecture-section">
        <ArchitectureInfo />
      </section>

      <footer className="app-footer">
        <p>Built with React + C# AWS Lambda (.NET 8) &bull; Simple &amp; Clean Architecture</p>
      </footer>
    </div>
  )
}

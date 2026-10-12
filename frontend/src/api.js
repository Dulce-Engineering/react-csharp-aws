// API helper to invoke C# AWS Lambda functions via Local Dev Server or deployed AWS API Gateway

const DEFAULT_BASE_URL = import.meta.env.VITE_API_BASE_URL || ''

export async function fetchGreeting(name, language = 'en', baseUrl = DEFAULT_BASE_URL) {
  const url = `${baseUrl}/api/greeting`
  const res = await fetch(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ name, language }),
  })

  if (!res.ok) {
    const errorText = await res.text()
    throw new Error(`Greeting Lambda error (${res.status}): ${errorText || res.statusText}`)
  }

  return await res.json()
}

export async function calculate(a, b, operation = 'add', baseUrl = DEFAULT_BASE_URL) {
  const url = `${baseUrl}/api/calculator`
  const res = await fetch(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ a: Number(a), b: Number(b), operation }),
  })

  const data = await res.json()
  if (!res.ok) {
    throw new Error(data.error || `Calculator Lambda error (${res.status})`)
  }

  return data
}

export async function checkBackendHealth(baseUrl = DEFAULT_BASE_URL) {
  try {
    const res = await fetch(`${baseUrl}/`, { method: 'GET' })
    if (res.ok) {
      return { ok: true, data: await res.json() }
    }
    return { ok: false, error: `Status ${res.status}` }
  } catch (err) {
    return { ok: false, error: err.message }
  }
}

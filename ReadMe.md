# Guild Challenge: Rate-Limited API Platform

## Scenario

You are building a public API platform for our platform.

Due to this we need to care about how our api scale and handle load.

Clients authenticate using API keys and must be rate limited to prevent abuse.

Your goal is to design and implement a rate limiting system.

---

## Constraints

- No off-the-shelf rate limiting libraries and or services.

---

## Starting Point

You are provided with:

- A working API project
    - ApiService (this is the api to protect)
    - Testing app (simulates client usage)
    - ServiceDefaults project (contains default policies and endpoints)
- API Key Authentication via `X-API-Key` header

### API Key Behaviour

- Valid API key → request continues
- Invalid or missing API key → return `401 Unauthorized`

---

## Core Requirement

### Per-Client Rate Limiting

- Each API key is limited to **100 requests per minute**
- If exceeded → return `429 Too Many Requests`

---

## Implementation Guidance

Think of differet rate limiting designs.
- Fixed window
- Sliding window
- Token bucket
- Leaky bucket
- Other

Where do you write the limiting code?
- outside the system
- middleware
- database layer
- other

Be prepared to explain your choice and tradeoffs.

---

## Bonus work:

### Client Tiers

Support different limits per client:

- Free → 100 requests/min
- Premium → 1000 requests/min

### Concurrent Request Limiting

- Limit in-flight requests per client (e.g. max 5)
- If exceeded → reject the request

### Geography-Based Rate Limiting

- Determine client location (you may mock this)
- Apply limits based on API key and geography

## Things to Think About

- How does your system handle bursts of traffic?
- What happens under high load?
- Can your design scale?
- Think of edge cases!
    - server restarts?
    - Race conditions?
    - Scaling ?
- How strict are you going to be? do you allow an amount of fuzzyness for improved latency?
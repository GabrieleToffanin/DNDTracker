# DNDTracker UI

A FoundryVTT-inspired React + Vite frontend for the DNDTracker backend.

## Features

| Tab | Description |
|---|---|
| **Characters** | View all heroes with HP bars, ability scores, conditions; update HP and add conditions |
| **Combat** | Start encounters, track initiative order, advance turns, update HP / conditions per combatant |
| **Quests** | View and add campaign quests |
| **NPCs** | Manage non-player characters |
| **Locations** | Browse locations with optional map images |
| **Loot** | Track items found (magic items highlighted) |
| **Sessions** | Session log with DM notes |
| **Monsters** | Monster stat block library, reusable in combat setup |
| **Dice** | Dice roller with expression syntax (`2d6+3`), modifier, context and roll history |

## Setup

```bash
cd ui
cp .env.example .env        # edit VITE_API_URL if your backend is not on port 5169
npm install
npm run dev                 # http://localhost:5173
```

The dev server proxies `/api/*` requests to the backend automatically.

## CORS

The backend must allow the UI origin.  
If you run the UI on `http://localhost:5173`, add the following to `appsettings.Development.json` (or configure CORS in `Program.cs`):

```json
"AllowedOrigins": ["http://localhost:5173"]
```

## Build for production

```bash
npm run build               # outputs to ui/dist/
```

Serve `dist/` with any static file server (nginx, Caddy, Azure Static Web Apps, etc.).
Set the `VITE_API_URL` environment variable at build time to point to your production API.

## Stack

- React 19 + TypeScript
- Vite 8
- React Router v7
- Axios


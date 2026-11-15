# NCMS Frontend (Vue 3 + Vite)

Vue port of the old HTML/JS frontend. Half done: login, layout, permission-based routing,
Dashboard (read-only), Users and Modules (full CRUD). The other pages are placeholders.

```bash
cd Frontend
npm install
npm run dev        # http://localhost:5173
```

The dev server forwards `/api` to the .NET API (default `http://localhost:5256`, change with
`VITE_API_PROXY` in `.env`). In Codespaces forward port 5173 and keep the API running on 5256.

## Still to port (from the old files)
Debts, Prices, Expenses, Suppliers, Reports, Roles, Permissions, Audit logs, dashboard
sales/withdrawal editing, change-password. Their API calls already exist in `src/api/endpoints.js`.
Add a view in `src/views/`, then map it in `built` inside `src/router/index.js`.

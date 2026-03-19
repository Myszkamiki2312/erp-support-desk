# ERP Support Desk Angular Client

Frontend `Angular` dla projektu `ERP Support Desk`.

Klient korzysta z backendu `.NET 8` i komunikuje sie z API:

- `GET /api/dashboard`
- `GET /api/tickets`
- `POST /api/tickets`
- `GET /api/support/meta`

## Uruchomienie

```bash
npm install
npm start -- --host 127.0.0.1
```

Domyslny adres:

- `http://127.0.0.1:4200`

Backend powinien byc uruchomiony na:

- `http://localhost:5080`

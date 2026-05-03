# WEX Card API

A C# ASP.NET Core Web API for managing cards with credit limits, recording purchase transactions, and retrieving transactions with currency conversion using the [Treasury Reporting Rates of Exchange API](https://fiscaldata.treasury.gov/datasets/treasury-reporting-rates-exchange/treasury-reporting-rates-of-exchange).

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://docs.docker.com/get-docker/) (for PostgreSQL)

## Running the Application

1. Start the PostgreSQL database:

```bash
docker compose up -d
```

2. Run the API:

```bash
dotnet run --project src/WexApi
```

The API will be available at `http://localhost:5200`.

## Running Tests

```bash
dotnet test
```

Unit tests use an in-memory database. Integration tests for currency conversion make live calls to the Treasury API.

## API Endpoints

### Cards

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/cards` | Create a new card |
| GET | `/api/cards/{id}` | Retrieve a card by ID |

**Create a Card**

```bash
curl -X POST http://localhost:5200/api/cards \
  -H "Content-Type: application/json" \
  -d '{"creditLimit": 5000}'
```

Response (201 Created):
```json
{
  "id": "847d12a9-9efb-4856-8a9d-66f0f883f63f",
  "creditLimit": 5000
}
```

### Transactions

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/transactions` | Create a new transaction |
| GET | `/api/transactions/{id}` | Retrieve a transaction |
| GET | `/api/transactions/{id}?currency={currency}` | Retrieve a transaction converted to a target currency |

**Create a Transaction**

```bash
curl -X POST http://localhost:5200/api/transactions \
  -H "Content-Type: application/json" \
  -d '{"description": "Office supplies", "cardId": "<card-id>", "amountUsd": 100}'
```

**Retrieve a Transaction with Currency Conversion**

```bash
curl http://localhost:5200/api/transactions/<transaction-id>?currency=Euro
```

Response (200 OK):
```json
{
  "id": "...",
  "description": "Office supplies",
  "transactionDate": "2026-05-03T04:16:25Z",
  "originalAmount": 100,
  "exchangeRate": 0.85,
  "convertedAmount": 85.0
}
```

The `currency` parameter accepts currency names as they appear in the Treasury Reporting Rates of Exchange API (e.g., "Euro", "Dollar", "Yen").

If no exchange rate is available within 6 months on or before the transaction date, a 404 is returned.

## Assumptions & Design Decisions

- **Transaction date is set at creation time.** The transaction date is assigned when the POST request is received, under the assumption that this is the first time the transaction has existed in the system.

- **All dates are stored in UTC.** The Treasury API's record dates are also assumed to follow UTC conventions, ensuring consistent date comparisons for exchange rate lookups.

- **Currency is specified by name, not ISO code.** The `currency` query parameter expects the full currency name (e.g., "Euro", "Yen") as it appears in the Treasury Reporting Rates of Exchange API, rather than an ISO 4217 code (e.g., "EUR", "JPY").

- **Transaction amounts are stored in USD.** All amounts are recorded in US dollars. Currency conversion is applied only at retrieval time using the Treasury API exchange rates.

- **Requirement #4 (card balance) was not implemented.** Per the assignment guidelines, I tried to stick to the four hour time period as was unable to complete requirement four in that time.

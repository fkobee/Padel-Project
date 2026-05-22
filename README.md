<div align="center">

# Qversity — Fintech/Banking Data Engineering Project

**An end-to-end ELT data platform for a fictional LATAM Fintech/Banking company**

Ingests a raw JSON dataset from AWS S3, transforms it through a **Bronze → Silver → Gold** lakehouse architecture on PostgreSQL, and exposes business insights through a 4-page PowerBI dashboard answering 24 business questions.

[![Airflow](https://img.shields.io/badge/Airflow-2.7.3-017CEE?logo=apacheairflow&logoColor=white)](https://airflow.apache.org/)
[![PySpark](https://img.shields.io/badge/PySpark-3.5.0-E25A1C?logo=apachespark&logoColor=white)](https://spark.apache.org/)
[![dbt](https://img.shields.io/badge/dbt-1.7.0-FF694B?logo=dbt&logoColor=white)](https://www.getdbt.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-4169E1?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![PowerBI](https://img.shields.io/badge/PowerBI-Desktop-F2C811?logo=powerbi&logoColor=black)](https://powerbi.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![Python](https://img.shields.io/badge/Python-3.11-3776AB?logo=python&logoColor=white)](https://www.python.org/)

</div>

---

## Table of Contents

| Section | Content |
|---|---|
| [1. Project Overview](#1-project-overview) | Business context, goals, and scope |
| [2. Participant](#2-participant) | Author and submission details |
| [3. Architecture](#3-architecture) | Layered ELT design and data flow |
| [4. How to Run the Pipeline](#4-how-to-run-the-pipeline) | Step-by-step reproduction instructions |
| [5. Technology Stack](#5-technology-stack) | Roles and versions of each tool |
| [6. Data Model (ERD)](#6-data-model-erd) | Tables, columns, and relationships |
| [7. PySpark Logic](#7-pyspark-logic) | Flattening, deduplication, and date normalization |
| [8. PowerBI Dashboard](#8-powerbi-dashboard) | 4 pages, slicers, and narratives |
| [9. Key Business Insights](#9-key-business-insights) | Findings drawn from the data |
| [10. Design Decisions & Assumptions](#10-design-decisions--assumptions) | Non-obvious choices and rationale |
| [11. Data Quality (EDA Findings)](#11-data-quality-eda-findings) | Issues detected and their treatment |
| [12. Tests Implemented](#12-tests-implemented) | dbt test coverage |
| [13. Repository Structure](#13-repository-structure) | File and folder layout |
| [14. Git Tags (Milestones)](#14-git-tags-milestones) | Versioning and delivery checkpoints |

---

## 1. Project Overview

### Business Context

The client is a fictional Fintech/Banking company operating across seven LATAM countries: **Colombia (CO), Uruguay (UY), Argentina (AR), Mexico (MX), Chile (CL), Peru (PE), and Brazil (BR)**.

The source dataset contains **5,000 customer records**, each enriched with nested arrays of accounts, transactions, and loans, plus credit profile and digital engagement information. The business needs a unified analytics platform to answer **24 strategic questions** spanning revenue and profitability, risk and credit, customer demographics, transaction patterns, digital engagement, and product mix.

### Project Goals

The pipeline must:

- Ingest a single raw JSON file from AWS S3
- Implement a **Bronze / Silver / Gold** lakehouse architecture on PostgreSQL
- Use **Apache Airflow** for orchestration
- Use **PySpark** (inside the Airflow container) for nested array flattening and deduplication
- Use **dbt** for SQL transformations, dimensional modeling, and data quality testing
- Use **PowerBI** to expose insights through a 4-page dashboard
- Run fully on **Docker Compose**
- Be versioned and delivered via **GitHub** with five milestone tags

### High-Level Data Flow

```
S3 (JSON)  →  Airflow  →  Bronze (jsonb)  →  PySpark (Silver staging)  →  dbt (Silver)  →  dbt (Gold)  →  PowerBI
```

Every step is reproducible from a clean machine with a single command (`docker compose up -d --build`) plus a click in the Airflow UI.

---

## 2. Participant

| Field | Value |
|---|---|
| **Name** | Franco Cáceres Miguel |
| **Email** | francobi28@hotmail.com |
| **City** | Montevideo |
| **Cohort** | Qversity 2026 |
| **Repository** | `qversity-data-2026-montevideo-FrancoCaceres` (private) |
| **Collaborators added** | `@serasio`, `@lualopezpe`, `@luciafrances`, `@AgusOlivera` |

---

## 3. Architecture

### Layered ELT Design

The project follows the **medallion architecture** (Bronze / Silver / Gold), with clearly separated responsibilities at each layer. Each step writes its output to PostgreSQL under a dedicated schema, allowing the next step to consume it independently.

```
                       ┌───────────────────────────┐
                       │  S3 — fintech_banking_    │
                       │       dataset.json        │
                       └─────────────┬─────────────┘
                                     │ HTTP download
                                     ▼
                       ┌───────────────────────────┐
                       │  Apache Airflow (DAG)     │
                       │  qversity_fintech_        │
                       │  pipeline                 │
                       └─────────────┬─────────────┘
                                     │ orchestrates 4 sequential tasks
                                     ▼
   ╔═════════════════════════════════════════════════════════════╗
   ║                          BRONZE                             ║
   ║  schema = bronze                                            ║
   ║  bronze.raw_fintech_data (id, data jsonb, load_timestamp)   ║
   ║  - Faithful copy of source                                  ║
   ║  - TRUNCATE + INSERT on every run (idempotent)              ║
   ╚═════════════════════════════════════════════════════════════╝
                                     │
                       JDBC read by PySpark
                                     ▼
   ┌─────────────────────────────────────────────────────────────┐
   │  PySpark — spark/flatten_bronze.py                          │
   │  (runs inside the Airflow container, local mode)            │
   │  - Parses jsonb with an explicit schema                     │
   │  - Flattens nested arrays (accounts, transactions, loans)   │
   │  - Merges credit_info and digital_engagement into customers │
   │  - Deduplicates by primary key                              │
   │  - Normalizes 3 mixed date formats                          │
   └─────────────────────────────────────────────────────────────┘
                                     │
                       JDBC write (overwrite)
                                     ▼
   ╔═════════════════════════════════════════════════════════════╗
   ║                    SILVER — STAGING                         ║
   ║  schema = silver                                            ║
   ║  silver.stg_customers      (5,000 rows)                     ║
   ║  silver.stg_accounts       (17,529 rows)                    ║
   ║  silver.stg_transactions   (85,067 rows)                    ║
   ║  silver.stg_loans          (7,663 rows)                     ║
   ╚═════════════════════════════════════════════════════════════╝
                                     │
                       consumed by dbt models
                                     ▼
   ┌─────────────────────────────────────────────────────────────┐
   │  dbt — Silver models                                        │
   │  - Cleaning: TRIM, NULL handling, type casting              │
   │  - Standardization: Spanish → English translation           │
   │  - USD conversion via fixed FX rate table                   │
   │  - Dimensional modeling (dim_customers, dim_accounts,       │
   │    fct_transactions, fct_loans)                             │
   │  - 34 dbt tests (unique, not-null, accepted values,         │
   │    referential integrity, custom singular tests)            │
   └─────────────────────────────────────────────────────────────┘
                                     │
                                     ▼
   ╔═════════════════════════════════════════════════════════════╗
   ║                    SILVER — MODELED                         ║
   ║  schema = silver                                            ║
   ║  silver.dim_customers / dim_accounts                        ║
   ║  silver.fct_transactions / fct_loans                        ║
   ╚═════════════════════════════════════════════════════════════╝
                                     │
                       consumed by dbt Gold models
                                     ▼
   ┌─────────────────────────────────────────────────────────────┐
   │  dbt — Gold models                                          │
   │  - One analytics table per dashboard page                   │
   │  - Business logic: revenue, delinquency, interest income    │
   │  - Aggregations and joins from Silver                       │
   └─────────────────────────────────────────────────────────────┘
                                     │
                                     ▼
   ╔═════════════════════════════════════════════════════════════╗
   ║                          GOLD                               ║
   ║  schema = gold                                              ║
   ║  gold.gold_executive_overview                               ║
   ║  gold.gold_revenue_transactions                             ║
   ║  gold.gold_risk_credit                                      ║
   ║  gold.gold_customer_engagement                              ║
   ╚═════════════════════════════════════════════════════════════╝
                                     │
                       PowerBI connects directly
                                     ▼
                       ┌───────────────────────────┐
                       │  PowerBI Desktop          │
                       │  4-page dashboard         │
                       │  answering 24 business    │
                       │  questions                │
                       └───────────────────────────┘
```

### Layer Responsibilities Summary

| Layer | Tool | Responsibility | Output Volume |
|---|---|---|---|
| **Bronze** | Airflow + PostgreSQL | Persist raw JSON faithfully as `jsonb` | 5,100 rows |
| **Silver — Staging** | PySpark | Flatten arrays, deduplicate, normalize dates | 4 tables, ~115K rows |
| **Silver — Modeled** | dbt | Clean, translate, convert to USD, build dims & facts | 4 tables, same volume |
| **Gold** | dbt | Build analytics tables aligned with business questions | 4 tables, one per page |
| **Dashboard** | PowerBI | 4-page visualization with slicers | `.pbix` + 4 screenshots |

---

## 4. How to Run the Pipeline

### Prerequisites

| Requirement | Notes |
|---|---|
| **Docker Desktop** | Must be running before any step |
| **Git** | To clone the repository |
| **PowerBI Desktop** | Needed only to open `dashboard.pbix` (Windows) |
| **RAM** | At least 4 GB available for Docker |
| **Disk space** | ~2 GB for the Docker images |

### Step 1 — Clone the Repository

```bash
git clone <repo-url>
cd qversity-data-2026-montevideo-FrancoCaceres
```

### Step 2 — Configure Environment Variables

The repository ships with `env.example`, which lists every variable the project needs (with empty values). Copy it to `.env` and populate the values:

```bash
cp env.example .env
# Edit .env — populate the variables with your local values
```

Required variables include `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB`, `S3_BUCKET`, `S3_KEY`, and Airflow configuration variables.

### Step 3 — Build and Start the Containers

```bash
docker compose up -d --build
```

The **first** build takes a few minutes because it installs Java (required by PySpark's JVM), Python dependencies, and the dbt Postgres adapter. Subsequent runs are much faster.

### Step 4 — Verify That Services Are Healthy

```bash
docker compose ps
```

You should see three services running:

| Service | Expected Status | Port |
|---|---|---|
| `postgres` | `Up (healthy)` | `5432` |
| `airflow` | `Up` | `8080` |
| `dbt` | `Up` | — |

### Step 5 — Run the Pipeline from Airflow

1. Open the Airflow UI at **http://localhost:8080**
2. Log in with username `admin` and password `admin`
3. Find the DAG **`qversity_fintech_pipeline`** in the list
4. Toggle the DAG **on** (switch in the leftmost column)
5. Click the **▶ Trigger DAG** button on the right side

The DAG executes four tasks **sequentially**:

```
bronze_ingest_from_s3
        │
        ▼
silver_flatten_pyspark
        │
        ▼
silver_dbt_transform
        │
        ▼
gold_dbt_transform
```

Total runtime is approximately **2–4 minutes** on a typical laptop.

### Step 6 — Inspect the Data

```bash
docker compose exec postgres psql -U qversity-admin -d qversity
```

Inside `psql`:

```sql
\dn                    -- list schemas
\dt bronze.*           -- list bronze tables
\dt silver.*           -- list silver tables
\dt gold.*             -- list gold tables

SELECT COUNT(*) FROM bronze.raw_fintech_data;     -- 5,100
SELECT COUNT(*) FROM silver.dim_customers;        -- 5,000
SELECT COUNT(*) FROM silver.fct_transactions;     -- 85,067
SELECT COUNT(*) FROM gold.gold_executive_overview;-- 5,000
```

### Step 7 — Open the PowerBI Dashboard

Open `powerbi/dashboard.pbix` in **PowerBI Desktop**. If you need to reconfigure the connection to PostgreSQL:

| Field | Value |
|---|---|
| Server | `localhost:5432` |
| Database | `qversity` |
| Schema | `gold` |
| Username | `qversity-admin` |
| Password | `qversity-admin` |

### Useful dbt Commands (Optional, for Debugging)

```bash
# Enter the Airflow container (where dbt is installed)
docker compose exec airflow bash

# Inside the container, navigate to the dbt project
cd /opt/airflow/dbt

# Run all models
dbt run --profiles-dir /opt/airflow/dbt --project-dir /opt/airflow/dbt

# Run only Silver or Gold
dbt run --select silver --profiles-dir /opt/airflow/dbt --project-dir /opt/airflow/dbt
dbt run --select gold   --profiles-dir /opt/airflow/dbt --project-dir /opt/airflow/dbt

# Run all tests
dbt test --profiles-dir /opt/airflow/dbt --project-dir /opt/airflow/dbt
```

In normal operation, dbt is **invoked automatically by the Airflow DAG** — these commands exist only for ad-hoc debugging.

---

## 5. Technology Stack

| Tool | Version | Role in this project |
|---|---|---|
| **Docker & Docker Compose** | latest | Containerization of the full stack — `docker compose up` boots Postgres, Airflow, and dbt together |
| **PostgreSQL** | 15 | Warehouse with three schemas (`bronze`, `silver`, `gold`) holding all the data |
| **Apache Airflow** | 2.7.3 | DAG orchestration; runs the 4-task pipeline sequentially |
| **PySpark** | 3.5.0 | Nested array flattening, deduplication, JDBC I/O — runs inside the Airflow container |
| **dbt-core + dbt-postgres** | 1.7.0 | SQL transformations (Silver and Gold) and data quality testing |
| **PowerBI Desktop** | latest | Dashboard and visualization layer |
| **Python** | 3.11 | DAGs and PySpark scripts |
| **Java (OpenJDK)** | 11 | Required by PySpark's JVM runtime — installed inside the Airflow container |
| **PostgreSQL JDBC Driver** | 42.7.2 | Lets PySpark read from and write to PostgreSQL |
| **GitHub** | — | Version control, milestone tagging, and submission |

---

## 6. Data Model (ERD)

The pipeline produces a normalized relational model in **Silver** and consumer-ready analytics tables in **Gold**. The diagram below shows both layers and their relationships.

![ERD Diagram](docs/erd.png)

### Silver — Dimensions

#### `silver.dim_customers` — 5,000 rows

One row per customer. Combines the flat customer fields with the nested `credit_info` and `digital_engagement` objects (flattened by PySpark before dbt).

| Field group | Examples |
|---|---|
| **Identity** | `customer_id` (PK), `first_name`, `last_name`, `email`, `phone_number` |
| **Demographics** | `date_of_birth`, `gender`, `nationality`, `age`, `age_bucket` |
| **Geography** | `country`, `city`, `lat`, `lon`, `address` |
| **Lifecycle** | `registration_date`, `tenure_months`, `status`, `kyc_status` |
| **Segmentation** | `customer_segment`, `risk_score`, `risk_bucket`, `relationship_manager` |
| **Credit info** | `credit_score`, `credit_score_bucket`, `utilization_pct`, `total_limit`, `late_payments_12m`, `bankruptcy_flag`, … |
| **Digital engagement** | `mobile_app_registered`, `preferred_channel`, `avg_monthly_logins`, `last_login_date`, … |

#### `silver.dim_accounts` — 17,529 rows

One row per account, linked to `dim_customers` via `customer_id`.

Columns: `account_id` (PK), `customer_id` (FK), `account_type`, `currency`, `balance_local`, `balance_usd`, `credit_limit_local`, `credit_limit_usd`, `interest_rate`, `opened_date`, `status`, `branch_code`.

### Silver — Facts

#### `silver.fct_transactions` — 85,067 rows

One row per transaction, linked to both customers and accounts.

| Field group | Examples |
|---|---|
| **Identity** | `transaction_id` (PK), `customer_id` (FK), `account_id` (FK) |
| **Temporal** | `transaction_date`, `day_of_week`, `day_of_week_num`, `month_num`, `year_num` |
| **Financial** | `amount_local`, `amount_usd`, `currency`, `local_currency` |
| **Classification** | `transaction_type`, `category`, `merchant`, `channel`, `status` |
| **Derived flags** | `is_failed`, `is_international` |

#### `silver.fct_loans` — 7,663 rows

One row per loan, linked to `dim_customers` via `customer_id`.

Columns: `loan_id` (PK), `customer_id` (FK), `loan_type`, `currency`, `principal_local`, `principal_usd`, `outstanding_balance_local`, `outstanding_balance_usd`, `interest_rate`, `term_months`, `monthly_payment_usd`, `start_date`, `end_date`, `loan_status`, `days_past_due`, `dpd_bucket`, `is_delinquent`, `collateral_type`.

### Gold — Analytics Tables

Each Gold table is purpose-built for a specific dashboard page. The grain is explicit and the columns contain everything PowerBI needs to answer the relevant business questions, avoiding any joins on the BI side.

| Table | Grain | Dashboard Page | Questions Answered |
|---|---|---|---|
| `gold_executive_overview` | One row per customer | Page 1 — Executive | Q1, Q10, Q12, Q13, Q14 |
| `gold_revenue_transactions` | One row per transaction | Page 2 — Revenue | Q2, Q3, Q15, Q16, Q17, Q18, Q19 |
| `gold_risk_credit` | One row per loan | Page 3 — Risk | Q4, Q5, Q6, Q7, Q8, Q9, Q23 |
| `gold_customer_engagement` | One row per account | Page 4 — Engagement | Q11, Q20, Q21, Q22, Q24 |

**All 24 business questions are answered** through the combination of Gold tables and PowerBI visuals.

---

## 7. PySpark Logic

The script `spark/flatten_bronze.py` runs inside the Airflow container and bridges the raw `jsonb` data in Bronze with the structured Silver staging tables. The job is invoked by Airflow through `subprocess.run()` from the DAG.

### Why PySpark?

The project explicitly mandates PySpark for nested array flattening. While PostgreSQL itself could handle this dataset with SQL, PySpark provides a scalable and idiomatic way to process JSON arrays that would generalize to much larger volumes.

### Reading from Bronze

PySpark connects to PostgreSQL via JDBC and reads `bronze.raw_fintech_data`. The `data` column (type `jsonb`) is parsed into a typed structure using an **explicit schema** declared in the function `get_bronze_schema()`, ensuring predictable types regardless of any irregularities in the source.

### Flattening Nested Arrays

The nested arrays in each customer record are exploded into separate staging tables:

| Source array (nested in JSON) | Output table | Rows produced |
|---|---|---|
| `accounts[]` (2–5 per customer) | `silver.stg_accounts` | 17,529 |
| `transactions[]` (5–30 per customer) | `silver.stg_transactions` | 85,067 |
| `loans[]` (0–3 per customer) | `silver.stg_loans` | 7,663 |

Flat customer fields, together with the `credit_info{}` and `digital_engagement{}` nested objects, are merged into **`silver.stg_customers`** (5,000 rows). This means all customer-level information is in one table, ready for dbt to clean and model.

### Deduplication Logic

A **window function** partitions by the primary key (`customer_id`, `account_id`, `transaction_id`, or `loan_id`) and keeps the **first occurrence** per group:

```python
w = Window.partitionBy(primary_key).orderBy(primary_key)
df.withColumn("rn", row_number().over(w)).filter(col("rn") == 1)
```

Because the Bronze table is truncated before each ingestion and the source JSON is a static snapshot, deduplication acts as a **safety net** rather than performing significant work. In a real streaming scenario, this logic would be replaced with timestamp-based deduplication.

### Date Normalization

The source dataset contains **three mixed date formats**: `YYYY-MM-DD`, `DD/MM/YYYY`, and `YYYYMMDD`. A helper function detects each value's format using regular expressions and converts everything to a standard SQL `DATE` type before writing to Silver. Centralizing this in PySpark keeps the dbt Silver models clean — they receive consistent dates and do not need to re-implement parsing logic in SQL.

### Writing to Silver

Each DataFrame is written to PostgreSQL via JDBC in **`overwrite`** mode, mirroring the idempotency strategy used in Bronze. Running the DAG twice produces the same result as running it once.

---

## 8. PowerBI Dashboard

The dashboard has **four pages**, each connected directly to the `gold` schema in PostgreSQL. Every page provides slicers (filters) for `country`, `customer_segment`, and other context-relevant dimensions, so any KPI can be sliced interactively. All amounts are shown in **USD** for cross-country comparability.

### Page 1 — Executive Overview

**What it shows.** High-level KPIs (Total Customers, Assets Under Management, Active Customers, KYC Verified Rate); customer count by country and city (with drill-down); monthly acquisition trend; customer status and KYC distributions; average revenue per customer by segment.

**Business questions answered.** Q1 (avg revenue per customer by segment), Q10 (customer count by country/city), Q12 (acquisition trend), Q13 (status breakdown), Q14 (KYC status).

**Decisions it supports.** Strategic overview of the customer base health, acquisition cadence, and segment-level value distribution.

### Page 2 — Revenue & Transactions

**What it shows.** Transaction volume and value by category (combined chart); transaction count by day of the week; average transaction size and total revenue by channel; failed transaction rate by channel; international vs local patterns by country; total account balance by country (in USD).

**Business questions answered.** Q2 (balances by country), Q3 (revenue by channel), Q15 (categories by volume and value), Q16 (volume by day of week), Q17 (avg size by channel), Q18 (failed rate by channel), Q19 (international transfer patterns).

**Decisions it supports.** Channel investment priorities, category-level revenue analysis, identification of failure hotspots, cross-border activity monitoring.

### Page 3 — Risk & Credit

**What it shows.** Loan delinquency rate by segment; credit score distribution by country; utilization vs delinquency scatter plot; days past due distribution by loan type; loan portfolio composition (outstanding USD by type and status); interest income by loan type; customer risk segmentation.

**Business questions answered.** Q4 (interest income by loan type), Q5 (delinquency rate by segment), Q6 (credit score by country), Q7 (utilization vs delinquency), Q8 (days past due by loan type), Q9 (risk segmentation), Q23 (loan portfolio composition).

**Decisions it supports.** Credit risk policy adjustment, portfolio rebalancing, identification of delinquency drivers and concentrations.

### Page 4 — Customer & Engagement

**What it shows.** Age distribution by segment; mobile app adoption rate by segment; digital vs physical channel preference by age group; account type distribution; average number of products per customer by segment.

**Business questions answered.** Q11 (age distribution by segment), Q20 (mobile adoption by segment), Q21 (digital vs branch by age group), Q22 (popular account types), Q24 (avg products per customer by segment).

**Decisions it supports.** Digital strategy targeting, cross-selling opportunities, product mix optimization by age cohort.

> Screenshots of each page are stored in `powerbi/screenshots/`. The full `.pbix` file is at `powerbi/dashboard.pbix`.

---

## 9. Key Business Insights

These are the most relevant findings drawn directly from the Gold layer. The dataset is synthetic, so some of the patterns are dataset characteristics rather than real-world signals — flagged where relevant.

### Customer Base

- **5,000 customers** distributed across 7 LATAM countries, with **total AUM ≈ $4,259M USD** (after FX conversion).
- Country distribution is **balanced** — roughly 700 customers per country (range 666 in CL to 737 in BR and PE).
- Customer status is also **evenly split**: about 25% each in active, inactive, suspended, and closed.

### Revenue & Transactions

- **Mexico leads in total balance** ($649M USD), followed by Peru ($634M USD) and Brazil ($614M USD). After conversion to USD, all countries fall within a narrow $555M–$649M range
- Comparing balances in local currency would have been misleading. For example, COP has a much higher nominal value than BRL, so a direct comparison without conversion would have suggested Colombia dominates by orders of magnitude.
- **The failed transaction rate is essentially uniform across channels** (~25% in all five channels), suggesting the failure mechanism is not channel-specific in this dataset.
- **54% of transactions are international** — high cross-border activity is a defining characteristic of this customer base.

### Risk & Credit

- **Delinquency rate is ~50% across all segments** (48.9% SME → 49.9% private banking). The lack of meaningful differentiation between segments is a hallmark of the synthetic dataset; in a real scenario it would warrant urgent investigation into segmentation criteria.
- **`poor` is the dominant credit score bucket**: 1,733 customers (~35%), followed by `fair` (495), `good` (455), `unknown` (389), `very_good` (353), and `exceptional` (331).
- The 389 `unknown` customers correspond to records where `credit_score` was outside the valid 300–850 range and was set to NULL — see EDA findings below.

### Customer Engagement

- **`mobile` is the most popular preferred channel** with 1,058 customers, but the distribution across all five channels (mobile, branch, atm, web, phone) is fairly even.
- **Mobile adoption is approximately 50% in every segment** — there is no segment-driven digital adoption pattern in the data.
- The **55+ age bucket is the largest** (~2,033 customers, ~40%) because it is open-ended. The other buckets cover 10-year spans.

---

## 10. Design Decisions & Assumptions

This section documents the **non-obvious choices** made during the project, with the rationale for each.

### Bronze Layer

| Decision | Justification |
|---|---|
| `TRUNCATE` before each insert | Guarantees idempotency — the DAG can be re-run safely any number of times. Acceptable here because the source is a static snapshot. In a production scenario with incremental loads, this would be replaced by upsert logic. |
| Store the payload as `jsonb`, not parsed | Preserves the raw source faithfully. If the parsing logic changes later, there is no need to re-fetch the file from S3 — Bronze is the historical source of truth. |
| Schema `bronze.raw_fintech_data(id, data jsonb, load_timestamp)` | Minimal and explicit: surrogate key, raw payload, ingestion timestamp. Aligned with the project specification. |

### Silver — PySpark

| Decision | Justification |
|---|---|
| Deduplication by **first occurrence** ordered by primary key | Conservative and deterministic. Safe given the Bronze truncation strategy. |
| **Mixed date format detection** in PySpark | The source contains three formats (`YYYY-MM-DD`, `DD/MM/YYYY`, `YYYYMMDD`). Centralizing the parsing in PySpark keeps dbt models clean and avoids repeating regex logic in SQL. |
| `overwrite` mode for Silver staging writes | Same idempotency rationale as Bronze. |
| **Explicit schema** (not inferred) | More robust and predictable than schema inference, especially for nested types like `credit_info{}` and `digital_engagement{}`. |
| 2 GB driver memory in Spark | Sufficient for ~5,100 customers. In production this would scale with volume. |

### Silver — dbt

| Decision | Justification |
|---|---|
| **Conversion to USD** using fixed FX rates | Local currencies are not directly comparable (COP nominal value is far from BRL or USD). Confirmed with instructors. Local values are kept alongside USD for traceability. FX rates used (approx. 2024–2025 averages): COP 4000, UYU 40, ARS 1000, MXN 17, CLP 950, PEN 3.8, BRL 5, EUR 0.92, USD 1. |
| **Spanish → English translation** for `status`, `customer_segment`, `transaction_type` | The source contains mixed-language categorical values. Standardized to English for consistency with the project specification and dashboard clarity. |
| `TRIM()` on categorical fields | Source values contain leading/trailing whitespace that initially broke `accepted_values` tests. |
| `is_international` based on the **customer's country currency**, not a static list | A Uruguayan paying in ARS counts as an international transaction. Implemented via a join with `dim_customers` and a country→currency mapping. |
| `phone` classified as a **digital** channel | Confirmed with the instructor team in Slack. Channels are split as: digital = {mobile, web, phone}, physical = {atm, branch, pos}. |
| `credit_score` outside [300, 850] → **NULL** | EDA detected 515 corrupt values (range −99 to 999,999). The 300–850 range is defined in the project specification. Invalid values are treated as `unknown` in the bucket so they are not silently filtered out. |
| `is_delinquent` = status in (`delinquent`, `default`) **OR** `days_past_due > 0` | Captures both status-based and aging-based delinquency, avoiding under-reporting if the status field is stale relative to the days-past-due counter. |
| Macro `generate_schema_name` | By default, dbt concatenates `target.schema` with the model's `schema` config, producing `public_silver` instead of `silver`. The macro overrides this so the model's `schema` is used directly. |

### Gold — dbt

| Decision | Justification |
|---|---|
| **One Gold table per dashboard page** | Clear grain per table, easy to maintain, aligned with consumer needs. Avoids confusion when adding new visuals on the BI side. |
| `interest_income = outstanding_balance × interest_rate` | Confirmed assumption with the instructor: loans are assumed to run to `end_date` (no early payoff, since the dataset has no `paid_off_date` field). This approximates annual interest income. |
| `total_products = num_accounts + num_loans` per customer | Aggregates all products a customer holds across both tables. |

### Infrastructure

| Decision | Justification |
|---|---|
| **Custom Dockerfile** on top of `apache/airflow:2.7.3-python3.11` | The base image does not include Java, which PySpark needs to run its JVM. We add OpenJDK 11 and the required Python packages while keeping the exact Airflow version mandated by the project. |
| `spark.jars.packages` for the PostgreSQL JDBC driver | Spark downloads the driver from Maven Central at runtime instead of bundling it inside the Dockerfile. The result is a cleaner image with no vendored JARs in the repo. |
| `LocalExecutor` for Airflow | More efficient than `SequentialExecutor` for the multi-task DAG, while staying simple enough for a single-container setup. |
| Schedule = `None` (manual trigger only) | The pipeline runs against a static snapshot — no automatic scheduling is required, and manual triggers are easier to debug during development. |
| `env.example` contains the same variable names as `.env` but with **empty values** | Confirmed by the instructor team in Slack. Evaluators copy `env.example`, rename it to `.env`, and populate the variables with their own local values. |

---

## 11. Data Quality (EDA Findings)

Exploratory Data Analysis was performed via SQL queries against Silver and Gold tables, plus inspection of the raw JSON. Every finding below directly informed the cleaning logic and the dbt tests.

| Finding | Frequency | Treatment |
|---|---|---|
| `credit_score` outside the valid [300, 850] range | 515 customers (~10%) | Set to NULL; `credit_score_bucket = 'unknown'` |
| `registration_date` NULL | 298 customers (~6%) | Kept (not dropped); naturally excluded from temporal aggregations |
| `transaction_date` NULL | 5,320 transactions (~6%) | Kept; visually filtered in PowerBI (excluded from the day-of-week chart) |
| Leading/trailing whitespace in categorical fields | Multiple columns | `TRIM()` applied across all categorical columns |
| Mixed Spanish/English categorical values | `status`, `customer_segment`, `transaction_type` | Translated to English in Silver dbt models |
| Three mixed date formats (`YYYY-MM-DD`, `DD/MM/YYYY`, `YYYYMMDD`) | All date fields | Normalized in PySpark before being written to Silver |
| `EUR` currency present in some transactions | 887 transactions | Added to the FX rate table (0.92) |
| `mobile_app_registered` uniformly ~50% across segments | All segments | Documented as a dataset characteristic, not a modeling issue |
| `paid_off` loans always have `outstanding_balance = 0` | All `paid_off` loans | Expected behavior; visually verified in the loan portfolio chart |
| Categories with `null`, `n/a`, `na` string literals | Some transactions | Normalized to actual SQL NULL in Silver |

---

## 12. Tests Implemented

A total of **34 dbt tests** are implemented, all passing on every run.

### Uniqueness (4)
- `dim_customers.customer_id`
- `dim_accounts.account_id`
- `fct_transactions.transaction_id`
- `fct_loans.loan_id`

### Not-null (10+)
- Primary keys on all tables
- Key business fields: `email`, `country`, `amount_usd`, `amount_local`, `customer_id` on facts, etc.

### Accepted Values (12)
- `status` (active / inactive / suspended / closed)
- `customer_segment` (retail / premium / private_banking / sme)
- `kyc_status` (verified / pending / expired / rejected)
- `account_type`, `channel`, `transaction_type`, `loan_type`, `loan_status`
- `risk_bucket`, `credit_score_bucket`, `country`

### Referential Integrity (4)
- `dim_accounts.customer_id` → `dim_customers.customer_id`
- `fct_transactions.customer_id` → `dim_customers.customer_id`
- `fct_transactions.account_id` → `dim_accounts.account_id`
- `fct_loans.customer_id` → `dim_customers.customer_id`

### Custom Singular Tests (3)
| Test | Purpose |
|---|---|
| `assert_days_past_due_non_negative` | Ensures `days_past_due >= 0` in `fct_loans` (no negative aging values) |
| `assert_transaction_amount_positive` | Ensures `amount_usd > 0` in `fct_transactions` (no zero or negative amounts) |
| `test_credit_score_range` | Ensures `credit_score` is within [300, 850] after the NULL treatment for invalid source values |

---

## 13. Repository Structure

```
qversity-data-2026-montevideo-FrancoCaceres/
│
├── docker-compose.yml          # Multi-container setup (postgres, airflow, dbt)
├── Dockerfile                  # Custom Airflow image with Java + PySpark deps
├── env.example                 # Template for required environment variables (empty values)
├── requirements.txt            # Python dependencies
├── README.md                   # This file
├── .gitignore                  # Excludes .env, logs, __pycache__, etc.
│
├── dags/
│   └── qversity-dag.py         # Main Airflow DAG (Bronze → Silver → Gold)
│
├── spark/
│   └── flatten_bronze.py       # PySpark Silver staging job
│
├── dbt/
│   ├── dbt_project.yml         # dbt project config
│   ├── profiles.yml            # PostgreSQL connection profile
│   ├── macros/
│   │   └── generate_schema_name.sql    # Custom schema name resolver
│   ├── models/
│   │   ├── bronze/             # Bronze passthrough model
│   │   ├── silver/             # 4 cleaned dims & facts + schema.yml with tests
│   │   └── gold/               # 4 analytics models + schema.yml
│   └── tests/                  # 3 custom singular tests
│
├── powerbi/
│   ├── dashboard.pbix          # PowerBI dashboard file
│   └── screenshots/            # One screenshot per dashboard page
│
├── data/
│   └── raw/                    # Local copy of the source JSON (gitignored)
│
└── docs/
    └── erd.png                 # Entity-Relationship Diagram
```

---

## 14. Git Tags (Milestones)

The project is delivered in **five incremental milestones**, each marked with an annotated git tag.

| Tag | Milestone | What is delivered |
|---|---|---|
| `v0.1.0-bronze` | Bronze layer complete | 5,100 records ingested from S3 into `bronze.raw_fintech_data` |
| `v0.2.0-silver` | Silver layer complete | PySpark staging + dbt cleaned dims & facts, 34 tests passing |
| `v0.3.0-gold` | Gold layer complete | 4 analytics models answering all 24 business questions |
| `v0.4.0-powerbi` | PowerBI dashboard ready | 4 pages, all 24 questions visualized, screenshots committed |
| `v1.0.0` | Final submission | Full project including README, ERD, and all deliverables |

### Useful Commands

```bash
# List all tags
git tag -l

# Check out a specific milestone
git checkout v0.2.0-silver

# View the annotation of a tag
git show v0.2.0-silver
```

---

<div align="center">

**Qversity Technical Project v2**

Submitted by **Franco Cáceres** · Montevideo · 2026

</div>

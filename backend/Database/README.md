# Database Setup Instructions

This directory contains SQL scripts to set up the IncomeApp database tables in Supabase.

## Table Structure

The application uses the following tables:
1. **users** - Stores authenticated user profiles (email, name, picture)
2. **user_providers** - Links users to their OAuth providers (e.g., Facebook)
3. **financial_summaries** - Stores user financial summary data (income, savings, investments, net worth growth)
4. **expenses** - Stores user expense/category breakdown data
5. **insurances** - Stores insurance policy information
6. **debts** - Stores debt/installment tracking data

## Auth Tables

### Users Table

```sql
create table public.users (
  id uuid not null default gen_random_uuid (),
  email text not null,
  name text null,
  picture_url text null,
  created_at timestamp without time zone not null default now(),
  updated_at timestamp without time zone not null default now(),
  constraint users_pkey primary key (id)
);
```

### User Providers Table

```sql
create table public.user_providers (
  id uuid not null default gen_random_uuid (),
  user_id uuid not null,
  provider_name text not null,
  provider_user_id text not null,
  provider_email text null,
  provider_data jsonb null,
  created_at timestamp without time zone not null default now(),
  updated_at timestamp without time zone not null default now(),
  constraint user_providers_pkey primary key (id),
  constraint user_providers_user_id_fkey foreign key (user_id) references users (id) on delete cascade
);
```

## Setup Steps

### 1. Create Tables

Run the table creation scripts in your Supabase SQL Editor in the following order:

```sql
-- Run these scripts in order
00_create_financial_summaries_table.sql  -- Financial summary data
01_create_expenses_table.sql             -- Expense tracking
02_create_insurances_table.sql           -- Insurance policies
03_create_debts_table.sql                -- Debt/installments
04_remove_computed_debt_columns.sql      -- Drop remaining_amount & total_amount (computed, not stored)
```

These scripts will:
- Create the necessary tables with proper column types
- Add indexes for performance
- Enable Row Level Security (RLS)
- Create RLS policies to ensure users can only access their own data

### 2. Insert Sample Data

After creating the tables, you can insert sample data using the insert scripts:

> ⚠️ **IMPORTANT**: Replace `'YOUR_USER_ID_HERE'` with your actual user ID from the `auth.users` table.

To find your user ID:
1. Go to Supabase Dashboard → Authentication → Users
2. Click on your user
3. Copy the UUID

Then run the insert scripts:

```sql
-- Replace YOUR_USER_ID_HERE in each file before running
insert_expenses.sql
insert_insurances.sql
insert_debts.sql
```

## Table Schemas

### Financial Summaries Table

> [!IMPORTANT]
> **Row Level Security (RLS) is DISABLED** for this table. We use the Supabase Service Role Key which bypasses RLS, and security is handled via custom JWT authentication in the backend. RLS policies using `auth.uid()` only work with Supabase Auth, not custom JWT authentication.

| Column | Type | Description |
|--------|------|-------------|
| id | UUID | Primary key |
| user_id | UUID | Reference to the user (unique constraint) |
| income | DECIMAL(18,2) | User's income |
| total_savings | DECIMAL(18,2) | Total savings amount |
| total_investment | DECIMAL(18,2) | Total investment amount |
| net_worth_growth | DECIMAL(18,2) | Net worth growth |
| created_at | TIMESTAMP | When the record was created |
| updated_at | TIMESTAMP | When the record was last updated (auto-updated) |

**Implementation Details:**
- **Model**: `FinancialSummary.cs` - Uses Postgrest attributes for database mapping
- **Service**: `FinancialService.cs` - Fetches and updates summary data from database
- **Default Values**: New users automatically get default summary with all values set to 0
- **Security**: Security is enforced via JWT authentication at the API level. User ID is extracted from JWT claims and used to filter data queries
- **Auto-Update**: The `updated_at` field is automatically updated via database trigger

### Expenses Table
| Column | Type | Description |
|--------|------|-------------|
| id | UUID | Primary key |
| user_id | UUID | Foreign key to auth.users |
| name | TEXT | Expense name (e.g., "เงินเก็บ") |
| amount | DECIMAL(10,2) | Expense amount |
| type | TEXT | Type: Fixed, Variable, Family, Health |
| color | TEXT | Display color in hex |
| bank_app | TEXT | Bank/app name (Dime, Make, KTB, etc.) |
| is_highlighted | BOOLEAN | Indicates if the expense is highlighted |
| created_at | TIMESTAMP | Creation timestamp |
| updated_at | TIMESTAMP | Last update timestamp |

### Insurances Table
| Column | Type | Description |
|--------|------|-------------|
| id | UUID | Primary key |
| user_id | UUID | Foreign key to auth.users |
| provider | TEXT | Insurance provider (e.g., "AIA") |
| policy_name | TEXT | Policy name (e.g., "แผนชีวิต") |
| premium | DECIMAL(10,2) | Premium amount |
| due_date | TIMESTAMP | Payment due date |
| status | TEXT | Status: Paid, Upcoming, Overdue |
| created_at | TIMESTAMP | Creation timestamp |
| updated_at | TIMESTAMP | Last update timestamp |

### Debts Table
| Column | Type | Description |
|--------|------|-------------|
| id | UUID | Primary key |
| user_id | UUID | Foreign key to auth.users |
| name | TEXT | Debt name (e.g., "Shokz Openrun") |
| monthly_payment | DECIMAL(10,2) | Monthly installment amount |
| current_installment | INT | Current installment number |
| total_installments | INT | Total number of installments |
| created_at | TIMESTAMP | Creation timestamp |
| updated_at | TIMESTAMP | Last update timestamp |

> `remaining_amount` and `total_amount` are not stored — computed at runtime:
> `total_amount = monthly_payment × total_installments`
> `remaining_amount = monthly_payment × (total_installments − current_installment)`

## Row Level Security (RLS)

All tables have RLS enabled with the following policies:
- Users can only SELECT, INSERT, UPDATE, and DELETE their own records
- Access is controlled via `auth.uid() = user_id` check

This ensures data isolation between users.

## Backend Integration

The backend uses the following entity models to interact with these tables:
- `FinancialSummary.cs` → financial_summaries table
- `ExpenseEntity.cs` → expenses table
- `InsuranceEntity.cs` → insurances table  
- `DebtEntity.cs` → debts table

The `FinancialService.cs` now fetches data from Supabase instead of using in-memory storage.

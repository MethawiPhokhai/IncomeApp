-- Create financial_summaries table
-- This table stores financial summary data for each user
-- Run this SQL in Supabase SQL Editor

CREATE TABLE IF NOT EXISTS financial_summaries (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    income DECIMAL(18, 2) NOT NULL DEFAULT 0,
    total_savings DECIMAL(18, 2) NOT NULL DEFAULT 0,
    total_investment DECIMAL(18, 2) NOT NULL DEFAULT 0,
    net_worth_growth DECIMAL(18, 2) NOT NULL DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    
    -- Ensure one summary per user
    CONSTRAINT unique_user_summary UNIQUE(user_id)
);

-- Create index for faster user_id lookups
CREATE INDEX IF NOT EXISTS idx_financial_summaries_user_id ON financial_summaries(user_id);

-- Note: RLS (Row Level Security) is DISABLED for this table
-- We use Service Role Key which bypasses RLS, and we handle security via custom JWT auth
-- If you want to enable RLS in the future, you'll need to integrate with Supabase Auth

-- Create function to automatically update updated_at timestamp
CREATE OR REPLACE FUNCTION update_financial_summaries_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Create trigger to call the function on updates
DROP TRIGGER IF EXISTS trigger_update_financial_summaries_updated_at ON financial_summaries;
CREATE TRIGGER trigger_update_financial_summaries_updated_at
    BEFORE UPDATE ON financial_summaries
    FOR EACH ROW
    EXECUTE FUNCTION update_financial_summaries_updated_at();

-- Insert default data for testing (optional - remove if not needed)
-- Replace the user_id with an actual user ID from your users table
-- INSERT INTO financial_summaries (user_id, income, total_savings, total_investment, net_worth_growth)
-- VALUES ('00000000-0000-0000-0000-000000000000', 0, 0, 0, 0);

-- ============================================================================
-- OPTIONAL: If you already created the table with RLS enabled, run this fix
-- ============================================================================
-- This section is only needed if you previously ran a version of this script
-- that enabled RLS policies. If this is a fresh installation, skip this section.

-- Drop existing RLS policies if they exist
DROP POLICY IF EXISTS "Users can read own financial summary" ON financial_summaries;
DROP POLICY IF EXISTS "Users can insert own financial summary" ON financial_summaries;
DROP POLICY IF EXISTS "Users can update own financial summary" ON financial_summaries;
DROP POLICY IF EXISTS "Users can delete own financial summary" ON financial_summaries;

-- Disable RLS on the table
ALTER TABLE financial_summaries DISABLE ROW LEVEL SECURITY;

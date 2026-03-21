-- Create debts table
CREATE TABLE IF NOT EXISTS debts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES public.users(id) ON DELETE CASCADE,
    name TEXT NOT NULL,
    monthly_payment DECIMAL(10, 2) NOT NULL,
    current_installment INT NOT NULL,
    total_installments INT NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Create index on user_id for faster queries
CREATE INDEX IF NOT EXISTS idx_debts_user_id ON debts(user_id);

-- Enable Row Level Security
ALTER TABLE debts ENABLE ROW LEVEL SECURITY;

-- Create policy: Users can only see their own debts
CREATE POLICY "Users can view their own debts" ON debts
    FOR SELECT USING (auth.uid() = user_id);

-- Create policy: Users can insert their own debts
CREATE POLICY "Users can insert their own debts" ON debts
    FOR INSERT WITH CHECK (auth.uid() = user_id);

-- Create policy: Users can update their own debts
CREATE POLICY "Users can update their own debts" ON debts
    FOR UPDATE USING (auth.uid() = user_id);

-- Create policy: Users can delete their own debts
CREATE POLICY "Users can delete their own debts" ON debts
    FOR DELETE USING (auth.uid() = user_id);

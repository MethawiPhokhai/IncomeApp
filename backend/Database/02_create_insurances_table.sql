-- Create insurances table
CREATE TABLE IF NOT EXISTS insurances (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES public.users(id) ON DELETE CASCADE,
    provider TEXT NOT NULL,
    policy_name TEXT NOT NULL,
    premium DECIMAL(10, 2) NOT NULL,
    due_date TIMESTAMP WITH TIME ZONE NOT NULL,
    status TEXT NOT NULL CHECK (status IN ('Paid', 'Upcoming', 'Overdue')),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Create index on user_id for faster queries
CREATE INDEX IF NOT EXISTS idx_insurances_user_id ON insurances(user_id);

-- Enable Row Level Security
ALTER TABLE insurances ENABLE ROW LEVEL SECURITY;

-- Create policy: Users can only see their own insurances
CREATE POLICY "Users can view their own insurances" ON insurances
    FOR SELECT USING (auth.uid() = user_id);

-- Create policy: Users can insert their own insurances
CREATE POLICY "Users can insert their own insurances" ON insurances
    FOR INSERT WITH CHECK (auth.uid() = user_id);

-- Create policy: Users can update their own insurances
CREATE POLICY "Users can update their own insurances" ON insurances
    FOR UPDATE USING (auth.uid() = user_id);

-- Create policy: Users can delete their own insurances
CREATE POLICY "Users can delete their own insurances" ON insurances
    FOR DELETE USING (auth.uid() = user_id);

-- Remove computed columns from debts table
-- remaining_amount and total_amount are now calculated on the frontend:
--   total_amount     = monthly_payment * total_installments
--   remaining_amount = monthly_payment * (total_installments - current_installment)

ALTER TABLE debts
    DROP COLUMN IF EXISTS remaining_amount,
    DROP COLUMN IF EXISTS total_amount;

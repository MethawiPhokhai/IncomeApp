-- Insert sample debt data
-- Replace '20df0012-47cd-4c39-a28b-ac766fad676c' with your actual user_id from auth.users table

INSERT INTO debts (id, user_id, name, monthly_payment, current_installment, total_installments, remaining_amount, total_amount, created_at, updated_at) VALUES
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'Shokz Openrun', 340.00, 4, 10, 3000.00, 3400.00, NOW(), NOW()),
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'ค่าทนาย', 2000.00, 1, 3, 2040.00, 6000.00, NOW(), NOW());

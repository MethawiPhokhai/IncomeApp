-- Insert sample insurance data
-- Replace '20df0012-47cd-4c39-a28b-ac766fad676c' with your actual user_id from auth.users table

INSERT INTO insurances (id, user_id, provider, policy_name, premium, due_date, status, created_at, updated_at) VALUES
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'AIA', 'แผนชีวิต', 3500.00, NOW() + INTERVAL '25 days', 'Upcoming', NOW(), NOW()),
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'FWD', 'สุขภาพ', 2800.00, NOW() + INTERVAL '10 days', 'Upcoming', NOW(), NOW()),
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'Rabbit', 'อุบัติเหตุ', 450.00, NOW() + INTERVAL '5 days', 'Upcoming', NOW(), NOW()),
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'เมืองไทย', 'โรคร้ายแรง', 1200.00, NOW() + INTERVAL '18 days', 'Upcoming', NOW(), NOW());

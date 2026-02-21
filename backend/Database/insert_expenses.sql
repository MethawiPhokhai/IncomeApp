-- Insert sample expense data
-- Replace '20df0012-47cd-4c39-a28b-ac766fad676c' with your actual user_id from auth.users table

-- Fixed Expenses
INSERT INTO expenses (id, user_id, name, amount, type, color, bank_app, created_at, updated_at) VALUES
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'เงินเก็บ', 18000.00, 'Fixed', '#4F46E5', 'Dime', NOW(), NOW()),
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'ประกันเกษียร FWD 90/5', 2084.00, 'Fixed', '#4F46E5', 'Make', NOW(), NOW()),
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'ประกันอุบัติเหตุ เมืองไทย', 714.00, 'Fixed', '#4F46E5', 'Make', NOW(), NOW());

-- Variable Expenses
INSERT INTO expenses (id, user_id, name, amount, type, color, bank_app, created_at, updated_at) VALUES
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'ลงทุนต่างประเทศ', 15000.00, 'Variable', '#F59E0B', 'Dime', NOW(), NOW()),
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'ประกัน Rabbit', 5000.00, 'Variable', '#F59E0B', 'Make', NOW(), NOW()),
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'กองทุนฟรอย', 2000.00, 'Variable', '#F59E0B', 'Dime', NOW(), NOW()),
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'Condo (KTB)', 25000.00, 'Variable', '#F59E0B', 'KTB', NOW(), NOW()),
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'Condo ค่าส่วนกลาง', 1153.00, 'Variable', '#F59E0B', 'Make', NOW(), NOW()),
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'Condo ค่าไฟ-น้ำ', 1300.00, 'Variable', '#F59E0B', 'Kbank', NOW(), NOW()),
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'AIS มือถือ', 499.00, 'Variable', '#F59E0B', 'Kbank', NOW(), NOW()),
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'AIS Fiber', 641.00, 'Variable', '#F59E0B', 'Kbank', NOW(), NOW()),
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'Net 3BB', 749.00, 'Variable', '#F59E0B', 'Kbank', NOW(), NOW()),
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'กิน', 8500.00, 'Variable', '#F59E0B', 'Kbank', NOW(), NOW()),
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'หักภาษีรายเดือน', 5000.00, 'Variable', '#F59E0B', 'Office', NOW(), NOW()),
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'ประกันสังคม', 875.00, 'Variable', '#F59E0B', 'Office', NOW(), NOW()),
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'โอนให้แม่', 6000.00, 'Variable', '#F59E0B', 'Kbank', NOW(), NOW());

-- Health Expenses
INSERT INTO expenses (id, user_id, name, amount, type, color, bank_app, created_at, updated_at) VALUES
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'ค่ายาพ่อ', 2000.00, 'Health', '#10B981', 'Make', NOW(), NOW()),
(gen_random_uuid(), '20df0012-47cd-4c39-a28b-ac766fad676c', 'ประกันสุภาพ aliance', 2500.00, 'Health', '#10B981', 'Make', NOW(), NOW());

-- Chống import trùng
ALTER TABLE orders
ADD CONSTRAINT UQ_order UNIQUE (post_id, ad_id, created_at, revenue);

-- Tách settings khỏi hardcode
CREATE TABLE system_settings (
  [key] VARCHAR(100) PRIMARY KEY,
  [value] NVARCHAR(2000) NOT NULL
);

INSERT INTO system_settings([key], [value]) VALUES
('report_interval_minutes', '5'),
('lark_webhook', 'https://replace-me'),
('enable_worker', 'true');

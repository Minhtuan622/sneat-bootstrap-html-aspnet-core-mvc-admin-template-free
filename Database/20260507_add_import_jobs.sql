CREATE TABLE import_jobs (
  id BIGINT IDENTITY PRIMARY KEY,
  file_path NVARCHAR(1000) NOT NULL,
  original_file_name NVARCHAR(500) NOT NULL,
  status VARCHAR(30) NOT NULL,
  total_rows INT NOT NULL DEFAULT 0,
  processed_rows INT NOT NULL DEFAULT 0,
  success_count INT NOT NULL DEFAULT 0,
  failed_count INT NOT NULL DEFAULT 0,
  error_message NVARCHAR(MAX) NULL,
  created_by NVARCHAR(100) NOT NULL,
  created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  started_at DATETIME2 NULL,
  finished_at DATETIME2 NULL
);

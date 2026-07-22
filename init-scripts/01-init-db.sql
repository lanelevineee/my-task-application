-- Database initialization script for TaskManager
-- This runs automatically when PostgreSQL container starts for the first time

CREATE TABLE IF NOT EXISTS tasks (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(200) NOT NULL,
    description TEXT,
    status VARCHAR(50) NOT NULL DEFAULT 'Pending',
    priority VARCHAR(50) NOT NULL DEFAULT 'Medium',
    due_date TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_tasks_status ON tasks(status);
CREATE INDEX IF NOT EXISTS idx_tasks_due_date ON tasks(due_date);
CREATE INDEX IF NOT EXISTS idx_tasks_created_at ON tasks(created_at DESC);

-- Insert sample data
INSERT INTO tasks (title, description, status, priority, due_date) VALUES
    ('Learn CI/CD with GitHub Actions', 'Complete the GitHub Actions video course', 'InProgress', 'High', NOW() + INTERVAL '7 days'),
    ('Set up PostgreSQL with Docker', 'Configure PostgreSQL using docker-compose', 'Completed', 'High', NOW() - INTERVAL '1 day'),
    ('Build Task Manager API', 'Build a Task Manager API with ASP.NET Core, Dapper, and PostgreSQL', 'InProgress', 'High', NOW() + INTERVAL '3 days'),
    ('Set up GitHub Actions CI/CD', 'Configure CI/CD pipeline with GitHub Actions', 'Pending', 'Medium', NOW() + INTERVAL '14 days'),
    ('Deploy to cloud', 'Deploy the application to a cloud provider', 'Pending', 'Low', NOW() + INTERVAL '30 days')
ON CONFLICT DO NOTHING;
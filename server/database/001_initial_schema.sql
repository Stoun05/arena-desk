BEGIN;

CREATE TABLE IF NOT EXISTS users (
    id uuid PRIMARY KEY,
    username varchar(80) NOT NULL,
    password_hash varchar(255) NOT NULL,
    role varchar(24) NOT NULL CHECK (role IN ('Cashier', 'Administrator')),
    is_active boolean NOT NULL DEFAULT true,
    created_at_utc timestamptz NOT NULL,
    updated_at_utc timestamptz NOT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS ux_users_username ON users (username);

CREATE TABLE IF NOT EXISTS computers (
    id uuid PRIMARY KEY,
    code varchar(20) NOT NULL,
    display_name varchar(80) NOT NULL,
    tier varchar(16) NOT NULL CHECK (tier IN ('Standard', 'Vip')),
    status varchar(24) NOT NULL CHECK (status IN ('Available', 'Occupied', 'Ending', 'Locking', 'Offline')),
    end_action varchar(16) NOT NULL CHECK (end_action IN ('Logout', 'Sleep', 'Shutdown')),
    agent_id varchar(100),
    last_seen_at_utc timestamptz,
    created_at_utc timestamptz NOT NULL,
    updated_at_utc timestamptz NOT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS ux_computers_code ON computers (code);
CREATE UNIQUE INDEX IF NOT EXISTS ux_computers_agent_id ON computers (agent_id);
CREATE INDEX IF NOT EXISTS ix_computers_status ON computers (status);

CREATE TABLE IF NOT EXISTS tariffs (
    id uuid PRIMARY KEY,
    name varchar(80) NOT NULL,
    computer_tier varchar(16) NOT NULL CHECK (computer_tier IN ('Standard', 'Vip')),
    hourly_rate numeric(12,2) NOT NULL CONSTRAINT ck_tariffs_hourly_rate CHECK (hourly_rate > 0),
    currency char(3) NOT NULL,
    is_active boolean NOT NULL DEFAULT true,
    created_at_utc timestamptz NOT NULL,
    updated_at_utc timestamptz NOT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS ux_tariffs_name_tier ON tariffs (name, computer_tier);

CREATE TABLE IF NOT EXISTS sessions (
    id uuid PRIMARY KEY,
    computer_id uuid NOT NULL REFERENCES computers (id) ON DELETE RESTRICT,
    tariff_id uuid NOT NULL REFERENCES tariffs (id) ON DELETE RESTRICT,
    cashier_id uuid NOT NULL REFERENCES users (id) ON DELETE RESTRICT,
    customer_name varchar(120),
    started_at_utc timestamptz NOT NULL,
    ends_at_utc timestamptz NOT NULL,
    completed_at_utc timestamptz,
    initial_duration_minutes integer NOT NULL CONSTRAINT ck_sessions_initial_duration CHECK (initial_duration_minutes > 0),
    added_duration_minutes integer NOT NULL DEFAULT 0 CONSTRAINT ck_sessions_added_duration CHECK (added_duration_minutes >= 0),
    status varchar(24) NOT NULL CHECK (status IN ('Active', 'Ending', 'Completed', 'Cancelled')),
    hourly_rate_snapshot numeric(12,2) NOT NULL,
    initial_price numeric(12,2) NOT NULL,
    final_price numeric(12,2) NOT NULL,
    currency char(3) NOT NULL,
    created_at_utc timestamptz NOT NULL,
    updated_at_utc timestamptz NOT NULL,
    CONSTRAINT ck_sessions_end_after_start CHECK (ends_at_utc > started_at_utc)
);
CREATE UNIQUE INDEX IF NOT EXISTS ux_sessions_active_computer
    ON sessions (computer_id) WHERE status IN ('Active', 'Ending');
CREATE INDEX IF NOT EXISTS ix_sessions_started_at_utc ON sessions (started_at_utc);
CREATE INDEX IF NOT EXISTS ix_sessions_cashier_id ON sessions (cashier_id);

CREATE TABLE IF NOT EXISTS payments (
    id uuid PRIMARY KEY,
    session_id uuid NOT NULL REFERENCES sessions (id) ON DELETE RESTRICT,
    cashier_id uuid NOT NULL REFERENCES users (id) ON DELETE RESTRICT,
    amount numeric(12,2) NOT NULL CONSTRAINT ck_payments_amount CHECK (amount > 0),
    currency char(3) NOT NULL,
    method varchar(16) NOT NULL CHECK (method IN ('Cash', 'Card')),
    status varchar(16) NOT NULL CHECK (status IN ('Completed', 'Refunded')),
    refund_reason varchar(300),
    paid_at_utc timestamptz NOT NULL,
    refunded_at_utc timestamptz
);
CREATE INDEX IF NOT EXISTS ix_payments_session_id ON payments (session_id);
CREATE INDEX IF NOT EXISTS ix_payments_paid_at_utc ON payments (paid_at_utc);

CREATE TABLE IF NOT EXISTS audit_logs (
    id uuid PRIMARY KEY,
    user_id uuid REFERENCES users (id) ON DELETE SET NULL,
    action varchar(100) NOT NULL,
    entity_type varchar(100) NOT NULL,
    entity_id uuid,
    details_json jsonb,
    ip_address varchar(64),
    created_at_utc timestamptz NOT NULL
);
CREATE INDEX IF NOT EXISTS ix_audit_logs_created_at_utc ON audit_logs (created_at_utc);
CREATE INDEX IF NOT EXISTS ix_audit_logs_entity ON audit_logs (entity_type, entity_id);

INSERT INTO computers (id, code, display_name, tier, status, end_action, created_at_utc, updated_at_utc)
VALUES
    ('00000000-0000-0000-0000-000000000001', 'PC-01', 'PC-01', 'Standard', 'Available', 'Logout', '2026-08-04T00:00:00Z', '2026-08-04T00:00:00Z'),
    ('00000000-0000-0000-0000-000000000002', 'PC-02', 'PC-02', 'Standard', 'Available', 'Logout', '2026-08-04T00:00:00Z', '2026-08-04T00:00:00Z'),
    ('00000000-0000-0000-0000-000000000003', 'PC-03', 'PC-03', 'Standard', 'Available', 'Logout', '2026-08-04T00:00:00Z', '2026-08-04T00:00:00Z'),
    ('00000000-0000-0000-0000-000000000004', 'PC-04', 'PC-04', 'Standard', 'Available', 'Logout', '2026-08-04T00:00:00Z', '2026-08-04T00:00:00Z'),
    ('00000000-0000-0000-0000-000000000005', 'PC-05', 'PC-05', 'Standard', 'Available', 'Logout', '2026-08-04T00:00:00Z', '2026-08-04T00:00:00Z'),
    ('00000000-0000-0000-0000-000000000006', 'PC-06', 'PC-06', 'Standard', 'Available', 'Logout', '2026-08-04T00:00:00Z', '2026-08-04T00:00:00Z'),
    ('00000000-0000-0000-0000-000000000007', 'PC-07', 'PC-07', 'Vip', 'Available', 'Logout', '2026-08-04T00:00:00Z', '2026-08-04T00:00:00Z'),
    ('00000000-0000-0000-0000-000000000008', 'PC-08', 'PC-08', 'Vip', 'Available', 'Logout', '2026-08-04T00:00:00Z', '2026-08-04T00:00:00Z'),
    ('00000000-0000-0000-0000-000000000009', 'PC-09', 'PC-09', 'Vip', 'Available', 'Logout', '2026-08-04T00:00:00Z', '2026-08-04T00:00:00Z'),
    ('00000000-0000-0000-0000-000000000010', 'PC-10', 'PC-10', 'Vip', 'Available', 'Logout', '2026-08-04T00:00:00Z', '2026-08-04T00:00:00Z')
ON CONFLICT (id) DO NOTHING;

INSERT INTO tariffs (id, name, computer_tier, hourly_rate, currency, is_active, created_at_utc, updated_at_utc)
VALUES
    ('10000000-0000-0000-0000-000000000001', 'Standard', 'Standard', 15.00, 'TMT', true, '2026-08-04T00:00:00Z', '2026-08-04T00:00:00Z'),
    ('10000000-0000-0000-0000-000000000002', 'VIP', 'Vip', 20.00, 'TMT', true, '2026-08-04T00:00:00Z', '2026-08-04T00:00:00Z')
ON CONFLICT (id) DO NOTHING;

COMMIT;

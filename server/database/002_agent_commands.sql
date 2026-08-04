BEGIN;

CREATE TABLE IF NOT EXISTS agent_commands (
    id uuid PRIMARY KEY,
    computer_id uuid NOT NULL REFERENCES computers (id) ON DELETE RESTRICT,
    session_id uuid REFERENCES sessions (id) ON DELETE RESTRICT,
    type varchar(32) NOT NULL CHECK (type IN ('unlock', 'sync-session', 'logout', 'sleep', 'shutdown')),
    ends_at_utc timestamptz,
    end_action varchar(16),
    status varchar(20) NOT NULL CHECK (status IN ('Pending', 'Delivered', 'Acknowledged', 'Failed')),
    created_at_utc timestamptz NOT NULL,
    delivered_at_utc timestamptz,
    acknowledged_at_utc timestamptz,
    error varchar(500)
);
CREATE INDEX IF NOT EXISTS ix_agent_commands_delivery
    ON agent_commands (computer_id, status, created_at_utc);
CREATE INDEX IF NOT EXISTS ix_agent_commands_session_id
    ON agent_commands (session_id);

COMMIT;

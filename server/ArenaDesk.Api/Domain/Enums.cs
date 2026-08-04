namespace ArenaDesk.Api.Domain;

public enum UserRole
{
    Cashier,
    Administrator,
}

public enum ComputerTier
{
    Standard,
    Vip,
}

public enum ComputerStatus
{
    Available,
    Occupied,
    Ending,
    Locking,
    Offline,
}

public enum SessionStatus
{
    Active,
    Ending,
    Completed,
    Cancelled,
}

public enum PaymentMethod
{
    Cash,
    Card,
}

public enum PaymentStatus
{
    Completed,
    Refunded,
}

public enum SessionEndAction
{
    Logout,
    Sleep,
    Shutdown,
}

public enum AgentCommandStatus
{
    Pending,
    Delivered,
    Acknowledged,
    Failed,
}

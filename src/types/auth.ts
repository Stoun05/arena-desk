export type UserRole = "admin" | "cashier";

export type AuthUser = {
  id: string;
  username: string;
  role: UserRole;
};

export type AuthSession = {
  accessToken: string;
  expiresAtUtc: string;
  user: AuthUser;
};

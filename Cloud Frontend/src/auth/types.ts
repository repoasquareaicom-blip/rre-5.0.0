export type AuthUser = {
  id: number;
  name: string;
  username: string;
  email: string;
};

export type AuthSession = {
  token: string;
  user: AuthUser;
  remember: boolean;
  expires_at?: string | null;
};

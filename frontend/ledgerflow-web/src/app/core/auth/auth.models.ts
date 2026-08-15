
export interface AuthUser {
  id: string;
  email: string;
  displayName: string;
  roles: string[];
}

export interface LoginRequest {
  email: string;
  password: string;
  rememberMe: boolean;
}

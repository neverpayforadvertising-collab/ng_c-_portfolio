export const AppRoles = {
  admin: 'Admin',
  accountant: 'Accountant',
  viewer: 'Viewer'
} as const;

export type AppRole =
  typeof AppRoles[keyof typeof AppRoles];
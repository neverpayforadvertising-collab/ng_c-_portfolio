export interface Customer {
  id: string;

  companyName: string;
  contactName: string;
  email: string;

  phone?: string;

  city?: string;
  state?: string;

  country: string;

  isActive: boolean;

  createdAtUtc: string;
}
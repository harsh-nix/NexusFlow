export interface AppUser {
  id: number;
  fullName: string;
  email: string;
  role: string;
  phoneNumber: string | null;
  profilePictureUrl: string | null;
  isActive: boolean;
  createdAt: string;
}
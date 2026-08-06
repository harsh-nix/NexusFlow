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
export interface CreateUserRequest {
  fullName: string;
  email: string;
  password: string;
  role: string;
  phoneNumber?: string;
}

export interface UpdateUserRequest {
  fullName: string;
  role: string;
  phoneNumber?: string;
}
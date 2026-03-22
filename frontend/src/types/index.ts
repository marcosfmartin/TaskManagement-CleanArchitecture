export interface User {
  username: string;
}

export interface AuthResponse {
  message?: string;
  token?: string;
  username?: string;
}

export interface TaskItem {
  id: number;
  userId: number;
  title: string;
  description?: string;
  status: string;
  dueDate: string;
}

export interface CreateTaskDto {
  title: string;
  description?: string;
  dueDate: string;
}

export interface AuthDto {
  username: string;
  password?: string;
}
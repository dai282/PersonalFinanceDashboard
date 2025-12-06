
//now using auth0

// import api from './api';

// export interface RegisterData {
//   email: string;
//   password: string;
//   firstName: string;
//   lastName: string;
// }

// export interface LoginData {
//   email: string;
//   password: string;
// }

// export interface AuthResponse {
//   token: string;
//   email: string;
//   firstName: string;
//   lastName: string;
// }

// export const authService = {
//   register: async (data: RegisterData): Promise<AuthResponse> => {
//     const response = await api.post<AuthResponse>('/auth/register', data);
//     return response.data;
//   },

//   login: async (data: LoginData): Promise<AuthResponse> => {
//     const response = await api.post<AuthResponse>('/auth/login', data);
//     return response.data;
//   },

//   logout: () => {
//     localStorage.removeItem('token');
//     localStorage.removeItem('user');
//   },

//   getCurrentUser: (): AuthResponse | null => {
//     const userStr = localStorage.getItem('user');
//     return userStr ? JSON.parse(userStr) : null;
//   },

//   isAuthenticated: (): boolean => {
//     return !!localStorage.getItem('token');
//   },
// };
export {};
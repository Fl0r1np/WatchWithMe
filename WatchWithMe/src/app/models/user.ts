import { authMethod } from './auth-method';

export interface User {
  username: string;
  email: string;
  profilePicture: string;
  status: string;
  authMethod: authMethod;
}
import { authMethod } from './auth-method';
import { UserStatus } from './user-status';

export interface User {
  username: string;
  email: string;
  profilePicture: string;
  status: UserStatus;
  displayStatus: UserStatus;
  authMethod: authMethod;
  notifyBasic: boolean;
  notifyInvitations: boolean;
}
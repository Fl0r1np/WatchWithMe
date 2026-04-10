import { User } from "@app/models/user";
import { UserStatus } from "@app/models/user-status";

export class UserAccountUtils {

    // Converts the UserStatus enum value to a user-friendly string representation.
    public static convertDisplayStatusToString(status: UserStatus): string {

        if ( status === UserStatus.DoNotDisturb) {
            return "Do Not Disturb";
        }
        
        if ( status === UserStatus.InCall) {
            return "In Call";
        }

        if ( status === UserStatus.InRoom) {
            return "In Room";
        }

        return status.toString();

    } 

}
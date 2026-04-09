import { environment } from "@environments/environment"

// Variable containing the main endpoints info
const apiDomain = `${environment.apiDomain}/api`;
const userControllerURL = `${apiDomain}/user`;
const authControllerURL = `${apiDomain}/auth`;
const assetsControllerURL = `${apiDomain}/assets`;


export const ApiEndpoints = {

    // Endpoints for handling authentication
    login: `${authControllerURL}/login`,
    loginGoogle: `${authControllerURL}/login-google`,
    register: `${authControllerURL}/register`,
    logout: `${authControllerURL}/logout`,

    // Endpoints for handling user data
    getUserInfo: `${userControllerURL}/user-info`,
    updateUserUsername: `${userControllerURL}/update-username`,
    updateUserEmail: `${userControllerURL}/update-email`,
    updateUserProfilePicture: `${userControllerURL}/update-profile-picture`,
    updateUserPassword: `${userControllerURL}/update-password`,
    updateUserStatus: `${userControllerURL}/update-status`,
    updateUserDisplayStatus: `${userControllerURL}/update-display-status`,
    updateNotificationOptions: `${userControllerURL}/update-notification-options`,

    // Endpoints for handling assets
    getAvatarList: `${assetsControllerURL}/avatars`

}
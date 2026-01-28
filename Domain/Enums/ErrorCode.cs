namespace Domain.Enums
{
    public enum ErrorCode
    {
        BadRequest,
        ValidationError,
        //User 
        EmailalreadyExist = 101,
        EmailNotRegistered = 102,
        UserNotFound = 103,
        InvalidOtp = 104,
        RoleNotFound = 105,
        UserRoleNotFound = 106
    }
}

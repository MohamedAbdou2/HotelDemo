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
        //Room
        RoomNotFound = 201,
        RoomCreationFailed = 202,
        RoomUpdateFailed = 203,
        RoomDeletionFailed = 204,

        ServerError = 105,
        NotFound = 106
        RoleNotFound = 105,
        UserRoleNotFound = 106
    }
}

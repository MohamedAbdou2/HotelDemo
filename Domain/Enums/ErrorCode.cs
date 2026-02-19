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
        UserRoleNotFound = 106,
        ServerError = 107,
        NotFound = 108,
        FaildedToUpdateUser =109,
        InvalidCurrentPassword = 110,
        FailedToUpdateUserRole = 111,
        StaffRegisterFail = 112,
        UserAlreadyHaveThisRole = 113,
        CustomerNotFound = 114,

        //Room
        RoomNotFound = 201,
        RoomCreationFailed = 202,
        RoomUpdateFailed = 203,
        RoomDeletionFailed = 204,
        RoomNotAvailable = 205,


        //Offer 
        OfferNotFound = 301,
        OfferCreationFailed = 302,
        OfferUpdateFailed = 303,

        //Reservation 
        ReservationNotFound = 401,


        //Feedback
        FailedtoAddFeedback = 501,
        FeedbackAlreadyExist = 502,
        FeedBackDoesNotExist = 503,
        FailedToDeleteFeedback = 504,
        FailedToUpdateFeedback = 505



     
    }
}

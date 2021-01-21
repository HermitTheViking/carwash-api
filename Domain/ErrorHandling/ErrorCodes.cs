namespace Domain.ErrorHandling
{
    public static class ErrorCodes
    {
        public static readonly string UserNotFound = "UserNotFound";
        public static readonly string UserWithEmailNotFound = "UserWithEmailNotFound";
        public static readonly string WashIsOnGoing = "WashIsOnGoing";
        public static readonly string NoWashIsOnGoing = "NoWashIsOnGoing";
    }
}
using System;
using System.Globalization;
using System.Net;

namespace Domain.ErrorHandling
{
    public static class ExceptionFactory
    {
        public static Exception UserNotFoundException(string userId)
        {
            return new DomainException(
                        string.Format(CultureInfo.InvariantCulture, $"User with id:{userId} not found"),
                        ErrorCodes.UserNotFound,
                        HttpStatusCode.NotFound);
        }

        public static Exception UserWithEmailNotFoundException(string email)
        {
            return new DomainException(
                        string.Format(CultureInfo.InvariantCulture, $"User with email {email} not found"),
                        ErrorCodes.UserWithEmailNotFound,
                        HttpStatusCode.NotFound);
        }

        public static Exception WashIsOnGoingException()
        {
            return new DomainException(
                        string.Format(CultureInfo.InvariantCulture, $"There is already a wash on going"),
                        ErrorCodes.WashIsOnGoing,
                        HttpStatusCode.Conflict);
        }

        public static Exception NoWashIsOnGoingException()
        {
            return new DomainException(
                        string.Format(CultureInfo.InvariantCulture, $"There is no a wash on going"),
                        ErrorCodes.NoWashIsOnGoing,
                        HttpStatusCode.Conflict);
        }
    }
}
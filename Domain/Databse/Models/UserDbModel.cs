using Google.Cloud.Firestore;

namespace Domain.Databse.Models
{

    [FirestoreData]
    public class UserDbModel
    {
        public string Id { get; set; }

        [FirestoreProperty]
        public string Email { get; set; }
    }
}
using Google.Cloud.Firestore;
using System;

namespace Domain.Databse.Models
{
    [FirestoreData]
    public class TransactionDbModel
    {
        public int Id { get; set; }

        [FirestoreProperty]
        public DateTime Created { get; set; }

        [FirestoreProperty]
        public string EventType { get; set; }

        [FirestoreProperty]
        public string EventData { get; set; }
    }
}
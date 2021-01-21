using Google.Cloud.Firestore;
using System;

namespace Domain.Databse.Models
{
    [FirestoreData]
    public class WashDbModel
    {
        public string Id { get; set; }

        [FirestoreProperty]
        public string UserId { get; set; }

        [FirestoreProperty, ServerTimestamp]
        public DateTime StartTime { get; set; }

        [FirestoreProperty]
        public int Duration { get; set; }

        [FirestoreProperty]
        public int Type { get; set; }

        [FirestoreProperty]
        public bool Done { get; set; }
    }
}
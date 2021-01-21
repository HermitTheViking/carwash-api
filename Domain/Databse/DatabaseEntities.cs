using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Domain.Databse
{
    public class DatabaseEntities
    {
        private readonly IConfiguration _config;

        public DatabaseEntities(IConfiguration config)
        {
            _config = config;
            InitializeDb();
        }

        public string ProjectId { get; private set; }
        public FirestoreDb FirestoreDb { get; private set; }

        private async void InitializeDb()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", _config.GetSection("GOOGLE_APPLICATION_CREDENTIALS").Value);
            ProjectId = _config.GetSection("ProjectId").Value;
            FirestoreDb = FirestoreDb.Create(ProjectId);

            QuerySnapshot washSnapshot = await FirestoreDb.Collection("washes").GetSnapshotAsync();
            QuerySnapshot transactionSnapshot = await FirestoreDb.Collection("transactions").GetSnapshotAsync();

            if (washSnapshot.Count < 1)
            {
                await FirestoreDb.Collection("washes").AddAsync(new Dictionary<string, object>()
                {
                    { "UserId", string.Empty },
                    { "StartTime", DateTime.UtcNow },
                    { "Duration", 1 },
                    { "Type", 0 },
                    { "Done", true }
                });
            }

            if (transactionSnapshot.Count < 1)
            {
                await FirestoreDb.Collection("transactions").AddAsync(new Dictionary<string, object>()
                {
                    { "Created", DateTime.UtcNow },
                    { "EventType", string.Empty },
                    { "EventData", string.Empty }
                });
            }
        }
    }
}
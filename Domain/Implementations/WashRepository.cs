using Domain.Databse;
using Domain.Databse.Models;
using Domain.Security;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Implementations
{
    public class WashRepository : IWashRepository
    {
        private static readonly string _collection = "washes";
        private readonly DatabaseEntities _entities;
        private readonly ILogger<WashRepository> _logger;
        private readonly ICryptographic _crypto;
        private int _durationInSecounds;

        public WashRepository(
            DatabaseEntities entities,
            ICryptographic cryptographic,
            ILogger<WashRepository> logger)
        {
            _entities = entities ?? throw new ArgumentNullException();
            _crypto = cryptographic ?? throw new ArgumentNullException(nameof(cryptographic));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<WashDbModel>> GetAllAsync()
        {
            Query Query = _entities.FirestoreDb?.Collection(_collection);
            QuerySnapshot QuerySnapshot = await Query?.GetSnapshotAsync();
            var list = new List<WashDbModel>();

            try
            {
                foreach (DocumentSnapshot documentSnapshot in QuerySnapshot.Documents)
                {
                    if (documentSnapshot.Exists)
                    {
                        Dictionary<string, object> dictionary = documentSnapshot.ToDictionary();

                        list.Add(new WashDbModel()
                        {
                            Id = documentSnapshot.Id,
                            UserId = _crypto.Decrypt(dictionary["UserId"].ToString(), "J2uEDdYPYG4h996V"),
                            StartTime = Convert.ToDateTime(dictionary["StartTime"].ToString().Replace("Timestamp: ", "")),
                            Type = int.Parse(dictionary["Type"].ToString()),
                            Duration = int.Parse(dictionary["Duration"].ToString()),
                            Done = bool.Parse(dictionary["Done"].ToString())
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while prasing data from db");
            }

            return list.OrderBy(x => x.Type).ToList();
        }

        public async Task<List<WashDbModel>> GetAllByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) { return null; }

            userId = _crypto.Encrypt(userId, "J2uEDdYPYG4h996V");

            CollectionReference colRef = _entities.FirestoreDb?.Collection(_collection);
            Query Query = colRef?.WhereEqualTo("UserId", userId);
            QuerySnapshot QuerySnapshot = await Query.GetSnapshotAsync();
            var list = new List<WashDbModel>();

            foreach (DocumentSnapshot documentSnapshot in QuerySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    Dictionary<string, object> dictionary = documentSnapshot.ToDictionary();

                    list.Add(new WashDbModel()
                    {
                        Id = documentSnapshot.Id,
                        UserId = dictionary["UserId"].ToString(),
                        StartTime = Convert.ToDateTime(dictionary["StartTime"].ToString().Replace("Timestamp: ", "")),
                        Type = int.Parse(dictionary["Type"].ToString()),
                        Duration = int.Parse(dictionary["Duration"].ToString()),
                        Done = bool.Parse(dictionary["Done"].ToString())
                    });
                }
            }

            return list.OrderBy(x => x.Type).ToList();
        }

        public async Task<WashDbModel> GetRecentByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) { return null; }

            userId = _crypto.Encrypt(userId, "J2uEDdYPYG4h996V");

            CollectionReference colRef = _entities.FirestoreDb?.Collection(_collection);
            Query Query = colRef?.WhereEqualTo("UserId", userId);
            QuerySnapshot QuerySnapshot = await Query.GetSnapshotAsync();
            var list = new List<WashDbModel>();

            foreach (DocumentSnapshot documentSnapshot in QuerySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    Dictionary<string, object> dictionary = documentSnapshot.ToDictionary();

                    list.Add(new WashDbModel()
                    {
                        Id = documentSnapshot.Id,
                        UserId = userId,
                        StartTime = Convert.ToDateTime(dictionary["StartTime"].ToString().Replace("Timestamp: ", "")),
                        Type = int.Parse(dictionary["Type"].ToString()),
                        Duration = int.Parse(dictionary["Duration"].ToString()),
                        Done = bool.Parse(dictionary["Done"].ToString())
                    });
                }
            }

            return list.OrderBy(x => x.StartTime).LastOrDefault();
        }

        public async Task<WashDbModel> GetRecentByWashIdAsync(string washId)
        {
            if (string.IsNullOrEmpty(washId)) { return null; }

            DocumentReference docRef = _entities.FirestoreDb?.Collection(_collection)?.Document(washId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> dictionary = snapshot.ToDictionary();

                return new WashDbModel()
                {
                    Id = snapshot.Id,
                    UserId = _crypto.Decrypt(dictionary["UserId"].ToString(), "J2uEDdYPYG4h996V"),
                    StartTime = Convert.ToDateTime(dictionary["StartTime"].ToString().Replace("Timestamp: ", "")),
                    Type = int.Parse(dictionary["Type"].ToString()),
                    Duration = int.Parse(dictionary["Duration"].ToString()),
                    Done = bool.Parse(dictionary["Done"].ToString())
                };
            }

            return null;
        }

        public async Task<string> Add(WashDbModel washDbModel)
        {
            washDbModel.UserId = _crypto.Encrypt(washDbModel.UserId, "J2uEDdYPYG4h996V");

            DocumentReference docRef = await _entities.FirestoreDb?.Collection(_collection)?.AddAsync(washDbModel);
            return docRef.Id;
        }

        public async void Remove(WashDbModel washDbModel)
        {
            await _entities.FirestoreDb?.Collection(_collection)?.Document(washDbModel.Id.ToString()).DeleteAsync();
        }

        public async void Update(string washId, Dictionary<string, object> updates)
        {
            await _entities.FirestoreDb?.Collection(_collection)?.Document(washId)?.UpdateAsync(updates);
        }

        public bool IsWashOnGoingForUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) { return true; }

            WashDbModel dbModel = GetRecentByUserIdAsync(userId).Result;

            if (dbModel == null) { return true; } //TODO: Is this right

            return dbModel.Done;
        }

        public void StartWashThread(int duration, string threadName)
        {
            _durationInSecounds = duration * 60;
            Thread background = new Thread(Delay)
            {
                Name = threadName,
                IsBackground = true
            };
            background.Start();
        }

        private void Delay()
        {
            try
            {
                for (int i = 0; i < _durationInSecounds; i++)
                {
                    if (AbortThread(Thread.CurrentThread.Name)) { Thread.CurrentThread.Interrupt(); }
                    Console.WriteLine($"Running for { _durationInSecounds - i }. Name {Thread.CurrentThread.Name}");
                    Thread.Sleep(1000);
                }
            }
            catch (ThreadInterruptedException ex)
            {
                _logger.LogWarning($"Wash thread was interrupted. {ex.Message}");
            }
        }

        private bool AbortThread(string washId)
        {
            return GetRecentByWashIdAsync(washId[0..washId.IndexOf("-")]).Result.Done;
        }
    }
}
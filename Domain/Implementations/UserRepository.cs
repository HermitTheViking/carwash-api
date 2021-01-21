using Domain.Databse;
using Domain.Databse.Models;
using Domain.Security;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseEntities _entities;
        private static readonly string _collection = "users";
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(
            DatabaseEntities entities,
            ILogger<UserRepository> logger)
        {
            _entities = entities ?? throw new ArgumentNullException();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<UserDbModel>> GetAllAsync()
        {
            Query Query = _entities.FirestoreDb?.Collection(_collection);
            QuerySnapshot QuerySnapshot = await Query?.GetSnapshotAsync();
            var list = new List<UserDbModel>();

            foreach (DocumentSnapshot documentSnapshot in QuerySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    Dictionary<string, object> dictionary = documentSnapshot.ToDictionary();

                    list.Add(new UserDbModel()
                    {
                        Id = documentSnapshot.Id,
                        Email = dictionary["email"].ToString()
                    });
                }
            }

            return list.OrderBy(x => x.Email).ToList();
        }

        public async Task<UserDbModel> GetByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) { return null; }

            DocumentReference docRef = _entities.FirestoreDb?.Collection(_collection)?.Document(userId.ToString());
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> dictionary = snapshot.ToDictionary();
                return new UserDbModel()
                {
                    Id = snapshot.Id,
                    Email = dictionary["email"].ToString()
                };
            }

            return null;
        }

        public async Task<UserDbModel> GetByEmailAsync(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail)) { return null; }

            CollectionReference colRef = _entities.FirestoreDb?.Collection(_collection);
            Query query = colRef?.WhereEqualTo("email", userEmail);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    var dbModel = documentSnapshot.ConvertTo<UserDbModel>();
                    dbModel.Id = documentSnapshot.Id;
                    return dbModel;
                }
            }

            return null;
        }

        public async Task<string> Add(UserDbModel userDbModel)
        {
            DocumentReference docRef = await _entities.FirestoreDb?.Collection(_collection)?.AddAsync(userDbModel);
            return docRef.Id;
        }

        public async void Remove(UserDbModel userDbModel)
        {
            await _entities.FirestoreDb?.Collection(_collection)?.Document(userDbModel.Id.ToString()).DeleteAsync();
        }

        public async void Update(string userId, Dictionary<string, object> updates)
        {
            await _entities.FirestoreDb?.Collection(_collection)?.Document(userId)?.UpdateAsync(updates);
        }

        public bool IsEmailAlreadyInUse(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail)) { return false; }

            UserDbModel dbModel = GetByEmailAsync(userEmail).Result;
            return dbModel != null;
        }
    }
}
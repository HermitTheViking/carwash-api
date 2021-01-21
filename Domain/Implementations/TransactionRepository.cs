using Domain.Databse;
using Domain.Pagination;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Implementations
{
    public class TransactionRepository : BaseRepository, ITransactionRepository
    {
        private readonly DatabaseEntities _entities;
        private static readonly string _collection = "transactions";

        public TransactionRepository(DatabaseEntities entities)
        {
            _entities = entities ?? throw new ArgumentNullException();
        }

        public async Task<IEnumerable<Databse.Models.TransactionDbModel>> GetAllAsync()
        {
            Query Query = _entities.FirestoreDb?.Collection(_collection);
            QuerySnapshot QuerySnapshot = await Query?.GetSnapshotAsync();
            var list = new List<Databse.Models.TransactionDbModel>();

            foreach (DocumentSnapshot documentSnapshot in QuerySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    string json = JsonConvert.SerializeObject(documentSnapshot.ToDictionary());
                    list.Add(JsonConvert.DeserializeObject<Databse.Models.TransactionDbModel>(json));
                }
            }

            return list;
        }

        public async void Add(Databse.Models.TransactionDbModel transactionDbModel)
        {
            await _entities.FirestoreDb?.Collection(_collection)?.AddAsync(transactionDbModel);
        }

        public PaginationResult<Databse.Models.TransactionDbModel> GetByFilter(PaginationQuery paginationQuery)
        {
            return GetPaginatedResult(
                    paginationQuery,
                    GetAllAsync().Result.AsQueryable(),
                    (elements, filter) => elements.Where(x => x.EventType != null && x.EventType.Contains(paginationQuery.Filter)),
                    elements => elements.OrderBy(x => x.Created));
        }
    }
}
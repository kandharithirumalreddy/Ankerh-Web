using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Ankerh.Web.Models
{
    public static class DocumentDBRepository<T> where T : class
    {
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["database"];
        private static readonly string CollectionId = ConfigurationManager.AppSettings["collection"];
        private static DocumentClient client;

        public static async Task<T> GetItemAsync(string id)
        {
            try
            {
                Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public static async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }
        public static async Task<AnkerData> GetItemsAsync(AnkerhRequest request, int pageSize, string responseContinutationToken)
        {
            // Get a Database by querying for it by id
            Database db = client.CreateDatabaseQuery()
                                .Where(d => d.Id == DatabaseId)
                                .AsEnumerable()
                                .Single();

            // Use that Database's SelfLink to query for a DocumentCollection by id
            DocumentCollection coll = client.CreateDocumentCollectionQuery(db.SelfLink)
                                            .Where(c => c.Id == CollectionId)
                                            .AsEnumerable()
                                            .Single();

            StringBuilder sqlQuery =new StringBuilder ("SELECT c.id,c.BilagsNummer, c.Sagsnummer,c.SagsNavn,c.AfsnitTekst,c.AktivitetTekst,c.Kreditornummer,c.Kreditornavn,c.Filnavn,c.Fakturadato FROM c");
            StringBuilder filters = new StringBuilder();
            var parms = new SqlParameterCollection();
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                filters.Append(" c.Fakturadato between @startDate and @endDate");
                parms.Add(new SqlParameter("@startDate", request.StartDate));
                parms.Add(new SqlParameter("@endDate", request.EndDate.Value.AddDays(1)));
            }

            if (!string.IsNullOrEmpty(request.BilagsNummer))
            {
                filters = filters.Length > 0 ? filters.Append(" AND ") : filters;
                filters.Append(" (CONTAINS( c.BilagsNummer,@BilagsNummer))");
                parms.Add(new SqlParameter("@BilagsNummer", request.BilagsNummer));
            }

            if (!string.IsNullOrEmpty(request.Sagsnummer))
            {
                filters = filters.Length > 0 ? filters.Append(" AND ") : filters;
                filters.Append(" (CONTAINS( c.Sagsnummer,@Sagsnummer))");
                parms.Add(new SqlParameter("@Sagsnummer", request.Sagsnummer));
            }

            if (!string.IsNullOrEmpty(request.SagsNavn))
            {
                filters = filters.Length > 0 ? filters.Append(" AND ") : filters;
                filters.Append(" (CONTAINS( c.SagsNavn,@SagsNavn))");
                parms.Add(new SqlParameter("@SagsNavn", request.SagsNavn));
            }

            if (!string.IsNullOrEmpty(request.AfsnitTekst))
            {
                filters = filters.Length > 0 ? filters.Append(" AND ") : filters;
                filters.Append(" (CONTAINS( c.AfsnitTekst,@AfsnitTekst))");
                parms.Add(new SqlParameter("@AfsnitTekst", request.AfsnitTekst));
            }

            if (!string.IsNullOrEmpty(request.AktivitetTekst))
            {
                filters = filters.Length > 0 ? filters.Append(" AND ") : filters;
                filters.Append(" (CONTAINS( c.AktivitetTekst,@AktivitetTekst))");
                parms.Add(new SqlParameter("@AktivitetTekst", request.AktivitetTekst));
            }

            if (!string.IsNullOrEmpty(request.Kreditornummer))
            {
                filters = filters.Length > 0 ? filters.Append(" AND ") : filters;
                filters.Append(" (CONTAINS( c.Kreditornummer,@Kreditornummer))");
                parms.Add(new SqlParameter("@Kreditornummer", request.Kreditornummer));
            }

            if (!string.IsNullOrEmpty(request.Kreditornavn))
            {
                filters = filters.Length > 0 ? filters.Append(" AND ") : filters;
                filters.Append(" (CONTAINS( c.Kreditornavn,@Kreditornavn))");
                parms.Add(new SqlParameter("@Kreditornavn", request.Kreditornavn));
            }

            if (!string.IsNullOrEmpty(request.Filnavn))
            {
                filters = filters.Length > 0 ? filters.Append(" AND ") : filters;
                filters.Append(" (CONTAINS( c.Filnavn,@Filnavn))");
                parms.Add(new SqlParameter("@Filnavn", request.Filnavn));
            }
            
            if (filters.Length > 0)
            {
                sqlQuery.Append(" WHERE");
            }
            sqlQuery.Append(" {0}");
            filters.Append(" order by c.Fakturadato desc");
            var sqlquery = new SqlQuerySpec
            {
               
                QueryText = string.Format(sqlQuery.ToString(), filters.ToString()),
                Parameters = parms
            };
            var query = client
               .CreateDocumentQuery(coll.SelfLink, sqlquery, new FeedOptions { MaxItemCount = pageSize, RequestContinuation = responseContinutationToken })
               .AsDocumentQuery();
            List<AnkerDTO> results = new List<AnkerDTO>();
            if (query.HasMoreResults)
            {

                var data = new AnkerData();
                var result = await query.ExecuteNextAsync<AnkerDTO>();
                data.ContinutationToken = result.ResponseContinuation;
                data.Items.AddRange(result);
                return data;
            }

            return null;
        }
        public static async Task<AnkerData> GetItemsAsync(Expression<Func<T, bool>> predicate, int pageSize, string responseContinutationToken)
        {
            // Get a Database by querying for it by id
            Database db = client.CreateDatabaseQuery()
                                .Where(d => d.Id == DatabaseId)
                                .AsEnumerable()
                                .Single();

            // Use that Database's SelfLink to query for a DocumentCollection by id
            DocumentCollection coll = client.CreateDocumentCollectionQuery(db.SelfLink)
                                            .Where(c => c.Id == CollectionId)
                                            .AsEnumerable()
                                            .Single();

            var sql = "SELECT TOP 3 * FROM c";
            var d1 = new DateTime(2019, 02, 01);
            var d2 = new DateTime(2019, 02, 02);
            var sqlquery = new SqlQuerySpec
            {
                QueryText = "SELECT * FROM c  WHERE (c.Forfaldsdato between @d1 and @d2) order by c.Forfaldsdato desc",
                Parameters = new SqlParameterCollection()
            {
                          new SqlParameter("@d1",d1),
                           new SqlParameter("@d2", d2)
                    }
            };
            var query1 = client
               .CreateDocumentQuery(coll.SelfLink, sqlquery, new FeedOptions { MaxItemCount = pageSize, RequestContinuation = responseContinutationToken })
               .AsDocumentQuery();
            while (query1.HasMoreResults)
            {
                var documents = await query1.ExecuteNextAsync<AnkerDTO>();

                foreach (var document in documents)
                {
                    //Console.WriteLine(" PublicId: {0}; Magnitude: {1};", document.publicid,
                    //   document.magnitude);
                }
            }

            IDocumentQuery<T> query;
            if (predicate == null)
            {
                query = client.CreateDocumentQuery<T>(
               UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
               new FeedOptions { MaxItemCount = pageSize, RequestContinuation = responseContinutationToken })
               .AsDocumentQuery();
            }
            else
            {
                query = client.CreateDocumentQuery<T>(
              UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
              new FeedOptions { MaxItemCount = pageSize, RequestContinuation = responseContinutationToken })
              .Where(predicate)
              .AsDocumentQuery();
            }
            List<T> results = new List<T>();
            if (query.HasMoreResults)
            {

                var data = new AnkerData();
                var result = await query.ExecuteNextAsync<AnkerDTO>();
                data.ContinutationToken = result.ResponseContinuation;
                data.Items.AddRange(result);
                return data;
            }

            return null;
        }
        public static async Task<Document> CreateItemAsync(T item)
        {
            return await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item);
        }

        public static async Task<Document> UpdateItemAsync(string id, T item)
        {
            return await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), item);
        }

        public static async Task DeleteItemAsync(string id)
        {
            await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
        }

        public static void Initialize()
        {
            client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]), ConfigurationManager.AppSettings["authKey"], new ConnectionPolicy { EnableEndpointDiscovery = false });
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }

        private static async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        new DocumentCollection { Id = CollectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using PokerBot.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using PokerBot.Repository;
using System.Text.Json;

namespace PokerBot.Tests.Repository.Maven
{
    class MavenTestSetup
    {
        public static DbContextOptions<PokerDBContext> GetSqlLightContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<PokerDBContext>().UseSqlite(connection).Options;

            using (var context = new PokerDBContext(options))
            {
                context.Database.EnsureCreated();
            }

            using (var context = new PokerDBContext(options))
            {
                context.User.Add(new User()
                    {
                        SlackID = "slackid"
                    });
                context.SaveChanges();
            }
            return options;
        }
        public static async Task<PokerDBContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<PokerDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new PokerDBContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.User.CountAsync() <= 0)
            {
                databaseContext.User.Add(new User()
                {
                    SlackID = "slackid"
                });
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }
        public static HttpClient GetHttpClient(Object returnObject = null)
        {
            string returnObjJSON = "{ }";
            if (returnObject != null)
            {
                returnObjJSON = JsonSerializer.Serialize(returnObject);
            }
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = System.Net.HttpStatusCode.OK;
            httpResponse.Content = new StringContent(returnObjJSON, Encoding.UTF8, "application/json");

            Mock<HttpMessageHandler> mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            return new HttpClient(mockHandler.Object);
        }

        public static ISecrets GetSecrets()
        {
            return new Secrets();
        }

    }
}

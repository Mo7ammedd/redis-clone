using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using RedisClone.API;
using Xunit;

namespace RedisClone.IntegrationTests
{
    public class RedisControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public RedisControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task SetAndGet_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "test-key";
            var value = "test-value";

            // Act - Set
            var setResponse = await _client.PostAsync($"/api/redis/{key}",
                new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.OK, setResponse.StatusCode);

            // Act - Get
            var getResponse = await _client.GetAsync($"/api/redis/{key}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var result = await getResponse.Content.ReadAsStringAsync();
            Assert.Equal(value, result);
        }

        [Fact]
        public async Task Delete_ShouldRemoveKey()
        {
            // Arrange
            var key = "test-key";
            var value = "test-value";
            await _client.PostAsync($"/api/redis/{key}",
                new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json"));

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/redis/{key}");
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

            // Assert
            var getResponse = await _client.GetAsync($"/api/redis/{key}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task Expire_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "test-key";
            var value = "test-value";
            await _client.PostAsync($"/api/redis/{key}",
                new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json"));

            // Act
            var expireResponse = await _client.PostAsync($"/api/redis/{key}/expire?seconds=1", null);
            Assert.Equal(HttpStatusCode.OK, expireResponse.StatusCode);

            // Wait for expiration
            await Task.Delay(1100);

            // Assert
            var getResponse = await _client.GetAsync($"/api/redis/{key}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task ListOperations_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "test-list";
            var values = new[] { "value1", "value2", "value3" };

            // Act - Push values
            foreach (var value in values)
            {
                var pushResponse = await _client.PostAsync($"/api/redis/{key}/rpush",
                    new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json"));
                Assert.Equal(HttpStatusCode.OK, pushResponse.StatusCode);
            }

            // Act - Get range
            var rangeResponse = await _client.GetAsync($"/api/redis/{key}/lrange?start=0&stop=-1");
            Assert.Equal(HttpStatusCode.OK, rangeResponse.StatusCode);
            var result = await rangeResponse.Content.ReadAsStringAsync();
            var resultArray = JsonConvert.DeserializeObject<string[]>(result);

            // Assert
            Assert.Equal(values, resultArray);
        }

        [Fact]
        public async Task SetOperations_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "test-set";
            var value = "test-value";

            // Act - Add to set
            var addResponse = await _client.PostAsync($"/api/redis/{key}/sadd",
                new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.OK, addResponse.StatusCode);

            // Act - Check membership
            var isMemberResponse = await _client.GetAsync($"/api/redis/{key}/sismember?value={value}");
            Assert.Equal(HttpStatusCode.OK, isMemberResponse.StatusCode);
            var isMember = await isMemberResponse.Content.ReadAsStringAsync();
            Assert.Equal("true", isMember);

            // Act - Get members
            var membersResponse = await _client.GetAsync($"/api/redis/{key}/smembers");
            Assert.Equal(HttpStatusCode.OK, membersResponse.StatusCode);
            var members = await membersResponse.Content.ReadAsStringAsync();
            var membersArray = JsonConvert.DeserializeObject<string[]>(members);

            // Assert
            Assert.Single(membersArray);
            Assert.Contains(value, membersArray);
        }

        [Fact]
        public async Task HashOperations_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "test-hash";
            var field = "test-field";
            var value = "test-value";

            // Act - Set hash field
            var setResponse = await _client.PostAsync($"/api/redis/{key}/hset?field={field}",
                new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.OK, setResponse.StatusCode);

            // Act - Get hash field
            var getResponse = await _client.GetAsync($"/api/redis/{key}/hget?field={field}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var result = await getResponse.Content.ReadAsStringAsync();
            Assert.Equal(value, result);

            // Act - Get all fields
            var getAllResponse = await _client.GetAsync($"/api/redis/{key}/hgetall");
            Assert.Equal(HttpStatusCode.OK, getAllResponse.StatusCode);
            var allFields = await getAllResponse.Content.ReadAsStringAsync();
            var fieldsDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(allFields);

            // Assert
            Assert.Single(fieldsDict);
            Assert.Equal(value, fieldsDict[field]);
        }

        [Fact]
        public async Task SortedSetOperations_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "test-sorted-set";
            var member = "test-member";
            var score = 1.0;

            // Act - Add to sorted set
            var addResponse = await _client.PostAsync($"/api/redis/{key}/zadd?member={member}&score={score}", null);
            Assert.Equal(HttpStatusCode.OK, addResponse.StatusCode);

            // Act - Get range
            var rangeResponse = await _client.GetAsync($"/api/redis/{key}/zrange?start=0&stop=-1");
            Assert.Equal(HttpStatusCode.OK, rangeResponse.StatusCode);
            var result = await rangeResponse.Content.ReadAsStringAsync();
            var resultArray = JsonConvert.DeserializeObject<string[]>(result);

            // Assert
            Assert.Single(resultArray);
            Assert.Contains(member, resultArray);
        }

        [Fact]
        public async Task PubSubOperations_ShouldWorkCorrectly()
        {
            // Arrange
            var channel = "test-channel";
            var message = "test-message";

            // Act - Subscribe
            var subscribeResponse = await _client.PostAsync($"/api/redis/subscribe/{channel}", null);
            Assert.Equal(HttpStatusCode.OK, subscribeResponse.StatusCode);

            // Act - Publish
            var publishResponse = await _client.PostAsync($"/api/redis/publish/{channel}",
                new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.OK, publishResponse.StatusCode);
            var subscribers = await publishResponse.Content.ReadAsStringAsync();
            Assert.Equal("1", subscribers);

            // Act - Unsubscribe
            var unsubscribeResponse = await _client.PostAsync($"/api/redis/unsubscribe/{channel}", null);
            Assert.Equal(HttpStatusCode.OK, unsubscribeResponse.StatusCode);
        }
    }
} 
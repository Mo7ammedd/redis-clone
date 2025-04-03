using System;
using System.Threading.Tasks;
using RedisClone.Core;
using RedisClone.Infrastructure;
using Xunit;

namespace RedisClone.UnitTests
{
    public class RedisStoreTests
    {
        private readonly IRedisStore _redisStore;

        public RedisStoreTests()
        {
            _redisStore = new RedisStore();
        }

        [Fact]
        public async Task SetAndGet_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "test-key";
            var value = "test-value";

            // Act
            await _redisStore.SetAsync(key, value);
            var result = await _redisStore.GetAsync(key);

            // Assert
            Assert.Equal(value, result);
        }

        [Fact]
        public async Task Delete_ShouldRemoveKey()
        {
            // Arrange
            var key = "test-key";
            var value = "test-value";
            await _redisStore.SetAsync(key, value);

            // Act
            var result = await _redisStore.DeleteAsync(key);

            // Assert
            Assert.True(result);
            var deletedValue = await _redisStore.GetAsync(key);
            Assert.Null(deletedValue);
        }

        [Fact]
        public async Task Expire_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "test-key";
            var value = "test-value";
            await _redisStore.SetAsync(key, value);

            // Act
            var result = await _redisStore.SetExpirationAsync(key, TimeSpan.FromSeconds(1));

            // Assert
            Assert.True(result);
            var beforeExpiration = await _redisStore.GetAsync(key);
            Assert.Equal(value, beforeExpiration);

            // Wait for expiration
            await Task.Delay(1100);

            var afterExpiration = await _redisStore.GetAsync(key);
            Assert.Null(afterExpiration);
        }

        [Fact]
        public async Task ListOperations_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "test-list";
            var values = new[] { "value1", "value2", "value3" };

            // Act
            await _redisStore.RPushAsync(key, values);
            var result = await _redisStore.LIndexAsync(key, 0);

            // Assert
            Assert.Equal(values[0], result);
        }

        [Fact]
        public async Task SetOperations_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "test-set";
            var value = "test-value";

            // Act
            var addResult = await _redisStore.SAddAsync(key, value);
            var isMember = await _redisStore.SIsMemberAsync(key, value);
            var members = await _redisStore.SMembersAsync(key);

            // Assert
            Assert.Equal(1, addResult);
            Assert.True(isMember);
            Assert.Single(members);
            Assert.Contains(value, members);
        }

        [Fact]
        public async Task HashOperations_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "test-hash";
            var field = "test-field";
            var value = "test-value";

            // Act
            var setResult = await _redisStore.HSetAsync(key, field, value);
            var getResult = await _redisStore.HGetAsync(key, field);
            var allFields = await _redisStore.HGetAllAsync(key);

            // Assert
            Assert.True(setResult);
            Assert.Equal(value, getResult);
            Assert.Single(allFields);
            Assert.Equal(value, allFields[field]);
        }

        [Fact]
        public async Task SortedSetOperations_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "test-sorted-set";
            var member = "test-member";
            var score = 1.0;

            // Act
            var addResult = await _redisStore.ZAddAsync(key, (score, member));
            var range = await _redisStore.ZRangeAsync(key, 0, -1);
            var revRange = await _redisStore.ZRevRangeAsync(key, 0, -1);

            // Assert
            Assert.Equal(1, addResult);
            Assert.Single(range);
            Assert.Contains(member, range);
            Assert.Single(revRange);
            Assert.Contains(member, revRange);
        }

        [Fact]
        public async Task PubSubOperations_ShouldWorkCorrectly()
        {
            // Arrange
            var channel = "test-channel";
            var message = "test-message";
            var receivedMessage = false;

            // Act
            await _redisStore.SubscribeAsync(channel, async (msg) =>
            {
                if (msg == message)
                {
                    receivedMessage = true;
                }
            });

            var result = await _redisStore.PublishAsync(channel, message);

            // Wait for message processing
            await Task.Delay(100);

            // Assert
            Assert.True(result);
            Assert.True(receivedMessage);
        }

        [Fact]
        public async Task Get_ShouldReturnNull_WhenKeyDoesNotExist()
        {
            var result = await _redisStore.GetAsync("nonexistent");
            Assert.Null(result);
        }

        [Fact]
        public async Task Get_ShouldReturnNull_WhenKeyIsExpired()
        {
            await _redisStore.SetAsync("key", "value", TimeSpan.FromMilliseconds(100));
            await Task.Delay(150);
            var result = await _redisStore.GetAsync("key");
            Assert.Null(result);
        }

        [Fact]
        public async Task LPop_ShouldReturnNull_WhenListIsEmpty()
        {
            var result = await _redisStore.LPopAsync("key");
            Assert.Null(result);
        }

        [Fact]
        public async Task RPop_ShouldReturnNull_WhenListIsEmpty()
        {
            var result = await _redisStore.RPopAsync("key");
            Assert.Null(result);
        }

        [Fact]
        public async Task HGet_ShouldReturnNull_WhenFieldDoesNotExist()
        {
            var result = await _redisStore.HGetAsync("key", "field");
            Assert.Null(result);
        }
    }
} 
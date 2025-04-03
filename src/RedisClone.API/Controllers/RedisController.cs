using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedisClone.Core.Interfaces;

namespace RedisClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RedisController : ControllerBase
    {
        private readonly IRedisStore _redisStore;

        public RedisController(IRedisStore redisStore)
        {
            _redisStore = redisStore;
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key)
        {
            var value = await _redisStore.GetAsync(key);
            if (value == null)
                return NotFound();
            return Ok(value);
        }

        [HttpPost("{key}")]
        public async Task<IActionResult> Set(string key, [FromBody] string value, [FromQuery] int? expirationSeconds = null)
        {
            var expiration = expirationSeconds.HasValue ? TimeSpan.FromSeconds(expirationSeconds.Value) : (TimeSpan?)null;
            var result = await _redisStore.SetAsync(key, value, expiration);
            return result ? Ok() : BadRequest();
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete(string key)
        {
            var result = await _redisStore.DeleteAsync(key);
            return result ? Ok() : NotFound();
        }

        [HttpPost("{key}/expire")]
        public async Task<IActionResult> Expire(string key, [FromQuery] int seconds)
        {
            var result = await _redisStore.ExpireAsync(key, TimeSpan.FromSeconds(seconds));
            return result ? Ok() : NotFound();
        }

        [HttpPost("{key}/lpush")]
        public async Task<IActionResult> LPush(string key, [FromBody] string value)
        {
            var result = await _redisStore.LPushAsync(key, value);
            return Ok(result);
        }

        [HttpPost("{key}/rpush")]
        public async Task<IActionResult> RPush(string key, [FromBody] string value)
        {
            var result = await _redisStore.RPushAsync(key, value);
            return Ok(result);
        }

        [HttpPost("{key}/lpop")]
        public async Task<IActionResult> LPop(string key)
        {
            var value = await _redisStore.LPopAsync(key);
            if (value == null)
                return NotFound();
            return Ok(value);
        }

        [HttpPost("{key}/rpop")]
        public async Task<IActionResult> RPop(string key)
        {
            var value = await _redisStore.RPopAsync(key);
            if (value == null)
                return NotFound();
            return Ok(value);
        }

        [HttpGet("{key}/lrange")]
        public async Task<IActionResult> LRange(string key, [FromQuery] long start, [FromQuery] long stop)
        {
            var values = await _redisStore.LRangeAsync(key, start, stop);
            return Ok(values);
        }

        [HttpPost("{key}/sadd")]
        public async Task<IActionResult> SAdd(string key, [FromBody] string value)
        {
            var result = await _redisStore.SAddAsync(key, value);
            return Ok(result);
        }

        [HttpDelete("{key}/srem")]
        public async Task<IActionResult> SRem(string key, [FromBody] string value)
        {
            var result = await _redisStore.SRemAsync(key, value);
            return Ok(result);
        }

        [HttpGet("{key}/smembers")]
        public async Task<IActionResult> SMembers(string key)
        {
            var values = await _redisStore.SMembersAsync(key);
            return Ok(values);
        }

        [HttpGet("{key}/sismember")]
        public async Task<IActionResult> SIsMember(string key, [FromQuery] string value)
        {
            var result = await _redisStore.SIsMemberAsync(key, value);
            return Ok(result);
        }

        [HttpPost("{key}/hset")]
        public async Task<IActionResult> HSet(string key, [FromQuery] string field, [FromBody] string value)
        {
            var result = await _redisStore.HSetAsync(key, field, value);
            return result ? Ok() : BadRequest();
        }

        [HttpGet("{key}/hget")]
        public async Task<IActionResult> HGet(string key, [FromQuery] string field)
        {
            var value = await _redisStore.HGetAsync(key, field);
            if (value == null)
                return NotFound();
            return Ok(value);
        }

        [HttpGet("{key}/hgetall")]
        public async Task<IActionResult> HGetAll(string key)
        {
            var values = await _redisStore.HGetAllAsync(key);
            return Ok(values);
        }

        [HttpDelete("{key}/hdel")]
        public async Task<IActionResult> HDel(string key, [FromQuery] string field)
        {
            var result = await _redisStore.HDelAsync(key, field);
            return result ? Ok() : NotFound();
        }

        [HttpPost("{key}/zadd")]
        public async Task<IActionResult> ZAdd(string key, [FromQuery] string member, [FromQuery] double score)
        {
            var result = await _redisStore.ZAddAsync(key, member, score);
            return Ok(result);
        }

        [HttpDelete("{key}/zrem")]
        public async Task<IActionResult> ZRem(string key, [FromQuery] string member)
        {
            var result = await _redisStore.ZRemAsync(key, member);
            return Ok(result);
        }

        [HttpGet("{key}/zrange")]
        public async Task<IActionResult> ZRange(string key, [FromQuery] long start, [FromQuery] long stop, [FromQuery] bool withScores = false)
        {
            var values = await _redisStore.ZRangeAsync(key, start, stop, withScores);
            return Ok(values);
        }

        [HttpGet("{key}/zrevrange")]
        public async Task<IActionResult> ZRevRange(string key, [FromQuery] long start, [FromQuery] long stop, [FromQuery] bool withScores = false)
        {
            var values = await _redisStore.ZRevRangeAsync(key, start, stop, withScores);
            return Ok(values);
        }

        [HttpPost("publish/{channel}")]
        public async Task<IActionResult> Publish(string channel, [FromBody] string message)
        {
            var result = await _redisStore.PublishAsync(channel, message);
            return Ok(result);
        }

        [HttpPost("subscribe/{channel}")]
        public async Task<IActionResult> Subscribe(string channel)
        {
            await _redisStore.SubscribeAsync(channel, (ch, msg) =>
            {
                // Handle message in a background task
                Task.Run(() => Console.WriteLine($"Received message on channel {ch}: {msg}"));
            });
            return Ok();
        }

        [HttpPost("unsubscribe/{channel}")]
        public async Task<IActionResult> Unsubscribe(string channel)
        {
            await _redisStore.UnsubscribeAsync(channel);
            return Ok();
        }
    }
} 
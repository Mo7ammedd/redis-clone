# RedisClone

A lightweight, in-memory Database implemented in C# that provides a subset of Redis functionality with a similar API.

## Overview

RedisClone is a .NET implementation of a Redis-like key-value store that supports various data types and operations. It's designed to be used as a local cache or for testing purposes where a full Redis instance isn't required.

## Project Structure

```
RedisClone/
├── src/
│   ├── RedisClone.Core/           # Core interfaces and models
│   ├── RedisClone.Infrastructure/ # Implementation of Redis operations
│   └── RedisClone.API/           # REST API layer
└── tests/
    ├── RedisClone.UnitTests/     # Unit tests for individual components
    └── RedisClone.IntegrationTests/ # Integration tests for the entire system
```

## Features

### Data Types
- **Strings**: Basic key-value storage with support for bit operations
- **Lists**: Ordered collections of strings with operations on both ends
- **Sets**: Unordered collections of unique strings with set operations
- **Hashes**: Field-value pairs with atomic operations
- **Sorted Sets**: Ordered collections of unique strings with associated scores
- **Pub/Sub**: Publish/Subscribe messaging system
- **Transactions**: Support for atomic operations

### Key Operations
- `SetAsync`: Store a string value with optional expiration
- `GetAsync`: Retrieve a string value
- `DeleteAsync`: Remove a key
- `ExistsAsync`: Check if a key exists
- `TTLAsync`: Get time to live for a key
- `SetExpirationAsync`: Set expiration time for a key
- `RemoveExpirationAsync`: Remove expiration time from a key
- `IncrementAsync`/`DecrementAsync`: Atomic counter operations
- `AppendAsync`: Append to string value
- `GetRangeAsync`/`SetRangeAsync`: String manipulation
- `StrLenAsync`: Get string length
- Bit operations: `SetBitAsync`, `GetBitAsync`, `BitCountAsync`, `BitOpAsync`, `BitPosAsync`

### List Operations
- `LPushAsync`: Add elements to the head of a list
- `RPushAsync`: Add elements to the tail of a list
- `LPopAsync`: Remove and return the first element of a list
- `RPopAsync`: Remove and return the last element of a list
- `LRangeAsync`: Get a range of elements from a list
- `LIndexAsync`: Get an element from a list by index
- `LLenAsync`: Get the length of a list
- `LSetAsync`: Set element at index
- `LRemAsync`: Remove elements from list
- `LTrimAsync`: Trim list to specified range

### Set Operations
- `SAddAsync`: Add members to a set
- `SRemAsync`: Remove members from a set
- `SMembersAsync`: Get all members of a set
- `SIsMemberAsync`: Check if a value is a member of a set
- `SCardAsync`: Get set cardinality
- `SPopAsync`: Remove and return random member
- `SRandMemberAsync`: Get random member(s)
- Set operations: `SInterStoreAsync`, `SUnionStoreAsync`, `SDiffStoreAsync`

### Hash Operations
- `HSetAsync`: Set a hash field
- `HGetAsync`: Get a hash field
- `HGetAllAsync`: Get all fields and values in a hash
- `HDelAsync`: Delete hash field(s)
- `HExistsAsync`: Check if field exists
- `HIncrementByAsync`: Increment field value
- `HKeysAsync`: Get all hash fields
- `HLenAsync`: Get number of fields
- `HValsAsync`: Get all hash values

### Sorted Set Operations
- `ZAddAsync`: Add members to a sorted set
- `ZRemAsync`: Remove members from a sorted set
- `ZScoreAsync`: Get member score
- `ZIncrementByAsync`: Increment member score
- `ZCardAsync`: Get set cardinality
- `ZCountAsync`: Count members in score range
- `ZRangeAsync`/`ZRevRangeAsync`: Get range of members
- `ZRangeByScoreAsync`/`ZRevRangeByScoreAsync`: Get members by score range
- `ZRankAsync`/`ZRevRankAsync`: Get member rank
- `ZRemRangeByRankAsync`/`ZRemRangeByScoreAsync`: Remove members by rank/score
- Set operations: `ZUnionStoreAsync`, `ZInterStoreAsync`

### Pub/Sub Operations
- `PublishAsync`: Publish a message to a channel
- `SubscribeAsync`: Subscribe to a channel
- `UnsubscribeAsync`: Unsubscribe from a channel

### Transaction Support
- `BeginTransactionAsync`: Start a transaction
- `CommitTransactionAsync`: Commit the transaction
- `RollbackTransactionAsync`: Rollback the transaction

## Architecture

The project follows a clean architecture pattern with clear separation of concerns:

### Core Layer (`RedisClone.Core`)
- Contains interfaces and models
- Defines the contract for Redis operations
- Includes data models like `RedisValue` for storing different types of data
- Provides the public API that users interact with

### Infrastructure Layer (`RedisClone.Infrastructure`)
- Implements the core functionality
- Contains specialized operation classes:
  - `KeyOperations`: Basic key-value operations
  - `ListOperations`: List-specific operations
  - `SetOperations`: Set-specific operations
  - `HashOperations`: Hash-specific operations
  - `SortedSetOperations`: Sorted set-specific operations
- `RedisStore`: Main class that coordinates all operations
- Uses `ConcurrentDictionary` for thread-safe storage
- Implements proper locking mechanisms for atomic operations

### API Layer (`RedisClone.API`)
- Provides a REST API interface to the Redis functionality
- Handles HTTP requests and responses
- Maps API endpoints to Redis operations

## Thread Safety

RedisClone is designed to be thread-safe:
- Uses `ConcurrentDictionary` for the main store
- Implements proper locking mechanisms for operations
- Handles concurrent access to shared resources
- All operations are atomic where required
- Uses async/await for non-blocking operations

## Testing

The project includes comprehensive testing:

### Unit Tests
- Tests individual components in isolation
- Covers all data types and operations
- Tests edge cases and error conditions
- Verifies thread safety
- Tests transaction behavior

### Integration Tests
- Tests the entire system working together
- Verifies API endpoints
- Tests real-world scenarios
- Ensures proper interaction between components

## Usage

### Basic Example

```csharp
using RedisClone.Core;
using RedisClone.Infrastructure;

// Create a new Redis store
IRedisStore redis = new RedisStore();

// Store a value
await redis.SetAsync("mykey", "myvalue");

// Retrieve the value
string? value = await redis.GetAsync("mykey");
```

### List Operations Example

```csharp
// Add items to a list
await redis.RPushAsync("mylist", "item1", "item2", "item3");

// Get items from the list
var items = await redis.LRangeAsync("mylist", 0, -1);
```

### Hash Operations Example

```csharp
// Set hash fields
await redis.HSetAsync("myhash", "field1", "value1");
await redis.HSetAsync("myhash", "field2", "value2");

// Get all hash fields and values
var hash = await redis.HGetAllAsync("myhash");
```

### Sorted Set Example

```csharp
// Add items to a sorted set
await redis.ZAddAsync("myset", ("member1", 1.0), ("member2", 2.0));

// Get items in score range
var items = await redis.ZRangeByScoreAsync("myset", 0, 2);
```

### Pub/Sub Example

```csharp
// Subscribe to a channel
await redis.SubscribeAsync("mychannel", async (message) => {
    Console.WriteLine($"Received: {message}");
});

// Publish a message
await redis.PublishAsync("mychannel", "Hello, Redis!");
```

## Performance Considerations

- All operations are designed to be efficient
- Uses in-memory storage for fast access
- Implements proper locking to minimize contention
- Supports atomic operations for consistency
- Uses async/await for non-blocking operations

## Limitations

- Data is not persisted (in-memory only)
- No replication support
- Limited subset of Redis commands
- No authentication/authorization
- No clustering support

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details. 
# RedisClone

A lightweight, in-memory Database implemented in C# that provides a subset of Redis functionality with a similar API.

## Overview

RedisClone is a .NET implementation of a Redis-like key-value store that supports various data types and operations. It's designed to be used as a local cache or for testing purposes where a full Redis instance isn't required.

## Features

### Data Types
- **Strings**: Basic key-value storage
- **Lists**: Ordered collections of strings
- **Sets**: Unordered collections of unique strings
- **Hashes**: Field-value pairs
- **Sorted Sets**: Ordered collections of unique strings with associated scores

### Key Operations
- `SetAsync`: Store a string value
- `GetAsync`: Retrieve a string value
- `DeleteAsync`: Remove a key
- `ExistsAsync`: Check if a key exists
- `TTLAsync`: Get time to live for a key
- `SetExpirationAsync`: Set expiration time for a key
- `RemoveExpirationAsync`: Remove expiration time from a key

### List Operations
- `LPushAsync`: Add elements to the head of a list
- `RPushAsync`: Add elements to the tail of a list
- `LPopAsync`: Remove and return the first element of a list
- `RPopAsync`: Remove and return the last element of a list
- `LRangeAsync`: Get a range of elements from a list
- `LIndexAsync`: Get an element from a list by index
- `LLenAsync`: Get the length of a list

### Set Operations
- `SAddAsync`: Add members to a set
- `SRemAsync`: Remove members from a set
- `SMembersAsync`: Get all members of a set
- `SIsMemberAsync`: Check if a value is a member of a set

### Hash Operations
- `HSetAsync`: Set a hash field
- `HGetAsync`: Get a hash field
- `HGetAllAsync`: Get all fields and values in a hash
- `HDelAsync`: Delete a hash field

### Sorted Set Operations
- `ZAddAsync`: Add members to a sorted set
- `ZRemAsync`: Remove members from a sorted set
- `ZRangeAsync`: Get a range of members from a sorted set
- `ZRevRangeAsync`: Get a range of members from a sorted set in reverse order

### Pub/Sub Operations
- `PublishAsync`: Publish a message to a channel
- `SubscribeAsync`: Subscribe to a channel
- `UnsubscribeAsync`: Unsubscribe from a channel

## Architecture

The project is structured into several layers:

### Core Layer (`RedisClone.Core`)
- Contains interfaces and models
- Defines the contract for Redis operations
- Includes data models like `RedisValue` for storing different types of data

### Infrastructure Layer (`RedisClone.Infrastructure`)
- Implements the core functionality
- Contains specialized operation classes:
  - `KeyOperations`: Basic key-value operations
  - `ListOperations`: List-specific operations
  - `SetOperations`: Set-specific operations
  - `HashOperations`: Hash-specific operations
  - `SortedSetOperations`: Sorted set-specific operations
- `RedisStore`: Main class that coordinates all operations

## Thread Safety

RedisClone is designed to be thread-safe:
- Uses `ConcurrentDictionary` for the main store
- Implements proper locking mechanisms for operations
- Handles concurrent access to shared resources

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

// Set with expiration
await redis.SetAsync("tempkey", "tempvalue", TimeSpan.FromMinutes(5));
```

### List Operations

```csharp
// Add items to a list
await redis.RPushAsync("mylist", "item1", "item2", "item3");

// Get list length
var length = await redis.LLenAsync("mylist");

// Get item at index
var item = await redis.LIndexAsync("mylist", 1);
```

### Set Operations

```csharp
// Add members to a set
await redis.SAddAsync("myset", "member1", "member2");

// Check membership
bool isMember = await redis.SIsMemberAsync("myset", "member1");

// Get all members
var members = await redis.SMembersAsync("myset");
```

## Testing

The project includes comprehensive unit tests covering:
- Basic key-value operations
- List operations
- Set operations
- Hash operations
- Sorted set operations
- Pub/Sub functionality
- Expiration handling

Run tests using:
```bash
dotnet test
```

## Limitations

- In-memory storage only (no persistence)
- Single-instance only (no clustering)
- Subset of Redis commands implemented
- No support for Redis-specific features like:
  - Lua scripting
  - Streams
  - Geo commands
  - HyperLogLog
  - Bit operations

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details. 
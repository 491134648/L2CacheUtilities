
L2CacheUtilities is an open source caching library that contains basic usages and some advanced usages of caching which can help us to handle caching more easier!



## Nuget Packages

| Package Name |  Version | Downloads
|--------------|  ------- | ----
| FH.Cache.Core | ![](https://img.shields.io/nuget/v/FH.Cache.Core.svg) | ![](https://img.shields.io/nuget/dt/FH.Cache.Core.svg)
| FH.Cache.Redis | ![](https://img.shields.io/nuget/v/FH.Cache.Redis.svg) | ![](https://img.shields.io/nuget/dt/FH.Cache.Redis.svg)
| FH.Cache.Serialization.MessagePack | ![](https://img.shields.io/nuget/v/FH.Cache.Serialization.MessagePack.svg) | ![](https://img.shields.io/nuget/dt/FH.Cache.Serialization.MessagePack.svg)
| FH.Cache.Serialization.Json | ![](https://img.shields.io/nuget/v/FH.Cache.Serialization.Json.svg) | ![](https://img.shields.io/nuget/dt/FH.Cache.Serialization.Json.svg)
| FH.Cache.Serialization.Protobuf | ![](https://img.shields.io/nuget/v/FH.Cache.Serialization.Protobuf.svg) | ![](https://img.shields.io/nuget/dt/FH.Cache.Serialization.Protobuf.svg)

## Basic Usages 

### Step 1 : Install the package

Choose caching provider that you need and install it via Nuget.

```
Install-Package FH.Cache.Core
Install-Package FH.Cache.Redis
Install-Package FH.Cache.Serialization.MessagePack
Install-Package FH.Cache.Serialization.Json
Install-Package FH.Cache.Serialization.Protobuf
```
OR
```
<PackageReference Include="FH.Cache.Core" Version="0.0.1" />
<PackageReference Include="FH.Cache.Redis" Version="0.0.1" />
<PackageReference Include="FH.Cache.Serialization.MessagePack" Version="0.0.1" />
<PackageReference Include="FH.Cache.Serialization.Json" Version="0.0.1" />
<PackageReference Include="FH.Cache.Serialization.Protobuf" Version="0.0.1" />
```

### Step 2 : Configure appsettings.json

```json

  "caching": {
    "inmemory": {
      "MaxRdSecond": 120,
      "EnableLogging": false,
      "LockMs": 5000,
      "SleepMs": 300,
      "DBConfig": {
        "SizeLimit": 10000,
        "ExpirationScanFrequency": 60
      }
    },
    "redis": {
      "MaxRdSecond": 120,
      "EnableLogging": false,
      "LockMs": 5000,
      "SleepMs": 300,
      "dbconfig": {
        "Password": "",
        "IsSsl": false,
        "SslHost": null,
        "ConnectionTimeout": 5000,
        "AllowAdmin": false,
        "AbortOnConnectFail": false,
        "Endpoints": [
          {
            "Host": "127.0.0.1",
            "Port": 6379
          }
        ],
        "Database": 3
      }
    }
  }
  ```

### Step 3 : Configure Startup class

L2CacheUtilities provider has it's own configuration options.

Here is a sample configuration for InMemory and Redis caching provider.

```csharp
public class Startup
{
    //...
    
    public void ConfigureServices(IServiceCollection services)
    {
        //configuration
       services.AddMemoryServices(Configuration)
            .AddRedisServices(Configuration)
            .AddRedisL2Services()
            .AddMessagePackServices();   
    }    
}
```

###  Step 4 : Write code in your controller 

```csharp
public class HomeController : Controller
    {
        private readonly ICachingProvider _memoryProvider;
        private readonly ICachingProvider _redisProvider;
        private readonly IL2CacheProvider _l2CacheProvider;
        private readonly ICachingProviderFactory _providerFactory;
        public HomeController(ICachingProviderFactory providerFactory,
            IL2CacheProvider l2CacheProvider)
        {
            _providerFactory = providerFactory;
            _l2CacheProvider = l2CacheProvider;
            _memoryProvider = _providerFactory.GetCachingProvider(CachingConstValue.DefaultInMemoryName);
            _redisProvider = _providerFactory.GetCachingProvider(CachingConstValue.DefaultRedisName);
        }
        #region Memory Test
        public IActionResult Memory()
        {
            DefaultInMemoryCachingProvider cache = (DefaultInMemoryCachingProvider)_memoryProvider;
            return Json(cache.Keys);
        }
        public IActionResult MemoryGet(string key)
        {
            var value = _memoryProvider.Get<string>(key);

            return Json(value);
        }
        public IActionResult MemoryAdd(string key, string value)
        {
            _memoryProvider.Set(key, value, TimeSpan.FromSeconds(60));
            return Content("Success");
        }
        public IActionResult MemoryUpdate(string key, string value)
        {
            _memoryProvider.Set(key, value, TimeSpan.FromSeconds(60));
            return Content("Success");
        }
        public IActionResult MemoryDelete(string key)
        {
            _memoryProvider.Remove(key);
            return Content("Success");
        }
        #endregion
        #region Redis Test

        public IActionResult RedisGet(string key)
        {
            var value = _redisProvider.Get<string>(key);

            return Json(value);
        }
        public IActionResult RedisAdd(string key, string value)
        {
            _redisProvider.Set(key, value, TimeSpan.FromSeconds(60));
            return Content("Success");
        }
        public IActionResult RedisUpdate(string key, string value)
        {
            _redisProvider.Set(key, value, TimeSpan.FromSeconds(60));
            return Content("Success");
        }
        public IActionResult RedisDelete(string key)
        {
            _redisProvider.Remove(key);
            return Content("Success");
        }
        #endregion
        #region L2 Test

        public IActionResult L2Get(string key)
        {
            var value = _l2CacheProvider.Get<string>(key);

            return Json(value);
        }
        public IActionResult L2Add(string key, string value)
        {
            _l2CacheProvider.Set(key, value, TimeSpan.FromSeconds(60));
            return Content("Success");
        }
        public IActionResult L2Update(string key, string value)
        {
            _l2CacheProvider.Set(key, value, TimeSpan.FromSeconds(60));
            return Content("Success");
        }
        public IActionResult L2Delete(string key)
        {
            _l2CacheProvider.Remove(key);
            return Content("Success");
        }
        #endregion
    }
```

## Examples

See [sample](https://github.com/491134648/L2CacheUtilities/tree/master/sample)

## Todo List

See [ToDo List]
Dashboard app

## Contributing

Pull requests, issues and commentary! 


L2CacheUtilities is an open source caching library that contains basic usages and some advanced usages of caching which can help us to handle caching more easier!




## Basic Usages 

### Step 1 : Configure appsettings.json

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

### Step 2 : Configure Startup class

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

###  Step 3 : Write code in your controller 

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
        #region ƒ⁄¥Ê≤‚ ‘
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
        #region Redis≤‚ ‘

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
        #region L2≤‚ ‘

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

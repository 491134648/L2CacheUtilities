using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FH.WebApp.Models;
using FH.Cache.Core.InMemory;
using FH.Cache.Core;

namespace FH.WebApp.Controllers
{
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
        #region 内存测试
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
        #region Redis测试

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
        #region L2测试

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
}

using booking_system.Models;
using booking_system.Redis;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;



namespace booking_system.Controllers
{
    [Controller]
    public abstract class BaseController : ControllerBase
    {

        private readonly RedisService _redisService;
        protected IDatabase RedisReadDb { get; }
        protected IDatabase RedisWriteDb { get; }

        public BaseController(RedisService redisService)
        {
            var dbIndex = int.Parse(Environment.GetEnvironmentVariable("REDIS_DB_INDEX")!);
            _redisService = redisService;
            RedisReadDb = _redisService.GetRedisReadDatabase(dbIndex);
            RedisWriteDb = _redisService.GetRedisWriteDatabase(dbIndex);
        }

        protected async Task<bool> SetRedisAsync(string key, object value, TimeSpan expiry)
        {
            try
            {
                SentrySdk.AddBreadcrumb("SetRedisAsync.", level: BreadcrumbLevel.Info);
                string jsonValue = JsonConvert.SerializeObject(value);
                return await RedisWriteDb.StringSetAsync(key, jsonValue, expiry);
            }
            catch (Exception ex)
            {
                SentrySdk.AddBreadcrumb("Fail SetRedisAsync", level: BreadcrumbLevel.Error);
                SentrySdk.CaptureException(ex);
                return false;
            }
        }

        protected async Task<object?> GetRedisAsync(string key)
        {
            try
            {
                SentrySdk.AddBreadcrumb("GetRedisAsync.", level: BreadcrumbLevel.Info);
                // return await RedisReadDb.StringGetAsync(key);
                string? jsonValue = await RedisReadDb.StringGetAsync(key);
                if (jsonValue != null)
                {
                    return JsonConvert.DeserializeObject(jsonValue)!;
                }
                else
                {
                    return null;
                }
            }

            catch (Exception ex)
            {
                SentrySdk.AddBreadcrumb("Fail GetRedisAsync", level: BreadcrumbLevel.Error);
                SentrySdk.CaptureException(ex);
                return false;
            }
        }
        protected async Task<bool> DeleteRedisAsync(string key)
        {
            try
            {
                SentrySdk.AddBreadcrumb("DeleteRedisAsync.", level: BreadcrumbLevel.Info);
                var result = await RedisWriteDb.KeyDeleteAsync(key);
                return result;
                // Check if the key exists


            }
            catch (Exception ex)
            {
                SentrySdk.AddBreadcrumb("Fail DeleteRedisAsync", level: BreadcrumbLevel.Error);
                SentrySdk.CaptureException(ex);
                return false;
            }
        }


        protected async Task<bool> StoreSubscriptionRedisAsync(string UserId, string UserCreditHistory, string Transaction, string Package)
        {

            try
            {
                JObject obj = JObject.Parse(Transaction);

                _ = DateTime.TryParse((string)obj["ExpiredAt"]!, out DateTime ExpiredAt);

                string key = $"{UserId}_booking";
                string value = Transaction;
                TimeSpan expiry = ExpiredAt.ToUniversalTime() - DateTime.UtcNow;
                return await SetRedisAsync(key, value, expiry);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        protected async Task<bool> StoreSubscriptionRedisAsync(Guid UserId, UserCreditHistory UserCreditHistory, Transaction Transaction, Package Package)
        {
            dynamic subResponse = new
            {
                expiry = Transaction.ExpiredAt,
                amount = Package.Price,
                currency = Package.Currency,
                platform = Transaction!.Platform,
                planId = Package.PlanId,
            };

            try
            {

                string key = $"{UserId}_booking";
                // string value = JsonConvert.SerializeObject(subResponse, settings);
                TimeSpan expiry = Transaction.ExpiredAt!.Value.ToUniversalTime() - DateTime.UtcNow;
                SentrySdk.AddBreadcrumb("Successful Store Subscription RedisAsync", level: BreadcrumbLevel.Info);
                return await SetRedisAsync(key, subResponse, expiry);
            }
            catch (Exception ex)
            {
                SentrySdk.AddBreadcrumb("Fail Store Subscription RedisAsync", level: BreadcrumbLevel.Error);
                SentrySdk.CaptureException(ex);
                throw;
            }

        }
    }
}
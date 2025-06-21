
using booking_system.Controllers;
using booking_system.Data;
using booking_system.Models;
using booking_system.Redis;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Sentry;
using Sentry.Protocol;

using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using WavePayRequestModel;
using WaveUtility;
using static Google.Apis.Requests.BatchRequest;


namespace booking_system.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WavePayController : BaseController
    {
        private ILogger<WavePayController> _logger;
        private DataContext _db;

        public WavePayController(DataContext dbContext, ILogger<WavePayController> logger, RedisService redisService) : base(redisService)

        {
            _logger = logger;
            _db = dbContext;
        }


        [Route("CreateTransaction")]
        [HttpPost]
        public async Task<IActionResult> WavePayment([FromBody] WavePayRequest request)
        {
            if (!ModelState.IsValid)
            {
                SentrySdk.AddBreadcrumb("Invalid request.", level: BreadcrumbLevel.Warning);
                SentrySdk.CaptureMessage("Invalid request.");
                return BadRequest("Invalid request.");
            }

            var package = await _db.Packages.FirstOrDefaultAsync(sp => sp.PlanId == request.planId && sp.IsActive == true);

            if (package == null)
            {
                return BadRequest("Subscription Plan not found");
            }

            var getPlatform = await _db.PaymentGateways.FirstOrDefaultAsync(p => p.Id == package.GatewayId);

            if (getPlatform == null)
            {
                return BadRequest("Payment Gateway not found");
            }

            string platform = getPlatform.Platform.ToString();
            string backend_result_url = Environment.GetEnvironmentVariable("BACKEND_RESULT_URL")!;
            string frontend_result_url = Environment.GetEnvironmentVariable("FRONTEND_RESULT_URL")!;
            int amount = (int)package.Price;
            string currency = package.Currency!;
            int time_to_live_in_seconds = 600;
            string title = "Subscription";
            string payment_description = "Test UAT Wave Payment";
            string merchant_name = "booking";
            var itemsArray = new[] { new { name = title, amount = amount } };
            string items = JsonSerializer.Serialize(itemsArray);
            var secret = Environment.GetEnvironmentVariable("WAVE_SECRECTKEY")!;
            string merchant_id = Environment.GetEnvironmentVariable("WAVE_MERCHANTID")!;
            Guid merchant_reference = Guid.NewGuid();

            string merchant_reference_id = $"{merchant_reference}=su";

            GatewayRawEvent gate = new()
            {
                GateWayOrderId = merchant_reference_id,
                EventType = platform,
            };
            _db.GatewayRawEvents.Add(gate);

            Transaction transaction = new()
            {
                CreatedAt = DateTime.UtcNow,
                Platform = platform,
                Amount = package.Price,
                Currency = package.Currency,
                Status = TransactionStatus.pending,
                GateWayOrderId = merchant_reference_id,
                GateWayRawEventId = gate.Id,
                UserId = request.userId,
                PackageId = package.Id,
                RequestedPlan = package.PlanId
            };
            _db.Transactions.Add(transaction);

            await _db.SaveChangesAsync();

            string order_id = transaction.Id.ToString();

            string message1 = string.Concat(
                time_to_live_in_seconds,
                merchant_id,
                order_id,
                amount,
                backend_result_url,
                merchant_reference_id
            );

            string hash = HashHMAC(message1, secret);

            var requestData = new
            {
                merchant_id,
                order_id,
                merchant_reference_id,
                frontend_result_url,
                backend_result_url,
                amount = Convert.ToInt64(amount),
                time_to_live_in_seconds,
                payment_description,
                currency,
                hash,
                merchant_name,
                items
            };

            gate.RequestPayload = JsonSerializer.Serialize(requestData);
            await _db.SaveChangesAsync();

            string jsonBody = JsonSerializer.Serialize(requestData);

            using HttpClient client = new();
            var waveEndpointUrl = Environment.GetEnvironmentVariable("WAVEPAY_URL")!;
            HttpContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.PostAsync(waveEndpointUrl, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            gate.TranResponsePayload = JsonSerializer.Serialize(responseContent);
            await _db.SaveChangesAsync();

            if (response.IsSuccessStatusCode)
            {
                var responseObject = JObject.Parse(responseContent);
                string transaction_id = responseObject.Value<string>("transaction_id")!;

                var returnUrl = Environment.GetEnvironmentVariable("WAVERETURN_URL")!;
                var redirectUrl = $"{returnUrl}?transaction_id={transaction_id}";

                return Ok(redirectUrl);
            }
            else
            {
                var responseObject = JObject.Parse(responseContent);
                string message = responseObject.Value<string>("message") ?? "Failed to process the payment.";

                return response.StatusCode switch
                {
                    HttpStatusCode.Conflict => Conflict(message),
                    HttpStatusCode.BadRequest => BadRequest(message),
                    HttpStatusCode.UnprocessableEntity => UnprocessableEntity(message),
                    _ => BadRequest("Failed to process the payment.")
                };
            }
        }

        public static string HashHMAC(string message, string secret)
        {
            try
            {
                using HMACSHA256 hmac = new(Encoding.UTF8.GetBytes(secret));
                byte[] signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                SentrySdk.AddBreadcrumb("Successful hash WavePay Code", level: BreadcrumbLevel.Info);
                return BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();
            }
            catch (Exception ex)
            {
                SentrySdk.AddBreadcrumb("Fail Hash", level: BreadcrumbLevel.Error);
                SentrySdk.CaptureException(ex);
                throw;
            }

        }
        //this api will be used in notify lambda, doesn't use from subscription service
        [Route("Callback")]
        [HttpPost]
        public async Task<IActionResult> WaveBackEnd(WaveCallbackResponse response)
        {
            if (!ModelState.IsValid)
            {
                SentrySdk.AddBreadcrumb("Invalid request.", level: BreadcrumbLevel.Warning);
                SentrySdk.CaptureMessage("Invalid request.");
                return BadRequest("Invalid request.");
            }

            string message = $"{response.status}{response.timeToLiveSeconds}{response.merchantId}{response.orderId}{response.amount}{response.backendResultUrl}{response.merchantReferenceId}{response.initiatorMsisdn}{response.transactionId}{response.paymentRequestId}{response.requestTime}";
            var secret = Environment.GetEnvironmentVariable("WAVE_SECRECTKEY");

            string hash = HashHMAC(message, secret!);

            string merchant_id = Environment.GetEnvironmentVariable("WAVE_MERCHANTID")!;

            if (response.merchantId != merchant_id)
            {
                SentrySdk.AddBreadcrumb("Merchant ID mismatch.", level: BreadcrumbLevel.Warning);
                SentrySdk.CaptureMessage("Merchant ID mismatch.");
                return Unauthorized("Merchant ID mismatch.");
            }

            if (response.hashValue != hash)
            {
                SentrySdk.AddBreadcrumb("Hash is not equal!", level: BreadcrumbLevel.Warning);
                SentrySdk.CaptureMessage("Hash is not equal!");
                return Unauthorized("Hash is not equal!");
            }

            var gate = await _db.GatewayRawEvents.FirstOrDefaultAsync(g => g.GateWayOrderId == response.merchantReferenceId);

            if (gate == null)
            {
                return BadRequest("Invalid Payment Id.");
            }

            gate.CallbackResponsePayload = JsonSerializer.Serialize(message);
            await _db.SaveChangesAsync();

            var transaction = await _db.Transactions.FirstOrDefaultAsync(t => t.GateWayOrderId == response.merchantReferenceId);

            if (transaction == null)
            {
                return BadRequest("Transaction not found");
            }

            // Handle failure statuses early
            if (response.status == "INSUFFICIENT_BALANCE" || response.status == "ACCOUNT_LOCKED" || response.status == "SCHEDULER_TRANSACTION_TIMED_OUT")
            {
                return BadRequest("Failed");
            }

            // Handle success status (or any other that should proceed)
            var package = await _db.Packages.FirstOrDefaultAsync(p => p.PlanId == transaction.RequestedPlan && p.IsActive == true);

            if (package == null)
            {
                SentrySdk.AddBreadcrumb("Plan not found", level: BreadcrumbLevel.Warning);
                SentrySdk.CaptureMessage("Plan not found");
                return BadRequest("Plan not found");
            }

            double duration = (double)package.ExpiredDuration!;
            DateTime expiredAt = DateTime.UtcNow.Date.AddDays(duration);
            expiredAt = new DateTime(expiredAt.Year, expiredAt.Month, expiredAt.Day, 23, 59, 59).ToUniversalTime();

            var deltaTran = new Delta<Transaction>();
            deltaTran.TrySetPropertyValue("ExpiredAt", expiredAt);
            deltaTran.TrySetPropertyValue("Status", TransactionStatus.success);
            deltaTran.TrySetPropertyValue("GateRefCode", response.paymentRequestId);
            deltaTran.TrySetPropertyValue("AppliedPlan", transaction.RequestedPlan);
            deltaTran.Patch(transaction);

            UserCreditHistory userCreditHistory = new()
            {
                UserId = transaction.UserId,
                CreditAmount = package.Credit,
                Type = CreditType.purchase,
            };

            var user = await _db.Users.FindAsync(transaction.UserId);
            if (user != null)
            {
                user.CreditBalance += package.Credit;
            }

            _db.UserCreditHistories.Add(userCreditHistory);

            await _db.SaveChangesAsync();

            await StoreSubscriptionRedisAsync(transaction.UserId, userCreditHistory, transaction, package);

            return Ok("Success");
        }


    }
}



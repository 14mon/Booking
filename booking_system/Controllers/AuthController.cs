using System.Security.Cryptography;
using System.Text;
using booking_system.Data;
using booking_system.Models;
using booking_system.Utils;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

[ApiController]
[Route("v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly DataContext _db;
    private readonly ILogger<AuthController> _logger;
    private readonly string FirebaseAPIKEY;

    public AuthController(DataContext dbContext, ILogger<AuthController> logger)
    {
        _logger = logger;
        _db = dbContext;
        FirebaseAPIKEY = Environment.GetEnvironmentVariable("Firebase_API_KEY")!;

    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register(CustomerRequest request)
    {
        try
        {
            var emailExist = _db.Users.Where(c => c.Email == request.Email).Any();

            if (emailExist)
            {
                return BadRequest("Email Already Exist");
            }
            var phoneNum = _db.Users.Where(c => c.Phone == request.Phone).Any();
            if (phoneNum)
            {
                return BadRequest("PhoneNumber Already Exist");
            }
            UserRecordArgs args = new UserRecordArgs()
            {
                Email = request.Email,
                EmailVerified = false,
                Password = request.Password,
                Disabled = false,
            };
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);

            Guid token = Guid.NewGuid();

            var URL = Environment.GetEnvironmentVariable("VERIFICATION_URL");
            var VerificationURL = $"{URL}?token={token}";

            EmailUtility.Verification(request.Email!, VerificationURL);

            var user = new User
            {
                Name = request.Name!,
                Phone = request.Phone,
                Email = userRecord.Email,
                CountryId = (Guid)request.CountryId!,
                Address = request.Address,
                RegisterDate = DateTime.UtcNow,
                Status = UserStatus.active,
                FirebaseUserId = userRecord.Uid,
                LoginType = Convert.ToString(userRecord.ProviderData[0].ProviderId),

            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();


            return Ok(new
            {
                message = "Register successful",
                userId = user.Id
            });


        }
        catch (Exception error)
        {
            return BadRequest(new { message = error.Message });

        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var customer = await _db.Users.FirstOrDefaultAsync(e => e.Email == request.Email);
            if (customer == null)
            {
                return BadRequest("Need to Register.");
            }

            if (customer.Status == UserStatus.ban)
            {
                return BadRequest("Your account is banned.");
            }

            if (customer.Status == UserStatus.delete)
            {
                return BadRequest("Your account is deleted.");
            }
            var client = new HttpClient();
            var payload = new
            {
                email = request.Email,
                password = request.Password,
                returnSecureToken = true
            };
            var FirebaseAPIKEY = Environment.GetEnvironmentVariable("Firebase_API_KEY");
            var response = await client.PostAsJsonAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={FirebaseAPIKEY}", payload);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Invalid email or password.");
            }

            var firebaseResponse = await response.Content.ReadFromJsonAsync<FirebaseLoginResponse>();

            // Optional: get user info from Admin SDK
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(request.Email);

            return Ok(new
            {
                Message = "Login successful",
                Name = userRecord?.DisplayName,
                Email = userRecord?.Email,
                PhoneNumber = userRecord?.PhoneNumber,
                PhotoUrl = userRecord?.PhotoUrl,
                userId = customer.Id,
            });
        }
        catch (FirebaseAuthException ex)
        {
            SentrySdk.AddBreadcrumb("Error occurred during login.", level: BreadcrumbLevel.Error);
            SentrySdk.CaptureException(ex);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            return StatusCode(500, ex);
        }
    }

    [HttpPost("anonymous")]
    public async Task<IActionResult> AnonymousLogin(string IdToken)
    {
        try
        {
            FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(IdToken);

            string uid = decodedToken.Uid;
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);

            return Ok(new
            {
                Message = "Anonymous login successful",
            });
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogError(ex, "Firebase auth error.");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during anonymous login.");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            // Validate the request
            if (string.IsNullOrEmpty(request.Email) ||
                string.IsNullOrEmpty(request.OldPassword) ||
                string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest("Email, old password, and new password are required.");
            }

            if (request.NewPassword.Length < 6)
            {
                return BadRequest("New password must be at least 6 characters long.");
            }

            if (request.OldPassword == request.NewPassword)
            {
                return BadRequest("New password must be different from the current password.");
            }

            // Check if customer exists in database
            var customer = await _db.Users.FirstOrDefaultAsync(c => c.Email == request.Email);
            if (customer == null)
            {
                return BadRequest("Customer not found.");
            }

            if (customer.Status == UserStatus.ban)
            {
                return BadRequest("Your account is banned.");
            }

            if (customer.Status == UserStatus.delete)
            {
                return BadRequest("Your account is deleted.");
            }

            // Get Firebase user record
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(request.Email);
            if (userRecord == null)
            {
                return BadRequest("User not found in Firebase.");
            }

            // Verify the old password by attempting to sign in
            bool isOldPasswordValid = await VerifyPasswordAsync(request.Email, request.OldPassword);
            if (!isOldPasswordValid)
            {
                return BadRequest("Current password is incorrect.");
            }

            // Update the password in Firebase
            UserRecordArgs updateArgs = new UserRecordArgs()
            {
                Uid = userRecord.Uid,
                Password = request.NewPassword
            };

            await FirebaseAuth.DefaultInstance.UpdateUserAsync(updateArgs);

            return Ok(new
            {
                message = "Password changed successfully",
                userId = customer.Id
            });
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogError(ex, "Firebase auth error during password change.");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during password change.");
            return StatusCode(500, new { message = "Internal server error." });
        }
    }

    private async Task<bool> VerifyPasswordAsync(string email, string oldPassword)
    {
        try
        {
            var payload = new
            {
                email = email,
                password = oldPassword,
                returnSecureToken = true
            };

            var _httpClient = new HttpClient();

            var response = await _httpClient.PostAsJsonAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={FirebaseAPIKEY}",
                payload);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying password for email: {Email}", email);
            return false;
        }
    }

    [HttpPost("password-reset")]
    public async Task<IActionResult> ResetPassword(string Email)
    {
        try
        {
            var resetLink = await FirebaseAuth.DefaultInstance.GeneratePasswordResetLinkAsync(Email);

            // Send the customized reset link via email
            EmailUtility.PasswordReset(Email, resetLink);

            return Ok("Password reset link sent to your email.");
        }
        catch (FirebaseAuthException ex)
        {
            SentrySdk.AddBreadcrumb("Error occurred during password reset.", level: BreadcrumbLevel.Error);
            SentrySdk.CaptureException(ex); // Log exception to Sentry
            _logger.LogError(ex, "Error generating password reset link.");
            return BadRequest(ex.Message);
        }
    }
}


public class FirebaseLoginResponse
{
    public string IdToken { get; set; }
    public string Email { get; set; }
    public string RefreshToken { get; set; }
    public string ExpiresIn { get; set; }
    public string LocalId { get; set; }
    public bool Registered { get; set; }
}


public class ChangePasswordRequest
{
    public string Email { get; set; } = default!;
    public string OldPassword { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
}
public class LoginRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public class CustomerRequest
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public string? Password { get; set; }
}

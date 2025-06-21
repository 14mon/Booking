using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Net.Http.Headers;

namespace booking_system.Models;

public class User : BaseModel
{
    public User()
    {
        UserCreditHistories = [];
        Transactions = [];
    }
    public string Name { get; set; }
    public int CreditBalance { get; set; }
    public string? Phone { get; set; }
    public string Email { get; set; }
    public Guid CountryId { get; set; }
    [Column(TypeName = "date")]
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? ProfileImage { get; set; }
    public DateTime RegisterDate { get; set; }
    public UserStatus Status { get; set; }
    public string FirebaseUserId { get; set; } = default!;
    public string? LoginType { get; set; }
    public bool IsEmailVerified { get; set; }
    public Guid? VerificationToken { get; set; } = null;
    public virtual Country? Country { get; set; }
    public virtual ICollection<UserCreditHistory> UserCreditHistories { get; set; }
    public virtual ICollection<Transaction> Transactions { get; set; }

}


using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace booking_system.Utils
{
    public class EmailUtility
    {
        private static readonly string MailSenderAddress = Environment.GetEnvironmentVariable("MAIL_SENDER_ADDRESS")!;
        private static readonly string MailSenderName = Environment.GetEnvironmentVariable("MAIL_SENDER_NAME")!;
        private static readonly string MailPassword = Environment.GetEnvironmentVariable("MAIL_PASSWORD")!;

        public static void PasswordReset(string toEmail, string resetLink)
        {
            var fromAddress = new MailAddress(MailSenderAddress, MailSenderName);
            var toAddress = new MailAddress(toEmail);
            string subject = "Password Reset Verification";
            string body = $@"
                <html>
                <body>
                    <p>This email verifies your password reset request. To set a new password, click the button below:</p>
                    <p><a href='{resetLink}' style='display: inline-block; padding: 10px 20px; font-size: 16px; color: #fff; background-color: #28a745; text-decoration: none; border-radius: 5px;'>RESET PASSWORD</a></p>
                    <p>If you didn't request this reset, please disregard this message. For enhanced security, consider securing your account.</p>
                    <br/>
                    <p>Best regards,</p>
                    <p>_____ Team</p>
                </body>
                </html>";

            SendEmail(fromAddress, toAddress, subject, body);
        }


        public static void Verification(string toEmail, string verificationUrl)
        {
            var fromAddress = new MailAddress(MailSenderAddress, MailSenderName);
            var toAddress = new MailAddress(toEmail);
            string subject = "Email Address Verification";
            var body = $@"
                <html>
                <body>
                    <p>Welcome to ____!</p>
                    <p>To activate your account and access our platform, please confirm your email address by clicking the button below. Occasionally, the email may land in your spam mail.</p>
                    <p>
                        <a href='{verificationUrl}' style='display: inline-block; padding: 10px 20px; font-size: 16px; color: #ffffff; background-color: #007bff; border-radius: 5px; text-decoration: none;'>Verification Button</a>
                    </p>
                    <p>Best Regards,</p>
                    <p>_____ Team</p>
                </body>
                </html>";

            SendEmail(fromAddress, toAddress, subject, body);
        }

        private static void SendEmail(MailAddress fromAddress, MailAddress toAddress, string subject, string body)
        {
            try
            {
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, MailPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }
}


using System.Net.Mail;
using System.Net;

namespace RestaurantWebsiteApplication.email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("trupthirestaurant@gmail.com"),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(email);

                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("trupthirestaurant@gmail.com", "zfqf jjks wbqh qjbw");

                await client.SendMailAsync(mailMessage);
            }
        }

    }
}

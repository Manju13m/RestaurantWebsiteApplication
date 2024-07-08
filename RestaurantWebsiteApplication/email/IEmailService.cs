namespace RestaurantWebsiteApplication.email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message)
        {
            throw new NotImplementedException();
        }
    }
}

using Mailjet.Client;
using Mailjet.Client.Resources;
using Mailjet.Client.TransactionalEmails;
using Microsoft.AspNetCore.Mvc;

namespace UserAuthDotBet2_WithDatabase
{
    [ApiController]
    [Route("api/email")]
    public class EmailController : ControllerBase
    {
        private readonly ILogger<EmailController> _logger;
        public EmailController(ILogger<EmailController> logger)
        {
            _logger = logger;

        }

        [HttpGet]
        public async Task SendMailAPI([FromQuery] string userEmail, string otpString)
        {
            await SendMail(userEmail, otpString);
        }

        public static async Task SendMail(string userEmail, string otpString)
        {
            MailjetClient client = new MailjetClient(FixedVariable.publicKey, FixedVariable.privateKey);

            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource
            };

            // construct your email with builder
            var email = new TransactionalEmailBuilder()
                   .WithFrom(new SendContact("sandeep.rawat@narayanagroup.com"))
                   .WithSubject("Otp for login")
                   .WithHtmlPart($"<h1>Hello user your otp is : {otpString}</h1>")
                   .WithTo(new SendContact($"{userEmail}"))
                   .Build();

            // invoke API to send email
            var response = await client.SendTransactionalEmailAsync(email);

            // check response
            Console.WriteLine(response);
        }

    }
}
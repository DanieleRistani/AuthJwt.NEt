using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace AuthJwt.Mail
{
    [Route("[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.mailtrap.io")
                {
                    Port = 2525,
                    Credentials = new NetworkCredential("4e2a2918d1d033", "0c9cb1af6d3cf8"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("testemaildan98202@gmail.com"),
                    Subject = request.Subject,
                    Body = request.Message,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(request.ToEmail);

                await smtpClient.SendMailAsync(mailMessage);

                return Ok(new { message = "Email inviata con successo" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}


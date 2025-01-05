using System.Net.Mail;
using System.Net;

namespace TP.Application.Common.Interfaces;

public class EmailService
{
    public void Send(string email,string Code)
    {
        SmtpClient client = new SmtpClient("smtp.gmail.com") 
        {
            Port = 587,
            Credentials = new NetworkCredential("email.tradingprocess@gmail.com", "pukm fpge bedu ptbf"),
            EnableSsl = true, 
        };

        MailMessage mailMessage = new MailMessage
        {
            From = new MailAddress("r.shavaly@gmail.com"),
            Subject = "Your single-use code",
            Body = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
<meta charset=""UTF-8"">
<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
<title>Single-Use Code</title>
<style>
  body {{
    font-family: Arial, sans-serif;
    margin: 0;
    padding: 20px;
    background-color: #f4f4f4;
    color: #333;
  }}
  .email-container {{
    max-width: 600px;
    margin: 0 auto;
    background: #ffffff;
    padding: 20px;
    border-radius: 8px;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.05);
  }}
  .email-header {{
    font-size: 1.5em;
    margin-bottom: 20px;
  }}
  .code {{
    font-size: 1.5em;
    color: #007bff;
    margin: 10px 0;
  }}
  .footer {{
    margin-top: 20px;
    padding-top: 20px;
    border-top: 1px solid #eee;
    font-size: 0.9em;
    text-align: center;
  }}
</style>
</head>
<body>
  <div class=""email-container"">
    <div class=""email-header"">
      Hi,
    </div>
    <p>We received your request for a single-use code to use with your TP account.</p>
    <p>Your single-use code is: <strong class=""code"">{Code}</strong></p>
    <p>If you didn't request this code, you can safely ignore this email. Someone else might have typed your email address by mistake.</p>
    <div class=""footer"">
      Thanks,<br>
      The Trading Process team
    </div>
  </div>
</body>
</html>
            ",
            IsBodyHtml = true,
        };
        mailMessage.To.Add(email);

        try
        {
            // Send the email
            client.Send(mailMessage);
            Console.WriteLine("Email sent successfully");
        }
        catch (Exception ex)
        {
            // Handle any errors
            Console.WriteLine("Error occurred while sending the email: " + ex.Message);
        }
    }
}

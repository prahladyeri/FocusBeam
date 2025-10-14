/**
 * Mailer.cs
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

namespace focusbeam.Helpers
{
    public static class Mailer
    {

        public static void SendEmail(string[] to, string subject, string body, string[] attachments = null)
        {
            attachments = attachments ?? new string[0];
            string htmlBody = FileHelper.ReadEmbeddedResource("focusbeam.files.email.html")
                .Replace("{{body}}", body)
                .Replace("{{subject}}", subject)
                .Replace("{{regards}}", $"{Application.ProductName} Support")
                ;
            var fromAddress = new MailAddress("support@example.com", $"{Application.ProductName} Support");

            var smtp = new SmtpClient
            {
                Host = "mail.outlook.com",  // e.g., smtp.gmail.com
                Port = 587,                      // or 465 for SSL, or 25
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, "xxxxxxxx")
            };

            using (var message = new MailMessage())
            {
                message.From = fromAddress;
                foreach (string email in to)
                {
                    message.To.Add(new MailAddress(email));
                }
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = htmlBody;

                foreach (string attachmentPath in attachments)
                {
                    string mimeType = MimeTypeHelper.GetMimeType(attachmentPath);
                    Attachment attachment = new Attachment(attachmentPath, mimeType);
                    message.Attachments.Add(attachment);
                }

                smtp.Send(message);
            }
        }
    }
}

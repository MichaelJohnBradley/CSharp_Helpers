using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;

public static class EmailHelper
    {
        public static void Email(string subject, string body, string to, bool isHtml, List<EmailAttachment> attachments, List<string> cc = null, string bcc = null, string from = null, string fromDisplayName = null)
        {
            var email = new Email
            {
                Subject = subject,
                Body = body,
                To = to,
                Attachments = attachments,
                Cc = cc == null ? null : string.Join(";", cc),
                Bcc = bcc,
                From = from,
                FromDisplayName = fromDisplayName,
                CreatedOn = DateTime.UtcNow,
                IsHtml = isHtml

            }; 
            
            SendEmail(email);
        }

        private static bool SendEmail(Email email)
        {
            try
            {
                if (email.From == null)
                {
                    email.From = "";
                    email.FromDisplayName = "";
                }

                var smtp = new SmtpClient
                {
                    Host = "",
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("", "")
                };

                var mail = new MailMessage();
                foreach (var address in email.To.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    mail.To.Add(address);

                if (email.Cc != null)
                    foreach (var address in email.Cc.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                        mail.CC.Add(address);

                if (email.Bcc != null)
                    foreach (var address in email.Bcc.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                        mail.Bcc.Add(address);

                mail.From = new MailAddress(email.From, email.FromDisplayName);
                mail.Subject = email.Subject;
                mail.Body = email.Body;
                mail.IsBodyHtml = email.IsHtml;

                if (email.Attachments != null)
                    foreach (var att in email.Attachments)
                        mail.Attachments.Add(new Attachment(new MemoryStream(Convert.FromBase64String(att.Base64EncodedBytes)), att.FileName, att.Mime) { ContentId = att.ContentId });

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Error trying to send email: {ex.Message}; {ex.InnerException}");
                return false;
            }
            Console.WriteLine($@"Successfully sent email to {email.To}");
            return true;
        }
    }

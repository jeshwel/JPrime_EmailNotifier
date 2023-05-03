using JPrime_EmailNotifier.Business.Contracts;
using JPrime_EmailNotifier.Models;
using HandlebarsDotNet;
using System;
using System.IO;
using System.Net.Mail;
using System.Text.Json.Nodes;

namespace JPrime_EmailNotifier.Business
{
    public class OrgEmail: IOrgEmail
    {
        private readonly string templatesBaseDir;

        public OrgEmail(string TemplatesBaseDir)
        {
            templatesBaseDir = TemplatesBaseDir;
        }
        public MailMessage GetMailMessage(string QueueMessage)
        {
            var qMessageJson = JsonNode.Parse(QueueMessage);
            //Note: JsonNode Property names are case sensitive.
            var emailConfig = qMessageJson["EmailConfig"];
            var templateType = Convert.ToString(emailConfig["TemplateType"]);
            if (string.IsNullOrEmpty(templateType))
                throw new Exception("[APP-ERROR] : Email template type is required for processing of queue message");
            var toAddress = Convert.ToString(emailConfig["ToAddress"]);
            var fromAddress = Convert.ToString(emailConfig["FromAddress"]);
            if (string.IsNullOrEmpty(toAddress) || string.IsNullOrEmpty(fromAddress))
                throw new Exception("[APP-ERROR] : Queue message missing addresses.");

            MailMessage msg = new MailMessage();
            msg.To.Add(toAddress);
            msg.From = new MailAddress(fromAddress);
            var emailContent = GetEmailContent(templateType, qMessageJson);
            msg.Subject = emailContent.Subject ?? "Email subject content not defined.";
            msg.Body = emailContent.HtmlBody ?? "Email template/content not defined.";
            msg.IsBodyHtml = true;
            return msg;
        }

        public void SendEmail(MailMessage EMessage)
        {
            var smtpHost = Environment.GetEnvironmentVariable("JPrime_SMTPHost");
            if (string.IsNullOrWhiteSpace(smtpHost))
                throw new Exception("[APP-ERROR] : SMTP/Email Host not configured.");
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Port = 587; // You can use Port 25 if 587 is blocked 
            client.Host = smtpHost;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Send(EMessage);
        }

        private EmailContent GetEmailContent(string TemplateType, JsonNode qMessageJson)
        {
            EmailContent result = new EmailContent();
            var dataFields = new object();
            switch (TemplateType)
            {
                case "MISSING-JOB":
                    dataFields = new
                    {
                        Name = Convert.ToString(qMessageJson["Name"]),
                        Frequency = Convert.ToString(qMessageJson["Frequency"]),
                        LastSuccessfulRunDate = Convert.ToString(qMessageJson["LastSuccessfulRunDate"]),
                        ContactNames = Convert.ToString(qMessageJson["EmailConfig"]["ContactNames"]),
                        DataExpectedOn = Convert.ToString(qMessageJson["DataExpectedOn"]),
                        ExpectedTransferDate = Convert.ToString(qMessageJson["ExpectedTransferDate"])
                    };
                    result = PopulateEmailContent(TemplateType, dataFields);
                    break;
                case "ERROR-SUPPORT":
                    result.HtmlBody = $"JobName: {qMessageJson["Error"]} | Freq: {qMessageJson["Frequency"]} | Test UndefinedProp: {qMessageJson["UndefinedProp"]} This is a test message using SMTP from AzureFunction.";
                    break;
            }
            return result;
        }
        
        private EmailContent PopulateEmailContent(string TemplateType, dynamic DataFields)
        {
            EmailContent result = new EmailContent();
            TemplateType = TemplateType.ToLower();
            string sourceTemplate = File.ReadAllText(Path.Combine(templatesBaseDir, $"{TemplateType}.htm"));
            string templateMetaData = File.ReadAllText(Path.Combine(templatesBaseDir, $"{TemplateType}-data.json"));
            var metaDataJson = JsonNode.Parse(templateMetaData);
            result.Subject = Convert.ToString(metaDataJson["Subject"]);
            var template = Handlebars.Compile(sourceTemplate);
            result.HtmlBody = template(DataFields);
            return result;
        }
    }
}

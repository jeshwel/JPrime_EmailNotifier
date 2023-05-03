using JPrime_EmailNotifier.Business;
using JPrime_EmailNotifier.Business.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Mail;

namespace JPrime_EmailNotifier
{
    public class OrgEmailNotifierFn
    {
       private ILogger logger;
       [FunctionName("OrgEmailNotifier")]
        public void Run([QueueTrigger("%OrgEmailQueueName%", Connection = "StorageConnection")] string queueMessage, ExecutionContext executionContext, ILogger log)
        {
            //Test commit email change
            //TODO: Handle org-jobs-email-queue-dev-poison
            //TODO: Move connections and keys to Az KeyVault.
            logger = log;
            string templatesBaseDir = Path.Combine(executionContext.FunctionAppDirectory, "EmailTemplates");
            SendEmail(queueMessage, templatesBaseDir);
        }

        private void SendEmail(string queueMessage, string templatesBaseDir)
        {
            IOrgEmail orgEmail = new OrgEmail(templatesBaseDir);
            MailMessage msg = orgEmail.GetMailMessage(queueMessage);
            logger.LogInformation("[GetMailMessage] completed.");
            orgEmail.SendEmail(msg);
        }
    }
}

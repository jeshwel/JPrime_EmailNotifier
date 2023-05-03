using JPrime_EmailNotifier.Tests.Data.Contracts;
using System.Text.Json;

namespace JPrime_EmailNotifier.Tests.Data
{
    public class MockData: IMockData
    {
        public string GetQMessageForMissingJob()
        {
            var emailConfig = new { ToAddress= "jon@yourorg.com", TemplateType = "MISSING-JOB", FromAddress= "dev.no-reply@yourorg.com",
                ContactNames="Jon Smith, Dan V"};
            var emailMessage = new
            {
                Name = "la-org-jobx",
                Frequency = "Day",
                EmailConfig = emailConfig,
                LastSuccessfulRunDate = DateTime.Now.ToString(),
                ExpectedTransferDate = DateTime.Now.AddDays(1).ToString(),
                DataExpectedOn = "ALLDAYS"
            };
            return JsonSerializer.Serialize(emailMessage);
        }
    }
}

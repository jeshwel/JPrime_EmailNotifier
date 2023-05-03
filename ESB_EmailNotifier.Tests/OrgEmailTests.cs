using JPrime_EmailNotifier.Business;
using JPrime_EmailNotifier.Business.Contracts;
using JPrime_EmailNotifier.Tests.Data;
using JPrime_EmailNotifier.Tests.Data.Contracts;
using NUnit.Framework;

namespace JPrime_EmailNotifier.Tests
{
    public class OrgEmailTests
    {
        private IOrgEmail orgEmail;
        private IMockData mockData;
        private static string mainProjectName = "JPrime_EmailNotifier";
        private static string testProjectName = "JPrime_EmailNotifier.Tests";
        private string missingAlertQMessage;
        [SetUp]
        public void Setup()
        {
            var templateDir = Directory.GetCurrentDirectory();
            //In Debug mode the email templates will be located in the main project path ..\JPrime_EmailNotifier\bin\Debug\net6.0\EmailTemplates
            //TODO: Move the email templates to Blob Storage.
            templateDir = Path.Combine(templateDir.Replace(testProjectName, mainProjectName), "EmailTemplates");
            orgEmail = new OrgEmail(templateDir);
            mockData=new MockData();
            missingAlertQMessage = mockData.GetQMessageForMissingJob();
        }

        [Test]
        public void MissingAlert_ContainsSenderReceiverAddresses()
        {
            var mailMsg = orgEmail.GetMailMessage(missingAlertQMessage);
            Assert.That(mailMsg.To.First().Address, Does.Contain("@yourorg.com"));
            Assert.That(mailMsg.From?.Address, Is.EqualTo("dev.no-reply@yourorg.com"));
        }

        [Test]
        public void MissingAlert_HasValidSubjectLine()
        {

            var mailMsg = orgEmail.GetMailMessage(missingAlertQMessage);
            Assert.IsTrue(mailMsg.Subject == "WARNING ALERT: Job Activity missing!!");
        }

        [Test]
        public void MissingAlert_HasValidEmailContent()
        {
            var mailMsg = orgEmail.GetMailMessage(missingAlertQMessage);
            Assert.That(mailMsg.Body, Does.Contain("la-org-jobx"));
            Assert.That(mailMsg.Body, Does.Contain("ALLDAYS"));
        }

        //SendEmail test need not be part of the RunAll Tests.
        [Test]
        [Ignore("Whenever you want to test email sending comment out this attribute.")]
        public void MissingAlert_VerifyEmailSending()
        {
            var mailMsg = orgEmail.GetMailMessage(missingAlertQMessage);
            orgEmail.SendEmail(mailMsg);
        }

    }
}
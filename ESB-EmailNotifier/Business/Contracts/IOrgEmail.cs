using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace JPrime_EmailNotifier.Business.Contracts
{
    public interface IOrgEmail
    {
        MailMessage GetMailMessage(string QueueMessage);
        void SendEmail(MailMessage EMessage);
    }
}

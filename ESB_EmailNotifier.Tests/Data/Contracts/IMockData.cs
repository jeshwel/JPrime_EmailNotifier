using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPrime_EmailNotifier.Tests.Data.Contracts
{
    public interface IMockData
    {
        string GetQMessageForMissingJob();
    }
}

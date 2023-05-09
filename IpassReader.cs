using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginData
{
    interface IPassReader
    {
        IEnumerable<CredentialModel> ReadPasswords();
        string BrowserName { get; }
    }
}

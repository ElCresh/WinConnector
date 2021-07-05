using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace WinConnector
{
    abstract class Connector
    {
        // URI Docs > https://docs.microsoft.com/it-it/dotnet/api/system.uri?view=net-5.0
        // AbsolutePath: /Home/Index.htm
        // AbsoluteUri: https://user:password@www.contoso.com:80/Home/Index.htm?q1=v1&q2=v2#FragmentName
        // DnsSafeHost: www.contoso.com
        // Fragment: #FragmentName
        // Host: www.contoso.com
        // HostNameType: Dns
        // IdnHost: www.contoso.com
        // IsAbsoluteUri: True
        // IsDefaultPort: False
        // IsFile: False
        // IsLoopback: False
        // IsUnc: False
        // LocalPath: /Home/Index.htm
        // OriginalString: https://user:password@www.contoso.com:80/Home/Index.htm?q1=v1&q2=v2#FragmentName
        // PathAndQuery: /Home/Index.htm?q1=v1&q2=v2
        // Port: 80
        // Query: ?q1=v1&q2=v2
        // Scheme: https
        // Segments: /, Home/, Index.htm
        // UserEscaped: False
        // UserInfo: user:password
        public abstract bool Connect(Uri uri);
        public abstract void Disconnect();
        public abstract bool IsConnected();
        public abstract Task<bool> Download(string filePath);
        public abstract ArrayList GetListOfCurrentDirectory();

    }
}

using System.Security.Authentication;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace Seer.Utils;


public class Constant
{
    //public static string Host = "://localhost:8080";
    public static string Host = "s://ww2.hakureif.site:8080";

    //public static string MatchHost = "://localhost:8080";
    public static string MatchHost = "s://www.hakureif.site:8080";

    public static string _version = "1.1.5";

    

    public Constant()
    {
        
    }

    public static TouchSocketConfig GetWsConfig(string url)
    {
        if (url.Contains("localhost") || url.Contains("192.168.1.3"))
        {
            return new TouchSocketConfig().SetRemoteIPHost(url);
        }
        string targetHost = "";
        if (url.Contains("imrightchen"))
        {
            targetHost = "www.imrightchen.live";
        }
        if (url.Contains("www.hakureif"))
        {
            targetHost = "www.hakureif.site";
        }
        if (url.Contains("ww2.hakureif"))
        {
            targetHost = "ww2.hakureif.site";
        }
        return new TouchSocketConfig().SetRemoteIPHost(url).SetClientSslOption(new ClientSslOption()
        {
            SslProtocols = SslProtocols.Tls12,
            TargetHost = targetHost,
            CertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
        });
    }
}
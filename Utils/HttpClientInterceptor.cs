using System.Diagnostics;

namespace Seer.Utils;

public static class HttpClientInterceptor
{
    public delegate void GetTokenHandler();
    // 基于上面的委托定义事件
    public static event GetTokenHandler? GetTokenEvent;

    private static void OnGetToken()
    {
        GetTokenEvent?.Invoke();
    }
    
    private static SocketsHttpHandler HttpHandler { get; } = new()
    {
        UseCookies = false, // 是否自动处理cookie
    };

    private static HttpClient HttpClient;
    
    public static void Set_Userid(string userid)
    {
        HttpClient = new(HttpHandler);
        userid = AesUtil.AesEncrypt(userid);
        HttpClient.DefaultRequestHeaders.Add("seer-userid", userid);
    }

    public static void Set_Token(string token)
    {
        HttpClient.DefaultRequestHeaders.Add("seer-token", token);
        OnGetToken();
    }

    public static void Set_Host(string host)
    {
        Debug.WriteLine("HttpClient Set_Host");
        HttpClient.BaseAddress = new Uri(host);
    }

    public static HttpClient Get_HttpClient()
    {
        return HttpClient;
    }
}
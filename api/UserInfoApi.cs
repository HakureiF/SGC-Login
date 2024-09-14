using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using Seer.CustomException;
using Seer.Utils;

namespace Seer.api;

public static class UserInfoApi
{
    //private const string Host = Constant.Host;
    
    public static async Task<string?> GetNickName()
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();
        Debug.WriteLine(httpClient.DefaultRequestHeaders.ToString());
        string url = $"http{Constant.Host}/api/user-information/getNickname";
        var res = await httpClient.GetAsync(url);
        if (!res.IsSuccessStatusCode) throw new RequestException("请求失败");
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);
        if (resUtil == null) throw new RequestException("没有返回结果");
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200) throw new RequestException("请求不通过");
        var nickname = resUtil.GetValueOrDefault("data").Deserialize<string>();
        return nickname;
    }

    public static async Task SendEliteInfo(List<int> elites)
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();
        var res = await httpClient.PostAsJsonAsync("/api/user-config/sendElite", elites);
        if (!res.IsSuccessStatusCode) throw new RequestException("请求失败");
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);
        if (resUtil == null) throw new RequestException("请求失败");
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200) throw new RequestException("请求失败");
        else MessageBox.Show("同步成功");
    }
}
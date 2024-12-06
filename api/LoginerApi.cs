using System.Diagnostics;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text.Json;
using Microsoft.VisualBasic.ApplicationServices;
using Seer.CustomException;
using Seer.Utils;
using TouchSocket.Core;

namespace Seer.api;

public static class LoginerApi
{
    //private static string Host = Constant.Host;


    public static async Task<bool> Login(Dictionary<string, string> loginState)
    {
        HttpClient httpClient = new();
        var res = await httpClient.PostAsJsonAsync($"http{Constant.Host}/api/login-information/loginerlogin", loginState);
        if (!res.IsSuccessStatusCode)
        {
            httpClient.Dispose();
            MessageBox.Show("验证失败"); 
            return false;
        }
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);
        if (resUtil != null)
        {
            if (resUtil.GetValueOrDefault("code").Deserialize<int>() == 200)
            {
                httpClient.Dispose();
                return true;
            }
            else
            {
                var mess = resUtil.GetValueOrDefault("message").Deserialize<string>();
                if (mess != null && mess != "")
                {
                    MessageBox.Show(mess);
                }
            }
        }
        else
        {
            MessageBox.Show("验证失败");
        }
        httpClient.Dispose();
        return false;
    }


    public static async Task<bool> VerifyPass(Dictionary<string, string> loginState)
    {
        HttpClient httpClient = new();
        var res = await httpClient.PostAsJsonAsync($"http{Constant.Host}/api/loginer/verifyPass", loginState);
        if (!res.IsSuccessStatusCode)
        {
            httpClient.Dispose();
            MessageBox.Show("验证失败");
            return false;
        }
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);
        if (resUtil != null)
        {
            if (resUtil.GetValueOrDefault("code").Deserialize<int>() == 200)
            {
                httpClient.Dispose();
                MessageBox.Show("验证成功");
                return true;
            }
            else
            {
                var mess = resUtil.GetValueOrDefault("message").Deserialize<string>();
                if (mess != null && mess != "")
                {
                    MessageBox.Show(mess);
                }
            }
        }
        else
        {
            MessageBox.Show("验证失败");
        }
        httpClient.Dispose();
        return false;
    }

    public new static async Task<string?> GetType(string userId, string password)
    {
        HttpClient httpClient = new();

        var res1 = await httpClient.GetAsync($"http{Constant.Host}/api/loginer/getType?userId={userId}&password={password}");
        if (!res1.IsSuccessStatusCode) { httpClient.Dispose(); return null; }
        var responseBody = await res1.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);
        if (resUtil == null) { httpClient.Dispose(); return null; }
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200) { httpClient.Dispose(); return null; }
        var playerType = resUtil.GetValueOrDefault("data").Deserialize<string>();
        httpClient.Dispose();
        return playerType;
    }

    public static async Task SendEliteInfo(string userId, string password, List<int> elites)
    {
        HttpClient httpClient = new();
        var res = await httpClient.PostAsJsonAsync($"http{Constant.Host}/api/loginer/sendElite?userId={userId}&password={password}", elites);
        if (!res.IsSuccessStatusCode) 
        {
            httpClient.Dispose();
            throw new RequestException("请求失败"); 
        }
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);
        if (resUtil == null) { httpClient.Dispose(); throw new RequestException("请求失败"); }
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200) { httpClient.Dispose(); throw new RequestException("请求失败"); }
        else MessageBox.Show("同步成功");
        httpClient.Dispose();
    }

    public static async Task<List<string>?> GetPlayer1Pick(string userId, string password)
    {
        HttpClient httpClient = new();

        var res1 = await httpClient.GetAsync($"http{Constant.Host}/api/loginer/getPlayer1PickList?userId={userId}&password={password}");
        if (!res1.IsSuccessStatusCode) { httpClient.Dispose(); return null; }
        var responseBody = await res1.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);
        if (resUtil == null) { httpClient.Dispose(); return null; }
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200) { httpClient.Dispose(); return null; }
        var player1Pick = resUtil.GetValueOrDefault("data").Deserialize<List<string>>();
        httpClient.Dispose();
        return player1Pick;
    }

    public static async Task<List<string>?> GetPlayer2Pick(string userId, string password)
    {
        HttpClient httpClient = new();

        var res1 = await httpClient.GetAsync($"http{Constant.Host}/api/loginer/getPlayer2PickList?userId={userId}&password={password}");
        if (!res1.IsSuccessStatusCode) { httpClient.Dispose(); return null; }
        var responseBody = await res1.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);
        if (resUtil == null) { httpClient.Dispose(); return null; }
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200) { httpClient.Dispose(); return null; }
        var player2Pick = resUtil.GetValueOrDefault("data").Deserialize<List<string>>();
        httpClient.Dispose();
        return player2Pick;
    }

    

    public static async Task GetAnnouncement()
    {
        HttpClient httpClient = new();
        var res = await httpClient.GetAsync($"http{Constant.Host}/api/announcement/getLoginerAnnouncement");
        if (!res.IsSuccessStatusCode) throw new RequestException("请求失败");
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);
        if (resUtil != null)
        {
            if (resUtil.GetValueOrDefault("code").Deserialize<int>() == 200)
            {
                var announcement = resUtil.GetValueOrDefault("data").Deserialize<Dictionary<string, string>>();
                if (announcement != null && announcement.ContainsKey("announcement"))
                {
                    //MessageBox.Show(announcement["announcement"], "公告");
                    var anncounceBox = new Announce();
                    anncounceBox.InitializeAnnounce(announcement["announcement"]);
                    anncounceBox.Show();
                }
                if (announcement != null && announcement.ContainsKey("version"))
                {
                    string version = announcement["version"];
                    Debug.WriteLine("remote version - " + version);
                    if (version != Constant._version)
                    {
                        if(MessageBox.Show("有新版本", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            string url = announcement["download"];
                            Process p = new Process();
                            p.StartInfo.FileName = "cmd.exe";
                            p.StartInfo.UseShellExecute = false;    //不使用shell启动
                            p.StartInfo.RedirectStandardInput = true;//cmd接受标准输入
                            p.StartInfo.RedirectStandardOutput = false;//cmd不输出
                            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                            p.StartInfo.CreateNoWindow = true;//不显示窗口
                            p.Start();

                            //向cmd窗口发送输入信息 后面的&exit告诉cmd运行好之后就退出
                            p.StandardInput.WriteLine("start " + url + "&exit");
                            p.StandardInput.AutoFlush = true;
                            p.WaitForExit();//等待程序执行完退出进程
                            p.Close();
                        }
                    }
                }
            }
        }
        httpClient.Dispose();
    }
}
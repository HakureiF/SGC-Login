using System.Diagnostics;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text.Json;
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
    
    public static async Task GetAnnouncement()
    {
        Debug.WriteLine("test");
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
                    MessageBox.Show(announcement["announcement"], "公告");
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
    }
}
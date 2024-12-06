using Microsoft.VisualBasic.ApplicationServices;
using Seer.DTO;
using Seer.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Seer.api;

public static class ConventionalGameApi
{
    //private static string MatchHost = Constant.MatchHost;
    public static async Task<bool> CheckGameState()
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();

        var res = await httpClient.GetAsync("/api/game-information/gameState");
        if (!res.IsSuccessStatusCode) throw new Exception("链接失败");
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody) ?? throw new Exception("链接失败");
        return resUtil.GetValueOrDefault("code").Deserialize<int>() == 202;
    }

    public static async Task<List<RaceGroup>?> GetGroups()
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();
        
        var res1 = await httpClient.GetAsync("/api/race-group/groups");
        if (!res1.IsSuccessStatusCode) throw new Exception("链接失败");
        var responseBody = await res1.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody) ?? throw new Exception("链接失败");
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200) return null;
        var groups = resUtil.GetValueOrDefault("data").Deserialize<List<RaceGroup>>();
        return groups;
    }

    public static async Task<List<RaceGroup>?> SearchGroups()
    {
        HttpClient httpClient = new();

        var res1 = await httpClient.GetAsync($"http{Constant.MatchHost}/api/race-group/searchGroup?group=match");
        if (!res1.IsSuccessStatusCode) { httpClient.Dispose(); throw new Exception("链接失败"); }
        var responseBody = await res1.Content.ReadAsStringAsync();
        httpClient.Dispose();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody) ?? throw new Exception("链接失败");
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200) return null;
        var groups = resUtil.GetValueOrDefault("data").Deserialize<List<RaceGroup>>();
        return groups;
    }



    public static async Task<Dictionary<string, string>?> GenerateGame(string groupId)
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();

        var res = await httpClient.GetAsync("/api/game-information/generateConventionalGame?groupId=" + groupId);
        if (!res.IsSuccessStatusCode) throw new Exception("链接失败");
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody) ?? throw new Exception("链接失败");
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200) return null;
        var game = resUtil.GetValueOrDefault("data").Deserialize<Dictionary<string, string>>();
        return game;
    }



    public static async Task<bool> JoinConventionalGame(Dictionary<string, string> data)
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();

        var res = await httpClient.PostAsJsonAsync("/api/game-information/joinConventionalGame", data);
        if (!res.IsSuccessStatusCode) throw new Exception("链接失败");
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody) ?? throw new Exception("链接失败");
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200)
        {
            var message = resUtil.GetValueOrDefault("message").Deserialize<string>();
            MessageBox.Show(message);
            return false;
        }
        return true;
    }


    public static async Task<string?> VerifySuit(Dictionary<string, object> data)
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();

        var res = await httpClient.PostAsJsonAsync("/api/conventional/verifySuit", data);
        if (!res.IsSuccessStatusCode) throw new Exception("链接失败");
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody) ?? throw new Exception("链接失败");
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200)
        {
            var message = resUtil.GetValueOrDefault("message").Deserialize<string>();
            return message;
        }
        return null;
    }

    public static async Task SetConventionalSuit(string suitId)
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();

        var res = await httpClient.GetAsync("/api/game-information/setConventionalSuit?suitId=" + suitId);
        if (!res.IsSuccessStatusCode) throw new Exception("链接失败");
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody) ?? throw new Exception("链接失败");
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() == 200) return;
        var mess = resUtil.GetValueOrDefault("message").Deserialize<string>();
        if (mess != null && mess != "")
        {
            MessageBox.Show(mess);
        }
    }

    public static async Task<Dictionary<string, string>?> GetPickSuit()
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();

        var res = await httpClient.GetAsync("/api/game-information/getPickSuit");
        if (!res.IsSuccessStatusCode) throw new Exception("链接失败");
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody) ?? throw new Exception("链接失败");
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200) return null;
        var suit = resUtil.GetValueOrDefault("data").Deserialize<Dictionary<string, string>>();
        return suit;
    }

    public static async Task<bool> ExitGame()
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();

        var res = await httpClient.GetAsync("/api/game-information/exitGame");
        if (!res.IsSuccessStatusCode) throw new Exception("链接失败");
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody) ?? throw new Exception("链接失败");
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() == 200) return true;
        var mess = resUtil.GetValueOrDefault("message").Deserialize<string>();
        if (mess != null)
        {
            MessageBox.Show(mess); 
        }
        return false;
    }


    public static async Task RemoveGameCache(int? mimiId)
    {
        if (mimiId == null) return;
        HttpClient httpClient = new();
        var userid = AesUtil.AesEncrypt("seeraccount" + mimiId);
        httpClient.DefaultRequestHeaders.Add("seer-userid", userid);
        var res = await httpClient.GetAsync($"http{Constant.MatchHost}/api/game-information/removeGameCache");
        if (!res.IsSuccessStatusCode) throw new Exception("链接失败");
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody) ?? throw new Exception("链接失败");
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() == 200)
        {
            MessageBox.Show("清除成功!");
        }
        else
        {
            var mess = resUtil.GetValueOrDefault("message").Deserialize<string>();
            if (mess != null)
            {
                MessageBox.Show(mess);
            }
        }
        httpClient.Dispose();
    }


    public static async Task<string?> VerifyBag2(Dictionary<string, object> data)
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();

        var res = await httpClient.PostAsJsonAsync("/api/conventional/verifyBag", data);
        if (!res.IsSuccessStatusCode) throw new Exception("链接失败");
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody) ?? throw new Exception("链接失败");
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200)
        {
            var message = resUtil.GetValueOrDefault("message").Deserialize<string>();
            return message;
        }
        return null;
    }

    public static async Task FreshBag2(List<Pet> pets)
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();

        var res = await httpClient.PostAsJsonAsync("/api/conventional/freshBag", pets);
        if (!res.IsSuccessStatusCode) throw new Exception("链接失败");
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody) ?? throw new Exception("链接失败");
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() == 200) return;
        var mess = resUtil.GetValueOrDefault("message").Deserialize<string>();
        if (mess != null && mess != "")
        {
            MessageBox.Show(mess);
        }
    }

    public static async Task<Dictionary<string, List<Pet>>?> GetPetState()
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();

        var res = await httpClient.GetAsync("/api/conventional/getPetState");
        if (!res.IsSuccessStatusCode) throw new Exception("链接失败");
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody) ?? throw new Exception("链接失败");
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200) return null;
        var petstate = resUtil.GetValueOrDefault("data").Deserialize<Dictionary<string, List<Pet>>>();
        return petstate;
    }

    public static async Task<int> GetBanNum()
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();

        var res = await httpClient.GetAsync("/api/conventional/getBanNum");
        if (!res.IsSuccessStatusCode) throw new Exception("链接失败");
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200) return 0;
        var banNum = resUtil.GetValueOrDefault("data").Deserialize<int>();
        return banNum;
    }
}

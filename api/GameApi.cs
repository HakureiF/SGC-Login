using Seer.Utils;
using System.Diagnostics;
using System.Text.Json;
using TouchSocket.Core;

namespace Seer.api;

public static class GameApi
{

    public new static async Task<string?> GetType()
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();

        var res1 = await httpClient.GetAsync("/api/game-information/getType");
        if (!res1.IsSuccessStatusCode) return null;
        var responseBody = await res1.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);
        if (resUtil == null)return null;
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200) return null;
        var playerType = resUtil.GetValueOrDefault("data").Deserialize<string>();
        Debug.WriteLine("playerType:" + playerType);
        return playerType;
    }

    public static async Task<List<string>?> GetPlayer1Pick()
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();

        var res1 = await httpClient.GetAsync("/api/game-information/getPlayer1PickList");
        if (!res1.IsSuccessStatusCode) return null;
        var responseBody = await res1.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);
        if (resUtil == null) return null;
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200) return null;
        var player1Pick = resUtil.GetValueOrDefault("data").Deserialize<List<string>>();
        Debug.WriteLine("player1Pick:" + player1Pick.Count());
        return player1Pick;
    }

    public static async Task<List<string>?> GetPlayer2Pick()
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();

        var res1 = await httpClient.GetAsync("/api/game-information/getPlayer2PickList");
        if (!res1.IsSuccessStatusCode) return null;
        var responseBody = await res1.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);
        if (resUtil == null) return null;
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200) return null;
        var player2Pick = resUtil.GetValueOrDefault("data").Deserialize<List<string>>();
        Debug.WriteLine("player2Pick:" + player2Pick.Count());
        return player2Pick;
    }



    public static async Task<string?> GetPhase()
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();

        var res = await httpClient.GetAsync("/api/game-information/getPhase");
        if (!res.IsSuccessStatusCode) return null;
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);
        if (resUtil == null) return null;
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200) return null;
        var phase = resUtil.GetValueOrDefault("data").Deserialize<string>();
        Debug.WriteLine("game phase:" + phase);
        return phase;
    }

    public static async Task<int> GetRoomId()
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();

        var res = await httpClient.GetAsync("/api/game-information/getRoomId");
        if (!res.IsSuccessStatusCode) throw new Exception("链接失败");
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);
        if (resUtil == null) return 0;
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200) return 0;
        var roomId = resUtil.GetValueOrDefault("data").Deserialize<int>();
        Debug.WriteLine("roomId:" + roomId);
        return roomId;
    }

    public static async Task<Dictionary<string, string>> GetPlayers()
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();

        var res = await httpClient.GetAsync("/api/game-information/getPlayers");
        if (!res.IsSuccessStatusCode) throw new Exception("链接失败");
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() == 201)
        {
            MessageBox.Show("此次匹配对局已结束，原因可能是对方掉线");
        }
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200) return null;
        var players = resUtil.GetValueOrDefault("data").Deserialize<Dictionary<string, string>>();
        return players;
    }

    public static async Task<int> GetCountTime()
    {
        var httpClient = HttpClientInterceptor.Get_HttpClient();

        var res = await httpClient.GetAsync("/api/game-information/getCountTime");
        if (!res.IsSuccessStatusCode) throw new Exception("链接失败");
        var responseBody = await res.Content.ReadAsStringAsync();
        var resUtil = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);
        if (resUtil.GetValueOrDefault("code").Deserialize<int>() != 200) return 0;
        var count = resUtil.GetValueOrDefault("data").Deserialize<int>();
        Debug.WriteLine("CountTime:" + count);
        return count;
    }
}

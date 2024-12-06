using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Web.WebView2.Core;
using Seer.api;
using Seer.handler;
using Seer.Utils;
using TouchSocket.Core;
using TouchSocket.Http.WebSockets;
using TouchSocket.Sockets;
using static Seer.DTO.Store;

namespace Seer;

//public delegate void SetGameInfo();
//public delegate void SetRoomId();
//public delegate void ShowConventionalGame();

public delegate void SendWebViewMess(string mess);
public delegate void SendWsMess(string mess);
public delegate void ForwardWsMess(string mess);
public delegate void EmitFromSubForms(int signal);

public sealed partial class Main : Form
{
    public static string? PlayerType { get; set; }
    public static int RoomId { get; set; }
    //private List<Dictionary<string, int>> bagStore;
    private int? MiMiId { get; set; }
    private SgcWsHandler sgcWsHandler { get; set; }

    public Main()
    {
        InitializeComponent();
        Text = @"SGC比赛登录器 当前版本" + Constant._version + "    ！！！本登录器仅在QQ群787839277，876757129内流通，请勿使用他人转发提供的登录器，以防盗号风险！！！";
        //Text = @"SGC比赛登录器（Beta版本仅由 @HakureiF 发布，若看到他人发布请勿使用）";
        Resize += Form_Resize;
        LocationChanged += Location_Change;
        InitializeAsync();
        PetUtil.InitPetsData();


        /*        ShowFooter();
                footer.Location = new Point(Location.X, Location.Y + Size.Height);*/
        ShowPeakLogger(null);
        peakLogger.Location = new Point(Location.X + Size.Width, Location.Y);
        sgcWsHandler = new(new ForwardWsMess(ForwardWsMess), new EmitFromSubForms(EmitFromSubForms));
    }

    private async void InitializeAsync()
    {
        //PetUtil.InitPetsData();
        //await SeerApi.SetHeadJsons();
        await LoginerApi.GetAnnouncement();
        await webView.EnsureCoreWebView2Async(null);

    }

    private void CloseAll(object? sender, FormClosedEventArgs e)
    {
        System.Environment.Exit(0);
    }

    // 确保你已经初始化了WebView2控件并可以访问到CoreWebView2对象

    // 在 CoreWebView2 初始化完成事件处理程序中订阅 WebResourceRequested 事件
    private async void WebView_CoreWebViewInitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
    {
        // 确保CoreWebView2已成功初始化
        if (!e.IsSuccess) return;
        Debug.WriteLine("init success");
        // 禁用webview调试
        //webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
        //webView.CoreWebView2.AddWebResourceRequestedFilter("**", CoreWebView2WebResourceContext.All);
        //webView.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
        //webView.CoreWebView2.WebResourceResponseReceived +=
        await webView.CoreWebView2.ExecuteScriptAsync(JsScript.LoginListener);
        await webView.CoreWebView2.ExecuteScriptAsync(JsScript.WebViewListener + JsScript.LoginListener);
        

        webView.CoreWebView2.IsMuted = true;
        webView.CoreWebView2.WebMessageReceived += GetMessage;
    }

    private static void CoreWebView2_WebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
    {
        Debug.WriteLine("request------");
        // 获取请求的 URL
        var url = e.Request.Uri;
        // 获取请求的 method
        var method = e.Request.Method;

        Debug.WriteLine(method + ":" + url);
        var header = e.Request.Headers;
        var body = e.Request.Content;
        if (body == null) return;
        using StreamReader reader = new(body);
        var content = reader.ReadToEnd();
        // 对content进行处理，它包含了流中的所有文本内容
        Debug.WriteLine("header:" + header);
        Debug.WriteLine("content:" + content);

        //requestHeaders.SetHeader("Custom", "Value");
    }
    // 监听webview消息
    private void GetMessage(object? sender, CoreWebView2WebMessageReceivedEventArgs args)
    {
        try
        {
            var message = args.TryGetWebMessageAsString();
            if (message == null) return;
            Debug.WriteLine("mess from webview" + message);
            var mess = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(message);
            if (mess == null || !mess.ContainsKey("type")) return;


            //即时响应类消息
            if (mess.GetValue("type").Deserialize<string>().Contains("unLogin")) //未登录
            {
                MessageBox.Show(@"请先登录游戏！");
            }
            if (mess.GetValue("type").Deserialize<string>().Contains("mimiId")) //当前用户米米号
            {
                webView.CoreWebView2.ExecuteScriptAsync(JsScript.RewriteScript);
                var mimiId = mess.GetValue("data").Deserialize<int>();
                MiMiId = mimiId;
                string wsMess = "mimiId" + mimiId;
                if (mess.ContainsKey("signal") && mess.GetValue("signal").Deserialize<string>().Contains("matchVerify"))
                {
                    sgcWsHandler.StartConnect(mimiId, "Match");
                }
                if (mess.ContainsKey("signal") && mess.GetValue("signal").Deserialize<string>().Contains("generateGame"))
                {
                    sgcWsHandler.StartConnect(mimiId, "Create");
                }
                if (mess.ContainsKey("signal") && mess.GetValue("signal").Deserialize<string>().Contains("joinGame"))
                {
                    sgcWsHandler.StartConnect(mimiId, "Join");
                }
                //else if (Dialog.MyWsClient.Online)
                //{
                //    Dialog.MyWsClient.SendWithWS(wsMess);
                //}
                if (bagStore != null)
                {
                    bagStore.setMimiId(mimiId);
                }
            }
            if (mess.GetValue("type").Deserialize<string>().Contains("rivalMimiId"))
            {
                var rivalMimiId = mess.GetValue("data").Deserialize<int>();
                MessageBox.Show(@"对方的米米号是" + rivalMimiId);
            }

            //登陆器顶部选项触发类消息
            if (mess.GetValue("type").Deserialize<string>().Contains("roomIdCreated")) //房间号
            {
                var roomId = mess.GetValue("data").Deserialize<int>();
                Debug.WriteLine(roomId);
                SendWsMess("RoomId" + roomId);
            }
            if (mess.GetValue("type").Deserialize<string>().Contains("eliteInfo")) //精英收藏
            {
                var ids = mess.GetValue("data").Deserialize<List<int>>();
                Debug.WriteLine(ids);
            }

            //巅峰记牌相关消息
            if (mess.GetValue("type").Deserialize<string>().Contains("rivalPet")) //对方精灵
            {
                var ids = mess.GetValue("data").Deserialize<List<int>>();
                ShowPeakLogger(ids);
                Debug.WriteLine(ids);
            }
            if (mess.GetValue("type").Deserialize<string>().Contains("isRoomOwner")) //是否是房主
            {
                var mark = mess.GetValue("data").Deserialize<bool>();
                peakLogger.SetOwnerMark(mark);
            }
            if (mess.GetValue("type").Deserialize<string>().Contains("loggerPetId")) //对方新登场精灵id
            {
                var id = mess.GetValue("data").Deserialize<int>();
                if (id != 0)
                {
                    peakLogger.RivalPetShow(id);
                }
            }
            if (mess.GetValue("type").Deserialize<string>().Contains("eliteInfo")) //对方精灵被击败
            {
                if (mess.GetValue("type").Deserialize<string>().Contains("UnLoad"))
                {
                    MessageBox.Show("精英收藏未加载，请打开精灵仓库再尝试");
                }
                else
                {
                    var ids = mess.GetValue("data").Deserialize<List<int>>();
                    if (ids != null && ids.Count > 0)
                    {
                        var store = StoreUtil.getStore();
                        if (store != null)
                        {
                            LoginerApi.SendEliteInfo(store.userid, store.passwd, ids);
                        }
                        //UserInfoApi.SendEliteInfo(ids);
                    }
                }

            }
            if (mess.GetValue("type").Deserialize<string>().Contains("petDefeated")) //对方精灵被击败
            {
                var id = mess.GetValue("data").Deserialize<int>();
                if (id != 0)
                {
                    peakLogger.RivalPetDefeated(id);
                }
            }

            //123模式相关消息
            if (mess.GetValue("type").Deserialize<string>().Contains("bagInfo")) //己方当前的背包
            {
                //bagStore = mess["data2"].Deserialize<List<Dictionary<string, int>>>();
                if (bagStore != null)
                {
                    bagStore.freshStoreBag(mess);
                }
                if (mess.GetValue("signal").Deserialize<string>().Contains("matchVerify"))
                {
                    match.RecvWebViewMess(mess);
                }
                else if (conventionalGame != null)
                {
                    conventionalGame.RecvWebViewMess(mess);
                }
            }
            if (mess.GetValue("type").Deserialize<string>().Contains("suitId"))
            {
                if (mess.GetValue("signal").Deserialize<string>().Contains("matchVerify"))
                {
                    if (match.RecvWebViewMess(mess))
                    {
                        var wbvmess = JsMessUtil<int>.MessJson("getBag", 0, "matchVerify");
                        webView.CoreWebView2.PostWebMessageAsJson(wbvmess);
                    }
                }
                else
                {
                    conventionalGame.RecvWebViewMess(mess);
                }
            }
            if (mess.GetValue("type").Deserialize<string>().Contains("isWinner"))
            {
                var isWinner = mess.GetValue("data").Deserialize<bool>();
                string wsMess = "isWinner" + isWinner;
                SendWsMess(wsMess);
                //conventionalGame.GameOver(isWinner);
            }
            /*
            if (mess.GetValue("type").Deserialize<string>().Contains("reStoreBag")) //己方当前的背包
            {
                if (mess.GetValue("signal").Deserialize<string>().Contains("endGame"))
                {
                    SendWsMess("reStoreBag endGame");
                }
            }
            */
            if (mess.GetValue("type").Deserialize<string>().Contains("fightOverClick"))
            {
                SendWsMess("endGame");
                EmitFromSubForms(8);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            Debug.WriteLine("Error");
        }
    }

    /**
     * 
     */
    public void SendWebViewMess(string mess)
    { // 发送WebView消息中转
        Debug.WriteLine($"{mess}");
        Invoke(new EventHandler(delegate
        {
            webView.CoreWebView2.PostWebMessageAsJson(mess);
        }));
    }
    public void SendWsMess(string mess)
    { // 发送Ws消息中转
        //if (Dialog.MyWsClient.Online)
        //{
        //    Dialog.MyWsClient.SendAsync(mess);
        //}
        //if (Match.MatchWsClient.Online)
        //{
        //    Match.SendFromMatchWS(mess);
        //}
        SgcWsHandler.SendMess(mess);
    }
    public async void ForwardWsMess(string mess)
    { // 接收Ws消息中转
        if (conventionalGame != null)
        {
            if (mess.Contains("WaitingPeriodResult") || mess.Contains("onMatch") || mess.Contains("Ban"))
            {
                await InitPlayerType();
                List<int> peakHeads = new List<int>();
                if (PlayerType == "Player1" && conventionalGame.player2Pets != null)
                {
                    foreach (var pet in conventionalGame.player2Pets)
                    {
                        peakHeads.Add(pet.id);
                    }
                }
                if (PlayerType == "Player2" && conventionalGame.player1Pets != null)
                {
                    foreach (var pet in conventionalGame.player1Pets)
                    {
                        peakHeads.Add(pet.id);
                    }
                }
                ShowPeakLogger(peakHeads);
            }
            await conventionalGame.RecvWsMess(mess);
        }
    }
    public async void EmitFromSubForms(int signal)
    { // 子窗体向Main传递信号量
        switch (signal)
        {
            /*            case 1: // 设置pick底栏精灵头像
                            await InitPickHeads(); break;*/
            case 2: // 设置游戏房间号
                await InitRoomId(); break;
            case 3: // 展示12ban3界面
                ShowConventionalGame(); break;
            case 4:
                await InitPlayerType(); break;
            /*            case 5:
                            if (bagStore != null && bagStore.Count > 0)
                            {
                                var wbvmess = JsMessUtil<List<Dictionary<string, int>>>.MessJson("reStoreBag", bagStore, "endGame");
                                SendWebViewMess(wbvmess);
                            }
                            break;*/
            case 6: //掉线不重连，关闭12ban3界面
                if (conventionalGame != null && conventionalGame.Visible == true)
                {
                    conventionalGame.RecvWsMess("offLine");
                }
                break;
            case 7:
                if (peakLogger != null)
                {
                    peakLogger.petDataEnable = true;
                }
                break;
            case 8:
                if (peakLogger != null)
                {
                    peakLogger.petDataEnable = false;
                }
                break;
            case 101:

                ShowMatchDialog();
                break;
            case 102:
                ShowGenerate();
                break;
            case 103:
                ShowJoin();
                break;
            case 999:
                SgcWsHandler.CloseConnect();
                break;
            default: break;
        }
    }

    /**
     *
     */
    private async Task InitPlayerType()
    {
        PlayerType = await GameApi.GetType();
    }
    private async Task InitPickHeads()
    {
        await InitPlayerType();
        ShowFooter();
        //footer.SetPickHeads(PlayerType);
    }
    private async Task InitRoomId()
    {
        RoomId = await GameApi.GetRoomId();
        if (RoomId != 0)
        {
            //MessageBox.Show("对手已创建游戏房间");
            var mess = JsMessUtil<int>.MessJson("joinRoom", RoomId);
            Debug.WriteLine(mess);
            SendWebViewMess(mess);
        }
    }
    public void ShowConventionalGame()
    {
        var sendWsMess = new SendWsMess(SendWsMess);
        var sendWebViewMess = new SendWebViewMess(SendWebViewMess);
        var emitFromSubForms = new EmitFromSubForms(EmitFromSubForms);


        Invoke(new EventHandler(delegate
        {
            if (conventionalGame is not null)
            { //conventionalGame开启过
                if (conventionalGame.IsDisposed)
                { //conventionalGame已销毁
                    conventionalGame = new ConventionalGame(sendWsMess, sendWebViewMess, emitFromSubForms)
                    {
                        StartPosition = FormStartPosition.CenterScreen
                    };
                }
            }
            else
            { //conventionalGame未开启
                conventionalGame = new ConventionalGame(sendWsMess, sendWebViewMess, emitFromSubForms)
                {
                    StartPosition = FormStartPosition.CenterScreen
                };

            }
            conventionalGame.Show();
        }));
    }

    /**
     * 以下方法均绑定控件
     */
    private void Form_Resize(object? sender, EventArgs e)
    {
        webView.Size = ClientSize - new Size(webView.Location);
    }
    private void Location_Change(object? sender, EventArgs e)
    {
        if (peakLogger != null)
        {
            peakLogger.Location = new Point(Location.X + Size.Width, Location.Y);
        }
        if (footer != null)
        {
            footer.Location = new Point(Location.X, Location.Y + Size.Height);
        }
    }
    private void Refresh_Click(object sender, EventArgs e)
    {
        webView.Reload();
        webView.CoreWebView2.ExecuteScriptAsync(JsScript.WebViewListener);
        webView.CoreWebView2.ExecuteScriptAsync(JsScript.LoginListener);
        EmitFromSubForms(8);
    }
    /*private async void MovePetsToBag(object sender, EventArgs e)
    {
        if (!Dialog.MyWsClient.Online && !Match.MatchWsClient.Online)
        {
            MessageBox.Show(@"请先登录SGC平台！");
            return;
        }
        var ids = new List<int>();
        if (PlayerType == null || (Footer.Player1Pick == null && Footer.Player2Pick == null))
        {
            MessageBox.Show(@"请先完成bp！");
            return;
        }
        ids.AddRange(PlayerType.Equals("Player1")
            ? Footer.Player1Pick.Select(int.Parse)
            : Footer.Player2Pick.Select(int.Parse));

        var mess = JsMessUtil<List<int>>.MessJson("addToBag", ids);
        Debug.WriteLine(mess);
        webView.CoreWebView2.PostWebMessageAsJson(mess);
    }*/
    private void CreateFreeWar(object sender, EventArgs e)
    {
        var mess = JsMessUtil<string>.MessJson("createRoom", "");
        webView.CoreWebView2.PostWebMessageAsJson(mess);
    }
    private void joinRoomButton_Click(object sender, EventArgs e)
    {
        //if (!Dialog.MyWsClient.Online && !Match.MatchWsClient.Online)
        //{
        //    MessageBox.Show(@"请先登录SGC平台！");
        //    return;
        //}

        if (RoomId <= 0) return;
        var mess = JsMessUtil<int>.MessJson("joinRoom", RoomId);
        Debug.WriteLine(mess);
        webView.CoreWebView2.PostWebMessageAsJson(mess);
    }
    private void LoginDialogBtn_Click(object sender, EventArgs e)
    {
        //if (Match.MatchWsClient.Online)
        //{
        //    MessageBox.Show(@"匹配对局中无法登录！");
        //    return;
        //}
        //if (Dialog.MyWsClient.Online)
        //{ //ws还处于连接状态
        //    MessageBox.Show(@"您已登录！");
        //    return;
        //}
        var forwardWsMess = new ForwardWsMess(ForwardWsMess);
        var emitFromSubForms = new EmitFromSubForms(EmitFromSubForms);
        if (dialog is not null)
        { //dialog开启过
            if (dialog.IsDisposed)
            { //dialog已销毁
                dialog = new Dialog(forwardWsMess, emitFromSubForms)
                {
                    StartPosition = FormStartPosition.CenterScreen
                };
                dialog.Show();
            }
            else
            { //dialog未销毁，在后台
                dialog.Show();
            }
        }
        else
        { //dialog未开启
            dialog = new Dialog(forwardWsMess, emitFromSubForms)
            {
                StartPosition = FormStartPosition.CenterScreen
            };
            dialog.Show();
        }
    }
    private void GenerateGameButton_Click(object sender, EventArgs e)
    {
        var mess = JsMessUtil<int>.MessJson("getSelfMimiId", 0, "generateGame");
        webView.CoreWebView2.PostWebMessageAsJson(mess);

        //var emitFromSubForms = new EmitFromSubForms(EmitFromSubForms);
        //if (!Dialog.MyWsClient.Online)
        //{
        //    MessageBox.Show(@"请先登录SGC平台！");
        //    return;
        //}
        //if (!await ConventionalGameApi.CheckGameState())
        //{
        //    MessageBox.Show(@"处于比赛中！");
        //    ShowConventionalGame();
        //    return;
        //}
        //if (generateConventionalGame is not null)
        //{
        //    if (generateConventionalGame.IsDisposed)
        //    {

        //        generateConventionalGame = new GenerateConventionalGame(emitFromSubForms)
        //        {
        //            StartPosition = FormStartPosition.CenterScreen
        //        };
        //        generateConventionalGame.Show();
        //    }
        //    else
        //    {
        //        generateConventionalGame.Show();
        //    }
        //}
        //else
        //{
        //    generateConventionalGame = new GenerateConventionalGame(emitFromSubForms)
        //    {
        //        StartPosition = FormStartPosition.CenterScreen
        //    };
        //    generateConventionalGame.Show();
        //}
    }

    private void ShowGenerate()
    {
        var emitFromSubForms = new EmitFromSubForms(EmitFromSubForms);
        Invoke(new EventHandler(delegate
        {
            if (generateConventionalGame is not null)
            {
                if (generateConventionalGame.IsDisposed)
                {

                    generateConventionalGame = new GenerateConventionalGame(emitFromSubForms)
                    {
                        StartPosition = FormStartPosition.CenterScreen
                    };
                    generateConventionalGame.Show();
                }
                else
                {
                    generateConventionalGame.Show();
                }
            }
            else
            {
                generateConventionalGame = new GenerateConventionalGame(emitFromSubForms)
                {
                    StartPosition = FormStartPosition.CenterScreen
                };
                generateConventionalGame.Show();
            }
        }));
    }

    private void JoinGameButton_Click(object sender, EventArgs e)
    {
        var mess = JsMessUtil<int>.MessJson("getSelfMimiId", 0, "joinGame");
        webView.CoreWebView2.PostWebMessageAsJson(mess);
        //var emitFromSubForms = new EmitFromSubForms(EmitFromSubForms);
        //if (!Dialog.MyWsClient.Online)
        //{
        //    MessageBox.Show(@"请先登录SGC平台！");
        //    return;
        //}
        //if (!await ConventionalGameApi.CheckGameState())
        //{
        //    MessageBox.Show(@"处于比赛中！");
        //    ShowConventionalGame();
        //    return;
        //}
        //if (joinConventionalGame is not null)
        //{
        //    if (joinConventionalGame.IsDisposed)
        //    {
        //        joinConventionalGame = new JoinConventionalGame(emitFromSubForms)
        //        {
        //            StartPosition = FormStartPosition.CenterScreen
        //        };
        //        joinConventionalGame.Show();
        //    }
        //    else
        //    {
        //        joinConventionalGame.Show();
        //    }
        //}
        //else
        //{
        //    joinConventionalGame = new JoinConventionalGame(emitFromSubForms)
        //    {
        //        StartPosition = FormStartPosition.CenterScreen
        //    };
        //    joinConventionalGame.Show();
        //}
    }
    private void ShowJoin()
    {
        var emitFromSubForms = new EmitFromSubForms(EmitFromSubForms);
        Invoke(new EventHandler(delegate
        {
            if (joinConventionalGame is not null)
            {
                if (joinConventionalGame.IsDisposed)
                {
                    joinConventionalGame = new JoinConventionalGame(emitFromSubForms)
                    {
                        StartPosition = FormStartPosition.CenterScreen
                    };
                    joinConventionalGame.Show();
                }
                else
                {
                    joinConventionalGame.Show();
                }
            }
            else
            {
                joinConventionalGame = new JoinConventionalGame(emitFromSubForms)
                {
                    StartPosition = FormStartPosition.CenterScreen
                };
                joinConventionalGame.Show();
            }
        }));
    }

    private void MenuItem_Mute_Click(object sender, EventArgs e)
    {
        webView.CoreWebView2.IsMuted = webView.CoreWebView2.IsMuted == false;
    }
    private void ShowFooter()
    {
        var sendWebViewMess = new SendWebViewMess(SendWebViewMess);
        if (footer is not null)
        {
            if (footer.IsDisposed)
            {
                footer = new Footer(sendWebViewMess);
                footer.Show();
                footer.Location = new Point(Location.X, Location.Y + Size.Height);
                //footer.SetPickHeads(PlayerType);
            }
            else
            {
                footer.Show();
                footer.Location = new Point(Location.X, Location.Y + Size.Height);
                footer.SetPickHeads();
            }
        }
        else
        {
            footer = new Footer(sendWebViewMess);
            footer.Show();
            footer.Location = new Point(Location.X, Location.Y + Size.Height);
            //footer.SetPickHeads(PlayerType);
        }
    }

    private void ShowPeakLogger(List<int>? ids)
    {
        if (peakLogger is not null)
        {
            if (peakLogger.IsDisposed)
            {
                peakLogger = new PeakLogger();
                peakLogger.Show();
                peakLogger.Location = new Point(Location.X + Size.Width, Location.Y);
                if (ids != null) peakLogger.SetRivalPetHeads(ids);
            }
            else
            {
                peakLogger.Show();
                peakLogger.Location = new Point(Location.X + Size.Width, Location.Y);
                if (ids != null) peakLogger.SetRivalPetHeads(ids);
            }
        }
        else
        {
            peakLogger = new PeakLogger();
            peakLogger.Show();
            peakLogger.Location = new Point(Location.X + Size.Width, Location.Y);
            if (ids != null) peakLogger.SetRivalPetHeads(ids);
        }
    }
    private void PeakLogger_ToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (peakLogger is not null && peakLogger.Visible)
        {
            peakLogger.Close();
        }
        else
        {
            ShowPeakLogger(null);
        }
    }

    private void TestToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Debug.WriteLine("————————测试内容————————");

        var userid = AesUtil.AesEncrypt("jiuqing1");
        Debug.WriteLine($"{userid}");

        Debug.WriteLine("————————测试内容————————");
    }

    private void RivalMimiToolStripMenuItem_Click(object sender, EventArgs e)
    {
        //if (!Dialog.MyWsClient.Online && !Match.MatchWsClient.Online)
        //{
        //    MessageBox.Show(@"请先登录SGC平台！");
        //    return;
        //}
        var mess = JsMessUtil<int>.MessJson("getRivalMimiId", 0);
        webView.CoreWebView2.PostWebMessageAsJson(mess);
    }

    private void MatchToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var mess = JsMessUtil<int>.MessJson("getSelfMimiId", 0, "matchVerify");
        webView.CoreWebView2.PostWebMessageAsJson(mess);
    }

    private void ShowMatchDialog()
    {
        //if (Dialog.MyWsClient.Online)
        //{
        //    MessageBox.Show(@"SGC登录状态下不能匹配！请使用创建对战");
        //    return;
        //}
        var forwardWsMess = new ForwardWsMess(ForwardWsMess);
        var emitFromSubForms = new EmitFromSubForms(EmitFromSubForms);

        Invoke(new EventHandler(delegate
        {
            if (match is not null)
            {
                if (match.IsDisposed)
                {
                    match = new Match(forwardWsMess, emitFromSubForms)
                    {
                        StartPosition = FormStartPosition.CenterScreen
                    };
                    match.Show();
                }
                else
                {
                    match.Dispose();
                    match = new Match(forwardWsMess, emitFromSubForms)
                    {
                        StartPosition = FormStartPosition.CenterScreen
                    };
                    match.Show();
                }
            }
            else
            {
                match = new Match(forwardWsMess, emitFromSubForms)
                {
                    StartPosition = FormStartPosition.CenterScreen
                };
                match.Show();
            }

            var wbvmess = JsMessUtil<int>.MessJson("getSuit", 0, "matchVerify");
            webView.CoreWebView2.PostWebMessageAsJson(wbvmess);
        }));
    }

    private void LogoutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        //if (Dialog.MyWsClient.Online)
        //{
        //    Dialog.needReconnect = false;
        //    Dialog.MyWsClient.Close();
        //    Dialog.MyWsClient.Dispose();
        //    dialog.Dispose();
        //}
    }

    /*    private void RestoreBagMenuItem_Click(object sender, EventArgs e)
        {
            if (bagStore != null && bagStore.Count > 0)
            {
                var wbvmess = JsMessUtil<List<Dictionary<string, int>>>.MessJson("reStoreBag", bagStore);
                webView.CoreWebView2.PostWebMessageAsJson(wbvmess);
            }
        }*/

    private void SendEliteInfoItem_Click(object sender, EventArgs e)
    {
        //MessageBox.Show("程序员赶工中");
        //if (!Dialog.MyWsClient.Online)
        //{
        //    MessageBox.Show(@"请先登录SGC平台！");
        //    return;
        //}
        var wbvmess = JsMessUtil<int>.MessJson("getElite", 0);
        SendWebViewMess(wbvmess);
    }

    private void RemoveCacheItem_Click(object sender, EventArgs e)
    {
        Debug.WriteLine(MiMiId);
        if (MiMiId == null)
        {
            MessageBox.Show("请先登录游戏");
        }
        else
        {
            ConventionalGameApi.RemoveGameCache(MiMiId);
        }
    }

    void MainForm_KeyDown(object sender, KeyEventArgs e)
    {
        if (peakLogger != null)
        {
            peakLogger.PeakLogger_KeyDown(e);
        }
    }

    private void ShowBagStore()
    {
        var sendWebViewMess = new SendWebViewMess(SendWebViewMess);
        if (bagStore is not null)
        {
            if (bagStore.IsDisposed)
            {
                bagStore = new BagStoreForm(sendWebViewMess);
                bagStore.Show();
                bagStore.Location = new Point(Location.X + Size.Width / 4, Location.Y);
            }
            else
            {
                bagStore.Show();
                bagStore.Location = new Point(Location.X + Size.Width / 4, Location.Y);
            }
        }
        else
        {
            bagStore = new BagStoreForm(sendWebViewMess);
            bagStore.Show();
            bagStore.Location = new Point(Location.X + Size.Width / 4, Location.Y);
        }
    }

    private void BagStoreToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (bagStore is not null && bagStore.Visible)
        {
            bagStore.Close();
        }
        else
        {
            ShowBagStore();
        }
    }

    private void ShowpickToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (footer is not null && footer.Visible)
        {
            footer.Close();
        }
        //else if (!Dialog.MyWsClient.Online && !Match.MatchWsClient.Online)
        //{
        //    MessageBox.Show(@"请先登录SGC平台！");
        //    return;
        //}
        else
        {
            ShowFooter();
        }
    }
}
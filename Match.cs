using System.Diagnostics;
using System.Security.Authentication;
using System.Text.Json;
using Seer.api;
using Seer.DTO;
using Seer.Utils;
using TouchSocket.Core;
using TouchSocket.Http.WebSockets;
using TouchSocket.Sockets;

namespace Seer;

public partial class Match : Form
{
    private readonly ForwardWsMess _forwardWsMess;
    private readonly EmitFromSubForms _emitFromSubForms;

    //private const string MatchHost = Constant.MatchHost;
    public static WebSocketClient MatchWsClient { get; set; } = new();
    public static bool needReconnect { get; set; } = true;
    private static bool antiShake { get; set; } = false; // 防抖变量

    public static int _seerAccount { get; set; }

    private List<Pet>? bagPets { get; set; }
    private int suitId { get; set; }

    private string _wsUrl { get; set; }

    //private static Thread proThread { get; set; }
    //private static int heartbeatMiss;


    public Match(ForwardWsMess forwardWsMess, EmitFromSubForms emitFromSubForms, int seeraccount)
    {
        _forwardWsMess = forwardWsMess;
        _emitFromSubForms = emitFromSubForms;
        _seerAccount = seeraccount;
        InitializeComponent();
        CheckForIllegalCrossThreadCalls = false;
    }

    private void AfterConnect(ITcpClient client, MsgEventArgs e)
    {
        Debug.WriteLine("Ws has connected");
        // 添加头部
        HttpClientInterceptor.Set_Userid("seeraccount" + _seerAccount);
        antiShake = false;
    }

    private void ReceiveHandle(WebSocketClient c, WSDataFrame e)
    {
        switch (e.Opcode)
        {
            case WSDataType.Cont:
                break;
            case WSDataType.Text:
                if (!e.ToText().Contains("heartbeat"))
                {
                    Debug.WriteLine("Ws mess recv:" + e.ToText());
                    if (e.ToText().Contains("token"))
                    {
                        // 添加token头部
                        HttpClientInterceptor.Set_Token(e.ToText()[6..]);
                        HttpClientInterceptor.Set_Host("http" + Constant.MatchHost);
                        AfterHand();
                        needReconnect = true;
                        /*proThread = new Thread(() =>
                        {
                            try
                            {
                                while (MatchWsClient.Online)
                                {
                                    SendFromMatchWS("heartbeat");
                                    Thread.Sleep(1000);
                                    Interlocked.Increment(ref heartbeatMiss);

                                    if (heartbeatMiss > 4)
                                    {
                                        MatchWsClient.Close();
                                        MatchWsClient.Dispose();
                                    }
                                }
                            }
                            catch (ThreadInterruptedException ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }
                        });
                        proThread.Start();*/
                    }
                    else if (e.ToText().Contains("GameInit") || e.ToText().Contains("Conventional"))
                    {
                        _emitFromSubForms(1);
                        _emitFromSubForms(2);
                        _emitFromSubForms(4);
                    }
                    else if (e.ToText().Contains("RoomId"))
                    {
                        _emitFromSubForms(2);
                    }
                    else if (e.ToText() == "SuccessQuitMatch")
                    {
                        this.Visible = false;
                        needReconnect = false;
                        MatchWsClient.Close();
                    }
                    else if (e.ToText() == "onMatch")
                    {
                        _emitFromSubForms(3);
                        _forwardWsMess(e.ToText());
                        this.Visible = false;
                    }
                    else if (e.ToText() == "PlayerBanned")
                    {
                        MessageBox.Show("因为违反竞技规则，你已经被禁止进行匹配对局");
                        this.Visible = false;
                        needReconnect = false;
                        MatchWsClient.Close();
                    }
                    else if (e.ToText() == "RacePlayerNotFound")
                    {
                        MessageBox.Show("该轮次未导入此米米号");
                        this.Visible = false;
                        needReconnect = false;
                        MatchWsClient.Close();
                    }
                    else if (e.ToText() == "RacePlayerMaxCount")
                    {
                        MessageBox.Show("该轮次此米米号已经达到最大场次");
                        this.Visible = false;
                        needReconnect = false;
                        MatchWsClient.Close();
                    }
                    else
                    {
                        _forwardWsMess(e.ToText());
                    }

                    if (e.ToText().Contains("WaitingPeriodResult"))
                    {
                        _emitFromSubForms(7);
                    }
                    if (e.ToText().Contains("endGame"))
                    {
                        _emitFromSubForms(8);
                    }
                }
                break;
            case WSDataType.Binary:
                break;
            case WSDataType.Close:
                break;
            case WSDataType.Ping:
                break;
            case WSDataType.Pong:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void DisconnectHandle(ITcpClientBase client, DisconnectEventArgs e)
    {
        Debug.WriteLine("Ws disconnected");
        //proThread.Interrupt();
        if (!MatchWsClient.Online && needReconnect)
        {
            
            if (MessageBox.Show("匹配掉线，是否重连", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                StartMatch();
            }
            else
            {
                needReconnect = false;
                _emitFromSubForms(6);
                this.Close();
            }
            
            //MessageBox.Show("与匹配服务器的连接断开");
        }
        
        
        /*
        while (MatchWsClient != null && !MatchWsClient.Online && needReconnect)
        {
            Debug.WriteLine("Ws try to reconnect");
            try
            {
                MatchWsClient.Connect();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        */
    }

    private void StartMatch()
    {
        if (antiShake)
        {
            return;
        }
        antiShake = true;
        //Interlocked.Exchange(ref heartbeatMiss, 0);

        MatchWsClient.Close();
        MatchWsClient.Dispose();

        MatchWsClient = new();

        string url = $"ws{Constant.MatchHost}/loginer"; // Replace with the actual URL for the POST request.

        _wsUrl = url + "?userid=seeraccount" + _seerAccount + "&version=" + Constant._version;

        var config = Constant.GetWsConfig(_wsUrl);

        MatchWsClient.Setup(config);

        MatchWsClient.Connected += AfterConnect;

        MatchWsClient.Received += ReceiveHandle;

        MatchWsClient.Disconnected += DisconnectHandle;

        try
        {
            MatchWsClient.Connect();

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            antiShake = false;
        }
    }

    private void cancalButton_Click(object sender, EventArgs e)
    {
        if (MatchWsClient.Online)
        {
            SendFromMatchWS("QuitMatch");
        } 
        else
        {
            needReconnect = false;
            Close();
        }
    }

    public bool RecvWebViewMess(Dictionary<string, JsonElement> mess)
    {
        if (mess.GetValue("type").Deserialize<string>().Contains("bagInfo"))
        {
            bagPets = mess["data2"].Deserialize<List<Pet>>();
            if (PetUtil.CheckBag(bagPets))
            {
                StartMatch();
            }
        }
        else if (mess.GetValue("type").Deserialize<string>().Contains("suitId"))
        {
            var id = mess.GetValue("data").Deserialize<int>();
            if (id == 0)
            {
                MessageBox.Show(@"请穿上套装！");
                this.Visible = false;
                return false;
            }
            suitId = id;
        }
        return true;
    }

    private async void AfterHand()
    {
        if (!await ConventionalGameApi.CheckGameState())
        {
            _emitFromSubForms(3);
            this.Visible = false;
            return;
        }
        Dictionary<string, object> param = new Dictionary<string, object>
        {
            { "bagInfo", bagPets },
            { "matchGame", true }
        };
        string? bagmess = await ConventionalGameApi.VerifyBag2(param);
        if (bagmess != null)
        {
            MessageBox.Show(bagmess);
            this.Visible = false;
            needReconnect = false;
            MatchWsClient.Close();
            return;
        }


        Dictionary<string, object> suitVo = new Dictionary<string, object>
        {
            { "suitId", suitId },
            { "matchGame", true }
        };
        string? suitmess = await ConventionalGameApi.VerifySuit(suitVo);
        if (suitmess != null)
        {
            MessageBox.Show(suitmess);
            this.Visible = false;
            needReconnect = false;
            MatchWsClient.Close();
            return;
        }

        matchStateLabel.Text = "匹配中。。。";
        SendFromMatchWS("JoinMatch");
    }

    public static void SendFromMatchWS(string mess)
    {
        lock(MatchWsClient)
        {
            MatchWsClient.SendWithWS(mess);
        }
    }


    private void CloseAll(object? sender, FormClosedEventArgs e)
    {
        needReconnect = false;
        if (MatchWsClient.Online)
        {
            SendFromMatchWS("QuitMatch");
        }
    }
}
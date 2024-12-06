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

public partial class Dialog : Form
{
    //private readonly SetGameInfo _setGameInfo;
    //private readonly SetRoomId _setRoomId;
    private readonly ForwardWsMess _forwardWsMess;
    private readonly EmitFromSubForms _emitFromSubForms;

    //private const string Host = Constant.Host;
    //public static WebSocketClient MyWsClient { get; set; } = new();
    //public static bool needReconnect { get; set; } = true;
    //private static bool antiShake { get; set; } = false;

    //private static Thread proThread { get; set; }
    //private static int heartbeatMiss;


    //private static string _wsUrl { get; set; }

    public Dialog(ForwardWsMess forwardWsMess, EmitFromSubForms emitFromSubForms)
    {
        _forwardWsMess = forwardWsMess;
        _emitFromSubForms = emitFromSubForms;
        InitializeComponent();
        CheckForIllegalCrossThreadCalls = false;

        checkRemenberPasswd();
    }

    private void checkRemenberPasswd()
    {
        /*        string fileName = "store.json";
                if (File.Exists(Application.StartupPath + fileName))
                {
                    FileStream fsr = new FileStream(Application.StartupPath + fileName, FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fsr);
                    var store = JsonSerializer.Deserialize<Store>(sr.ReadLine());
                    fsr.Close();
                    sr.Close();
                    if (store != null)
                    {
                        idTextBox.Text = store.userid;
                        pwTextBox.Text = store.passwd;
                    }
                }*/
        var store = StoreUtil.getStore();
        if (store != null)
        {
            idTextBox.Text = store.userid;
            pwTextBox.Text = store.passwd;
        }
    }

    //private void AfterConnect(ITcpClient client, MsgEventArgs e)
    //{
    //    Debug.WriteLine("Ws has connected");
    //    var id = idTextBox.Text;
    //    // 添加头部
    //    HttpClientInterceptor.Set_Userid(id);
    //    HttpClientInterceptor.GetTokenEvent += Connect;
    //}

    //private async void Connect()
    //{
    //    //var nickname = await UserInfoApi.GetNickName();
    //    //idLabel.Text = @"欢迎，" + nickname;
    //    idLabel.Text = @"欢迎";
    //    Controls.Remove(loginButton);
    //    Controls.Remove(idTextBox);
    //    Controls.Remove(pwLabel);
    //    Controls.Remove(pwTextBox);

    //    //antiShake = false;
    //    await Task.Delay(1000);
    //    this.Visible = false;
    //}
    //private void ReceiveHandle(WebSocketClient c, WSDataFrame e)
    //{
    //    switch (e.Opcode)
    //    {
    //        case WSDataType.Cont:
    //            break;
    //        case WSDataType.Text:
    //            if (!e.ToText().Contains("heartbeat"))
    //            {
    //                Debug.WriteLine("Ws mess recv:" + e.ToText());
    //                if (e.ToText().Contains("token"))
    //                {
    //                    // 添加token头部
    //                    HttpClientInterceptor.Set_Token(e.ToText()[6..]);
    //                    HttpClientInterceptor.Set_Host("http" + Constant.Host);
    //                    needReconnect = true;
    //                    /*proThread = new Thread(() =>
    //                    {
    //                        while (MyWsClient.Online)
    //                        {
    //                            MyWsClient.SendWithWS("heartbeat");
    //                            Thread.Sleep(1000);

    //                            Interlocked.Increment(ref heartbeatMiss);

    //                            if (heartbeatMiss > 4)
    //                            {
    //                                MyWsClient.Close();
    //                                MyWsClient.Dispose();
    //                            }
    //                        }
    //                    });
    //                    proThread.Start();*/
    //                }
    //                else if (e.ToText().Contains("GameInit") || e.ToText().Contains("Conventional"))
    //                {
    //                    _emitFromSubForms(1);
    //                    _emitFromSubForms(2);
    //                    _emitFromSubForms(4);
    //                }
    //                else if (e.ToText().Contains("RoomId"))
    //                {
    //                    _emitFromSubForms(2);
    //                }
    //                else
    //                {
    //                    _forwardWsMess(e.ToText());
    //                }
    //            }
    //            break;
    //        case WSDataType.Binary:
    //            break;
    //        case WSDataType.Close:
    //            break;
    //        case WSDataType.Ping:
    //            break;
    //        case WSDataType.Pong:
    //            break;
    //        default:
    //            throw new ArgumentOutOfRangeException();
    //    }
    //}

    //private void DisconnectHandle(ITcpClientBase client, DisconnectEventArgs e)
    //{
    //    Debug.WriteLine("Ws disconnected");
    //    //antiShake = false;

    //    /*
    //    if (!MyWsClient.Online && needReconnect)
    //    {

    //        if (MessageBox.Show("登录掉线，是否重连", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
    //        {
    //            Re_Connect();
    //        }
    //        else
    //        {
    //            needReconnect = false;
    //            _emitFromSubForms(6);
    //        }

    //        //MessageBox.Show("与匹配服务器的连接断开");
    //    }
    //    */
    //}

    private async void Login()
    {
        //if (antiShake)
        //{
        //    return;
        //}

        var id = idTextBox.Text;
        var passwd = pwTextBox.Text;
        //bool loginBool = await LoginerApi.Login(new Dictionary<string, string>
        //{
        //    { "userid", id }, { "password", passwd }
        //});
        bool loginBool = await LoginerApi.VerifyPass(new Dictionary<string, string>
        {
            { "userid", id }, { "password", passwd }
        });
        if (!loginBool)
        {
            //antiShake = false;
            return;
        }
        var store = StoreUtil.getStore();
        if (store != null)
        {
            store.userid = id;
            store.passwd = passwd;
        }
        else
        {
            store = new();
        }
        StoreUtil.setStore(store);

        //if (rememberPwCheck.Checked)
        //{
        //    /*string fileName = "store.json";
        //    if (!File.Exists(Application.StartupPath + fileName))
        //    {
        //        MessageBox.Show("配置文件store.json丢失，请重启登录器");
        //    }
        //    else
        //    {
        //        FileStream fsr = new FileStream(Application.StartupPath + fileName, FileMode.Open, FileAccess.Read);
        //        StreamReader sr = new StreamReader(fsr);
        //        var store = JsonSerializer.Deserialize<Store>(sr.ReadLine());
        //        fsr.Close();
        //        sr.Close();

        //        if (store == null)
        //        {
        //            store = new Store();
        //        }
        //        store.userid = id;
        //        store.passwd = passwd;

        //        FileStream fsw = new FileStream(Application.StartupPath + fileName, FileMode.Open, FileAccess.Write);
        //        StreamWriter sw = new StreamWriter(fsw);


        //        sw.WriteLine(JsonSerializer.Serialize(store));

        //        sw.Close();
        //        fsw.Close();
        //    }*/
        //    var store = StoreUtil.getStore();
        //    if (store != null)
        //    {
        //        store.userid = id;
        //        store.passwd = passwd;
        //    }
        //    else
        //    {
        //        store = new();
        //    }
        //    StoreUtil.setStore(store);
        //}

        //MyWsClient.Close();
        //MyWsClient.Dispose();

        //MyWsClient = new();

        //string url = $"ws{Constant.Host}/loginer"; // Replace with the actual URL for the POST request.

        //// Create the data to be sent in the request body.


        //_wsUrl = url + "?userid=" + id + "&password=" + passwd + "&version=" + Constant._version;

        //var config = Constant.GetWsConfig(_wsUrl);

        //MyWsClient.Setup(config);

        //MyWsClient.Connected += AfterConnect;

        //MyWsClient.Received += ReceiveHandle;

        //MyWsClient.Disconnected += DisconnectHandle;

        //try
        //{
        //    MyWsClient.Connect();
        //}
        //catch (WebSocketConnectException ex)
        //{
        //    //antiShake = false;
        //    Debug.WriteLine(ex.Message);
        //}
    }
    
    /*
    private void Re_Connect()
    {
        if (antiShake)
        {
            return;
        }
        antiShake = true;

        MyWsClient.Close();
        MyWsClient.Dispose();

        MyWsClient = new();

        var config = Constant.GetWsConfig(_wsUrl);

        MyWsClient.Setup(config);

        MyWsClient.Connected += AfterConnect;

        MyWsClient.Received += ReceiveHandle;

        MyWsClient.Disconnected += DisconnectHandle;

        try
        {
            MyWsClient.Connect();
        }
        catch (WebSocketConnectException ex)
        {
            antiShake = false;
            Debug.WriteLine(ex.Message);
        }
    }
    */

    private void Aside_Load(object sender, EventArgs e)
    {

    }

    private void idTextBox_TextChanged(object sender, EventArgs e)
    {

    }

    private void pwTextBox_TextChanged(object sender, EventArgs e)
    {

    }

    private void pwTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            Login();
        }
    }

    private void loginButton_Click(object sender, EventArgs e)
    {
        Login();
    }

    private void form_Activated(object sender, EventArgs e)
    {
        idTextBox.Focus();
    }
}
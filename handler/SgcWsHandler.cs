using Seer.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Http.WebSockets;
using TouchSocket.Sockets;
using static Seer.DTO.Store;

namespace Seer.handler
{
    internal class SgcWsHandler
    {
        private readonly ForwardWsMess _forwardWsMess;
        private readonly EmitFromSubForms _emitFromSubForms;

        public static WebSocketClient WsClient { get; set; } = new();
        public static bool _online { get; set; } = false;
        public static int _seerAccount { get; set; }
        public static string _modMark { get; set; } = "";
        

        public SgcWsHandler(ForwardWsMess forwardWsMess, EmitFromSubForms emitFromSubForms)
        {
            _forwardWsMess = forwardWsMess;
            _emitFromSubForms = emitFromSubForms;
        }

        private void AfterConnect(ITcpClient client, MsgEventArgs e)
        {
            Debug.WriteLine("Ws has connected");
            _online = true;
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
                            HttpClientInterceptor.Set_Userid("seeraccount" + _seerAccount);
                            HttpClientInterceptor.Set_Token(e.ToText()[6..]);
                            HttpClientInterceptor.Set_Host("http" + Constant.MatchHost);
                            SendMess($"tokenGot{_modMark}");
                        }
                        else if (e.ToText().Equals("matchJoin"))
                        {
                            _emitFromSubForms(101);
                        }
                        else if (e.ToText().Equals("gameCreate"))
                        {
                            _emitFromSubForms(102);
                        }
                        else if (e.ToText().Equals("gameJoin"))
                        {
                            _emitFromSubForms(103);
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
                            WsClient.Close();
                        }
                        else if (e.ToText() == "onMatch")
                        {
                            _emitFromSubForms(3);
                            _emitFromSubForms(5);
                            _forwardWsMess(e.ToText());
                        }
                        else if (e.ToText() == "PlayerBanned")
                        {
                            MessageBox.Show("因为违反竞技规则，你已经被禁止进行匹配对局");
                            WsClient.Close();
                        }
                        else if (e.ToText() == "RacePlayerNotFound")
                        {
                            MessageBox.Show("该轮次未导入此米米号");
                            WsClient.Close();
                        }
                        else if (e.ToText() == "RacePlayerMaxCount")
                        {
                            MessageBox.Show("该轮次此米米号已经达到最大场次");
                            WsClient.Close();
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
                            WsClient.Close();
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
            _online = false;

        }

        public void StartConnect(int seerAccount, string modMark)
        {
            _seerAccount = seerAccount;
            _modMark = modMark;

            if (_online)
            {
                return;
            }

            if (WsClient != null)
            {
                WsClient.Dispose();
            }
            WsClient = new();
            string url = $"ws{Constant.MatchHost}/loginer?version={Constant._version}&userid=seeraccount{_seerAccount}"; 
            var config = Constant.GetWsConfig(url);
            WsClient.Setup(config);
            WsClient.Connected += AfterConnect;
            WsClient.Received += ReceiveHandle;
            WsClient.Disconnected += DisconnectHandle;
            try
            {
                WsClient.Connect();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static void SendMess(string mess)
        {
            if (_online)
            {
                WsClient.SendWithWS(mess);
            }
        }

        public static void CloseConnect()
        {
            if (_online)
            {
                WsClient.Close();
            }
        }


    }
}

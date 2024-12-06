using Microsoft.VisualBasic.ApplicationServices;
using Seer.api;
using Seer.DTO;
using Seer.handler;
using Seer.Utils;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Runtime.CompilerServices;
using System.Text.Json;
using TouchSocket.Core;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Seer
{
    public partial class ConventionalGame : Form
    {
        private readonly SendWsMess _sendWsMess;
        private readonly SendWebViewMess _sendWebViewMess;
        private readonly EmitFromSubForms _emitFromSubForms;

        public string? _player1SuitId { get; set; }
        public string? _player2SuitId { get; set; }

        private string? _phase { get; set; }
        private string? _type { get; set; }
        private int _countDown { get; set; } = 0;

        private bool _clickAble { get; set; } = false;


        public List<Pet>? player1Pets { get; set; }
        public List<Pet>? player2Pets { get; set; }

        private int _banNum { get; set; }
        private int _banCount { get; set; } = 0;
        private int _pickCount;

        private bool banOver { get; set; } = false;
        private bool firstOver { get; set; } = false;
        private bool remainOver { get; set; } = false;

        private void pickIncre()
        {
            Interlocked.Increment(ref _pickCount);
            Debug.WriteLine("当前线程名称: " + Thread.CurrentThread.Name);
            Debug.WriteLine("pickCount:" + _pickCount);
        }
        private void pickDecre()
        {
            Interlocked.Decrement(ref _pickCount);
            Debug.WriteLine("当前线程名称: " + Thread.CurrentThread.Name);
            Debug.WriteLine("pickCount:" + _pickCount);
        }


        public ConventionalGame(SendWsMess sendWsMess, SendWebViewMess sendWebViewMess, EmitFromSubForms emitFromSubForms)
        {
            _sendWsMess = sendWsMess;
            _sendWebViewMess = sendWebViewMess;
            _emitFromSubForms = emitFromSubForms;

            InitializeComponent();
            Init();
        }

        public async void ReSet()
        {
            banOver = false;
            firstOver = false;
            remainOver = false;
            //_banCount = 0;
            //_pickCount = 0;
        }

        private async Task Init()
        {
            await InitType();
            if (_type == null)
            {
                return;
            }
            _emitFromSubForms(4);
            await InitPhase();

            InitNickName();
            
            InitSuit();
            InitCountDown();
            InitBanNum();
            ReSet();

            bool isplayer1 = _type == "Player1";
            exitGameButton.Text = isplayer1 ? "关闭对战" : "退出对战";

            var mess1 = JsMessUtil<object>.MessJson("getBag", 0);
            
            switch (_phase)
            {
                case "WaitingStage":
                    gameTip.Text = "等待加入";
                    readyButton.Enabled = false;
                    startButton.Enabled = false;
                    _clickAble = false;
                    _sendWebViewMess(mess1);
                    
                    break;
                case "PreparationStage":
                    gameTip.Text = "等待准备";
                    readyButton.Enabled = isplayer1 ? false : true;
                    startButton.Enabled = false;
                    _clickAble = false;
                    _sendWebViewMess(mess1);
                    //ReSet();
                    break;
                case "ReadyStage":
                    gameTip.Text = isplayer1 ? "对方已准备" : "等待开始";
                    readyButton.Enabled = false;
                    startButton.Enabled = isplayer1 ? true : false;
                    _clickAble = false;
                    _sendWebViewMess(mess1);
                    //ReSet();
                    break;
                case "PlayerBanElf":
                    gameTip.Text = "ban精灵";
                    readyButton.Enabled = false;
                    startButton.Enabled = false;
                    await GetBags();
                    _clickAble = !banOver;
                    break;
                case "PlayerPickElfFirst":
                    gameTip.Text = "pick首发";
                    readyButton.Enabled = false;
                    startButton.Enabled = false;
                    await GetBags();
                    _clickAble = !firstOver;
                    break;
                case "PlayerPickElfRemain":
                    gameTip.Text = "pick其他";
                    readyButton.Enabled = false;
                    startButton.Enabled = false;
                    await GetBags();
                    _clickAble = !remainOver;
                    break;
                case "WaitingPeriodResult":
                    gameTip.Text = "等待结果";
                    readyButton.Enabled = false;
                    startButton.Enabled = false;
                    await GetBags();
                    _clickAble = false;
                    break;
                default:
                    gameTip.Text = string.Empty; break;
            }
        }

        public async Task RecvWsMess(string mess)
        {
            if (mess.Contains("PlayerBan") || mess.Contains("PlayerPick"))
            {
                _clickAble = true;
            }
            if (mess.Contains("matchGameOver"))
            {
                //_emitFromSubForms(5);
            }
            if (mess.Contains("offLine"))
            {
                SgcWsHandler.CloseConnect();
                MessageBox.Show("有一方退出或掉线", "掉线", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                this.Close();
                this.Dispose();
            }
            if (mess == "quitRoom")
            {
                var mess2 = JsMessUtil<int>.MessJson("quitRoom", 0);
                _sendWebViewMess(mess2);
                SgcWsHandler.CloseConnect();
                this.Close();
                this.Dispose();
            }
            if (mess.Contains("shutRoom"))
            {
                var mess2 = JsMessUtil<int>.MessJson("shutRoom", 0);
                _sendWebViewMess(mess2);
                SgcWsHandler.CloseConnect();
                this.Close();
                this.Dispose();
            }

            if (mess == "BanOver")
            {
                banOver = true;
                _clickAble = false;
            }
            else if (mess == "FirstOver")
            {
                firstOver = true;
                _clickAble = false;
            }
            else if (mess == "RemainOver")
            {
                remainOver = true;
                _clickAble = false;
            }
            else if (mess.Contains("endGame"))
            {
                this.Close();
                this.Dispose();
            }
            else
            {
                if (mess.Contains("WaitingPeriodResult"))
                {
                    await Init();
                    // bp结束，更换背包
                    Dictionary<int, Pet> bag = new();
                    if (_type == "Player1")
                    {
                        if (player1Pets == null) return;
                        int count = 1;
                        foreach (var pet in player1Pets)
                        {
                            if (pet.state == 2)
                            {
                                bag[0] = pet;
                                bag[pet.catchTime] = pet;
                            }
                            if (pet.state == 3)
                            {
                                bag[count] = pet;
                                bag[pet.catchTime] = pet;
                                count++;
                            }
                        }
                    }

                    if (_type == "Player2")
                    {
                        if (player2Pets == null) return;
                        int count = 1;
                        foreach (var pet in player2Pets)
                        {
                            if (pet.state == 2)
                            {
                                bag[0] = pet;
                                bag[pet.catchTime] = pet;
                            }
                            if (pet.state == 3)
                            {
                                bag[count] = pet;
                                bag[pet.catchTime] = pet;
                                count++;
                            }
                        }
                    }

                    if (bag.Count < 12)
                    {
                        MessageBox.Show("程序错误导致出战数量小于6，请手动结束此对局");
                    } 
                    else
                    {
                        var wmess = JsMessUtil<Dictionary<int, Pet>>.MessJson("addToBagFull", bag);
                        _sendWebViewMess(wmess);
                        _emitFromSubForms(1);
                        _emitFromSubForms(4);

                        if (_type == "Player1")
                        {
                            // bp结束，房主创建房间
                            var cmess = JsMessUtil<int>.MessJson("createRoom", 0);
                            _sendWebViewMess(cmess);
                        }
                    }
                    
                }
                else
                {
                    await Init();
                }
            }
        }

        public void RecvWebViewMess(Dictionary<string, JsonElement> mess)
        {
            if (mess.GetValue("type").Deserialize<string>().Contains("bagInfo")) //己方当前的背包
            {
                List<Pet>? bag = mess["data2"].Deserialize<List<Pet>>();
                if (bag == null)
                {
                    MessageBox.Show("获取背包数据失败，请重启登录器再尝试");
                }
                /*
                else if (_type != null && _type.Equals("Player1"))
                {
                    //player1Pets = bag;
                    PetStateAfterFresh(bag);
                }*/
                else
                {
                    //player2Pets = bag;
                    if (PetUtil.CheckBag(bag))
                    {
                        PetStateAfterFresh(bag);
                    }
                }
            }
            if (mess.GetValue("type").Deserialize<string>().Contains("suitId"))
            {
                var id = mess.GetValue("data").Deserialize<int>();
                if (_type != null && _type.Equals("Player1"))
                {
                    _player1SuitId = id.ToString();
                }
                else
                {
                    _player2SuitId = id.ToString();
                }
                ShowMySuit();
            }
        }

        private async Task InitPhase()
        {
            _phase = await GameApi.GetPhase();
        }

        private async Task InitType()
        {
            _type = await GameApi.GetType();
        }

        private async Task InitNickName()
        {
            var players = await GameApi.GetPlayers();
            player1NickName.Text = players.GetValueOrDefault("Player1");
            player2NickName.Text = players.GetValueOrDefault("Player2");
        }
        private async Task GetBags()
        {
            var petState = await ConventionalGameApi.GetPetState();
            if (petState == null) { return; }
            player1Pets = petState["Player1PetState"];
            player2Pets = petState["Player2PetState"];
            InitPetState();
        }
        private async Task PetStateAfterFresh(List<Pet> bag)
        {
            await ConventionalGameApi.FreshBag2(bag);
            await GetBags();
        }

        private void InitPetState()
        {
            _pickCount = 0;
            _banCount = 0;
            foreach (Control c in Controls)
            {
                if (player1Pets != null && c.Name.StartsWith("player1Pet"))
                {
                    var box = (PictureBox)c;
                    box.ImageLocation = "";
                    box.Refresh();
                    int petSeq = int.Parse(box.Name.Substring(10)) - 1;
                    if (petSeq < player1Pets.Count())
                    {
                        box.ImageLocation = PetUtil.handleHeadUrl(player1Pets[petSeq].id);
                        box.Refresh();
                        var label = box.Tag as Label;
                        switch (player1Pets[petSeq].state)
                        {
                            case 0:
                                label.Text = "";
                                break;
                            case 1:
                                label.ForeColor = Color.Red;
                                label.Text = "禁用";
                                if (_type == "Player2")
                                {
                                    _banCount++;
                                    if (_banCount == _banNum)
                                    {
                                        banOver = true;
                                    }
                                }
                                break;
                            case 2:
                                label.ForeColor = Color.Blue;
                                label.Text = "首发";
                                if (_type == "Player1")
                                {
                                    pickIncre();
                                    firstOver = true;
                                }
                                break;
                            case 3:
                                label.ForeColor = Color.Blue;
                                label.Text = "出战";
                                if (_type == "Player1")
                                {
                                    pickIncre();
                                    if (_pickCount == 6)
                                    {
                                        remainOver = true;
                                    }
                                }
                                break;
                            default: break;
                        }
                    }
                }
                if (player2Pets != null && c.Name.StartsWith("player2Pet"))
                {
                    var box = (PictureBox)c;
                    box.ImageLocation = "";
                    box.Refresh();
                    int petSeq = int.Parse(box.Name.Substring(10)) - 1;
                    if (petSeq < player2Pets.Count())
                    {
                        box.ImageLocation = PetUtil.handleHeadUrl(player2Pets[petSeq].id);
                        box.Refresh();
                        var label = box.Tag as Label;
                        switch (player2Pets[petSeq].state)
                        {
                            case 0:
                                label.Text = "";
                                break;
                            case 1:
                                label.ForeColor = Color.Red;
                                label.Text = "禁用";
                                if (_type == "Player1")
                                {
                                    _banCount++;
                                    if (_banCount == _banNum)
                                    {
                                        banOver = true;
                                    }
                                }
                                break;
                            case 2:
                                label.ForeColor = Color.Blue;
                                label.Text = "首发";
                                if (_type == "Player2")
                                {
                                    pickIncre();
                                    firstOver = true;
                                }
                                break;
                            case 3:
                                label.ForeColor = Color.Blue;
                                label.Text = "出战";
                                if (_type == "Player2")
                                {
                                    pickIncre();
                                    if (_pickCount == 6)
                                    {
                                        remainOver = true;
                                    }
                                }
                                break;
                            default: break;
                        }
                    }
                }
            }
        }

        private async Task InitSuit()
        {
            string suitUrl = "https://seerh5.61.com/resource/assets/item/cloth/suiticon/";

            //获取双方方套装
            var suit = await ConventionalGameApi.GetPickSuit();
            if (_phase != "WaitingStage" && _phase != "PreparationStage" && _phase != "ReadyStage")
            {
                if (_type != null && _type.Equals("Player1"))
                {
                    //player2Suit.ImageLocation = "";
                    if (suit.GetValueOrDefault("Player2PickSuit").Length > 1)
                    {
                        _player2SuitId = suit.GetValueOrDefault("Player2PickSuit");
                        player2Suit.ImageLocation = suitUrl + _player2SuitId + @".png";
                        player2Suit.Refresh();
                    }
                    Debug.WriteLine("Player2PickSuit:" + suit.GetValueOrDefault("Player2PickSuit"));
                }
                else if (_type != null && _type.Equals("Player2"))
                {
                    //player1Suit.ImageLocation = "";
                    if (suit.GetValueOrDefault("Player1PickSuit").Length > 1)
                    {
                        _player1SuitId = suit.GetValueOrDefault("Player1PickSuit");
                        player1Suit.ImageLocation = suitUrl + _player1SuitId + @".png";
                        player1Suit.Refresh();
                    }
                    Debug.WriteLine("Player1PickSuit:" + suit.GetValueOrDefault("Player1PickSuit"));
                }
            }

            var mess = JsMessUtil<object>.MessJson("getSuit", 0);
            _sendWebViewMess(mess);
        }

        public async void ShowMySuit()
        {
            string suitUrl = "https://seerh5.61.com/resource/assets/item/cloth/suiticon/";

            player1Suit.ImageLocation = suitUrl + _player1SuitId + @".png";
            player1Suit.Refresh();
            player2Suit.ImageLocation = suitUrl + _player2SuitId + @".png";
            player2Suit.Refresh();
        }



        private async void freshButton_Click(object sender, EventArgs e)
        {
            await InitType();
            await InitPhase();
            if (_phase == "WaitingStage" || _phase == "PreparationStage" || (_type == "Player1" && _phase == "ReadyStage"))
            {
                
                var mess2 = JsMessUtil<object>.MessJson("getSuit", 0);
                _sendWebViewMess(mess2);
            }
            Init();
        }


        private async void readyButton_Click(object sender, EventArgs e)
        {
            //SetBagSuit();
            string? bagmess = null;
            if (_type == "Player2" && _player2SuitId != null && player2Pets != null && PetUtil.CheckBag(player2Pets))
            {
                await ConventionalGameApi.SetConventionalSuit(_player2SuitId);
                await ConventionalGameApi.FreshBag2(player2Pets);
            }
            SgcWsHandler.SendMess("ready");
        }

        private async void startButton_Click(object sender, EventArgs e)
        {
            //SetBagSuit();
            string? bagmess = null;
            if (_type == "Player1" && _player1SuitId != null && player1Pets != null && PetUtil.CheckBag(player1Pets))
            {
                await ConventionalGameApi.SetConventionalSuit(_player1SuitId);
                await ConventionalGameApi.FreshBag2(player1Pets);
            }
            SgcWsHandler.SendMess("start");
        }

        private async void exitGameButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定关闭对战吗？", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (await ConventionalGameApi.ExitGame())
                {
                    this.Close();
                }
            }
        }

        private void player1Pet_Click(object sender, EventArgs e)
        {
            if (!_clickAble) return;
            var box = (PictureBox)sender;
            var label = box.Tag as Label;
            //int labelTag = label == null || label.Tag == null ? 0 : (int)label.Tag;
            int petSeq = int.Parse(box.Name.Substring(10))-1;
            if (player1Pets == null || petSeq >= player1Pets.Count()) return;
            if (_type == "Player1")
            { //player1只能在pick阶段点击player1的精灵头像
                if (_phase == "PlayerPickElfFirst")
                {
                    if (player1Pets[petSeq].state == 0)
                    {
                        if (MessageBox.Show("确定选择该精灵首发吗？", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            label.ForeColor = Color.Blue;
                            label.Text = "首发";
                            player1Pets[petSeq].state = 2;
                            SgcWsHandler.SendMess("PickElfFirst" + player1Pets[petSeq].id);
                            pickIncre();
                        }
                    }
                }
                if (_phase == "PlayerPickElfRemain")
                {
                    if (player1Pets[petSeq].state == 0)
                    {
                        if (_pickCount == 5)
                        {
                            if (MessageBox.Show("确定选择出战精灵吗？", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                label.ForeColor = Color.Blue;
                                label.Text = "出战";
                                player1Pets[petSeq].state = 3;
                                List<int> pickRemain = new();
                                foreach (Pet pet in player1Pets)
                                {
                                    if (pet.state == 3)
                                    {
                                        pickRemain.Add(pet.id);
                                    }
                                }
                                SgcWsHandler.SendMess("PickElfRemain" + JsonSerializer.Serialize(pickRemain));
                            }
                        }
                        if (_pickCount < 5)
                        {
                            label.ForeColor = Color.Blue;
                            label.Text = "出战";
                            player1Pets[petSeq].state = 3;
                            pickIncre();
                        }
                    }
                    else if (player1Pets[petSeq].state == 3)
                    {
                        label.Text = "";
                        player1Pets[petSeq].state = 0;
                        pickDecre();
                    }
                }
            }
            if (_type == "Player2" && _phase == "PlayerBanElf")
            { //player2只能在ban阶段点击player1的精灵头像
                if (player1Pets[petSeq].state == 0)
                {
                    if (_banCount == _banNum-1)
                    {
                        if (MessageBox.Show("确定禁用精灵吗？", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            label.ForeColor = Color.Red;
                            label.Text = "禁用";
                            player1Pets[petSeq].state = 1;

                            List<int> banids = new();
                            foreach (var pet in player1Pets)
                            {
                                if (pet.state == 1)
                                {
                                    banids.Add(pet.id);
                                }
                            }
                            SgcWsHandler.SendMess("PlayerBan" + JsonSerializer.Serialize(banids));
                        }
                    }
                    if (_banCount < _banNum-1)
                    {
                        label.ForeColor = Color.Red;
                        label.Text = "禁用";
                        player1Pets[petSeq].state = 1;
                        _banCount++;
                    }
                }
                else if (player1Pets[petSeq].state == 1)
                {
                    label.Text = "";
                    player1Pets[petSeq].state = 0;
                    _banCount--;
                }
            }
        }

        private void player2Pet_Click(object sender, EventArgs e)
        {
            if (!_clickAble) return;
            var box = (PictureBox)sender;
            var label = box.Tag as Label;
            //int labelTag = label == null || label.Tag == null ? 0 : (int)label.Tag;
            int petSeq = int.Parse(box.Name.Substring(10))-1;
            if (player2Pets == null || petSeq >= player2Pets.Count()) return;
            if (_type == "Player2")
            { //player2只能在pick阶段点击player2的精灵头像
                if (_phase == "PlayerPickElfFirst")
                {
                    if (player2Pets[petSeq].state == 0)
                    {
                        if (MessageBox.Show("确定选择该精灵首发吗？", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            label.ForeColor = Color.Blue;
                            label.Text = "首发";
                            player2Pets[petSeq].state = 2;
                            SgcWsHandler.SendMess("PickElfFirst" + player2Pets[petSeq].id);
                            pickIncre();
                        }
                    }
                }
                if (_phase == "PlayerPickElfRemain")
                {
                    if (player2Pets[petSeq].state == 0)
                    {
                        if (_pickCount == 5)
                        {
                            if (MessageBox.Show("确定选择出战精灵吗？", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                label.ForeColor = Color.Blue;
                                label.Text = "出战";
                                player2Pets[petSeq].state = 3;
                                List<int> pickRemain = new();
                                foreach (Pet pet in player2Pets)
                                {
                                    if (pet.state == 3)
                                    {
                                        pickRemain.Add(pet.id);
                                    }
                                }
                                SgcWsHandler.SendMess("PickElfRemain" + JsonSerializer.Serialize(pickRemain));
                            }
                        }
                        if (_pickCount < 5)
                        {
                            label.ForeColor = Color.Blue;
                            label.Text = "出战";
                            player2Pets[petSeq].state = 3;
                            pickIncre();
                        }
                    }
                    else if (player2Pets[petSeq].state == 3)
                    {
                        label.Text = "";
                        player2Pets[petSeq].state = 0;
                        pickDecre();
                    }
                }
            }
            if (_type == "Player1" && _phase == "PlayerBanElf")
            { //player1只能在ban阶段点击player2的精灵头像
                if (player2Pets[petSeq].state == 0)
                {
                    if (_banCount == _banNum-1)
                    {
                        if (MessageBox.Show("确定禁用精灵吗？", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            label.ForeColor = Color.Red;
                            label.Text = "禁用";
                            player2Pets[petSeq].state = 1;

                            List<int> banids = new();
                            foreach (Pet pet in player2Pets)
                            {
                                if (pet.state == 1)
                                {
                                    banids.Add(pet.id);
                                }
                            }
                            SgcWsHandler.SendMess("PlayerBan" + JsonSerializer.Serialize(banids));
                        }
                    }
                    if (_banCount < _banNum-1)
                    {
                        label.ForeColor = Color.Red;
                        label.Text = "禁用";
                        player2Pets[petSeq].state = 1;
                        _banCount++;
                    }
                }
                else if (player2Pets[petSeq].state == 1)
                {
                    label.Text = "";
                    player2Pets[petSeq].state = 0;
                    _banCount--;
                }
            }
        }

        private async void InitCountDown()
        {
            _countDown = await GameApi.GetCountTime();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_countDown <= 0)
            {
                return;
            }
            _countDown--;
            countDownLabel.Text = _countDown >= 10 ? _countDown.ToString() : "0" + _countDown;
        }

        private async void InitBanNum()
        {
            _banNum = await ConventionalGameApi.GetBanNum();
        }
    }
}

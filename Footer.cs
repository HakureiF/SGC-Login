using Seer.api;
using Seer.Utils;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Seer
{
    public partial class Footer : Form
    {

        private readonly SendWebViewMess _sendWebViewMess;

        public static List<string>? Player1Pick;
        public static List<string>? Player2Pick;

        public Footer(SendWebViewMess sendWebViewMess)
        {
            InitializeComponent();
            SetPickHeads();
            _sendWebViewMess = sendWebViewMess;
        }

        private void Footer_Load(object sender, EventArgs e)
        {

        }

        private async Task SetPlayer1Pick(bool isNowPlayer)
        {
            //string headUrl = "https://seerh5.61.com/resource/assets/pet/head/";
            Player1Pick = await GameApi.GetPlayer1Pick();
            if (Player1Pick == null) return;
            if (isNowPlayer) //当前登陆器用户是player1
            {
                for (var i = 1; i <= Player1Pick.Count; i++)
                {
                    foreach (Control c in Controls)
                    {
                        if (!c.Name.Equals("pictureBox" + i)) continue;
                        var box = (PictureBox)c;
                        box.ImageLocation = PetUtil.handleHeadUrl(int.Parse(Player1Pick[i - 1]));
                    }
                }
            }
            else
            {
                for (var i = 7; i <= Player1Pick.Count + 6; i++)
                {
                    foreach (Control c in Controls)
                    {
                        if (!c.Name.Equals("pictureBox" + i)) continue;
                        var box = (PictureBox)c;
                        box.ImageLocation = PetUtil.handleHeadUrl(int.Parse(Player1Pick[i - 7]));
                    }
                }
                for (var i = 12; i > Player1Pick.Count + 6; i--)
                {
                    foreach (Control c in Controls)
                    {
                        if (!c.Name.Equals("pictureBox" + i)) continue;
                        var box = (PictureBox)c;
                        box.ImageLocation = "https://cdn.imrightchen.live/website/file/assets/competitionUI/Unknown.png";
                    }
                }
            }
        }

        private async Task SetPlayer2Pick(bool isNowPlayer)
        {
            //string headUrl = "https://seerh5.61.com/resource/assets/pet/head/";
            Player2Pick = await GameApi.GetPlayer2Pick();
            if (Player2Pick == null) return;
            if (isNowPlayer) //当前登陆器用户是player2
            {
                for (var i = 1; i <= Player2Pick.Count; i++)
                {
                    foreach (Control c in Controls)
                    {
                        if (!c.Name.Equals("pictureBox" + i)) continue;
                        var box = (PictureBox)c;
                        box.ImageLocation = PetUtil.handleHeadUrl(int.Parse(Player2Pick[i - 1]));
                    }
                }
            }
            else
            {
                for (var i = 7; i <= Player2Pick.Count + 6; i++)
                {
                    foreach (Control c in Controls)
                    {
                        if (!c.Name.Equals("pictureBox" + i)) continue;
                        var box = (PictureBox)c;
                        box.ImageLocation = PetUtil.handleHeadUrl(int.Parse(Player2Pick[i - 7]));
                    }
                }
                for (var i = 12; i > Player2Pick.Count + 6; i--)
                {
                    foreach (Control c in Controls)
                    {
                        if (!c.Name.Equals("pictureBox" + i)) continue;
                        var box = (PictureBox)c;
                        box.ImageLocation = "https://cdn.imrightchen.live/website/file/assets/competitionUI/Unknown.png";
                    }
                }
            }
        }

        public async void SetPickHeads()
        {
            var type = await GameApi.GetType();
            Debug.WriteLine("set footer heads" + type);
            if (type == "Player1")
            {
                await SetPlayer1Pick(true);
                await SetPlayer2Pick(false);
            }
            if (type == "Player2")
            {
                await SetPlayer1Pick(false);
                await SetPlayer2Pick(true);
            }
        }

        private async void loadBagButton_Click(object sender, EventArgs e)
        {
            var type = await GameApi.GetType();
            if (type != null || Player1Pick != null || Player2Pick != null)
            {
                var ids = new List<int>();
                if (type == "Player1")
                {
                    ids.AddRange(Player1Pick.Select(int.Parse));
                }
                if (type == "Player2")
                {
                    ids.AddRange(Player2Pick.Select(int.Parse));
                }
                var mess = JsMessUtil<List<int>>.MessJson("addToBag", ids);
                _sendWebViewMess(mess);
            } 
            else
            {
                MessageBox.Show(@"请先完成bp！");
            }
        }
    }
}

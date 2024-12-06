using Seer.api;
using Seer.handler;
using Seer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Seer
{
    public partial class JoinConventionalGame : Form
    {

        private readonly EmitFromSubForms _emitFromSubForms;
        public JoinConventionalGame(EmitFromSubForms emitFromSubForms)
        {
            InitializeComponent();
            _emitFromSubForms = emitFromSubForms;
        }

        private async void joinButton_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "gameId", gameIdBox.Text },
                //{ "code", inviCodeBox.Text }
            };

            bool succ = await ConventionalGameApi.JoinConventionalGame(param);
            if (succ)
            {
                _emitFromSubForms(3);
                Close();
            }
        }

        private void CloseJoin(object? sender, FormClosedEventArgs e)
        {
            if (SgcWsHandler._modMark.Equals("Join"))
            {
                SgcWsHandler.CloseConnect();
            }
        }
    }
}

using Seer.api;
using Seer.DTO;
using Seer.handler;
using Seer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Seer
{
    public partial class GenerateConventionalGame : Form
    {
        private readonly EmitFromSubForms _emitFromSubForms;

        private List<RaceGroup> raceGroups = new List<RaceGroup>();

        public GenerateConventionalGame(EmitFromSubForms emitFromSubForms)
        {
            InitializeComponent();
            GetRaceGroupList();
            _emitFromSubForms = emitFromSubForms;
        }

        private async void GetRaceGroupList()
        {
            raceGroups = await ConventionalGameApi.SearchGroups() ?? new List<RaceGroup>();
            Debug.WriteLine(raceGroups.Count);
            RaceGroupList.DataSource = raceGroups;
            RaceGroupList.ValueMember = "groupId";
            RaceGroupList.DisplayMember = "groupName";
        }

        private void RaceGroupList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void generateGame_Click(object sender, EventArgs e)
        {
            if (RaceGroupList.SelectedValue == null)
            {
                return;
            }

            var game = await ConventionalGameApi.GenerateGame((RaceGroupList.SelectedItem as RaceGroup).groupId);
            var gameId = game != null ? game.GetValueOrDefault("gameId") : "";
            //var code = game != null ? game.GetValueOrDefault("code") : "";

            Clipboard.SetDataObject($"{gameId}");
            if (game != null)
            {
                SgcWsHandler._modMark = "";
                _emitFromSubForms(3);
                Close();
            }
            else
            {
                MessageBox.Show(@"房间创建失败");
            }
        }

        private void CloseGenerate(object? sender, FormClosedEventArgs e)
        {
            if (SgcWsHandler._modMark.Equals("Create"))
            {
                SgcWsHandler.CloseConnect();
            }
        }
    }
}


using Microsoft.VisualBasic;
using Seer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Seer
{
    public partial class PeakLogger : Form
    {
        private static int[] _pets = new int[13];
        private static List<int> _hasShow = new();
        private int index = 1;
        public PeakLogger()
        {
            InitializeComponent();
            KeyPreview = true;
        }

        public void SetRivalPetHeads(List<int>? ids)
        {
            if (ids == null) return;
            _pets = new int[13];
            _hasShow = new List<int>();
            foreach (Control c in Controls)
            {
                if (!c.Name.Contains("pictureBox")) continue;
                var box = (PictureBox)c;
                var label = box.Tag as Label;
                label.Text = "";
                label.Tag = 0;
            }

            for (var i = 1; i <= ids.Count; i++)
            {
                foreach (Control c in Controls)
                {
                    if (!c.Name.Equals("pictureBox" + i)) continue;
                    var box = (PictureBox)c;
                    box.BackColor = Color.White;
                    box.ImageLocation = PetUtil.handleHeadUrl(ids[i - 1]);
                    _pets[i] = ids[i - 1];
                }
            }
            ShowPetData();
        }

        public void RivalPetShow(int id)
        {
            if (_hasShow.Contains(id))
            {
                for (var i = 1; i < _pets.Length; i++)
                {
                    if (_pets[i] != id) continue;
                    foreach (Control c in Controls)
                    {
                        if (!c.Name.Equals("pictureBox" + i)) continue;
                        var box = (PictureBox)c;
                        var label = box.Tag as Label;
                        label.ForeColor = Color.Blue;
                        label.Text = "出战";
                        label.Tag = 1;
                    }
                }
            }
            else
            {
                _hasShow.Add(id);
                for (var i = 1; i < _pets.Length; i++)
                {
                    if (_pets[i] != id) continue;
                    foreach (Control c in Controls)
                    {
                        if (!c.Name.Equals("pictureBox" + i)) continue;
                        var box = (PictureBox)c;
                        var label = box.Tag as Label;
                        label.ForeColor = Color.Blue;
                        label.Text = "出战";
                        label.Tag = 1;
                    }
                }
            }
        }


        public void RivalPetDefeated(int id)
        {
            if (!_hasShow.Contains(id)) return;
            for (var i = 1; i < _pets.Length; i++)
            {
                if (_pets[i] != id) continue;
                foreach (Control c in Controls)
                {
                    if (!c.Name.Equals("pictureBox" + i)) continue;
                    var box = (PictureBox)c;
                    var label = box.Tag as Label;
                    label.ForeColor = Color.Red;
                    label.Text = "阵亡";
                    label.Tag = 2;
                }
            }
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            var pictureBox = (PictureBox)sender;
            var label = pictureBox.Tag as Label;
            int labelTag = label == null || label.Tag == null ? 0 : (int)label.Tag;

            if (labelTag == 0)
            {
                label.ForeColor = Color.Blue;
                label.Text = "出战";
                label.Tag = 1;
            }
            else if (labelTag == 1)
            {
                label.ForeColor = Color.Red;
                label.Text = "阵亡";
                label.Tag = 2;
            }
            else
            {
                label.Text = "";
                label.Tag = 0;
            }

            index = int.Parse(pictureBox.Name.Substring(10));
            ShowPetData();
        }


        public void SetOwnerMark(bool mark)
        {
            RoomOwnerMark.Text = mark ? "你当前是：房主" : "你当前是：挑战方";
        }

        public void PeakLogger_KeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (index <= 4) index += 12;
                    index -= 4;
                    ShowPetData();
                    break;
                case Keys.Down:
                    if (index >= 9) index -= 12;
                    index += 4;
                    ShowPetData();
                    break;
                case Keys.Left:
                    if (index <= 1) index = 12;
                    else index--;
                    ShowPetData();
                    break;
                case Keys.Right:
                    if (index >= 12) index = 1;
                    else index++;
                    ShowPetData();
                    break;
                default:
                    break;
            }
        }

        public bool petDataEnable { get; set; } = false;

        private void ShowPetData()
        {
            //if (!petDataEnable) return;
            foreach (Control c in Controls)
            {
                if (c.Name.Equals("pictureBox" + index))
                {
                    var box = (PictureBox)c;
                    var location = box.Location;
                    location.X -= 5;
                    location.Y -= 5;
                    bgBox.Location = location;
                    if (PetUtil.petsData != null && PetUtil.petsData.ContainsKey(_pets[index]))
                    petDataContent.Text = "\n\n" + PetUtil.petsData[_pets[index]] + "\n\n\n\n\n\n\n\n\n\n";
                }
            }
        }
    }
}

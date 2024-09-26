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
    public partial class Announce : Form
    {
        public Announce()
        {
            InitializeComponent();
        }

        public void InitializeAnnounce(string announce)
        {
            string[] lines = announce.Split("\\n");
            foreach (string line in lines)
            {
                announceContent.SelectionFont = new Font("宋体", 16, FontStyle.Bold);
                announceContent.AppendText(line);
                announceContent.AppendText(Environment.NewLine);
            }
        }
    }
}

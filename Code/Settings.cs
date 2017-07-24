using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenTKPlatformerExample
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SoundManager.volume = (float)trackBar1.Value / 10f;
            SoundManager.sounds[0].Play();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SoundManager.volume = (float)trackBar1.Value / 10f;

            this.Close();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            label2.Text += " " + OpenTK.Input.GamePad.GetState(0).IsConnected;
        }
    }
}

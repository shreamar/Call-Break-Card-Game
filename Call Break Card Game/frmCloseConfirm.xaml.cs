using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Call_Break_Card_Game
{
    /// <summary>
    /// Interaction logic for frmPlaceBid.xaml
    /// </summary>
    public partial class frmCloseConfirm : Window
    {
        public frmCloseConfirm()
        {
            InitializeComponent();

            Loaded += frmCloseConfirm_Loaded;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            PlaySound("tick.wav");
            System.Threading.Thread.Sleep(800);
            this.Close();
            Application.Current.Shutdown();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            PlaySound("tap.wav");
            this.Close();
        }

        private void PlaySound(string fileName)
        {
            Uri uri = new Uri("../../sfx/" + fileName, UriKind.Relative);
            SoundPlayer player = new SoundPlayer("../../sfx/" + fileName);

            //player.LoadAsync();
            player.Play();
        }

        private void frmCloseConfirm_Loaded(object sender, RoutedEventArgs e)
        {
            PlaySound("error.wav");
        }
    }
}

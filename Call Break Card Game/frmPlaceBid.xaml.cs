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
    public partial class frmPlaceBid : Window
    {
        public frmPlaceBid()
        {
            PlaySound("untap.wav");
            InitializeComponent();
        }

        private void btnPlaceBid_Click(object sender, RoutedEventArgs e)
        {
            PlaySound("switch5.wav");
            Game.PlaceBid(Game.HumanPlayerID, (int)sliderPlaceBid.Value);
            this.Close();
        }

        private void sliderPlaceBid_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PlaySound("chipLay1.wav");
            lblBid.Content = sliderPlaceBid.Value.ToString("n0");
        }

        private void PlaySound(string fileName)
        {
            Uri uri = new Uri("../../sfx/" + fileName, UriKind.Relative);
            SoundPlayer player = new SoundPlayer("../../sfx/" + fileName);

            //player.LoadAsync();
            player.Play();
        }
    }
}

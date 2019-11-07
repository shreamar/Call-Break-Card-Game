using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for frmStartGame.xaml
    /// </summary>
    public partial class frmStartGame : Window
    {
        public frmStartGame()
        {
            InitializeComponent();
        }

        private void sliderMaxHandsToPlay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblMaxHandsToPlay.Content = sliderMaxHandsToPlay.Value.ToString("n0");
        }

        private void btnDealCards_Click(object sender, RoutedEventArgs e)
        {
            Game.MaxHandsToPlay = int.Parse(lblMaxHandsToPlay.Content.ToString());

            MainWindow main = new MainWindow();
            main.Show();

            this.Close();
        }
    }
}

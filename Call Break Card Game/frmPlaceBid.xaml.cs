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
    /// Interaction logic for frmPlaceBid.xaml
    /// </summary>
    public partial class frmPlaceBid : Window
    {
        public frmPlaceBid()
        {
            InitializeComponent();
        }

        private void btnPlaceBid_Click(object sender, RoutedEventArgs e)
        {
            Game.Bidding[Game.HumanPlayerID] = (int)sliderPlaceBid.Value;
            this.Close();
        }

        private void sliderPlaceBid_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblBid.Content = sliderPlaceBid.Value.ToString("n0");
        }
    }
}

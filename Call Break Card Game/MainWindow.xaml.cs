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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Call_Break_Card_Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Game.InitializeGame("JAEGER");
        }

        private void btnDealCards_Click(object sender, RoutedEventArgs e)
        {
            //Game.InitializeGame("JAEGER");
            //Game.ReinitializeHand();

            //Game.InitializePLayers("JAEGER");

            Game.DealCards();

            for (int i = 0; i < 4; i++)
            {
                Label lblPlayer = (Label)this.FindName("lblPlayer0");
                ListBox lbCards= (ListBox)this.FindName("lbCards" + i);

                lblPlayer.Content = Game.Players[i].Name;
                lbCards.Items.Clear();

                foreach(Card card in Game.Players[i].Cards)
                {
                    lbCards.Items.Add(card.Name);
                }
            }
        }
    }
}


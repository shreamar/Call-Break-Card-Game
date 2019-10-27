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
            btnPlaceBids.IsEnabled = true;
            btnDealCards.IsEnabled = false;

            Game.DealCards();

            for (int i = 0; i < 4; i++)
            {
                Label lblPlayer = (Label)this.FindName("lblPlayer"+i);
                ListBox lbCards= (ListBox)this.FindName("lbCards" + i);

                lblPlayer.Content = Game.Players[i].Name;
                lbCards.Items.Clear();

                foreach(Card card in Game.Players[i].Cards)
                {
                    lbCards.Items.Add(card.Name);
                }
            }
        }

        private void btnPlaceBids_Click(object sender, RoutedEventArgs e)
        {
            btnPlaceBids.IsEnabled = false;

            Random r = new Random();
            int rnd = r.Next(999999999);

            for (int i = 0; i < 4; i++)
            {
                Random rand = new Random(rnd + i);
                int rndm = rand.Next(1, 6);

                Game.PlaceBid(i, rndm);

                Label lblPlayer = (Label)this.FindName("lblPlayer" + i);
                lblPlayer.Content = Game.Players[i].Name + "            0/" + Game.Bidding[i];
            }

            markPlayables();
        }

        private void markPlayables()
        {
            ListBox lbCards = (ListBox)this.FindName("lbCards" + Game.CurrentDealer);

            lbCards.Items.Clear();

            List<int> playables = Game.Players[Game.CurrentDealer].ListPlayableCards(Game.LeadCardID, Game.PowerCardID);

            foreach(Card card in Game.Players[Game.CurrentDealer].Cards)
            {
                bool match = false;
                foreach(int a in playables)
                {
                    if(card.ID == a)
                    {
                        match = true;
                        break;
                    }                   
                }
                if (match)
                {
                    lbCards.Items.Add(card.Name + "~");
                }
                else
                {
                    lbCards.Items.Add(card.Name);
                }
            }

            Label lblPlayer = (Label)this.FindName("lblPlayer" + Game.CurrentDealer);
            lblPlayer.Content = String.Format(">>>{0}          {1}/{2}", Game.Players[Game.CurrentDealer].Name, Game.TricksWon[Game.CurrentDealer], Game.Bidding[Game.CurrentDealer]);    
        }
    }
}


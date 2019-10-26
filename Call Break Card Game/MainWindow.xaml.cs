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
        Deck deck = new Deck();
        Player player = new Player("Amar", Player.PlayerType.Human);
        public MainWindow()
        {
            InitializeComponent();

            Card card = new Card();            

            lblTest.Content = player.Name;

            for (int i = 0; i < deck.Cards.Count; i+=4)
            {
                player.Cards.Add(deck.Cards[i]);
            }

            player.sortCards();
            for (int i = 0; i < player.Cards.Count; i++)
            {
                lbTest.Items.Add(player.Cards[i].Name);
            }
            //lblTest.Content = player.LastPlayedCard.Name;
            
        }

        private void btnShuffle_Click(object sender, RoutedEventArgs e)
        {
            lbPlayables.Items.Clear();
            
            Random rnd = new Random();

            int b = 0, a = 0;
            bool duplicate = false;

            List<int> indexCards = new List<int>();
            do
            {
                Random rnd1 = new Random(rnd.Next(9999999) + b + 999);
                Random rnd2 = new Random(rnd.Next(9999999) + a);

                foreach (Card card in player.Cards)
                {
                    b = rnd1.Next(52);
                    a = rnd2.Next(52);

                    if (a == card.ID || b == card.ID || a == b)
                    {
                        duplicate = true;
                        break;
                    }
                    duplicate = false;
                }
            } while (duplicate);


            if (Game.CardIDtoValue(a) > Game.CardIDtoValue(b) && Game.CardIDtoSuit(a) == Card.CardSuit.Spade)
            {
                indexCards = player.ListPlayableCards(b, a);
                lblTest.Content = "Lead: " + Game.CardIDtoCard(b).Name + "   Power: " + Game.CardIDtoCard(a).Name;
            }
            else
            {
                indexCards = player.ListPlayableCards(b, b);
                lblTest.Content = "Lead: " + Game.CardIDtoCard(b).Name + "   Power: " + Game.CardIDtoCard(b).Name;
            }
            

            /*
            int x = 26;
            int y = 26;
            List<int> indexCards = player.ListPlayableCards(x,y);
            lblTest.Content = "Lead: " + Game.CardIDtoCard(x).Name + "   Power: " + Game.CardIDtoCard(y).Name;
            */

            foreach (int id in indexCards)
            {
                lbPlayables.Items.Add(Game.CardIDtoCard(id).Name);
            }
            
        }        

        private void btnRedeal_Click(object sender, RoutedEventArgs e)
        {
            lbTest.Items.Clear();
            player.Cards.Clear();

            deck.ShuffleDeck();

            for (int i = 0; i < deck.Cards.Count; i += 4)
            {
                player.Cards.Add(deck.Cards[i]);
            }

            player.sortCards();
            for (int i = 0; i < player.Cards.Count; i++)
            {
                lbTest.Items.Add(player.Cards[i].Name);
            }
        }

        private void btnTest1_Click(object sender, RoutedEventArgs e)
        {
            Game.InitializePLayers("Jaeger");

            lbPlayables.Items.Clear();
            foreach (Player player in Game.Players)
            {
                lbPlayables.Items.Add(player.Name);
            }
        }

        private void btnDeal_Click(object sender, RoutedEventArgs e)
        {
            lbTest.Items.Clear();
            lbPlayables.Items.Clear();
            lb3.Items.Clear();
            lb4.Items.Clear();

            Game.InitializePLayers("Jaeger");

            Game.DealCards();

            lblTest.Content = Game.Players[0].Name;
            foreach (Card card in Game.Players[0].Cards)
            {
                lbTest.Items.Add(card.Name);
            }

            lblTest1.Content = Game.Players[1].Name;
            foreach (Card card in Game.Players[1].Cards)
            {
                lbPlayables.Items.Add(card.Name);
            }

            lblTest2.Content = Game.Players[2].Name;
            foreach (Card card in Game.Players[2].Cards)
            {
                lb3.Items.Add(card.Name);
            }

            lblTest3.Content = Game.Players[3].Name;
            foreach (Card card in Game.Players[3].Cards)
            {
                lb4.Items.Add(card.Name);
            }
        }
    }
}


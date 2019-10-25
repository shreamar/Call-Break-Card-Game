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

            /*
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


            if (Game.CardIDtoValue(a) > Game.CardIDtoValue(b) && Game.CardIDtoSuit(a) == 3)
            {
                indexCards = player.ListPlayableCards(b, a);
                lblTest.Content = "Lead: " + Game.CardIDtoCard(b).Name + "   Power: " + Game.CardIDtoCard(a).Name;
            }
            else
            {
                indexCards = player.ListPlayableCards(b, b);
                lblTest.Content = "Lead: " + Game.CardIDtoCard(b).Name + "   Power: " + Game.CardIDtoCard(b).Name;
            }
            */

            int x = 26;
            int y = 12;
            List<int> indexCards = player.ListPlayableCards(x,y);
            lblTest.Content = "Lead: " + Game.CardIDtoCard(x).Name + "   Power: " + Game.CardIDtoCard(y).Name;

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


    }
}


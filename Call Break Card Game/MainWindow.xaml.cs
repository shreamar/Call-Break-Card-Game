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
            lbTest.Items.Clear();

            player.playCard(0);
            lblTest.Content = player.LastPlayedCard.Name;
            
            for (int i = 0; i < player.CardCount; i++)
            {
                lbTest.Items.Add(player.Cards[i].Name);
            }
        }
    }
}

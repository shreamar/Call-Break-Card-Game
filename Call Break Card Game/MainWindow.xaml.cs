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
using System.Windows.Media.Effects;
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

            //fill background with image
            /*
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("/cards/extras/Background.jpg", UriKind.Relative);
            bitmap.EndInit();
            Image image = new Image();
            image.Source = bitmap;
            image.Stretch = Stretch.Fill;
            grdWindow.Children.Add(image);
            */
        }

        private void btnDealCards_Click(object sender, RoutedEventArgs e)
        {
            btnPlaceBids.IsEnabled = true;
            //btnDealCards.IsEnabled = false;

            Game.DealCards();

            //shows cards of the first players in image form in grid
            grdCards.Children.Clear();

            int counter = 1, a = 0;
            foreach (Card card in Game.Players[0].Cards)
            {
                if (counter % 1 == 0)
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(mapCardtoFile(card), UriKind.Relative);
                    bitmap.EndInit();
                    Image image = new Image();
                    image.Source = bitmap;
                    image.HorizontalAlignment = HorizontalAlignment.Left;
                    image.Width = 100;
                    image.Height = 200;
                    //a is the index of current card  
                    image.Margin = new Thickness(((grdCards.Width - (image.Width + (Game.Players[0].CardCount - 1) * 45)) / 2) + 45 * a, ((grdCards.Height) * 3) / 4, 0, 10);

                    //changes cursor to hand cursor when pointed at cards
                    image.Cursor = Cursors.Hand;

                    //mouse enter and leave events
                    image.MouseEnter += new MouseEventHandler(Image_MouseEnter);
                    image.MouseLeave += new MouseEventHandler(Image_MouseLeave);
                    image.MouseDown += new MouseButtonEventHandler(Image_BitmapBlurEffect);

                    grdCards.Children.Add(image);

                    a++;
                }

                counter++;
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
            }

            markPlayables();
        }

        private void markPlayables()
        {
            /////////////////////////////////////
        }

        private void lbCards0_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            playCardsNow();
        }

        private void playCardsNow()
        {
            ListBox lbCards = (ListBox)this.FindName("lbCards" + Game.CurrentDealer);

            foreach (int a in Game.Players[Game.CurrentDealer].PlayableIDs)
            {
                if (1 == 1)//lbCards.Items[lbCards.SelectedIndex].ToString == Game.CardIDtoCard(a).Name)
                {
                    lbCards.Items.Clear();
                    Game.Players[Game.CurrentDealer].playCard(a);
                    break;
                }
            }
            showPlayables();
            showPlayersCards(Game.CurrentDealer);
        }

        private void showPlayables()
        {
            //lbPlayable.Items.Clear();

            foreach (Card card in Game.CardsInTable)
            {
                // lbPlayable.Items.Add(card.Name);
            }
        }

        private void showPlayersCards(int id)
        {
            ListBox lbCards = (ListBox)this.FindName("lbCards" + id);

            lbCards.Items.Clear();

            foreach (Card card in Game.Players[id].Cards)
            {
                lbCards.Items.Add(card.Name);
            }
        }


        /// <summary>
        /// Maps cards to the corresponding .png files
        /// </summary>
        /// <param name="card"></param>
        /// <returns>filename of the corresponding card</returns>
        private string mapCardtoFile(Card card)
        {
            string cardNumber = "", cardSuit = "", number = "";
            if ((int)card.Number > 0 && (int)card.Number <= 9)
            {
                cardNumber = (int.Parse(card.Name[0].ToString()) + 1).ToString();
            }
            else
            {
                cardNumber = card.Number.ToString().ToLower();
            }

            //since the card with pictures of jack, queen, king are in the same card name ending in 2
            //e.g. King of Hearts has no picture in "king_of_hearts.png", but has image of kind in "king_of_hearts2.png"
            if ((int)card.Number > 9)
            {
                number = "2";
            }

            cardSuit = card.Suit.ToString().ToLower() + "s";

            return "/cards/" + cardNumber + "_of_" + cardSuit + number + ".png";
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            //image.Source = (ImageSource)img.FindResource("ImgBtnLightbulbOn");
            image.Margin = new Thickness(image.Margin.Left, (((grdCards.Height) * 3) / 4) - 30, 0, 10);
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            //image.Source = (ImageSource)img.FindResource("ImgBtnLightbulbOn");
            image.Margin = new Thickness(image.Margin.Left, ((grdCards.Height) * 3) / 4, 0, 10);
        }

        /// <summary>
        /// Drop shadow effect on card
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_DropShadowEffect(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            UIElement uie = image;

            //card edge glowing effect
            uie.Effect = new DropShadowEffect
            {
                Color = new Color { A = 0, R = 255, G = 0, B = 0 },
                Direction = 320,
                ShadowDepth = 0,
                Opacity = 50,
                BlurRadius = 15
            };

            //clears effect
            //image.ClearValue(EffectProperty);
        }

        /// <summary>
        /// Blur effect on card
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_BitmapBlurEffect(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            UIElement uie = image;

            uie.Effect = new BlurEffect
            {
                KernelType = KernelType.Gaussian,
                Radius = 5,
                RenderingBias = RenderingBias.Performance
            };

            //disable image when blurred
            image.IsEnabled = false;
        }


        private void Image_EmbossBitmapEffect(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
        }
    }
}




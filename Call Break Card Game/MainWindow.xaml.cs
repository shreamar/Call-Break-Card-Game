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

            //dropshadow effect on mandala
            imgMandala.Visibility = Visibility.Hidden;
            Image_DropShadowEffect(imgMandala, 0, 0, 0, 255);
        }

        private void btnDealCards_Click(object sender, RoutedEventArgs e)
        {
            btnPlaceBids.IsEnabled = true;
            //btnDealCards.IsEnabled = false;

            Game.DealCards();

            //clear canvas
            canvasGame.Children.Clear();

            //remove a card from human players card pile
            //Game.Players[Game.HumanPlayerID].playCard(Game.Players[Game.HumanPlayerID].PlayableIDs[0]);

            //show cards of bots
            ShowCardsOnCanvas_Bots();

            //show cards of human player
            ShowCardsOnCanvas_Human();
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
            if ((int)card.Number >= 1 && (int)card.Number <= 9)
            {
                cardNumber = ((int)card.Number + 1).ToString();
            }
            else
            {
                cardNumber = card.Number.ToString().ToLower();
            }

            //since the card with pictures of jack, queen, king are in the same card name ending in 2
            //e.g. King of Hearts has no picture in "king_of_hearts.png", but has image of kind in "king_of_hearts2.png"
            if ((int)card.Number >= 10)
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
            image.Margin = new Thickness(image.Margin.Left, (canvasGame.Height - image.Height) - 30, 0, 10);
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            //image.Source = (ImageSource)img.FindResource("ImgBtnLightbulbOn");
            image.Margin = new Thickness(image.Margin.Left, (canvasGame.Height - image.Height), 0, 10);
        }

        /// <summary>
        /// Drop shadow effect on card
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_DropShadowEffect(object sender, byte a = 0, byte r = 0, byte g = 0, byte b = 0,
            double direction = 320, double shadowdepth = 0, double opacity = 50, double blurradius = 15)
        {
            Image image = sender as Image;
            UIElement uie = image;

            //card edge glowing effect
            uie.Effect = new DropShadowEffect
            {
                Color = new Color { A = a, R = r, G = g, B = b },
                Direction = direction,
                ShadowDepth = shadowdepth,
                Opacity = opacity,
                BlurRadius = blurradius
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

        /// <summary>
        /// Shows interactive human cards on canvas
        /// </summary>
        private void ShowCardsOnCanvas_Human()
        {
            //canvasGame.Children.Clear();

            int counter = 0;
            foreach (Card card in Game.Players[Game.HumanPlayerID].Cards)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(mapCardtoFile(card), UriKind.Relative);
                bitmap.EndInit();
                Image image = new Image();
                image.Source = bitmap;
                image.Width = 120;
                image.Height = 200;

                image.HorizontalAlignment = HorizontalAlignment.Center;
                image.VerticalAlignment = VerticalAlignment.Bottom;

                //places human player's cards in the center bottom of the canvas
                image.Margin = new Thickness(((canvasGame.Width - (image.Width +
                    (Game.Players[Game.HumanPlayerID].CardCount - 1) * 85)) / 2) + 85 * counter,(canvasGame.Height-image.Height),0,0);

                //changes cursor to hand cursor when pointed at cards
                image.Cursor = Cursors.Hand;

                //slightly pulls up the given card when mouse enters
                image.MouseEnter += new MouseEventHandler(Image_MouseEnter);
                //puts the card back down when mouse leaves
                image.MouseLeave += new MouseEventHandler(Image_MouseLeave);

                image.MouseDown += new MouseButtonEventHandler(Image_BitmapBlurEffect);

                //adds the card to the canvas
                canvasGame.Children.Add(image);

                counter++;
            }
        }

        /// <summary>
        /// Show cards(unrevealed) of bots on canvas
        /// </summary>
        private void ShowCardsOnCanvas_Bots()
        {
            //canvasGame.Children.Clear();

            string faceDownCard = "/cards/extras/Back.png";

            //start point is after the human player's id
            for (int i = (Game.HumanPlayerID + 1) % 4, j = 0; j < 3; i++, j++)
            {
                //bot players id
                int id = i % 4;

                if (j == 0)//player on right handside
                {
                    for (int counter = 0; counter < Game.Players[id].CardCount; counter++)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(mapCardtoFile(Game.Players[id].Cards[counter]), UriKind.Relative);
                        bitmap.EndInit();
                        Image image = new Image();
                        image.Source = bitmap;
                        image.Width = 70;
                        image.Height = 150;

                        image.HorizontalAlignment = HorizontalAlignment.Left;

                        //places bot1 player's cards in the right of the canvas
                        image.Margin = new Thickness(canvasGame.Width - image.Width-5, ((canvasGame.Height - 
                            (((Game.Players[i].CardCount-1) * 28) + image.Height)) / 2) + (counter * 28) - 50, 0, 0);

                        canvasGame.Children.Add(image);
                    }
                }
                else if (j == 1)//player on the over the top
                {
                    for (int counter = 0; counter < Game.Players[id].CardCount; counter++)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(mapCardtoFile(Game.Players[id].Cards[counter]), UriKind.Relative);
                        bitmap.EndInit();
                        Image image = new Image();
                        image.Source = bitmap;
                        image.Width = 90;
                        image.Height = 100;

                        image.HorizontalAlignment = HorizontalAlignment.Center;
                        image.VerticalAlignment = VerticalAlignment.Top;

                        //places bot2 player's cards in the center top of the canvas
                        image.Margin = new Thickness(((canvasGame.Width - (image.Width +
                    (Game.Players[id].CardCount - 1) * 35)) / 2) + (35 * counter), 0,0,0);

                        canvasGame.Children.Add(image);
                    }
                }
                else if (j == 2)//player on left handside
                {
                    for (int counter = 0; counter < Game.Players[id].CardCount; counter++)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(mapCardtoFile(Game.Players[id].Cards[counter]), UriKind.Relative);
                        bitmap.EndInit();
                        Image image = new Image();
                        image.Source = bitmap;
                        image.Width = 70;
                        image.Height = 150;

                        image.HorizontalAlignment = HorizontalAlignment.Left;

                        //places bot1 player's cards in the right of the canvas
                        image.Margin = new Thickness(10, ((canvasGame.Height -
                            (((Game.Players[id].CardCount-1) * 28) + image.Height)) / 2) + (counter * 28) - 50, 0, 0);

                        canvasGame.Children.Add(image);
                    }
                }
            }
        }
    }
}




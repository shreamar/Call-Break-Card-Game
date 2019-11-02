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
using System.Windows.Media.Animation;
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

            ShowDealReady_Deck();
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

            //show labels
            Show_BidHuman();
            Show_BidsBots();

            //show cards of bots
            ShowCardsOnCanvas_Bots(true);

            //show cards of human player
            ShowCardsOnCanvas_Human();

            //ShowCardDeal_Animation();

            //Show_PlayedCards_Table(Game.HumanPlayerID, 25);
            //Show_PlayedCards_Table((Game.HumanPlayerID + 1) % 4, 35);
            //Show_PlayedCards_Table((Game.HumanPlayerID + 2) % 4, 45);
            //Show_PlayedCards_Table((Game.HumanPlayerID + 3) % 4, 15);

            PointCurrentPlayer_Canvas(3);
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
            Canvas.SetTop(image, -30);
            Canvas.SetLeft(image, 0);
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            //image.Source = (ImageSource)img.FindResource("ImgBtnLightbulbOn");         
            Canvas.SetTop(image, 0);
            Canvas.SetLeft(image, 0);
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
        private void ShowCardsOnCanvas_Human(bool showAnimation = false)
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
                image.Width = 150;
                image.Height = 190;

                image.HorizontalAlignment = HorizontalAlignment.Center;
                image.VerticalAlignment = VerticalAlignment.Bottom;

                //set animation speed to either show it or not
                double animationSpeed = showAnimation ? 0.5 : 0;
                //Animates card dealing to human player
                AnimateCardTranslation(image, 0, (canvasGame.Height - image.Height) + 13, ((canvasGame.Width - (image.Width +
                    (Game.Players[Game.HumanPlayerID].CardCount - 1) * 110)) / 2)
                    + 110 * counter, (canvasGame.Height - image.Height) + 13, animationSpeed);
               
                //places human player's cards in the center bottom of the canvas
                //image.Margin = new Thickness(((canvasGame.Width - (image.Width +
                //(Game.Players[Game.HumanPlayerID].CardCount - 1) * 110)) / 2) +
                //  110 * counter, (canvasGame.Height - image.Height) + 13, 0, 0);

                //image.Margin = new Thickness(0, 0, 0, 0);

                //changes cursor to hand cursor when pointed at cards
                image.Cursor = Cursors.Hand;

                //slightly pulls up the given card when mouse enters
                image.MouseEnter += new MouseEventHandler(Image_MouseEnter);
                //puts the card back down when mouse leaves
                image.MouseLeave += new MouseEventHandler(Image_MouseLeave);

                image.MouseDown += new MouseButtonEventHandler(Show_PlayedCards_Table);

                //adds the card to the canvas
                canvasGame.Children.Add(image);

                counter++;
            }
        }

        /// <summary>
        /// Show cards(unrevealed) of bots on canvas
        /// </summary>
        private void ShowCardsOnCanvas_Bots(bool showAnimation = false)
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
                        bitmap.UriSource = new Uri(faceDownCard, UriKind.Relative);
                        bitmap.EndInit();
                        Image image = new Image();
                        image.Source = bitmap;
                        image.Width = 70;
                        image.Height = 150;

                        image.HorizontalAlignment = HorizontalAlignment.Left;

                        //set animation speed to either show it or not
                        double animationSpeed = showAnimation ? 0.5 : 0;
                        //show animation of cards being dealt
                        AnimateCardTranslation(image, canvasGame.Width - image.Width - 5,0,canvasGame.Width - image.Width - 5, ((canvasGame.Height -
                            (((Game.Players[i].CardCount - 1) * 28) + image.Height)) / 2) + (counter * 28) - 50,animationSpeed);

                        image.Margin = new Thickness(0, 0, 0, 0);

                        canvasGame.Children.Add(image);
                    }
                }
                else if (j == 1)//player on the over the top
                {
                    for (int counter = 0; counter < Game.Players[id].CardCount; counter++)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(faceDownCard, UriKind.Relative);
                        bitmap.EndInit();
                        Image image = new Image();
                        image.Source = bitmap;
                        image.Width = 90;
                        image.Height = 100;

                        image.HorizontalAlignment = HorizontalAlignment.Center;
                        image.VerticalAlignment = VerticalAlignment.Top;

                        //places bot2 player's cards in the center top of the canvas
                        AnimateCardTranslation(image,0,0,((canvasGame.Width - (image.Width +
                    (Game.Players[id].CardCount - 1) * 35)) / 2) + (35 * counter),0,0.5);

                        image.Margin = new Thickness(0, 0, 0, 0);

                        canvasGame.Children.Add(image);
                    }
                }
                else if (j == 2)//player on left handside
                {
                    for (int counter = 0; counter < Game.Players[id].CardCount; counter++)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(faceDownCard, UriKind.Relative);
                        bitmap.EndInit();
                        Image image = new Image();
                        image.Source = bitmap;
                        image.Width = 70;
                        image.Height = 150;

                        image.HorizontalAlignment = HorizontalAlignment.Left;

                        //places bot1 player's cards in the right of the canvas
                        AnimateCardTranslation(image,0,0,10, ((canvasGame.Height -
                            (((Game.Players[id].CardCount - 1) * 28) + image.Height)) / 2) + (counter * 28) - 50,0.5);

                        image.Margin = new Thickness(0, 0, 0, 0);

                        canvasGame.Children.Add(image);
                    }
                }
            }
        }

        /// <summary>
        /// Shows bid, win count and icon for human player on canvas
        /// </summary>
        private void Show_BidHuman()
        {
            Label name = new Label();
            name.Content = "[You]";
            name.HorizontalAlignment = HorizontalAlignment.Center;
            name.VerticalAlignment = VerticalAlignment.Bottom;
            name.Margin = new Thickness((canvasGame.Width / 2) - 180, canvasGame.Height - 240, 0, 0);
            name.FontSize = 45;
            name.FontWeight = FontWeights.Bold;
            name.FontFamily = new FontFamily("Georgia");
            name.Foreground = Brushes.DarkGreen;

            Label bid = new Label();
            bid.Content = "Win: " + Game.TricksWon[Game.HumanPlayerID];
            bid.HorizontalAlignment = HorizontalAlignment.Center;
            bid.VerticalAlignment = VerticalAlignment.Bottom;
            bid.Margin = new Thickness((canvasGame.Width / 2) + 20, canvasGame.Height - 240, 0, 0);
            bid.FontSize = 15;
            bid.FontWeight = FontWeights.Bold;
            bid.FontFamily = new FontFamily("Courier");
            //bid.Foreground = Brushes.DarkGreen;
            bid.Background = Brushes.DeepSkyBlue;

            Label score = new Label();
            score.Content = "Bid: " + Game.Bidding[Game.HumanPlayerID];
            score.HorizontalAlignment = HorizontalAlignment.Center;
            score.VerticalAlignment = VerticalAlignment.Bottom;
            score.Margin = new Thickness((canvasGame.Width / 2) + 20, canvasGame.Height - 210, 0, 0);
            score.FontSize = 15;
            score.FontWeight = FontWeights.Bold;
            score.FontFamily = new FontFamily("Courier");
            //bid.Foreground = Brushes.DarkGreen;
            score.Background = Brushes.IndianRed;

            canvasGame.Children.Add(name);
            canvasGame.Children.Add(bid);
            canvasGame.Children.Add(score);

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("/cards/personicons/personicon.png", UriKind.Relative);
            bitmap.EndInit();
            Image image = new Image();
            image.Source = bitmap;
            image.Width = 70;
            image.Height = 70;

            image.HorizontalAlignment = HorizontalAlignment.Left;

            //places bot1 player's cards in the right of the canvas
            image.Margin = new Thickness(-50 + canvasGame.Width / 2, canvasGame.Height - 250, 0, 0);

            canvasGame.Children.Add(image);
        }

        /// <summary>
        /// Shows bid, win counts and names of bots, along with icon on canvas
        /// </summary>
        private void Show_BidsBots()
        {
            for (int i = (Game.HumanPlayerID + 1) % 4, j = 0; j < 3; i++, j++)
            {
                //bot players id
                int id = i % 4;
                string personIcon = "/cards/personicons/personicon" + Game.Players[id].IconNumber + ".png";

                if (j == 0)//player on right handside
                {
                    Label name = new Label();
                    name.Content = String.Format("[{0}]", Game.Players[id].Name);//Game.Players[id].Name);
                    name.HorizontalAlignment = HorizontalAlignment.Right;
                    name.VerticalAlignment = VerticalAlignment.Bottom;
                    name.Margin = new Thickness(canvasGame.Width - 205, canvasGame.Height / 2 - 205, 0, 0);
                    name.FontSize = 35;
                    name.FontWeight = FontWeights.Bold;
                    name.FontFamily = new FontFamily("Georgia");
                    name.Foreground = Brushes.SeaGreen;

                    Label bid = new Label();
                    bid.Content = "Win: " + Game.TricksWon[id];
                    bid.HorizontalAlignment = HorizontalAlignment.Center;
                    bid.VerticalAlignment = VerticalAlignment.Bottom;
                    bid.Margin = new Thickness(canvasGame.Width - 150, canvasGame.Height / 2 - 80, 0, 0);
                    bid.FontSize = 15;
                    bid.FontWeight = FontWeights.Bold;
                    bid.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    bid.Background = Brushes.DeepSkyBlue;

                    Label score = new Label();
                    score.Content = "Bid: " + Game.Bidding[id];
                    score.HorizontalAlignment = HorizontalAlignment.Center;
                    score.VerticalAlignment = VerticalAlignment.Bottom;
                    score.Margin = new Thickness(canvasGame.Width - 150, canvasGame.Height / 2 - 50, 0, 0);
                    score.FontSize = 15;
                    score.FontWeight = FontWeights.Bold;
                    score.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    score.Background = Brushes.IndianRed;

                    canvasGame.Children.Add(name);
                    canvasGame.Children.Add(bid);
                    canvasGame.Children.Add(score);

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(personIcon, UriKind.Relative);
                    bitmap.EndInit();
                    Image image = new Image();
                    image.Source = bitmap;
                    image.Width = 70;
                    image.Height = 70;

                    image.HorizontalAlignment = HorizontalAlignment.Left;

                    //places bot1 player's cards in the right of the canvas
                    image.Margin = new Thickness(canvasGame.Width - 160, canvasGame.Height / 2 - 155, 0, 0);

                    canvasGame.Children.Add(image);
                }
                else if (j == 1)//player over the top
                {
                    Label name = new Label();
                    name.Content = "[" + Game.Players[id].Name + "]";
                    name.HorizontalAlignment = HorizontalAlignment.Center;
                    name.VerticalAlignment = VerticalAlignment.Bottom;
                    name.Margin = new Thickness((canvasGame.Width / 2) - 175, 110, 0, 0);
                    name.FontSize = 35;
                    name.FontWeight = FontWeights.Bold;
                    name.FontFamily = new FontFamily("Georgia");
                    name.Foreground = Brushes.DarkGreen;

                    Label bid = new Label();
                    bid.Content = "Win: " + Game.TricksWon[Game.HumanPlayerID];
                    bid.HorizontalAlignment = HorizontalAlignment.Center;
                    bid.VerticalAlignment = VerticalAlignment.Bottom;
                    bid.Margin = new Thickness((canvasGame.Width / 2) + 25, 110, 0, 0);
                    bid.FontSize = 15;
                    bid.FontWeight = FontWeights.Bold;
                    bid.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    bid.Background = Brushes.DeepSkyBlue;

                    Label score = new Label();
                    score.Content = "Bid: " + Game.Bidding[Game.HumanPlayerID];
                    score.HorizontalAlignment = HorizontalAlignment.Center;
                    score.VerticalAlignment = VerticalAlignment.Bottom;
                    score.Margin = new Thickness((canvasGame.Width / 2) + 25, 140, 0, 0);
                    score.FontSize = 15;
                    score.FontWeight = FontWeights.Bold;
                    score.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    score.Background = Brushes.IndianRed;

                    canvasGame.Children.Add(name);
                    canvasGame.Children.Add(bid);
                    canvasGame.Children.Add(score);

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(personIcon, UriKind.Relative);
                    bitmap.EndInit();
                    Image image = new Image();
                    image.Source = bitmap;
                    image.Width = 70;
                    image.Height = 70;

                    image.HorizontalAlignment = HorizontalAlignment.Left;

                    //places bot1 player's cards in the right of the canvas
                    image.Margin = new Thickness(canvasGame.Width / 2 - 50, 105, 0, 0);

                    canvasGame.Children.Add(image);
                }
                else if (j == 2)//player on the left side
                {
                    Label name = new Label();
                    name.Content = String.Format("[{0}]", Game.Players[id].Name);//Game.Players[id].Name);
                    name.HorizontalAlignment = HorizontalAlignment.Right;
                    name.VerticalAlignment = VerticalAlignment.Bottom;
                    name.Margin = new Thickness(80, canvasGame.Height / 2 - 205, 0, 0);
                    name.FontSize = 35;
                    name.FontWeight = FontWeights.Bold;
                    name.FontFamily = new FontFamily("Georgia");
                    name.Foreground = Brushes.SeaGreen;

                    Label bid = new Label();
                    bid.Content = "Win: " + Game.TricksWon[id];
                    bid.HorizontalAlignment = HorizontalAlignment.Center;
                    bid.VerticalAlignment = VerticalAlignment.Bottom;
                    bid.Margin = new Thickness(90, canvasGame.Height / 2 - 80, 0, 0);
                    bid.FontSize = 15;
                    bid.FontWeight = FontWeights.Bold;
                    bid.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    bid.Background = Brushes.DeepSkyBlue;

                    Label score = new Label();
                    score.Content = "Bid: " + Game.Bidding[id];
                    score.HorizontalAlignment = HorizontalAlignment.Center;
                    score.VerticalAlignment = VerticalAlignment.Bottom;
                    score.Margin = new Thickness(90, canvasGame.Height / 2 - 50, 0, 0);
                    score.FontSize = 15;
                    score.FontWeight = FontWeights.Bold;
                    score.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    score.Background = Brushes.IndianRed;

                    canvasGame.Children.Add(name);
                    canvasGame.Children.Add(bid);
                    canvasGame.Children.Add(score);

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(personIcon, UriKind.Relative);
                    bitmap.EndInit();
                    Image image = new Image();
                    image.Source = bitmap;
                    image.Width = 70;
                    image.Height = 70;

                    image.HorizontalAlignment = HorizontalAlignment.Left;

                    //places bot1 player's cards in the right of the canvas
                    image.Margin = new Thickness(90, canvasGame.Height / 2 - 155, 0, 0);

                    canvasGame.Children.Add(image);
                }
            }
        }

        /// <summary>
        /// Shows played cards on the table given the playerID and cardID
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="cardID"></param>
        private void Show_PlayedCards_Table(Object sender, MouseEventArgs e)
        {
            int playerID, cardID = 49;
            Image image = sender as Image;
            //BitmapImage bitmap = new BitmapImage();
            //bitmap.BeginInit();
            //bitmap.UriSource = new Uri(mapCardtoFile(Game.CardIDtoCard(cardID)), UriKind.Relative);
            //bitmap.EndInit();
            //Image image = new Image();
            //image.Source = bitmap;
            image.Width = 115;
            image.Height = 200;
            image.HorizontalAlignment = HorizontalAlignment.Left;

            playerID = Game.HumanPlayerID;

            Random rnd = new Random();
            int rndm = rnd.Next(99999);

            if (playerID == Game.HumanPlayerID)//human players card
            {
                Random rand = new Random(rndm);
                int angle = rand.Next(41) - 20; //rest positoin is 0 degree, with -50 to 50 range

                //Vector offset = VisualTreeHelper.GetOffset(image);//position of the image in the container

                RotateTransform rotateTransform = new RotateTransform(angle);
                image.RenderTransform = rotateTransform;

                image.Margin = new Thickness(canvasGame.Width / 2 - 45, canvasGame.Height / 2 - 100, 0, 0);

                //disable card control after it's played
                image.IsEnabled = false;
            }
            else if ((Game.HumanPlayerID + 1) % 4 == playerID)//player on the right side
            {
                Random rand = new Random(rndm * 2);
                int angle = rand.Next(70, 111);//rest positoin is 90 degree, with 40 to 140 range

                RotateTransform rotateTransform = new RotateTransform(angle);
                image.RenderTransform = rotateTransform;
                image.Margin = new Thickness(canvasGame.Width / 2 + 180, canvasGame.Height / 2 - 100, 0, 0);
            }
            else if ((Game.HumanPlayerID + 2) % 4 == playerID)//player over the top
            {
                Random rand = new Random(rndm * 3);
                int angle = rand.Next(41)-20;//rest positoin is 180 degree, with 130 to 230 range

                RotateTransform rotateTransform = new RotateTransform(angle);
                image.RenderTransform = rotateTransform;
                image.Margin = new Thickness(canvasGame.Width / 2 -25, canvasGame.Height / 2-180, 0, 0);
            }
            else if ((Game.HumanPlayerID + 3) % 4 == playerID)//player over the top
            {
                Random rand = new Random(rndm * 3);
                int angle = rand.Next(70,111)*(-1);//rest positoin is -90 degree, with -70 to -110 range

                RotateTransform rotateTransform = new RotateTransform(angle);
                image.RenderTransform = rotateTransform;
                image.Margin = new Thickness(canvasGame.Width / 2 -150, canvasGame.Height / 2 - 20, 0, 0);
            }
            //canvasGame.Children.Add(image);
        }

        /// <summary>
        /// Animates movement of cards from one point to another
        /// </summary>
        /// <param name="target"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <param name="timeSpan"></param>
        public void AnimateCardTranslation(Image target, double startX, double startY, double endX, double endY, double timeSpan =1)
        {
            TranslateTransform trans = new TranslateTransform();
            target.RenderTransform = trans;
            DoubleAnimation anim1 = new DoubleAnimation(startX, endX, TimeSpan.FromSeconds(timeSpan));
            DoubleAnimation anim2 = new DoubleAnimation(startY, endY, TimeSpan.FromSeconds(timeSpan));
            trans.BeginAnimation(TranslateTransform.XProperty, anim1);
            trans.BeginAnimation(TranslateTransform.YProperty, anim2);
        }

        private void ShowCardDeal_Animation()
        {
            ShowDealReady_Deck(true);
        }

        private void ShowDealReady_Deck(bool deal=false)
        {
            string faceDownCard = "/cards/extras/Back.png";

            for (int i = 0; i < 52; i++)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(faceDownCard, UriKind.Relative);
                bitmap.EndInit();
                Image image = new Image();
                image.Source = bitmap;
                image.Width = 90;
                image.Height = 100;

                image.HorizontalAlignment = HorizontalAlignment.Center;
                image.VerticalAlignment = VerticalAlignment.Top;

                //places the deck of card based on current dealer
                int a = Game.CurrentDealer;
                int x = 0, y = 0;
                if(a == (Game.HumanPlayerID + 3) % 4)
                {
                    x = 80+i/2;
                    y = 300+i/2;
                }
                else if(a == (Game.HumanPlayerID + 1) % 4)
                {
                    x = ((int)(canvasGame.Width) - 170)-i/2;
                    y = 300+i/2;
                }
                else if(a == (Game.HumanPlayerID + 2) % 4)
                {
                    x = ((int)canvasGame.Width / 2) + 15 + i / 2;
                    y = 110 + i / 2;
                }
                else if(a == Game.HumanPlayerID)
                {
                    x = ((int)canvasGame.Width / 2) + 10 + i / 2;
                    y = (int)canvasGame.Height - 240 + i / 2;
                }
                
                image.Margin = new Thickness(x,y,0,0);

                canvasGame.Children.Add(image);
            }
        }

        /// <summary>
        /// Points arrow on the canvas to the current player
        /// </summary>
        /// <param name="playerID"></param>
        private void PointCurrentPlayer_Canvas(int playerID)
        {
            Label arrow = new Label();
            arrow.Content = "⮚";
            arrow.Foreground = Brushes.BurlyWood;
            arrow.FontSize = 45;            
             
            if (playerID == (Game.HumanPlayerID+1)%4)//player on the right side
            {
                arrow.Margin = new Thickness(canvasGame.Width - 240, canvasGame.Height / 2 - 215, 0, 0);
            }
            else if(playerID == (Game.HumanPlayerID+3)%4)//player on the left side
            {
                arrow.Content = "⮘";
                arrow.Margin = new Thickness(200, canvasGame.Height / 2 - 215, 0, 0);
            }
            else if(playerID == (Game.HumanPlayerID+2)%4)//player over the top
            {
                arrow.Margin = new Thickness((canvasGame.Width / 2) - 210, 100, 0, 0);
            }
            else if(playerID == Game.HumanPlayerID)//human player
            {
                arrow.Margin = new Thickness((canvasGame.Width / 2) - 215, canvasGame.Height - 245, 0, 0);
            }

            canvasGame.Children.Add(arrow);
        }
    }
}




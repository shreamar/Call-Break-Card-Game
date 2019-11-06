using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

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

            //Initializes game
            Game.InitializeGame("You", Game.MaxHandsToPlay);

            _ = GamePlayAsync(this, new EventArgs());
        }

        /// <summary>
        /// Performs whole game play 
        /// </summary>
        private async Task GamePlayAsync(object sender, EventArgs e)
        {

            for (int currentHand = 0; currentHand < Game.MaxHandsToPlay; currentHand++)//Iteration of hands played
            {
                //updates current hand data in Game class
                //Game.CurrentHand = currentHand;

                //Reinitialize components to restart hand
                Game.ReinitializeHand();
                TestWindow();

                //Shows deck of card in the dealers side
                ShowDealReady_Deck();

                lblBigInfo_Center.Content = "Dealing Cards...";
                lblBigInfo_Center.Visibility = Visibility.Visible;
                TestWindow();

                //Show players names and icons
                Show_PlayersName_Icon();

                //shows top bar
                //Show_Info_TopBar();
                TestWindow();
                lblTopBar.IsEnabled = false;

                //creates pause effect
                await Task.Delay(TimeSpan.FromMilliseconds(2000));

                //Deals cards to all players
                DealCards();
                TestWindow();

                //changes label to placing bids
                lblBigInfo_Center.Content = "Placing Bids...";

                /// <summary>
                /// Place biddings before playing the hand
                /// </summary>
                for (int i = (Game.CurrentDealer + 1) % 4, counter = 0; counter < 4; counter++, i++)
                {
                    int currentBidder = i % 4;
                    TestWindow();
                    if (currentBidder == Game.HumanPlayerID)//human player
                    {
                        //creates pause effect
                        await Task.Delay(TimeSpan.FromMilliseconds(500));

                        frmPlaceBid frmPlaceBid = new frmPlaceBid();
                        frmPlaceBid.ShowDialog();
                    }
                    else
                    {
                        PlaceBids_Auto(currentBidder);

                        //creates pause effect
                        await Task.Delay(TimeSpan.FromMilliseconds(1000));
                    }
                    TestWindow();

                    Show_PlayersBids_WinCounts(true, true, currentBidder);
                }
                //creates pause effect of 2000ms
                await Task.Delay(TimeSpan.FromMilliseconds(2000));

                //hides the placing bids label
                lblBigInfo_Center.Visibility = Visibility.Hidden;

                //Refresh canvas
                Refresh_Canvas(Game.CurrentDealer, true);
                TestWindow();
                for (int currentTrick = 0; currentTrick < 13; currentTrick++)//iteration of tricks played in a given hand
                {
                    for (int i = Game.CurrentDealer, j = 0; j < 4; i++, j++)//iteration of players
                    {
                        int currentPlayer = i % 4;

                        //creates pause effect
                        await Task.Delay(TimeSpan.FromMilliseconds(500));

                        //Point Current Player
                        //PointCurrentPlayer_Canvas(currentPlayer);

                        ///testing
                        string playables = Game.Players[currentPlayer].Name;
                        foreach (Card card in Game.Players[currentPlayer].Cards)
                        {
                            foreach (int id in Game.Players[currentPlayer].PlayableIDs)
                            {
                                if (id == card.ID)
                                {
                                    playables += " " + card.Name;
                                    break;
                                }
                            }
                        }
                        playables += "\r\nLead: " + (Game.CardIDtoCard(Game.LeadCardID) != null ? Game.CardIDtoCard(Game.LeadCardID).Name : "--") +
                            "\r\nPower: " + (Game.CardIDtoCard(Game.PowerCardID) != null ? Game.CardIDtoCard(Game.PowerCardID).Name : "--");
                        int a = 0;
                        //MessageBox.Show(playables);

                        //creates pause effect of 2000ms
                        //await Task.Delay(TimeSpan.FromMilliseconds(2000));

                        //if (currentPlayer == Game.HumanPlayerID)
                        //{
                        //    if (Game.Players[currentPlayer].IsPlayed)
                        //    {
                        //        //the human player has played a card then continue on to next iteration
                        //        continue;
                        //    }
                        //    else
                        //    {
                        //        //Refresh canvas
                        //        Refresh_Canvas(Game.HumanPlayerID);
                        //        TestWindow();
                        //    }
                        //}
                        //else
                        {
                            PlayCard_Bots(currentPlayer);
                            TestWindow();
                        }

                        ///testing
                        playables = "";
                        playables = "No. of cards in table: " + Game.CardsInTable.Count;
                        playables += "\r\nCards in table:";
                        for (int x = 0; x < Game.CardsInTable.Count; x++)
                        {
                            playables += " " + Game.CardsInTable[x].Name;
                        }
                        //playables += "\rLead: " + Game.CardIDtoCard(Game.LeadCardID) != null ? Game.CardIDtoCard(Game.LeadCardID).Name : "--" +
                        //   "\rPower: " + Game.CardIDtoCard(Game.PowerCardID) != null ? Game.CardIDtoCard(Game.PowerCardID).Name : "--";
                        //MessageBox.Show(playables);

                        TestWindow();
                    }
                    //creates pause effect
                    await Task.Delay(TimeSpan.FromMilliseconds(1000));

                    //Process the results of the current trick
                    Game.ProcessTrickWinner();

                    //Refresh Canvas and show trick winner animation
                    Refresh_Canvas(Game.CurrentTrickWinner, true, true);

                    //Clear cards from table in code
                    Game.CardsInTable.Clear();
                }
                //creates pause effect
                await Task.Delay(TimeSpan.FromMilliseconds(1000));

                //Updates Scoreboard at the end of the hand
                Game.UpdateScoreBoard();

                //Shows score board
                frmScoreBoard scoreBoard = new frmScoreBoard();
                scoreBoard.ShowDialog();
            }
        }

        /// <summary>
        /// Deals card to all players and show relevant information on canvas
        /// </summary>
        private void DealCards()
        {
            //Deals cards
            Game.DealCards();

            //clears canvas
            canvasGame.Children.Clear();

            //show player icons, names, bids and win counts
            Show_PlayersName_Icon();


            //Show bots cards with dealing animation
            ShowCardsOnCanvas_Bots(true);

            //Show human's cards with dealing animation
            ShowCardsOnCanvas_Human(true, false);

            //point current player
            PointCurrentPlayer_Canvas(Game.CurrentDealer);

            //Show top bar
            Show_Info_TopBar();
            TestWindow();
        }

        /// <summary>
        /// Places automatic bid for selected player ID
        /// [Needs better algorithm to perform this task]
        /// </summary>
        private void PlaceBids_Auto(int PlayerID)
        {
            Random rnd = new Random();

            int counter = 0;
            foreach (Player player in Game.Players)
            {
                if (counter == PlayerID)
                {
                    //Random rand = new Random();
                    Game.PlaceBid(PlayerID, rnd.Next(1, 7));
                    break;
                }
                counter++;
            }
        }

        private void Refresh_Canvas(int currentPlayer, bool showCardsOnTable = true, bool showTrickAnimation = false)
        {
            //First clear the canvas
            canvasGame.Children.Clear();

            //Add names and Icons
            Show_PlayersName_Icon();

            //Add the current player pointer
            PointCurrentPlayer_Canvas(currentPlayer);

            //Add bids and win counts for players
            Show_PlayersBids_WinCounts();

            //Add bot players cards
            if (showCardsOnTable)
            {
                ShowCardsOnCanvas_Bots(true, true);
            }
            else
            {
                ShowCardsOnCanvas_Bots();
            }

            //Add human player's cards
            if (currentPlayer == Game.HumanPlayerID)
            {
                ShowCardsOnCanvas_Human(true, true, true);              //if current player is human then enable card
            }
            else
            {
                ShowCardsOnCanvas_Human(false, false);  //if not disable cards control
            }

            if (showCardsOnTable)//show cards on table while the trick is being played
            {
                int i = (4 + currentPlayer - Game.CardsInTable.Count + 1) % 4;
                for (int j = 0; j < Game.CardsInTable.Count; j++)
                {
                    int id = i % 4;

                    if (showTrickAnimation)
                    {
                        //shows card translation animation at the end of the trick
                        Show_PlayedCards_Table(id, Game.CardsInTable[j], true, Game.CurrentTrickWinner);
                    }
                    else
                    {
                        Show_PlayedCards_Table(id, Game.CardsInTable[j]);
                    }
                    i++;
                }
            }

            //Refresh top bar
            //Show_Info_TopBar();
            TestWindow();
        }

        /// <summary>
        /// Plays card for bots
        /// </summary>
        /// <param name="currentPlayer"></param>
        private void PlayCard_Bots(int currentPlayer)
        {
            //Randomly selects a card from playables
            Random rnd = new Random();
            int cardID = Game.Players[currentPlayer].PlayableIDs[rnd.Next(Game.Players[currentPlayer].PlayableIDs.Count)];

            //Play the randomly selected card
            Game.Players[currentPlayer].PlayCard(cardID);

            Refresh_Canvas(currentPlayer,true);
            //Show_PlayedCards_Table(currentPlayer, cardID);
        }

        private void btnDealCards_Click(object sender, RoutedEventArgs e)
        {
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
                    Game.Players[Game.CurrentDealer].PlayCard(a);
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
        private string MapCardToFile(Card card)
        {
            string cardNumber = "", cardSuit = "", number = "";
            if ((int)card.Number >= 0 && (int)card.Number < 9)
            {
                cardNumber = ((int)card.Number + 2).ToString();
            }
            else
            {
                cardNumber = card.Number.ToString().ToLower();
            }

            //since the card with pictures of jack, queen, king are in the same card name ending in 2
            //e.g. King of Hearts has no picture in "king_of_hearts.png", but has image of kind in "king_of_hearts2.png"
            if ((int)card.Number >= 9 && (int)card.Number < 12)
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
        private void ShowCardsOnCanvas_Human(bool showAnimation = false, bool enableCards = true, bool inGameAnimation = false)
        {
            //canvasGame.Children.Clear();

            int counter = 0;
            foreach (Card card in Game.Players[Game.HumanPlayerID].Cards)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(MapCardToFile(card), UriKind.Relative);
                bitmap.EndInit();
                Image image = new Image();
                image.Source = bitmap;
                image.Width = 150;
                image.Height = 190;

                image.HorizontalAlignment = HorizontalAlignment.Center;
                image.VerticalAlignment = VerticalAlignment.Bottom;

                //set animation speed to either show it or not
                double animationSpeed = showAnimation ? 1 : 0;

                if (inGameAnimation)//animation of reducing card when a card is played
                {
                    AnimateCardTranslation(image, card.XPos, card.YPos, ((canvasGame.Width - (image.Width +
                   (Game.Players[Game.HumanPlayerID].CardCount - 1) * 110)) / 2)
                   + 110 * counter, (canvasGame.Height - image.Height) + 13, 0.25);
                }
                else
                {
                    //Animates card dealing to human player
                    AnimateCardTranslation(image, canvasGame.Width / 2, (canvasGame.Height - image.Height) + 13, ((canvasGame.Width - (image.Width +
                        (Game.Players[Game.HumanPlayerID].CardCount - 1) * 110)) / 2)
                        + 110 * counter, (canvasGame.Height - image.Height) + 13, animationSpeed);
                }

                //record the current postion of the card
                card.XPos = (int)(((canvasGame.Width - (image.Width +
                   (Game.Players[Game.HumanPlayerID].CardCount - 1) * 110)) / 2)
                   + 110 * counter);
                card.YPos = (int)((canvasGame.Height - image.Height) + 13);

                //Finds out if the given card is playable card or not
                int match = 0;
                foreach (int ndx in Game.Players[Game.HumanPlayerID].PlayableIDs)
                {
                    if (card.ID == ndx)
                    {
                        match++;
                        break;
                    }
                }

                //blurrs and disables the card if there is no match with playabel list
                if (match == 0)
                {
                    _ = new MouseEventHandler(Image_BitmapBlurEffect);
                }

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

                image.MouseDown += new MouseButtonEventHandler(Show_PlayedCards_Human);

                //adds the card to the canvas
                canvasGame.Children.Add(image);

                //enables or disables human players cards
                image.IsEnabled = enableCards;

                counter++;
            }
        }

        /// <summary>
        /// Show cards(unrevealed) of bots on canvas
        /// </summary>
        private void ShowCardsOnCanvas_Bots(bool showAnimation = false, bool inGameAnimation = false)
        {
            //canvasGame.Children.Clear();

            string faceDownCard = "/cards/extras/Back.png";

            //set animation speed to either show it or not
            double animationSpeed = showAnimation ? 1 : 0;

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

                        if (inGameAnimation)//animation of reducing card when a card is played
                        {
                            AnimateCardTranslation(image, Game.Players[id].Cards[counter].XPos, Game.Players[id].Cards[counter].YPos, 
                                canvasGame.Width - image.Width - 5,((canvasGame.Height - 
                                (((Game.Players[i].CardCount - 1) * 28) + image.Height)) / 2) + (counter * 28) - 50, 0.25);
                        }
                        else
                        {
                            //show animation of cards being dealt
                            AnimateCardTranslation(image, canvasGame.Width - image.Width - 5, canvasGame.Height / 2 - 100, canvasGame.Width - image.Width - 5, ((canvasGame.Height -
                            (((Game.Players[i].CardCount - 1) * 28) + image.Height)) / 2) + (counter * 28) - 50, animationSpeed);
                        }

                        //record the current position of the card in the canvas
                        Game.Players[id].Cards[counter].XPos = (int)(canvasGame.Width - image.Width - 5);
                        Game.Players[id].Cards[counter].YPos = (int)(((canvasGame.Height -
                            (((Game.Players[i].CardCount - 1) * 28) + image.Height)) / 2) + (counter * 28) - 50);

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

                        if (inGameAnimation)////animation of reducing card when a card is played
                        {
                            AnimateCardTranslation(image, Game.Players[id].Cards[counter].XPos, Game.Players[id].Cards[counter].YPos,
                                ((canvasGame.Width - (image.Width + (Game.Players[id].CardCount - 1) * 35)) / 2) + (35 * counter), 0, 0.25);
                        }
                        else
                        {
                            //places bot2 player's cards in the center top of the canvas
                            AnimateCardTranslation(image, canvasGame.Width / 2, 0, ((canvasGame.Width - (image.Width +
                                (Game.Players[id].CardCount - 1) * 35)) / 2) + (35 * counter), 0, animationSpeed);
                        }

                        //record the current position of the card in the canvas
                        Game.Players[id].Cards[counter].XPos = (int)(((canvasGame.Width - (image.Width + (Game.Players[id].CardCount - 1) * 35)) / 2) + (35 * counter));
                        Game.Players[id].Cards[counter].YPos = 0;

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

                        if (inGameAnimation)//animation of reducing card when a card is played
                        {
                            AnimateCardTranslation(image, Game.Players[id].Cards[counter].XPos, Game.Players[id].Cards[counter].YPos, 10, ((canvasGame.Height -
                           (((Game.Players[id].CardCount - 1) * 28) + image.Height)) / 2) + (counter * 28) - 50, 0.25);
                        }
                        else
                        {
                            //places bot1 player's cards in the right of the canvas
                            AnimateCardTranslation(image, 10, canvasGame.Height / 2 - 100, 10, ((canvasGame.Height -
                            (((Game.Players[id].CardCount - 1) * 28) + image.Height)) / 2) + (counter * 28) - 50, animationSpeed);
                        }

                        //record the current position the card on the canvas
                        Game.Players[id].Cards[counter].XPos = 10;
                        Game.Players[id].Cards[counter].YPos = (int)(((canvasGame.Height -
                         (((Game.Players[id].CardCount - 1) * 28) + image.Height)) / 2) + (counter * 28) - 50);

                        image.Margin = new Thickness(0, 0, 0, 0);

                        canvasGame.Children.Add(image);
                    }
                }
            }
        }

        /// <summary>
        /// Shows player's name and icon on the canvas
        /// </summary>
        private void Show_PlayersName_Icon()
        {
            for (int i = (Game.HumanPlayerID) % 4, j = 0; j < 4; i++, j++)
            {
                //player ids
                int id = i % 4;
                string personIcon = "/cards/personicons/personicon" + Game.Players[id].IconNumber + ".png";

                if (j == 0)//human player
                {
                    //label for name
                    Label name = new Label();
                    name.Content = "[You]";
                    name.HorizontalAlignment = HorizontalAlignment.Center;
                    name.VerticalAlignment = VerticalAlignment.Bottom;
                    name.Margin = new Thickness((canvasGame.Width / 2) - 180, canvasGame.Height - 240, 0, 0);
                    name.FontSize = 45;
                    name.FontWeight = FontWeights.Bold;
                    name.FontFamily = new FontFamily("Georgia");
                    name.Foreground = Brushes.DarkGreen;

                    //icon
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("/cards/personicons/personicon.png", UriKind.Relative);
                    bitmap.EndInit();
                    Image image = new Image();
                    image.Source = bitmap;
                    image.Width = 70;
                    image.Height = 70;
                    image.HorizontalAlignment = HorizontalAlignment.Left;
                    image.Margin = new Thickness(-50 + canvasGame.Width / 2, canvasGame.Height - 250, 0, 0);

                    canvasGame.Children.Add(name);
                    canvasGame.Children.Add(image);
                }
                else if (j == 1)//player on the right side
                {
                    //label for name
                    Label name = new Label();
                    name.Content = String.Format("[{0}]", Game.Players[id].Name);//Game.Players[id].Name);
                    name.HorizontalAlignment = HorizontalAlignment.Right;
                    name.VerticalAlignment = VerticalAlignment.Bottom;
                    name.Margin = new Thickness(canvasGame.Width - 205, canvasGame.Height / 2 - 205, 0, 0);
                    name.FontSize = 35;
                    name.FontWeight = FontWeights.Bold;
                    name.FontFamily = new FontFamily("Georgia");
                    name.Foreground = Brushes.SeaGreen;

                    //icon
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(personIcon, UriKind.Relative);
                    bitmap.EndInit();
                    Image image = new Image();
                    image.Source = bitmap;
                    image.Width = 70;
                    image.Height = 70;
                    image.HorizontalAlignment = HorizontalAlignment.Left;
                    image.Margin = new Thickness(canvasGame.Width - 160, canvasGame.Height / 2 - 155, 0, 0);

                    canvasGame.Children.Add(name);
                    canvasGame.Children.Add(image);
                }
                else if (j == 2)//player over the top
                {
                    //label for name
                    Label name = new Label();
                    name.Content = "[" + Game.Players[id].Name + "]";
                    name.HorizontalAlignment = HorizontalAlignment.Center;
                    name.VerticalAlignment = VerticalAlignment.Bottom;
                    name.Margin = new Thickness((canvasGame.Width / 2) - 175, 110, 0, 0);
                    name.FontSize = 35;
                    name.FontWeight = FontWeights.Bold;
                    name.FontFamily = new FontFamily("Georgia");
                    name.Foreground = Brushes.DarkGreen;

                    //icon
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(personIcon, UriKind.Relative);
                    bitmap.EndInit();
                    Image image = new Image();
                    image.Source = bitmap;
                    image.Width = 70;
                    image.Height = 70;
                    image.HorizontalAlignment = HorizontalAlignment.Left;
                    image.Margin = new Thickness(canvasGame.Width / 2 - 50, 105, 0, 0);

                    canvasGame.Children.Add(name);
                    canvasGame.Children.Add(image);
                }
                else if (j == 3)//player on the left hand side
                {
                    //label for name
                    Label name = new Label();
                    name.Content = String.Format("[{0}]", Game.Players[id].Name);//Game.Players[id].Name);
                    name.HorizontalAlignment = HorizontalAlignment.Right;
                    name.VerticalAlignment = VerticalAlignment.Bottom;
                    name.Margin = new Thickness(80, canvasGame.Height / 2 - 205, 0, 0);
                    name.FontSize = 35;
                    name.FontWeight = FontWeights.Bold;
                    name.FontFamily = new FontFamily("Georgia");
                    name.Foreground = Brushes.SeaGreen;

                    //icon
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(personIcon, UriKind.Relative);
                    bitmap.EndInit();
                    Image image = new Image();
                    image.Source = bitmap;
                    image.Width = 70;
                    image.Height = 70;
                    image.HorizontalAlignment = HorizontalAlignment.Left;
                    image.Margin = new Thickness(90, canvasGame.Height / 2 - 155, 0, 0);

                    canvasGame.Children.Add(name);
                    canvasGame.Children.Add(image);
                }
            }
        }

        /// <summary>
        /// Shows bids and win counts of all players on the canvas
        /// </summary>
        private void Show_PlayersBids_WinCounts(bool animate = false, bool showOnlyOne = false, int playerID = 0)
        {
            for (int i = (Game.HumanPlayerID) % 4, j = -1; j < 3; i++, j++)
            {
                //players id
                int id = i % 4;

                //jumps interation to the required player when only one players info have to be shown
                if (showOnlyOne)
                {
                    i += (4 - id + playerID) % 4;
                    j += (4 - id + playerID) % 4;
                }

                //updated player id
                id = i % 4;

                string personIcon = "/cards/personicons/personicon" + Game.Players[id].IconNumber + ".png";

                if (j == -1)//human player
                {
                    Label bid = new Label();
                    bid.Content = "Win: " + Game.TricksWon[Game.HumanPlayerID];
                    bid.HorizontalAlignment = HorizontalAlignment.Center;
                    bid.VerticalAlignment = VerticalAlignment.Bottom;
                    bid.FontSize = 15;
                    bid.FontWeight = FontWeights.Bold;
                    bid.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    bid.Background = Brushes.DeepSkyBlue;

                    Label score = new Label();
                    score.Content = "Bid: " + Game.Bidding[Game.HumanPlayerID];
                    score.HorizontalAlignment = HorizontalAlignment.Center;
                    score.VerticalAlignment = VerticalAlignment.Bottom;

                    score.FontSize = 15;
                    score.FontWeight = FontWeights.Bold;
                    score.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    score.Background = Brushes.IndianRed;

                    if (animate)
                    {
                        AnimateCardTranslation(bid, canvasGame.Width, canvasGame.Height - 240, (canvasGame.Width / 2) + 20, canvasGame.Height - 240, 0.5);
                        AnimateCardTranslation(score, canvasGame.Width, canvasGame.Height - 210, (canvasGame.Width / 2) + 20, canvasGame.Height - 210, 0.5);
                    }
                    else
                    {
                        bid.Margin = new Thickness((canvasGame.Width / 2) + 20, canvasGame.Height - 240, 0, 0);
                        score.Margin = new Thickness((canvasGame.Width / 2) + 20, canvasGame.Height - 210, 0, 0);
                    }

                    canvasGame.Children.Add(bid);
                    canvasGame.Children.Add(score);
                }
                else if (j == 0)//player on right handside
                {
                    Label bid = new Label();
                    bid.Content = "Win: " + Game.TricksWon[id];
                    bid.HorizontalAlignment = HorizontalAlignment.Center;
                    bid.VerticalAlignment = VerticalAlignment.Bottom;
                    bid.FontSize = 15;
                    bid.FontWeight = FontWeights.Bold;
                    bid.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    bid.Background = Brushes.DeepSkyBlue;

                    Label score = new Label();
                    score.Content = "Bid: " + Game.Bidding[id];
                    score.HorizontalAlignment = HorizontalAlignment.Center;
                    score.VerticalAlignment = VerticalAlignment.Bottom;
                    score.FontSize = 15;
                    score.FontWeight = FontWeights.Bold;
                    score.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    score.Background = Brushes.IndianRed;

                    if (animate)
                    {
                        AnimateCardTranslation(bid, canvasGame.Width - 150, 0, canvasGame.Width - 150, canvasGame.Height / 2 - 80, 0.4);
                        AnimateCardTranslation(score, canvasGame.Width - 150, 0, canvasGame.Width - 150, canvasGame.Height / 2 - 50, 0.4);
                    }
                    else
                    {
                        bid.Margin = new Thickness(canvasGame.Width - 150, canvasGame.Height / 2 - 80, 0, 0);
                        score.Margin = new Thickness(canvasGame.Width - 150, canvasGame.Height / 2 - 50, 0, 0);
                    }

                    canvasGame.Children.Add(bid);
                    canvasGame.Children.Add(score);
                }
                else if (j == 1)//player over the top
                {
                    Label bid = new Label();
                    bid.Content = "Win: " + Game.TricksWon[id];
                    bid.HorizontalAlignment = HorizontalAlignment.Center;
                    bid.VerticalAlignment = VerticalAlignment.Bottom;
                    bid.FontSize = 15;
                    bid.FontWeight = FontWeights.Bold;
                    bid.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    bid.Background = Brushes.DeepSkyBlue;

                    Label score = new Label();
                    score.Content = "Bid: " + Game.Bidding[id];
                    score.HorizontalAlignment = HorizontalAlignment.Center;
                    score.VerticalAlignment = VerticalAlignment.Bottom;
                    score.FontSize = 15;
                    score.FontWeight = FontWeights.Bold;
                    score.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    score.Background = Brushes.IndianRed;

                    if (animate)
                    {
                        AnimateCardTranslation(bid, 0, 110, (canvasGame.Width / 2) + 25, 110, 0.5);
                        AnimateCardTranslation(score, 0, 140, (canvasGame.Width / 2) + 25, 140, 0.5);
                    }
                    else
                    {
                        bid.Margin = new Thickness((canvasGame.Width / 2) + 25, 110, 0, 0);
                        score.Margin = new Thickness((canvasGame.Width / 2) + 25, 140, 0, 0);
                    }

                    canvasGame.Children.Add(bid);
                    canvasGame.Children.Add(score);
                }
                else if (j == 2)//player on the left side
                {
                    Label bid = new Label();
                    bid.Content = "Win: " + Game.TricksWon[id];
                    bid.HorizontalAlignment = HorizontalAlignment.Center;
                    bid.VerticalAlignment = VerticalAlignment.Bottom;
                    bid.FontSize = 15;
                    bid.FontWeight = FontWeights.Bold;
                    bid.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    bid.Background = Brushes.DeepSkyBlue;

                    Label score = new Label();
                    score.Content = "Bid: " + Game.Bidding[id];
                    score.HorizontalAlignment = HorizontalAlignment.Center;
                    score.VerticalAlignment = VerticalAlignment.Bottom;
                    score.FontSize = 15;
                    score.FontWeight = FontWeights.Bold;
                    score.FontFamily = new FontFamily("Courier");
                    //bid.Foreground = Brushes.DarkGreen;
                    score.Background = Brushes.IndianRed;

                    if (animate)
                    {
                        AnimateCardTranslation(bid, 90, canvasGame.Height, 90, canvasGame.Height / 2 - 80, 0.4);
                        AnimateCardTranslation(score, 90, canvasGame.Height, 90, canvasGame.Height / 2 - 50, 0.4);
                    }
                    else
                    {
                        bid.Margin = new Thickness(90, canvasGame.Height / 2 - 80, 0, 0);
                        score.Margin = new Thickness(90, canvasGame.Height / 2 - 50, 0, 0);
                    }

                    canvasGame.Children.Add(bid);
                    canvasGame.Children.Add(score);
                }

                //breaks loop if the required player is found and job is done
                if (showOnlyOne)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Shows played cards on the table given the playerID and cardID
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="cardID"></param>
        private void Show_PlayedCards_Table(int playerID, Card playedCard, bool animate = false, int winnerID = 0)
        {
            //playerID = Game.HumanPlayerID;//only for testing purpose

            Random rnd = new Random();
            int rndm = rnd.Next(99999);

            double directionX = 0, directionY = 0;
            //decides the direction cards go when trick is won
            if (winnerID == Game.HumanPlayerID) // when winner is human player
            {
                directionX = canvasGame.Width / 2 - 45;
                directionY = canvasGame.Height;
            }
            else if (winnerID == (Game.HumanPlayerID + 1) % 4) //winner is player on the right
            {
                directionX = canvasGame.Width + 100;
                directionY = canvasGame.Height / 2 - 160;
            }
            else if (winnerID == (Game.HumanPlayerID + 2) % 4) //winner is player over the top
            {
                directionX = canvasGame.Width / 2 - 55;
                directionY = -300;
            }
            else if (winnerID == (Game.HumanPlayerID + 3) % 4) //winner is on the left
            {
                directionX = -200;
                directionY = canvasGame.Height / 2 - 160;
            }

            double animateSpeed = 0.5;

            if (playerID == Game.HumanPlayerID)//human players card
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(MapCardToFile(playedCard), UriKind.Relative);
                bitmap.EndInit();
                Image image = new Image();
                image.Source = bitmap;
                image.Width = 115;
                image.Height = 200;
                image.HorizontalAlignment = HorizontalAlignment.Left;

                //Random rand = new Random(rndm);
                //int angle = rand.Next(41) - 20; //rest positoin is 0 degree, with -20 to 20 range

                if (animate)
                {
                    int angle = playedCard.Angle;
                    RotateTransform rotateTransform = new RotateTransform(angle);
                    image.RenderTransform = rotateTransform;
                    AnimateCardTranslation(image, canvasGame.Width / 2 - 45,
                    canvasGame.Height / 2 - 100, directionX, directionY, animateSpeed);
                }
                else
                {
                    //Vector offset = VisualTreeHelper.GetOffset(image);//position of the image in the container
                    int angle = playedCard.Angle;
                    RotateTransform rotateTransform = new RotateTransform(angle);
                    image.RenderTransform = rotateTransform;

                    //image.Margin = new Thickness(canvasGame.Width / 2 - 45, canvasGame.Height / 2 - 100, 0, 0);
                    Canvas.SetLeft(image, canvasGame.Width / 2 - 45);
                    Canvas.SetTop(image, canvasGame.Height / 2 - 100);
                }

                //image.IsEnabled = false;//disable card control after it's played
                canvasGame.Children.Add(image);
            }
            else if ((Game.HumanPlayerID + 1) % 4 == playerID)//player on the right side
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(MapCardToFile(playedCard), UriKind.Relative);
                bitmap.EndInit();
                Image image = new Image();
                image.Source = bitmap;
                image.Width = 115;
                image.Height = 200;
                image.HorizontalAlignment = HorizontalAlignment.Left;

                //Random rand = new Random(rndm * 2);
                //int angle = rand.Next(70, 111) * (-1);//rest positoin is 90 degree, with 70 to 110 range

                if (animate)
                {
                    AnimateCardTranslation(image, canvasGame.Width / 2,
                        canvasGame.Height / 2, directionX, directionY, animateSpeed);
                }
                else
                {
                    int angle = playedCard.Angle;
                    RotateTransform rotateTransform = new RotateTransform(angle);
                    image.RenderTransform = rotateTransform;

                    //image.Margin = new Thickness(canvasGame.Width / 2 + 180, canvasGame.Height / 2 - 100, 0, 0);
                    Canvas.SetLeft(image, canvasGame.Width / 2);
                    Canvas.SetTop(image, canvasGame.Height / 2);
                }

                canvasGame.Children.Add(image);
            }
            else if ((Game.HumanPlayerID + 2) % 4 == playerID)//player over the top
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(MapCardToFile(playedCard), UriKind.Relative);
                bitmap.EndInit();
                Image image = new Image();
                image.Source = bitmap;
                image.Width = 115;
                image.Height = 200;
                image.HorizontalAlignment = HorizontalAlignment.Left;

                //Random rand = new Random(rndm * 3);
                //int angle = rand.Next(41) - 20;//rest positoin is 0 degree, with 20 to -20 range



                if (animate)
                {
                    AnimateCardTranslation(image, canvasGame.Width / 2 - 25,
                        canvasGame.Height / 2 - 180, directionX, directionY, animateSpeed);
                }
                else
                {
                    int angle = playedCard.Angle;
                    RotateTransform rotateTransform = new RotateTransform(angle);
                    image.RenderTransform = rotateTransform;
                    //image.Margin = new Thickness(canvasGame.Width / 2 - 25, canvasGame.Height / 2 - 180, 0, 0);
                    Canvas.SetLeft(image, canvasGame.Width / 2 - 25);
                    Canvas.SetTop(image, canvasGame.Height / 2 - 180);
                }
                canvasGame.Children.Add(image);
            }
            else if ((Game.HumanPlayerID + 3) % 4 == playerID)//player over the top
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(MapCardToFile(playedCard), UriKind.Relative);
                bitmap.EndInit();
                Image image = new Image();
                image.Source = bitmap;
                image.Width = 115;
                image.Height = 200;
                image.HorizontalAlignment = HorizontalAlignment.Left;

                //Random rand = new Random(rndm * 3);
                //int angle = rand.Next(70, 111) * (-1);//rest positoin is -90 degree, with -70 to -110 range

                if (animate)
                {
                    AnimateCardTranslation(image, canvasGame.Width / 2 - 150,
                        canvasGame.Height / 2 - 20, directionX, directionY, animateSpeed);
                }
                else
                {
                    int angle = playedCard.Angle;
                    RotateTransform rotateTransform = new RotateTransform(angle);
                    image.RenderTransform = rotateTransform;

                    //image.Margin = new Thickness(canvasGame.Width / 2 - 150, canvasGame.Height / 2 - 20, 0, 0);
                    Canvas.SetLeft(image, canvasGame.Width / 2 - 150);
                    Canvas.SetTop(image, canvasGame.Height / 2 - 20);
                }
                canvasGame.Children.Add(image);
            }

        }

        /// <summary>
        /// Shows played cards for human
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Show_PlayedCards_Human(Object sender, MouseEventArgs e)
        {
            Image image = sender as Image;

            Game.Players[Game.HumanPlayerID].PlayCard(ImageToCardID(image));
            Show_PlayedCards_Table(Game.HumanPlayerID, Game.CardsInTable[Game.CardsInTable.Count - 1]);

            //Refresh canvas for next player
            //Refresh_Canvas((Game.HumanPlayerID + 1) % 4);
        }

        /// <summary>
        /// Animates movement of Image from one point to another
        /// </summary>
        /// <param name="target"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <param name="timeSpan"></param>
        public void AnimateCardTranslation(Image target, double startX, double startY,
            double endX, double endY, double timeSpan = 1)
        {
            TranslateTransform trans = new TranslateTransform();
            target.RenderTransform = trans;
            //RotateTransform rotateTransform = new RotateTransform();
            //target.RenderTransform = rotateTransform;
            DoubleAnimation animX = new DoubleAnimation(startX, endX, TimeSpan.FromSeconds(timeSpan));
            DoubleAnimation animY = new DoubleAnimation(startY, endY, TimeSpan.FromSeconds(timeSpan));
            //DoubleAnimation animAngle = new DoubleAnimation(startAngle, endAngle, TimeSpan.FromSeconds(timeSpan));
            trans.BeginAnimation(TranslateTransform.XProperty, animX, HandoffBehavior.Compose);
            trans.BeginAnimation(TranslateTransform.YProperty, animY, HandoffBehavior.Compose);
            //rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animAngle);
        }

        /// <summary>
        /// Animates movement of labels from one point to another
        /// Overloaded method
        /// </summary>
        /// <param name="target"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <param name="timeSpan"></param>
        public void AnimateCardTranslation(Label target, double startX, double startY,
            double endX, double endY, double timeSpan = 1)
        {
            TranslateTransform trans = new TranslateTransform();
            target.RenderTransform = trans;
            //RotateTransform rotateTransform = new RotateTransform();
            //target.RenderTransform = rotateTransform;
            DoubleAnimation animX = new DoubleAnimation(startX, endX, TimeSpan.FromSeconds(timeSpan));
            DoubleAnimation animY = new DoubleAnimation(startY, endY, TimeSpan.FromSeconds(timeSpan));
            //DoubleAnimation animAngle = new DoubleAnimation(startAngle, endAngle, TimeSpan.FromSeconds(timeSpan));
            trans.BeginAnimation(TranslateTransform.XProperty, animX, HandoffBehavior.Compose);
            trans.BeginAnimation(TranslateTransform.YProperty, animY, HandoffBehavior.Compose);
            //rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animAngle);
        }

        /// <summary>
        /// Animates rotation of card from one angle to other
        /// </summary>
        /// <param name="target"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="timeSpan"></param>
        private void AnimateCardRotation(Image target, double startAngle, double endAngle, double timeSpan)
        {
            RotateTransform rotate = new RotateTransform();
            target.RenderTransform = rotate;
            DoubleAnimation animAngle = new DoubleAnimation(startAngle, endAngle, TimeSpan.FromSeconds(timeSpan));
            rotate.BeginAnimation(RotateTransform.AngleProperty, animAngle, HandoffBehavior.Compose);
        }

        private void ShowCardDeal_Animation()
        {
            ShowDealReady_Deck(true);
        }

        /// <summary>
        /// Shows deck of card on the given dealers side
        /// </summary>
        /// <param name="deal"></param>
        private void ShowDealReady_Deck(bool deal = false)
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
                if (a == (Game.HumanPlayerID + 3) % 4)
                {
                    x = 80 + i / 2;
                    y = 300 + i / 2;
                }
                else if (a == (Game.HumanPlayerID + 1) % 4)
                {
                    x = ((int)(canvasGame.Width) - 170) - i / 2;
                    y = 300 + i / 2;
                }
                else if (a == (Game.HumanPlayerID + 2) % 4)
                {
                    x = ((int)canvasGame.Width / 2) + 15 + i / 2;
                    y = 110 + i / 2;
                }
                else if (a == Game.HumanPlayerID)
                {
                    x = ((int)canvasGame.Width / 2) + 10 + i / 2;
                    y = (int)canvasGame.Height - 240 + i / 2;
                }

                image.Margin = new Thickness(x, y, 0, 0);

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
            arrow.Foreground = Brushes.Yellow;
            arrow.FontSize = 115;
            arrow.FontWeight = FontWeights.UltraBold;

            if (playerID == (Game.HumanPlayerID + 1) % 4)//player on the right side
            {
                arrow.Margin = new Thickness(canvasGame.Width - 265, canvasGame.Height / 2 - 215, 0, 0);
            }
            else if (playerID == (Game.HumanPlayerID + 3) % 4)//player on the left side
            {
                arrow.Content = "⮘";
                arrow.Margin = new Thickness(155, canvasGame.Height / 2 - 215, 0, 0);
            }
            else if (playerID == (Game.HumanPlayerID + 2) % 4)//player over the top
            {
                arrow.Margin = new Thickness((canvasGame.Width / 2) - 270, 55, 0, 0);
            }
            else if (playerID == Game.HumanPlayerID)//human player
            {
                arrow.Margin = new Thickness((canvasGame.Width / 2) - 280, canvasGame.Height - 310, 0, 0);
            }

            canvasGame.Children.Add(arrow);
        }

        /// <summary>
        /// Returns corresponding card ID of the associated image control
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private int ImageToCardID(Image image)
        {
            string imagePath = (image.Source as BitmapImage).UriSource.ToString();
            string[] splitPath = imagePath.Split('/');
            string fileName = splitPath[splitPath.Length - 1];

            int i;
            for (i = 0; i < 52; i++)
            {
                if ("/cards/" + fileName == MapCardToFile(Game.CardIDtoCard(i)))
                {
                    break;
                }
            }
            return i;
        }

        /// <summary>
        /// Shows information about game on top bar
        /// </summary>
        private void Show_Info_TopBar()
        {
            lblTopBar.Content = String.Format("Current Player: [{0}]          Hands Played: [{1}/{2}]          Tricks Won: [{3}]          Score: [{4}]",
                Game.Players[Game.CurrentDealer].Name, Game.CurrentHand + 1, Game.MaxHandsToPlay,
                Game.TricksWon[Game.HumanPlayerID], Game.CumulativeScore[Game.HumanPlayerID]);
        }

        private void TestWindow()
        {
            lblTopBar.FontSize = 12;
            lblTopBar.Content = "";

            foreach (Player player in Game.Players)
            {
                lblTopBar.Content += player.Name + ": ";
                foreach (Card card in player.Cards)
                {
                    lblTopBar.Content += card.Name + " ";
                }
                lblTopBar.Content += " | ";
            }
        }
    }
}




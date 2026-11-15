using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class frmInstruction : Window
    {
        public frmInstruction()
        {
            InitializeComponent();
            Instructions();

            PlaySound("draw.wav");
        }

        private void Instructions()
        {
            TextBlock textBlock = new TextBlock();
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            scrollViewer.Content = textBlock;      
            textBlock.TextAlignment = TextAlignment.Left;
            textBlock.FontFamily = new FontFamily("Verdana");
            textBlock.FontSize = 17;
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.Foreground = Brushes.LightCyan;

            // Clear any existing content
            textBlock.Inlines.Clear();

            // Define a gorgeous multi-tiered color palette for a green background
            var goldHeaderBrush = new SolidColorBrush(Color.FromRgb(255, 215, 0));     // Aesthetic Gold for main titles
            var cyanAccentBrush = new SolidColorBrush(Color.FromRgb(103, 232, 249));   // Bright Cyan for keywords/notes
            var peachHighlightBrush = new SolidColorBrush(Color.FromRgb(254, 240, 138));  // Soft Cream for examples/rules
            var whiteBodyBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));   // Crisp White for primary text
            var mutedSageBrush = new SolidColorBrush(Color.FromRgb(209, 250, 229));   // Soft Sage for footer / references

            // Helper for section headers
            void AddHeader(string title)
            {
                textBlock.Inlines.Add(new Run(title + "\n")
                {
                    Foreground = goldHeaderBrush,
                    FontWeight = FontWeights.Bold,
                    FontSize = 15
                });
            }

            // 1. Introduction
            textBlock.Inlines.Add(new Run(
                "Call Break (also known as Call Bridge) is a game of tricks, trumps, and bidding which is popular in South Asia (especially Nepal, India, and Bangladesh). It seems to be very similar to the card game 'Contract Bridge'. The rules vary from place to place but this game uses the standard rules.\n\n"
            )
            { Foreground = whiteBodyBrush, FontSize = 13 });

            // 2. Players and Cards (Spades trump moved to its own bullet point)
            AddHeader("Players and Cards:");
            textBlock.Inlines.Add(new Run("• ") { Foreground = cyanAccentBrush, FontWeight = FontWeights.Bold, FontSize = 13 });
            textBlock.Inlines.Add(new Run("This game is played by 4 people using a standard 52 - card pack.\n") { Foreground = whiteBodyBrush, FontSize = 13 });

            textBlock.Inlines.Add(new Run("• ") { Foreground = cyanAccentBrush, FontWeight = FontWeights.Bold, FontSize = 13 });
            textBlock.Inlines.Add(new Run("The cards of each suit rank from high to low ") { Foreground = whiteBodyBrush, FontSize = 13 });
            textBlock.Inlines.Add(new Run("A - K - Q - J - 10 - 9 - 8 - 7 - 6 - 5 - 4 - 3 - 2") { Foreground = peachHighlightBrush, FontWeight = FontWeights.SemiBold, FontSize = 13 });
            textBlock.Inlines.Add(new Run(".\n") { Foreground = whiteBodyBrush, FontSize = 13 });

            textBlock.Inlines.Add(new Run("• ") { Foreground = cyanAccentBrush, FontWeight = FontWeights.Bold, FontSize = 13 });
            textBlock.Inlines.Add(new Run("Spades are permanent trumps: any card of the Spade suit beats any card of any other suit.\n") { Foreground = whiteBodyBrush, FontSize = 13 });

            textBlock.Inlines.Add(new Run("• ") { Foreground = cyanAccentBrush, FontWeight = FontWeights.Bold, FontSize = 13 });
            textBlock.Inlines.Add(new Run("Deal and play are counter-clockwise.\n\n") { Foreground = whiteBodyBrush, FontSize = 13 });

            // 3. Deal
            AddHeader("Deal:");
            textBlock.Inlines.Add(new Run("• ") { Foreground = cyanAccentBrush, FontWeight = FontWeights.Bold, FontSize = 13 });
            textBlock.Inlines.Add(new Run("Any player may deal first: subsequently the turn to deal passes to the right.\n") { Foreground = whiteBodyBrush, FontSize = 13 });

            textBlock.Inlines.Add(new Run("• ") { Foreground = cyanAccentBrush, FontWeight = FontWeights.Bold, FontSize = 13 });
            textBlock.Inlines.Add(new Run("The dealer deals out all the cards, one at a time, face down, so that each player has 13 cards. The players pick up their cards and look at them.\n\n") { Foreground = whiteBodyBrush, FontSize = 13 });

            // 4. Bidding
            AddHeader("Bidding:");
            textBlock.Inlines.Add(new Run(
                "Starting with the player to dealer's right, and continuing counter-clockwise around the table, ending with the dealer, each player bids a number, which must be at least 1. Bid is the number of tricks you could possibly win within the dealt hand. The maximum sensible bid is 8, as any bid greater or equals to 8 counts for bonus call (see 'Scoring' section for information about bonus call). In this game, the tricks bid are known as "
            )
            { Foreground = whiteBodyBrush, FontSize = 13 });
            textBlock.Inlines.Add(new Run("\"calls\"") { Foreground = cyanAccentBrush, FontWeight = FontWeights.Bold, FontSize = 13 });
            textBlock.Inlines.Add(new Run(" (Hence, the name 'Call Break').\n\n") { Foreground = whiteBodyBrush, FontSize = 13 });

            // 5. Play
            AddHeader("Play:");
            textBlock.Inlines.Add(new Run("• ") { Foreground = cyanAccentBrush, FontWeight = FontWeights.Bold, FontSize = 13 });
            textBlock.Inlines.Add(new Run("The player to dealer's right leads to the first trick, and subsequently the winner of each trick leads to the next.\n") { Foreground = whiteBodyBrush, FontSize = 13 });

            textBlock.Inlines.Add(new Run("• ") { Foreground = cyanAccentBrush, FontWeight = FontWeights.Bold, FontSize = 13 });
            textBlock.Inlines.Add(new Run("Any card may be led, and the other three players must follow the suit if they can. A player who cannot follow the suit must trump with a spade, provided that this spade is high enough to beat any spades already in the trick. A player who has no cards of the suit led and no spades high enough to head the trick may play any card.\n") { Foreground = whiteBodyBrush, FontSize = 13 });

            textBlock.Inlines.Add(new Run("• ") { Foreground = cyanAccentBrush, FontWeight = FontWeights.Bold, FontSize = 13 });
            textBlock.Inlines.Add(new Run("The trick is won by the player of the highest spade in it, or if it contains no spade, by the player of the highest card of the suit that was led.\n\n") { Foreground = whiteBodyBrush, FontSize = 13 });

            textBlock.Inlines.Add(new Run("Note that:\n") { Foreground = cyanAccentBrush, FontWeight = FontWeights.SemiBold, FontSize = 13 });
            textBlock.Inlines.Add(new Run("1. ") { Foreground = peachHighlightBrush, FontWeight = FontWeights.Bold, FontSize = 13 });
            textBlock.Inlines.Add(new Run("A player who is able to play a card of the suit that was led is not obliged to head the trick. This also applies when spades are led: players may play higher or lower spades as they wish.\n") { Foreground = whiteBodyBrush, FontSize = 13 });

            textBlock.Inlines.Add(new Run("2. ") { Foreground = peachHighlightBrush, FontWeight = FontWeights.Bold, FontSize = 13 });
            textBlock.Inlines.Add(new Run("A player who has no cards of a suit is said to be \"off\" that suit. If it is off the suit that was led, and there are no spades in the trick yet, the player must play a spade if possible. If there is already a spade in the trick, the player who is \"off\" the lead suit must play a higher spade if possible. If the player only has lower spades, s/he can \"waste\" one of these spades to avoid taking an unwanted trick later or can throw a card of another suit.\n\n") { Foreground = whiteBodyBrush, FontSize = 13 });

            // 6. Scoring
            AddHeader("Scoring:");
            textBlock.Inlines.Add(new Run(
                "To succeed, a player must win the number of tricks called or more tricks than the call. If a player succeeds, the number called is added to his/her cumulative score. Otherwise, the negative value of bid is the score you get. This is why it is crucial that you make the bid as close as possible to how many tricks you can actually within the dealt hand\n\n"
            )
            { Foreground = whiteBodyBrush, FontSize = 13 });

            textBlock.Inlines.Add(new Run("• For example: ") { Foreground = peachHighlightBrush, FontWeight = FontWeights.SemiBold, FontSize = 13 });
            textBlock.Inlines.Add(new Run(
                "A player who calls 4 must win 4 or more tricks to succeed, and in this case gains 4 points and 0.1 points for each extra tricks won. Say, the player wins 5 tricks for a bid of 4 then they scores 4.1. Winning 3 or fewer tricks for a bid of 4 counts as a loss, and the player loses 4 points.\n\n"
            )
            { Foreground = whiteBodyBrush, FontSize = 13 });

            textBlock.Inlines.Add(new Run("• Bonus call: ") { Foreground = cyanAccentBrush, FontWeight = FontWeights.SemiBold, FontSize = 13 });
            textBlock.Inlines.Add(new Run(
                "Calls of 8 tricks or more are bonus calls. A player who calls 8 or more scores 13 points(instead of the amount of the call) if successful. If unsuccessful, the amount of the call is subtracted as usual. So for example a player who calls 8 scores +13 for 8 or more tricks, but -8 for 7 or fewer.\n\n" +
                "There is no fixed end to the game. Players continue for as long as they wish, and when the game ends the player with the highest cumulative score is the winner.\n\n"
            )
            { Foreground = whiteBodyBrush, FontSize = 13 });

            // 7. Footer & Credits
            textBlock.Inlines.Add(new Run("Reference: https://www.pagat.com/auctionwhist/call_bridge.html\n\n")
            {
                Foreground = mutedSageBrush,
                FontStyle = FontStyles.Italic,
                FontSize = 12
            });
            textBlock.Inlines.Add(new Run("Developed by: Amar Shrestha\nSpecial thanks to Binod Sujakhu for game insights and advice.")
            {
                Foreground = mutedSageBrush,
                FontSize = 11
            });

            grdInstruction.Children.Add(scrollViewer);
        }

        private void PlaySound(string fileName)
        {
            Uri uri = new Uri("/sfx/" + fileName, UriKind.Relative);
            SoundPlayer player = new SoundPlayer(GetResourceStream(uri.ToString()));

            //player.LoadAsync();
            player.Play();
        }

        //gets the path stream for files flagged as embedded resources
        protected static Stream GetResourceStream(string resourcePath)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<string> resourceNames = new List<string>(assembly.GetManifestResourceNames());

            resourcePath = resourcePath.Replace(@"/", ".");
            resourcePath = resourceNames.FirstOrDefault(r => r.Contains(resourcePath));

            if (resourcePath == null)
                throw new FileNotFoundException("Resource not found");

            return assembly.GetManifestResourceStream(resourcePath);
        }
    }
}

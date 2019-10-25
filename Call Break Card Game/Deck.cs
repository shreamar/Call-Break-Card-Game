using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Call_Break_Card_Game
{
    class Deck //:ISwapCards<Card>
    {
        private List<Card> _Cards = new List<Card>();
        private bool _Shuffled;

        public Deck()
        {
            _Shuffled = false;
            createCards();
            shuffleDeck();
        }

        public List<Card> Cards
        {
            get { return _Cards; }
            set { _Cards = value; }
        }

        public bool Shuffled
        {
            get { return _Shuffled; }
        }

        /// <summary>
        /// Creates the deck of cards
        /// </summary>
        private void createCards()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    Cards.Add(new Card((Card.CardNumber)j, (Card.CardSuit)i));
                }
            }
        }

        /// <summary>
        /// Shuffles the deck of cards
        /// </summary>
        public void shuffleDeck()
        {
            //create first random number as seed
            Random random = new Random();
            int rnd = random.Next(1, 999999999);

            for (int i = 0; i < 1000; i++)
            {
                //updates the seeds everytime so they are pseudorandom and all are not same
                Random random1 = new Random(rnd + i);
                int rndIndex = random1.Next(52);

                //swaps first card with randomly generated indexed card
                swapCards(Cards, 0, rndIndex);
            }
            _Shuffled = true;
        }
        
        public void swapCards(List<Card> deck, int i, int j)
        {
            Card temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }
        
    }
}

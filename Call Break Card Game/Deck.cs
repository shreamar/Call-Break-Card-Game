using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Call_Break_Card_Game
{
    class Deck
    {
        private Card[] _Cards;
        private bool _Shuffled = false;

        public Deck()
        {
            _Cards = new Card[52];
            //initialize the deck
            for (int i = 0; i < _Cards.Length; i++)
            {
                _Cards[i] = new Card();
            }

            createCards();
            shuffleDeck();
        }

        public Card[] Cards
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
            int counter = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    Cards[counter].Number = (Card.CardNumber)j;
                    Cards[counter].Suit = (Card.CardSuit)i;
                    counter++;
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
                swapCards(ref Cards[0],ref Cards[rndIndex]);
            }
            _Shuffled = true;
        }

        private void swapCards(ref Card a,ref Card b)
        {
            Card temp = a;
            a = b;
            b = temp;
        }
    }
}

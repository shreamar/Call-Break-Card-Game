using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Call_Break_Card_Game
{
    public class Card
    {
        private CardNumber _Number;
        private CardSuit _Suit;
        private bool _IsPlayed = false;
        private int _Angle;

        public Card()
        {
            _Number = CardNumber.Ace;
            _Suit = CardSuit.Club;
            _Angle = 0;
        }

        public Card(CardNumber number, CardSuit suit)
        {
            _Number = number;
            _Suit = suit;
            _Angle = 0;
        }

        public CardNumber Number
        {
            get { return _Number; }
            set { _Number = value; }
        }

        public CardSuit Suit
        {
            get { return _Suit; }
            set { _Suit = value; }
        }

        public int ID
        {
            get { return ((int)Suit * 13) + (int)Number; }        //2-Club is 0, A-Spade is 51
        }

        /// <summary>
        /// Hierarchical value a card given its suit and number
        /// </summary>
        public int Value
        {
            get
            {
                //Ace is highest among numbers and all other numbers have values given their order
                //Spade has highest value while all other suits have same value
                int suitValue = Suit == CardSuit.Spade ? 13 : 0;
                int numberValue = (int)Number;

                //examples: 5-H = 5-D = 5-C =4, A-H = A-D = A-C =12, 2-S = 13, K-S = 24, A-S =25
                return suitValue + numberValue;
            }
        }

        public bool IsPlayed
        {
            get { return _IsPlayed; }
            set { _IsPlayed = value; }
        }

        public int Angle
        {
            get { return _Angle; }
            set { _Angle = value; }
        }

        /// <summary>
        /// Displays the name of the given card
        /// For example, 9-Heart
        /// </summary>
        public string Name
        {
            get
            {
                string name = "";

                //since Two = 0, 9 = Jack
                if ((int)Number <= 8 && (int)Number >= 0)
                {
                    name = ((int)Number + 2).ToString();
                }
                else if ((int)Number >= 9)
                {
                    //picks only the first letter of face/ace cards
                    name = Number.ToString()[0].ToString();
                }

                if (Suit == CardSuit.Diamond)
                {
                    name += '♦'.ToString();
                }
                else if (Suit == CardSuit.Club)
                {
                    name += '♣'.ToString();
                }
                else if (Suit == CardSuit.Heart)
                {
                    name += '♥'.ToString();
                }
                else if (Suit == CardSuit.Spade)
                {
                    name += '♠'.ToString();
                }

                return name;

            }
        }

        /// <summary>
        /// Creates deep copy of card
        /// </summary>
        /// <returns></returns>
        public Card CreateDeepCopy()
        {
            return new Card(this.Number, this.Suit);
        }

        public enum CardSuit { Diamond, Club, Heart, Spade };

        public enum CardNumber { Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace};

    }
}

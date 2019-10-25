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
        private bool _Thrown = false;

        public Card()
        {
            _Number = CardNumber.Two;
            _Suit = CardSuit.Club;
        }

        public Card(CardNumber number, CardSuit suit)
        {
            _Number = number;
            _Suit = suit;
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
            get { return ((int)Suit * 10) + (int)Number; }        //Two-Club is 0, Ace-Spade is 51
        }

        public bool Thrown
        {
            get { return _Thrown; }
            set { _Thrown = value; }
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
                if ((int)Number < 9)
                {
                    name = ((int)Number+2).ToString();
                }
                else if ((int)Number >= 9)
                {
                    //picks only the first letter of face/ace cards
                    name = Number.ToString()[0].ToString();
                }

                return name + "-" + Suit.ToString();
            }
        }

        /// <summary>
        /// Creates deep copy of card
        /// </summary>
        /// <returns></returns>
        public Card createDeepCopy()
        {
            return new Card(this.Number, this.Suit);
        }


        /// <summary>
        /// no specific order except Spade is trump card with highest index value
        /// </summary>
        public enum CardSuit { Club, Diamond, Heart, Spade}; 

        /// <summary>
        /// 2 is lowest value where as Ace is higher than King
        /// </summary>
        public enum CardNumber {Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King};
        

    }
}

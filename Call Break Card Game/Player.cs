using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Call_Break_Card_Game
{
    class Player : ISwapCards<Card>
    {
        private List<Card> _Cards = new List<Card>();
        private string _Name;
        private int _Score;
        private int _ID;
        private PlayerType _Type;
        private Card _LastPlayedCard;

        public Player()
        {
            Name = "Random Player";
            Score = 0;
            Type = PlayerType.Bot;
            ID = 0;
            _LastPlayedCard = null;
        }

        public Player(string name, PlayerType type)
        {
            Name = name;
            Score = 0;
            Type = type;
            ID = 0;
            _LastPlayedCard = null;
        }

        /// <summary>
        /// Keeps track of how many cards player has 
        /// </summary>
        public int CardCount
        {
            get { return _Cards.Count; }
        }

        public List<Card> Cards
        {
            get { return _Cards; }
            set { _Cards = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public int Score
        {
            get { return _Score; }
            set { _Score = value; }
        }

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public PlayerType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        public Card LastPlayedCard
        {
            get { return _LastPlayedCard; }
            set { _LastPlayedCard = value; }
        }

        public void swapCards(Card a, Card b)
        {
            Card temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// Sorts players cards based on the cards ID
        /// </summary>
        public void sortCards()
        {
            Cards.Sort((a, b) => (a.ID.CompareTo(b.ID)));
        }

        /// <summary>
        /// Plays the card for the given trick
        /// Removes the given card from the hand
        /// </summary>
        /// <param name="cardIndex">Index of card to play</param>
        public void playCard(int cardIndex)
        {
            LastPlayedCard = Cards[cardIndex];
            Cards.Remove(Cards[cardIndex]);
        }

        /// <summary>
        /// Player type human or computer
        /// </summary>
        public enum PlayerType { Human, Bot };
    }
}

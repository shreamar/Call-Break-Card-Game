using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Call_Break_Card_Game
{
    class Player:ISwapCards<Card>
    {
        private int _CardCount;
        private List<Card> _Cards = new List<Card>();
        private string _Name;
        private int _Score;
        private int _ID;
        private PlayerType _Type;

        public Player()
        {
            Name = "Random Player";
            Score = 0;
            Type = PlayerType.Bot;
        }

        public Player(string name, PlayerType type)
        {
            Name = name;
            Score = 0;
            Type = type;
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

        public void swapCards(ref Card a, ref Card b)
        {
            Card temp = a;
            a = b;
            b = temp;
        }

        public void sortCards()
        {
            if(Cards.Count > 0)
            {
                Cards.Sort((a, b) => (a.ID.CompareTo(b.ID)));
            }
        }

        /// <summary>
        /// Player type human or computer
        /// </summary>
        public enum PlayerType { Human, Bot};
    }
}

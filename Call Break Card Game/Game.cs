using System;
using System.Collections.Generic;

namespace Call_Break_Card_Game
{
    public static class Game
    {
        private static Player[] _Players;
        private static int _MaxHandsToPlay;
        private static Deck _DeckOfCards;
        private static int _CurrentHand;
        private static double[,] _ScoreBoard;
        private static int[] _TricksWon;
        private static int[] _Bidding;
        private static int _LeadCardID;
        private static int _PowerCardID;
        private static List<Card> _CardsInTable = new List<Card>();
        private static int _CurrentDealer;
        private static int _CurrentTrickWinner;
        private static int _CurrentPlayer;

        public static Player[] Players
        {
            get
            {
                return _Players;
            }
            set
            {
                _Players = value;
            }
        }

        public static int MaxHandsToPlay
        {
            get { return _MaxHandsToPlay; }
            set { _MaxHandsToPlay = value; }
        }

        public static Deck DeckOfCards
        {
            get { return _DeckOfCards; }
            //set { _DeckOfCards = value; }
        }

        public static int CurrentHand
        {
            get { return _CurrentHand; }
            set { _CurrentHand = value; }
        }

        public static double[,] ScoreBoard
        {
            get { return _ScoreBoard; }
        }

        public static int[] Bidding
        {
            get { return _Bidding; }
            set { _Bidding = value; }
        }

        public static int CurrentTrickWinner
        {
            get
            {
                return _CurrentTrickWinner;
            }
        }

        public static int CurrentPlayer
        {
            get { return _CurrentPlayer; }
            set { _CurrentPlayer = value; }
        }

        public static int CumulativeWinner
        {
            get
            {
                int max = 0;
                for (int i = 1; i < 4; i++)
                {
                    if (CumulativeScore[i] > CumulativeScore[max])
                    {
                        max = i;
                    }
                }

                return max;
            }
        }

        /// <summary>
        /// First card played in the table is lead card
        /// </summary>
        public static int LeadCardID
        {
            get
            {
                if (CardsInTable.Count > 0)
                {
                    _LeadCardID = CardsInTable[0].ID;
                }
                else
                {
                    _LeadCardID = -1;
                }
                return _LeadCardID;
            }
            //set { _LeadCardID = value; }
        }

        /// <summary>
        /// Finds the power card among the cards played in the table
        /// </summary>
        public static int PowerCardID
        {
            get
            {
                _PowerCardID = LeadCardID;
                foreach (Card card in CardsInTable)
                {
                    if (LeadCardID > 51 || LeadCardID < 0)
                    {
                        _PowerCardID = -1;
                    }
                    else if (card.Suit == CardIDtoSuit(_PowerCardID) && card.Value > CardIDtoValue(_PowerCardID) ||
                    (card.Suit != CardIDtoSuit(_PowerCardID) && card.Suit == Card.CardSuit.Spade))
                    {
                        _PowerCardID = card.ID;
                    }
                }

                return _PowerCardID;
            }
            //set { _PowerCardID = value; }
        }

        public static List<Card> CardsInTable
        {
            get { return _CardsInTable; }
            set { _CardsInTable = value; }
        }

        public static int CurrentDealer
        {
            get { return _CurrentDealer; }
        }

        public static int[] TricksWon
        {
            get { return _TricksWon; }
            set { _TricksWon = value; }
        }

        public static int HumanPlayerID
        {
            get
            {
                int id = 0;
                foreach (Player player in Players)
                {
                    if (player.Type == Player.PlayerType.Human)
                    {
                        id = player.ID;
                        break;
                    }
                }

                return id;
            }
        }

        /// <summary>
        /// Gets cumulative scores of each player
        /// </summary>
        public static double[] CumulativeScore
        {
            get
            {
                double[] scores = { 0, 0, 0, 0 };
                for (int i = 0; i <= ScoreBoard.GetUpperBound(1); i++)//players
                {
                    for (int j = 0; j <= ScoreBoard.GetUpperBound(0); j++)//hands
                    {
                        scores[i] += ScoreBoard[j, i];
                    }
                }
                return scores;
            }
        }

        /// <summary>
        /// Associates card ID to the player
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        public static int CardBelongsTo(int cardID)
        {
            int playerID = 0;
            foreach (Player player in Players)
            {
                foreach (Card card in player.Cards)
                {
                    if (card.ID == cardID)
                    {
                        playerID = player.ID;
                        break;
                    }
                }
            }
            return playerID;
        }

        /// <summary>
        /// Initializes all players and puts them in random order
        /// </summary>
        /// <param name="playerName"></param>
        public static void InitializePLayers(string playerName)
        {
            //Initialize Players
            _Players = new Player[4];

            _Players[0] = new Player(playerName, Player.PlayerType.Human);
            for (int i = 1; i <= 3; i++)
            {
                _Players[i] = new Player(i == 1 ? "Zion" : (i == 2 ? "Eren" : "Sage"), Player.PlayerType.Bot);
            }

            Random random = new Random();
            int rand = random.Next(999999999);

            for (int i = 0; i < 1000; i++)
            {
                Random rnd = new Random(rand * i);
                int index = rnd.Next(4);

                Player temp = _Players[0];
                _Players[0] = _Players[index];
                _Players[index] = temp;
            }

            //assign IDs to players
            for (int i = 0; i < 4; i++)
            {
                _Players[i].ID = i;
                //Assign random icon number for each player
                _Players[i].IconNumber = (new Random(rand + i).Next(1, 14));
            }
        }

        /// <summary>
        /// Deals all cards from the deck to all players,
        /// As the card is being dealt to players, the given cards are being removed from deck pile at the same time.
        /// </summary>
        public static void DealCards()
        {
            //_DeckOfCards = new Deck();
            DeckOfCards.ShuffleDeck();

            bool alreadyDealt = false;

            if (DeckOfCards.Cards.Count != 0)
            {
                for (int i = 0; i < 13; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        //Adds the card from top of the deck to players pile
                        Players[j].Cards.Add(DeckOfCards.Cards[0]);
                        //Removes the card from top of the deck since now its moved to player's pile
                        DeckOfCards.Cards.Remove(DeckOfCards.Cards[0]);
                    }
                }

                //Sort each players cards in thier own stack
                foreach (Player player in Players)
                {
                    player.sortCards();
                }

                //flag as already dealt
                alreadyDealt = true;
            }

            if (CheckNeedForRedealingCard() || !alreadyDealt)
            {
                for (int i = 0; i < 4; i++)
                {
                    foreach (Card card in Players[i].Cards)
                    {
                        //puts back players cards in deck
                        DeckOfCards.Cards.Add(card);
                    }
                    //removes cards from the players pile
                    Players[i].Cards.Clear();
                }
                //Re-shuffles deck
                DeckOfCards.ShuffleDeck();
                //Deals the cards to players again
                DealCards();
            }
        }

        /// <summary>
        /// Checks if cards need to be redealt.
        /// If there are no neither of ace cards, face cards or spade cards in any of the players pile, needs to redeal
        /// </summary>
        /// <returns></returns>
        private static bool CheckNeedForRedealingCard()
        {
            int counter = 0;
            for (int i = 0; i < 4; i++)
            {
                counter = 0;
                foreach (Card card in Players[i].Cards)
                {
                    //check for any facecards, ace cards and spade cards
                    if (((int)card.Number >= 9 && (int)card.Number <= 12) || card.Suit == Card.CardSuit.Spade)
                    {
                        counter++;
                    }
                }

                if (counter == 0)
                {
                    break;
                }
            }
            return (counter == 0);
        }

        /// <summary>
        /// Moves cards from table to deck, updates CurrentDealer and HandsWon by the given player, and returns the winner's player ID
        /// </summary>
        /// <returns>Players ID of the trick winner,
        ///  returns -1 if the operation didn't suceed</returns>
        public static void ProcessTrickWinner()
        {
            int winner = 0;
            int counter = 0;
            //check if all players have played their card for the trick
            if (CardsInTable.Count == 4)
            {
                foreach (Card card in CardsInTable)
                {
                    if (card.ID == PowerCardID)
                    {
                        //lead card is thrown by the person with their turn, so index of cards in table are based on turn
                        winner = (CurrentDealer + counter) % 4;

                        //update current trick winner
                        _CurrentTrickWinner = winner;
                        //now turn goes the winner of the trick;
                        _CurrentDealer = winner;
                    }
                    counter++;
                    //reset isPlayed property of the card
                    card.IsPlayed = false;

                    //add the cards in table back to deck
                    DeckOfCards.Cards.Add(card);
                }
                //clears cards from the table
                //CardsInTable.Clear();

                //updates the TricksWon
                TricksWon[winner]++;

                //reset played card flag as false
                foreach (Player player in Game.Players)
                {
                    player.HasPlayed = false;
                }
            }
        }

        /// <summary>
        /// Updates scoreboard given the bidding number and tricks won
        /// Also updates current hand</summary>
        public static void UpdateScoreBoard()
        {
            for (int i = 0; i < 4; i++)
            {
                if (Bidding[i] == TricksWon[i])
                {
                    ScoreBoard[CurrentHand, i] = Bidding[i];
                }
                else if (Bidding[i] > TricksWon[i])
                {
                    ScoreBoard[CurrentHand, i] = Bidding[i] * (-1);
                }
                else if (Bidding[i] < TricksWon[i])
                {
                    ScoreBoard[CurrentHand, i] = Bidding[i] + ((0.1) * (TricksWon[i] - Bidding[i]));
                }

                if (Bidding[i] >= 8 && Bidding[i] >= TricksWon[i])//bonus call
                {
                    ScoreBoard[CurrentDealer, i] = 13;
                }
            }

            //Updates CurrentHand
            _CurrentHand++;
        }

        /// <summary>
        /// Places bid for the given player in the current
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="bid"></param>
        public static void PlaceBid(int playerID, int bid)
        {
            if (bid < 13 && bid > 0)
            {
                Bidding[playerID] = bid;
            }
            else
            {
                //default bid is 1
                Bidding[playerID] = 1;
            }
        }

        /// <summary>
        /// Initializes basic components to start a new game
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="maxHands"></param>
        public static void InitializeGame(string playerName = "You", int maxHands = 5)
        {
            //Initialize Players
            InitializePLayers(playerName);

            //Default number of hands to play is 5
            if (maxHands < 1 || maxHands > 20)
            {
                _MaxHandsToPlay = 5;
            }
            else
            {
                _MaxHandsToPlay = maxHands;
            }

            //Initialize deck of cards
            _DeckOfCards = new Deck();

            //Set current hand to 0
            _CurrentHand = 0;

            //Initialize score board
            _ScoreBoard = new double[MaxHandsToPlay, 4];

            //Initialize tricks won by players in given hand
            _TricksWon = new int[4];

            //Initialize bidding board
            _Bidding = new int[4];
            for (int i = 0; i < 4; i++)
            {
                //default and lowest bid is 1
                _Bidding[i] = 1;
            }

            //Initializes cards in table
            _CardsInTable.Clear();

            //Set current dealer to 0
            _CurrentDealer = 0;
        }

        /// <summary>
        /// Initializes components to restart a hand
        /// </summary>
        public static void ReinitializeHand()
        {
            for (int i = 0; i < 4; i++)
            {
                //reset tricks won for new hand of game
                TricksWon[i] = 0;

                //reset bidding board for new hand of game
                Bidding[i] = 1;
            }
            //Clear cards in table
            CardsInTable.Clear();

        }

        /// <summary>
        /// Converts cardID to card number
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        public static Card.CardNumber CardIDtoNumber(int cardID)
        {
            return (Card.CardNumber)(cardID % 13);
        }

        /// <summary>
        /// Converts cardID to card suit
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        public static Card.CardSuit CardIDtoSuit(int cardID)
        {
            return (Card.CardSuit)((cardID - (int)(CardIDtoNumber(cardID))) / 13);
        }

        /// <summary>
        /// Converts cardID number into card
        /// </summary>
        /// <param name="cardID">Unique ID of the card ranging 0-51</param>
        /// <returns>Corresponding card of given ID, returns null if invalid ID</returns>
        public static Card CardIDtoCard(int cardID)
        {
            if (cardID >= 0 && cardID < 52)
            {
                return (new Card((Card.CardNumber)CardIDtoNumber(cardID), (Card.CardSuit)CardIDtoSuit(cardID)));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Converts CardID to card value
        /// </summary>
        /// <param name="cardID">CardID of the said card</param>
        /// <returns>Return number ranging 0-25, returns -1 if invalid cardID</returns>
        public static int CardIDtoValue(int cardID)
        {
            if (cardID >= 0 && cardID < 52)
            {
                return CardIDtoCard(cardID).Value;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Process Lead Card and Power Card, and validity checks and make changes if required
        /// </summary>
        /// <param name="leadCardID"></param>
        /// <param name="powerCardID"></param>
        /// <returns>Corrected power and lead card ID values</returns>
        public static int[] LeadVsPowerCard(int leadCardID, int powerCardID)
        {
            int[] index = new int[2];

            //if the power card is higher ranking but not same suit and is not spade then power and lead card are same
            //or if power card has less rank than lead card
            if (((Game.CardIDtoSuit(leadCardID) != Game.CardIDtoSuit(powerCardID) &&
                (Game.CardIDtoValue(leadCardID) > Game.CardIDtoValue(powerCardID) && Game.CardIDtoSuit(leadCardID) != Card.CardSuit.Spade))) ||
                Game.CardIDtoValue(powerCardID) <= Game.CardIDtoValue(leadCardID))
            {
                powerCardID = leadCardID;
            }

            index[0] = leadCardID;
            index[1] = powerCardID;

            return index;
        }

        /// <summary>
        /// Checks if one or maore player has same max cumulative score to decide if the game is tied
        /// </summary>
        /// <returns></returns>
        public static bool isTied()
        {
            int tie = 0;
            foreach(Player player in Game.Players)
            {
                if(CumulativeScore[player.ID] == CumulativeScore[Players[CumulativeWinner].ID])
                {
                    tie++;
                }
            }
            //only one player must have highest cumulative score to be winner otherwise its a tie
            return (tie > 1 ? true : false);
        }
    }
}

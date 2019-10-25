using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Call_Break_Card_Game
{
    public interface ISwapCards<Card>
    {
        void swapCards(Card a, Card b);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    public class PerfectMemoryAI : MemoryAI
    {
        private Dictionary<Player, List<Card>> playedCardsByPlayer;

        public PerfectMemoryAI(Player player, GameManager game) : base(player, game)
        {
            ResetMemory();
        }

        public override void ResetMemory()
        {
            base.ResetMemory();
            playedCardsByPlayer = new Dictionary<Player, List<Card>>();
            foreach (Player p in game.Players)
                playedCardsByPlayer[p] = new List<Card>();
        }

        public override void ProcessOtherPlayerCard(Player otherPlayer, Card card)
        {
            if (otherPlayer != player)
            {
                if (Round.Pile.Count > 0)
                {
                    var leadCard = Round.Pile[0];
                    if (leadCard.Suit != card.Suit)
                        memory[otherPlayer].NoCardsOfSuitLeft(leadCard.Suit);
                }
                if (card.Suit == Round.Trump)
                    trumpsPlayed++;
            }
            playedCards.Add(card);
            playedCardsByPlayer[otherPlayer].Add(card);
        }
        

    }
    
}

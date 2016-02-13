using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Whist.GameLogic.ControlEntities
{
    public interface IPlayTricks
    {
        bool TrickInProgress { get; }
        Player PileOwner { get; }
        Player CurrentPlayer { get; }
        List<Card> Pile { get; }
        bool InTrickPhase { get; }

        Player EndTrick();
        bool IsValidPlay(Card card);
        bool PlayCard(Card card);
        IList<Card> GetPlayerCards(Player player);
        IList<Card> GetPlayerCards();
    }
}
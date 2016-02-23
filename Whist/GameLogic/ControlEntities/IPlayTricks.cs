using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Whist.GameLogic.ControlEntities
{
    public interface IPlayTricks
    {
        bool TrickInProgress { get; }
        Player PileOwner { get; }
        Player CurrentPlayer { get; }
        ObservableCollection<Card> Pile { get; }
        Dictionary<Player, Card> CardPlayedByPlayer { get; }
        bool InTrickPhase { get; }
        IList<Player> PlayersLeft { get; }

        Player EndTrick();
        bool IsValidPlay(Card card);
        bool PlayCard(Card card);
        ObservableCollection<Card> GetPlayerCards(Player player);
        ObservableCollection<Card> GetPlayerCards();
    }
}
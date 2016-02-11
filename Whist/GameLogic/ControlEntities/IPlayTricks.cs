﻿using System.Collections.Generic;

namespace Whist.GameLogic.ControlEntities
{
    public interface IPlayTricks
    {
        bool HasTrickEnded { get; }
        Player PileOwner { get; }
        Player CurrentPlayer { get; }
        List<Card> Pile { get; }
        bool InTrickPhase { get; }

        Player EndTrick();
        bool PlayCard(Card card);
        IList<Card> GetPlayerCards(Player player);
        IList<Card> GetPlayerCards();
    }
}
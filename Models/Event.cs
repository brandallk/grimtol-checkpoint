using System;
using System.Collections.Generic;
using grimtol_checkpoint.Interfaces;
using grimtol_checkpoint.Enums;

namespace grimtol_checkpoint.Models
{
  public class Event
  {
    // TODO: EVENTS SHOULD HAVE AN ACCOSIATED CONDITION (I.E. PRESENCE/ABSENCE OF AN ITEM IN PLAYER INVENTORY) WHEREBY THEY EITHER DO OR DO NOT OCCUR
    public string Description { get; set; }
    public string RequiredItemName { get; set; } // Name of an item that is required to be in use for this event to fire
    public string ForbiddenItemName { get; set; } // Name of an item that cannot be in use for this event to fire
    public PlayerStatus Effect { get; set; }

    public bool ShouldFire(Game game)
    {
      if (RequiredItemName != null) // If an item is required for this event to happen
      {
        if (game.CurrentPlayer.Inventory.Find(item => item.Name == RequiredItemName) != null) // If that item is found in player's inventory...
        {
          if (game.CurrentPlayer.Inventory.Find(item => item.Name == RequiredItemName).InUse) // ...and player is 'using' it
          {
            return true; // The event should fire
          }
        }
      }
      else // Otherwise, if an item is not required for this event to happen
      {
        if (ForbiddenItemName != null) // If a certain item cannot be in use for this event to happen
        {
          if (game.CurrentPlayer.Inventory.Count == 0 // If the player's inventory is empty...
              || game.CurrentPlayer.Inventory.Find(item => item.Name == ForbiddenItemName) == null // ...or the forbidden item is not in the player's inventory
              || !game.CurrentPlayer.Inventory.Find(item => item.Name == ForbiddenItemName).InUse) // ...or the forbidden item, though in inventory, is not 'InUse'
          {
            return true; // The event should fire
          }
        }
      }

      return false; // Otherwise, the event should not fire
    }

  }
}
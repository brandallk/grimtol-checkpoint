using System;
using System.Collections.Generic;
using grimtol_checkpoint.Interfaces;
using grimtol_checkpoint.Enums;

namespace grimtol_checkpoint.Models
{
  public class Event
  {
    public string Description { get; set; }
    public string RequiredItemName { get; set; } // Name of an item that is required to be in use for this event to fire
    public string ForbiddenItemName { get; set; } // Name of an item that cannot be in use for this event to fire
    public bool Deactivated { get; set; }
    public PlayerStatus Effect { get; set; }

    public Event()
    {
      Effect = PlayerStatus.playing;
      Deactivated = false;
    }

    // Determine if the given room-event should fire or not, depending on the in-use items in the player's inventory
    public bool ShouldFire(Player player)
    {
      if (Deactivated == true)
          {
            return false;
          }
      if (RequiredItemName != null) // If an item is required for this event to happen
      {
        if (player.Inventory.Find(item => item.Name == RequiredItemName) != null // If that item is found in player's inventory...
            && player.Inventory.Find(item => item.Name == RequiredItemName).InUse) // ...and player is 'using' it        
        {
          return true; // The event should fire
        }
      }
      else // Otherwise, if an item is not required for this event to happen
      {
        if (ForbiddenItemName != null) // If a certain item cannot be in use for this event to happen
        {
          if (player.Inventory.Count == 0 // If the player's inventory is empty...
              || player.Inventory.Find(item => item.Name == ForbiddenItemName) == null // ...or the forbidden item is not in the player's inventory
              || !player.Inventory.Find(item => item.Name == ForbiddenItemName).InUse) // ...or the forbidden item, though in inventory, is not 'InUse'
          {
            return true; // The event should fire
          }
        }
        else // Otherwise, if no item is either required or forbidden for this event (e.g. the event should always fire unless it is deactivated)
        {
          // DEBUG -- This condition is probably not needed:
          if (Deactivated == false)
          {
            return true; // The event should fire
          }
        }
      }

      return false; // Otherwise, the event should not fire
    }

  }
}
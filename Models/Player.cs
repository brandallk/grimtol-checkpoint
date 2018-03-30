using System;
using System.Collections.Generic;
using grimtol_checkpoint.Interfaces;
using grimtol_checkpoint.Enums;

namespace grimtol_checkpoint.Models
{
  public class Player : IPlayer
  {
    public int Score { get; set; }
    public PlayerStatus Status { get; set; } // Keep track of whether the player is still viably 'playing', has 'lost', has 'won'...

    public List<Item> Inventory { get; set; } // A list of items the player has 'taken'
    public List<Room> VisitedRooms { get; set; } // Keep track of the rooms the player has visited
    public VictoryConditions PlayerVictoryConditions { get; set; } // Keep track of which of two alternative win conditions (if any) the player has met

    public Player()
    {
      Score = 0;
      Status = PlayerStatus.playing;
      Inventory = new List<Item>();
      VisitedRooms = new List<Room>();
      PlayerVictoryConditions = VictoryConditions.none;
    }

    // Add an item to the player's inventory
    public void Take(Item item)
    {
      if (item.Takeable && item.Locked == false) // Can't 'take' and item unless it's 'takeable' and not 'locked'
      {
        Inventory.Add(item);
      }
      else
      {
        Console.WriteLine("Can't take this item");
      }
    }

    public void PrintInventory()
    {
      if (Inventory.Count > 0)
      {
        string inventory = "";
        foreach (Item item in Inventory)
        {
          inventory += item.Name + "   ";
        }
        Console.WriteLine($"Your inventory: {inventory}");
      }
      else
      {
        Console.WriteLine("No items in your inventory");
      }
    }

  }

}
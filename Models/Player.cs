using System;
using System.Collections.Generic;
using grimtol_checkpoint.Interfaces;
using grimtol_checkpoint.Enums;

namespace grimtol_checkpoint.Models
{
  public class Player : IPlayer
  {
    public int Score { get; set; }
    public PlayerStatus Status { get; set; }

    public List<Item> Inventory { get; set; }
    public List<Room> VisitedRooms { get; set; }

    public Player()
    {
      Score = 0;
      Status = PlayerStatus.playing;
      Inventory = new List<Item>();
      VisitedRooms = new List<Room>();
    }

    public void Take(Item item)
    {
      if (item.Takeable && item.Locked == false) // Can't 'take' and item unless it's 'takeable' and not 'locked'
      {
        this.Inventory.Add(item);
      }
      else
      {
        Console.WriteLine("Can't take this item");
      }
    }

    public void PrintInventory()
    {
      if (this.Inventory.Count > 0)
      {
        string inventory = "";
        foreach (Item item in this.Inventory)
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
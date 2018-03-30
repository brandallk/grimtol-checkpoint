using System;
using System.Collections.Generic;
using grimtol_checkpoint.Interfaces;

namespace grimtol_checkpoint.Models
{
  public class Room : IRoom
  {
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Item> Items { get; set; }
    public Dictionary<string, Room> Exits { get; set; }
    public List<Event> Events { get; set; }
    public Item Door { get; set; }

    public void UseItem(Item item)
    {
      // ( Item use is handled elsewhere: Models/Game:UseItem() )
    }

    public bool Exit(string direction) => this.Exits.ContainsKey(direction);

    public void PrintDescription()
    {
      Console.WriteLine(this.Description);
    }

  }
}
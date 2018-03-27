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

    public void UseItem(Item item)
    {

    }

    public bool Exit(string direction) => this.Exits.ContainsKey(direction);

    public void Enter()
    {
      Console.WriteLine(this.Description);

      string options = "Your exit options include: ";
      foreach (KeyValuePair<string, Room> direction in this.Exits)
      {
        options += "'" + direction.Key + "' ";
      }

      Console.WriteLine(options);
    }

  }
}
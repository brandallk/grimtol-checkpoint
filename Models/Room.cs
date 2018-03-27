using System.Collections.Generic;
using grimtol_checkpoint.Interfaces;

namespace grimtol_checkpoint.Models
{
  public class Room : IRoom
  {
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Item> Items { get; set; }

    public void UseItem(Item item)
    {
      
    }
  }
}
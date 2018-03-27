using System.Collections.Generic;
using grimtol_checkpoint.Interfaces;

namespace grimtol_checkpoint.Models
{
  public class Player : IPlayer
  {
    public int Score { get; set; }
    public List<Item> Inventory { get; set; }
  }
}
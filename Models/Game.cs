using System.Collections.Generic;
using grimtol_checkpoint.Interfaces;

namespace grimtol_checkpoint.Models
{
  public class Game : IGame
  {
    public Room CurrentRoom { get; set; }
    public Player CurrentPlayer { get; set; }

    public void Reset()
    {
      
    }

    public void Setup()
    {
      
    }

    public void UseItem(string itemName)
    {
      
    }
  }
}
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

    public Player()
    {
      Score = 0;
      Status = PlayerStatus.playing;
      Inventory = new List<Item>();
    }

    public void Take(Item item) => this.Inventory.Add(item);

  }

}
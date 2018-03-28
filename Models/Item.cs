using System.Collections.Generic;
using grimtol_checkpoint.Interfaces;
using grimtol_checkpoint.Enums;

namespace grimtol_checkpoint.Models
{
  public class Item : IItem
  {
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Takeable { get; set; } // Item must be 'taken' before it can be used
    public bool Useable { get; set; } // Item cannot be 'taken', and can be used without being 'taken'
    public Room UseLocation { get; set; }
    public string UseDescription { get; set; }
    public PlayerStatus UseEffect { get; set; }
    public bool InUse { get; set; }
  }
}
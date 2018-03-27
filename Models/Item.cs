using System.Collections.Generic;
using grimtol_checkpoint.Interfaces;

namespace grimtol_checkpoint.Models
{
  public class Item : IItem
  {
    public string Name { get; set; }
    public string Description { get; set; }
  }
}
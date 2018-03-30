using System.Collections.Generic;
using grimtol_checkpoint.Interfaces;
using grimtol_checkpoint.Enums;

namespace grimtol_checkpoint.Models
{
  public class Item : IItem
  {
    public string Name { get; set; }
    public string Description { get; set; } // ? Do I need this ?
    public bool Takeable { get; set; } // Item must be 'taken' before it can be used
    public bool Useable { get; set; } // Item cannot be 'taken', and can be used without being 'taken'
    public Room UseLocation { get; set; } // The room (if any) to which use of this item is restricted
    public string UseDescription { get; set; } // Describes what happens when the item is used
    public bool Locked { get; set; } // TRUE if the item must be unlocked before it's used
    public PlayerStatus EffectOnPlayer { get; set; } // Effect of using this item on player's status (e.g. playing, lost, won)
    public VictoryConditions EffectOnVicConds { get; set; }
    public Dictionary<Item, bool> EffectOnOtherItem { get; set; } // The name of the item effected and the bool value of the effect on that item's 'locked' state
    public bool InUse { get; set; } // Whether or not the item is being 'used' by the player

    public Item()
    {
      Locked = false;
      EffectOnVicConds = VictoryConditions.none;
    }
  }
}
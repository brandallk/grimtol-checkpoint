using System;
using System.Collections.Generic;
using grimtol_checkpoint.Interfaces;
using grimtol_checkpoint.Enums;

namespace grimtol_checkpoint.Models
{
  public class Event
  {
    // TODO: EVENTS SHOULD HAVE AN ACCOSIATED CONDITION (I.E. PRESENCE/ABSENCE OF AN ITEM IN PLAYER INVENTORY) WHEREBY THEY EITHER DO OR DO NOT OCCUR
    public string Description { get; set; }
    public bool TriggerCondition { get; set; }
    // public bool BlockCondition { get; set; }
    public PlayerStatus Effect { get; set; }
  }
}
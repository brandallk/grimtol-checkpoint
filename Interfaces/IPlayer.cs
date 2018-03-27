using System.Collections.Generic;
using grimtol_checkpoint.Models;

namespace grimtol_checkpoint.Interfaces
{
    public interface IPlayer
    {
        int Score { get; set; }
        List<Item> Inventory { get; set; }

    }
}
using System.Collections.Generic;
using grimtol_checkpoint.Models;

namespace grimtol_checkpoint.Interfaces
{
    public interface IRoom
    {
        string Name { get; set; }
        string Description { get; set; }
        List<Item> Items { get; set; }

        void UseItem(Item item);

    }
}
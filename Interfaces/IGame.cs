using System.Collections.Generic;
using grimtol_checkpoint.Models;

namespace grimtol_checkpoint.Interfaces
{
    public interface IGame
    {
        Room CurrentRoom { get; set; }
        Player CurrentPlayer { get; set; }

        void Setup();
        void Reset();

        //No need to Pass a room since Items can only be used in the CurrentRoom
        void UseItem(string itemName);

    }
}
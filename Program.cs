using System;
using System.Collections.Generic;
using grimtol_checkpoint.Models;
using grimtol_checkpoint.Enums;

namespace grimtol_checkpoint
{
  class Program
  {
    static void Main(string[] args)
    {
      Game game = new Game();
      game.Reset(); // Start or re-start the game
    }
  }
}

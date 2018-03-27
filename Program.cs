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

      Player player = new Player();
      Game game = new Game();
      game.CurrentPlayer = player;
      game.Setup();

      while (player.Status == PlayerStatus.playing)
      {
        game.TakeTurn();
      }

    }
  }
}

using System.Collections.Generic;
using grimtol_checkpoint.Interfaces;

namespace grimtol_checkpoint.Models
{
  public class Game : IGame
  {
    private Dictionary<string, Room> Rooms;
    public Room CurrentRoom { get; set; }
    public Player CurrentPlayer { get; set; }

    public Dictionary<string, Room> SetupRooms()
    {
      Room hallway = new Room();
      Room barracks = new Room();
      Room castleCourtyard = new Room();
      Room captainsQuarters = new Room();
      Room guardRoom = new Room();
      Room dungeon = new Room();
      Room squireTower = new Room();
      Room warRoom = new Room();
      Room throneRoom = new Room();

      hallway.Name = "Hallway";
      hallway.Description = "You find yourself in a small hall there doesnt appear to be anything of interest here. You see a passages to the north and south, and a door to the west.";
      hallway.Items = new List<Item>();
      hallway.Exits = new Dictionary<string, Room>()
      {
        {"north", barracks},
        {"east", castleCourtyard},
        {"south", captainsQuarters}
      };

      barracks.Name = "Barracks";
      barracks.Description = "You see a room with several sleeping guards, The room smells of sweaty men. The bed closest to you is empty and there are several uniforms tossed about.";
      barracks.Items = new List<Item>();
      barracks.Exits = new Dictionary<string, Room>()
      {
        {"south", hallway}
      };

      captainsQuarters.Name = "Captain's Quarters";
      captainsQuarters.Description = "As you approach the captains Quarters you swallow hard and notice your lips are dry, Stepping into the room you see a few small tables and maps of the countryside sprawled out.";
      captainsQuarters.Items = new List<Item>();
      captainsQuarters.Exits = new Dictionary<string, Room>()
      {
        {"north", hallway},
        {"east, turn north", castleCourtyard},
        {"east", guardRoom}
      };

      castleCourtyard.Name = "Castle Courtyard";
      castleCourtyard.Description = "You step into the large castle courtyard there is a flowing fountain in the middle of the grounds and a few guards patrolling the area.";
      castleCourtyard.Items = new List<Item>();
      castleCourtyard.Exits = new Dictionary<string, Room>()
      {
        {"north", throneRoom},
        {"north, turn east", squireTower},
        {"west", hallway},
        {"south, turn west", captainsQuarters},
        {"south, turn east", guardRoom}
      };

      guardRoom.Name = "Guard Room";
      guardRoom.Description = "Pushing open the door of the guard room you look around and notice the room is empty, There are a few small tools in the corner and a chair propped against the wall near the that likely leads to the dungeon.";
      guardRoom.Items = new List<Item>();
      guardRoom.Exits = new Dictionary<string, Room>()
      {
        {"west", captainsQuarters},
        {"west, turn north", castleCourtyard},
        {"north", dungeon}
      };

      dungeon.Name = "Dungeon";
      dungeon.Description = "As you descend the stairs to the dungeon you notice a harsh chill to the air. Landing a the base of the stairs you see what the remains of a previous prisoner.";
      dungeon.Items = new List<Item>();
      dungeon.Exits = new Dictionary<string, Room>()
      {
        {"south", guardRoom}
      };

      squireTower.Name = "Squire Tower";
      squireTower.Description = "As you finish climbing the stairs to the squire tower you see a messenger nestled in his bed. His messenger overcoat is hanging from his bed post.";
      squireTower.Items = new List<Item>();
      squireTower.Exits = new Dictionary<string, Room>()
      {
        {"west, turn south", castleCourtyard},
        {"west, turn north", throneRoom}
      };

      warRoom.Name = "War Room";
      warRoom.Description = "Steping into the war room you see several maps spread across tables. On the maps many of the villages have been marked for purification. You also notice several dishes of prepared food to the side perhaps the war council will be meeting soon.";
      warRoom.Items = new List<Item>();
      warRoom.Exits = new Dictionary<string, Room>()
      {
        {"south", squireTower}
      };

      throneRoom.Name = "Throne Room";
      throneRoom.Description = "As you unlock the door and swing it wide you see an enormous hall stretching out before you. At the opposite end of the hall sitting on his throne you see the dark lord. The Dark Lord shouts at you demanding why you dared to interrupt him during his Ritual of Evil Summoning... Dumbfounded you mutter an incoherent response. Becoming more enraged the Dark Lord complains that you just ruined his concentration and he will now have to start the ritual over... Quickly striding towards you he smirks at least I know have a sacrificial volunteer. Plunging his jewel encrusted dagger into your heart your world slowly fades away.";
      throneRoom.Items = new List<Item>();
      throneRoom.Exits = new Dictionary<string, Room>()
      {
        {"south", castleCourtyard},
        {"south, turn west", squireTower}
      };

      return new Dictionary<string, Room>()
      {
        {hallway.Name, hallway},
        {barracks.Name, barracks},
        {castleCourtyard.Name, castleCourtyard},
        {captainsQuarters.Name, captainsQuarters},
        {guardRoom.Name, guardRoom},
        {dungeon.Name, dungeon},
        {squireTower.Name, squireTower},
        {warRoom.Name, warRoom},
        {throneRoom.Name, throneRoom},
      };

    }

    public void Reset()
    {

    }

    public void Setup()
    {

    }

    public void UseItem(string itemName)
    {

    }
  }
}
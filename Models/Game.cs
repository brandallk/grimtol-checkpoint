using System;
using System.Collections.Generic;
using grimtol_checkpoint.Interfaces;
using grimtol_checkpoint.Enums;

namespace grimtol_checkpoint.Models
{
  public class Game : IGame
  {
    private Dictionary<string, Room> Rooms;
    public Room CurrentRoom { get; set; }
    public Player CurrentPlayer { get; set; }

    public void Reset()
    {

    }

    public void Setup()
    {
      this.Rooms = SetupRooms();
      this.CurrentRoom = this.Rooms["Hallway"];
      Console.Clear();
      Console.WriteLine("Welcome to the game!");
    }

    public void UseItem(string itemName)
    {
      Item foundItem = this.CurrentRoom.Items.Find(roomItem => roomItem.Name.ToLower() == itemName);
      Item inventoryItem = this.CurrentPlayer.Inventory.Find(invItem => invItem.Name.ToLower() == itemName);

      // DEBUG --
      Console.WriteLine($"foundItem: {foundItem.Name}");
      Console.WriteLine($"inventoryItem: {inventoryItem.Name}");

      if (foundItem != null && foundItem.Useable)
      {
        if (!foundItem.Takeable && (foundItem.UseLocation == null || foundItem.UseLocation == this.CurrentRoom)) // To use an item found in a room (without taking it first), it must be takeable and it must be useable in the current room
        {
          Console.WriteLine(foundItem.UseDescription);
          this.CurrentPlayer.Status = foundItem.UseEffect;
          foundItem.InUse = true;
        }
      }
      else if (inventoryItem != null) // To use an inventory item, it must have already been taken (because that's how it got added to the inventory), and it must be useable in the current room
      {
        if (foundItem.UseLocation == null || foundItem.UseLocation == this.CurrentRoom)
        {
          Console.WriteLine(inventoryItem.UseDescription);
          this.CurrentPlayer.Status = inventoryItem.UseEffect;
          foundItem.InUse = true;
        }
      }

      // DEBUG --
      foreach (Item item in this.CurrentPlayer.Inventory)
      {
        Console.WriteLine($"you have: {item.Name} and its InUse is {item.InUse}");
      }
    }

    public void PrintOptions()
    {
      string options = "'help'   'quit'   'look'   'inventory'   ";
      foreach (KeyValuePair<string, Room> exitDirection in this.CurrentRoom.Exits)
      {
        options += "'go " + exitDirection.Key + "'   ";
      }
      foreach (Item item in this.CurrentRoom.Items)
      {
        if (item.Takeable) // item must be taken to be used
        {
          options += "'take " + item.Name + "'   ";
        }
        if (item.Useable) // item does not need to be taken to be used
        {
          options += "'use " + item.Name + "'   ";
        }
      }
      foreach (Item item in this.CurrentPlayer.Inventory)
      {
        options += "'use " + item.Name + "'   ";
      }
      Console.WriteLine(options);
    }

    public void TakeTurn()
    {
      if (this.CurrentRoom.Events != null && this.CurrentRoom.Events.Count > 0)
      {
        foreach (Event evt in this.CurrentRoom.Events)
        {
          // IN PROGRESS: WHY IS THE TRIGGER CONDITION APPARENTLY TRUE AND FIRING WHEN ENTER COURTYARD WITH UNIFORM IN INVENTORY AND INUSE?!!
          if (evt.TriggerCondition)
          {
            // DEBUG --
            bool cond1 = this.CurrentPlayer.Inventory.Find(item => item.Name == "Guard Uniform") != null;
            bool cond2 = this.CurrentPlayer.Inventory.Find(item => item.Name == "Guard Uniform").InUse;
            Console.WriteLine($"this.CurrentPlayer.Inventory.Find(item => item.Name == Guard Uniform) != null   is   {cond1}");
            Console.WriteLine($"this.CurrentPlayer.Inventory.Find(item => item.Name == Guard Uniform).InUse   is   {cond2}");

            Console.WriteLine(evt.Description);
            this.CurrentPlayer.Status = evt.Effect;
          }
        }
      }

      bool validOption = false;
      while (!validOption && this.CurrentPlayer.Status == PlayerStatus.playing)
      {
        Console.WriteLine("What do you do now?");

        string action = Console.ReadLine();
        if (action.ToLower() == "help")
        {
          this.PrintOptions();
        }
        else if (action.ToLower() == "quit")
        {
          validOption = true;
          this.CurrentPlayer.Status = PlayerStatus.quit;
        }
        else if (action.ToLower() == "inventory")
        {
          validOption = true;
          this.CurrentPlayer.PrintInventory();
        }
        else if (action.Length >= 4 && action.ToLower().Substring(0, 3) == "go ")
        {
          string direction = action.ToLower().Substring(3);
          if (this.CurrentRoom.Exit(direction))
          {
            validOption = true;
            this.CurrentRoom = this.CurrentRoom.Exits[direction];
          }
          else
          {
            Console.WriteLine("That option is invalid. Try again.");
          }
        }
        else if (action.ToLower() == "look")
        {
          this.CurrentRoom.PrintDescription();
        }
        else if (action.Length >= 6 && action.ToLower().Substring(0, 5) == "take ")
        {
          string itemName = action.ToLower().Substring(5);
          Item item = this.CurrentRoom.Items.Find(roomItem => roomItem.Name.ToLower() == itemName);
          if (item != null)
          {
            validOption = true;
            this.CurrentPlayer.Take(item);
          }
          else
          {
            Console.WriteLine("That option is invalid. Try again.");
          }
        }
        else if (action.Length >= 5 && action.ToLower().Substring(0, 4) == "use ")
        {
          string itemName = action.ToLower().Substring(4);
          Item foundItem = this.CurrentRoom.Items.Find(roomItem => roomItem.Name.ToLower() == itemName);
          Item inventoryItem = this.CurrentPlayer.Inventory.Find(invItem => invItem.Name.ToLower() == itemName);
          if (foundItem != null || inventoryItem != null)
          {
            validOption = true;
            this.UseItem(itemName);
          }
          else
          {
            Console.WriteLine("That option is invalid. Try again.");
          }
        }
        else
        {
          Console.WriteLine("That option is invalid. Try again.");
        }
      }
    }

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
      barracks.Description = "You see a room with several sleeping guards. The room smells of sweaty men. Near you are a wide bed and a narrow bed; both appear empty. There are several uniforms tossed about.";
      barracks.Items = new List<Item>()
      {
        new Item() { Name = "Guard Uniform", Description = "Guard Uniform", Takeable = true, Useable = false, UseDescription = "You are now disguised as a guard.", UseEffect = PlayerStatus.playing },
        new Item() { Name = "Wide Bed", Description = "Wide Bed", Takeable = false, Useable = true, UseLocation = barracks, UseDescription = "You climb into the bed and pretend to be asleep. A few minutes later several guards walk into the room. One approaches you to wake you... (GUARD) 'Hey Get Up! it's your turn for watch, Go relieve Shigeru in the Guard Room!' Quickly, you climb out of the bed.", UseEffect = PlayerStatus.playing },
        new Item() { Name = "Narrow Bed", Description = "Narrow Bed", Takeable = false, Useable = true, UseLocation = barracks, UseDescription = "(GUARD) 'What do you think you're doing? Hey you're not Leroy! Quick, Jenkins, sieze him...' Jenkins, a bit over-zelous, swings his sword, cleaving you in half... ", UseEffect = PlayerStatus.lost }
      };
      barracks.Exits = new Dictionary<string, Room>()
      {
        {"south", hallway}
      };

      // TODO: NEED TO DEFINE EVENTS THAT HAPPEN ON ENTERING THIS ROOM. AN EVENT'S OCCURRENCE OR EFFECT SHOULD BE DEPENDENT ON THE LIST OF ITEMS A PLAYER IS USING.
      captainsQuarters.Name = "Captain's Quarters";
      captainsQuarters.Description = "As you approach the captains Quarters you swallow hard and notice your lips are dry, Stepping into the room you see a few small tables and maps of the countryside sprawled out.";
      captainsQuarters.Items = new List<Item>()
      {
        // TODO: NEED TO FILL IN THESE ITEM'S USEDESCRIPTION, USELOCATION, ETC.
        // TODO: NEED TO CHECK FOR ANY ADDITIONAL USEABLE ITEMS IN THIS ROOM
        new Item() { Name = "Key", Description = "Key" },
        new Item() { Name = "Note", Description = "Note" },
        new Item() { Name = "Vial", Description = "Vial" }
      };
      captainsQuarters.Exits = new Dictionary<string, Room>()
      {
        {"north", hallway},
        {"east, turn north", castleCourtyard},
        {"east", guardRoom}
      };

      // TODO: NEED TO DEFINE EVENTS THAT HAPPEN ON ENTERING THIS ROOM
      castleCourtyard.Name = "Castle Courtyard";
      castleCourtyard.Description = "You step into the large castle courtyard there is a flowing fountain in the middle of the grounds and a few guards patrolling the area.";
      castleCourtyard.Items = new List<Item>();
      castleCourtyard.Events = new List<Event>()
      {
        new Event() { Description = "Oi, long night tonight I wish I was in my bed. If your just getting on shift your should go talk to the captain.", TriggerCondition = this.CurrentPlayer.Inventory.Find(item => item.Name == "Guard Uniform") != null && this.CurrentPlayer.Inventory.Find(item => item.Name == "Guard Uniform").InUse, Effect = PlayerStatus.playing },
        new Event() { Description = "To your left you see a guard approaching you. (GUARD) 'Wat? Who the blazes are you?' Quickly he raises the alarm and several of the crossbow men turn and fire on you. You realize you have made a grave mistake as a bolt slams into your body... ", TriggerCondition = this.CurrentPlayer.Inventory.Find(item => item.Name == "Guard Uniform") == null || !this.CurrentPlayer.Inventory.Find(item => item.Name == "Guard Uniform").InUse, Effect = PlayerStatus.lost }
      };
      castleCourtyard.Exits = new Dictionary<string, Room>()
      {
        {"north", throneRoom},
        {"north, turn east", squireTower},
        {"west", hallway},
        {"south, turn west", captainsQuarters},
        {"south, turn east", guardRoom}
      };

      // TODO: NEED TO DEFINE EVENTS THAT HAPPEN ON ENTERING THIS ROOM
      guardRoom.Name = "Guard Room";
      guardRoom.Description = "Pushing open the door of the guard room you look around and notice the room is empty, There are a few small tools in the corner and a chair propped against the wall near the that likely leads to the dungeon.";
      guardRoom.Items = new List<Item>()
      {
        // TODO: NEED TO FILL IN THESE ITEM'S USEDESCRIPTION, USELOCATION, ETC.
        // TODO: NEED TO CHECK FOR ANY ADDITIONAL USEABLE ITEMS IN THIS ROOM
        new Item() { Name = "Hammer", Description = "Hammer" }
      };
      guardRoom.Exits = new Dictionary<string, Room>()
      {
        {"west", captainsQuarters},
        {"west, turn north", castleCourtyard},
        {"north", dungeon}
      };

      // TODO: NEED TO DEFINE EVENTS THAT HAPPEN ON ENTERING THIS ROOM
      dungeon.Name = "Dungeon";
      dungeon.Description = "As you descend the stairs to the dungeon you notice a harsh chill to the air. Landing a the base of the stairs you see what the remains of a previous prisoner.";
      dungeon.Items = new List<Item>()
      {
        // TODO: NEED TO FILL IN THESE ITEM'S USEDESCRIPTION, USELOCATION, ETC.
        // TODO: NEED TO CHECK FOR ANY ADDITIONAL USEABLE ITEMS IN THIS ROOM
        new Item() { Name = "Broken Lock", Description = "Broken Lock" }
      };
      dungeon.Exits = new Dictionary<string, Room>()
      {
        {"south", guardRoom}
      };

      // TODO: NEED TO DEFINE EVENTS THAT HAPPEN ON ENTERING THIS ROOM
      squireTower.Name = "Squire Tower";
      squireTower.Description = "As you finish climbing the stairs to the squire tower you see a messenger nestled in his bed. His messenger overcoat is hanging from his bed post.";
      squireTower.Items = new List<Item>()
      {
        // TODO: NEED TO FILL IN THESE ITEM'S USEDESCRIPTION, USELOCATION, ETC.
        // TODO: NEED TO CHECK FOR ANY ADDITIONAL USEABLE ITEMS IN THIS ROOM
        new Item() { Name = "Messenger Overcoat", Description = "Messenger Overcoat" }
      };
      squireTower.Exits = new Dictionary<string, Room>()
      {
        {"west, turn south", castleCourtyard},
        {"west, turn north", throneRoom}
      };

      // TODO: NEED TO DEFINE EVENTS THAT HAPPEN ON ENTERING THIS ROOM
      warRoom.Name = "War Room";
      warRoom.Description = "Steping into the war room you see several maps spread across tables. On the maps many of the villages have been marked for purification. You also notice several dishes of prepared food to the side perhaps the war council will be meeting soon.";
      warRoom.Items = new List<Item>()
      {
        // TODO: NEED TO FILL IN THESE ITEM'S USEDESCRIPTION, USELOCATION, ETC.
        // TODO: NEED TO CHECK FOR ANY ADDITIONAL USEABLE ITEMS IN THIS ROOM
        new Item() { Name = "Window", Description = "Window" }
      };
      warRoom.Exits = new Dictionary<string, Room>()
      {
        {"south", squireTower}
      };

      // TODO: NEED TO DEFINE EVENTS THAT HAPPEN ON ENTERING THIS ROOM
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

  }
}
using System;
using System.Collections.Generic;
using grimtol_checkpoint.Interfaces;
using grimtol_checkpoint.Enums;

namespace grimtol_checkpoint.Models
{
  public class Game : IGame
  {
    private Dictionary<string, Room> Rooms; // The collection of rooms belonging to the game.
    public Room CurrentRoom { get; set; }
    public Player CurrentPlayer { get; set; }

    public void Reset() // Start or restart the game
    {
      Setup(); // Do basic game setup

      // Continue taking turns as long as the player's status remains 'playing'
      while (CurrentPlayer.Status == PlayerStatus.playing)
      {
        TakeTurn();
      }

      // Handle the end of a game if it has been lost or won.
      if (CurrentPlayer.Status == PlayerStatus.lost)
      {
        Console.WriteLine("\nYou have died, the rebellion has failed.");
      }
      else if (CurrentPlayer.Status == PlayerStatus.won)
      {
        if (CurrentPlayer.PlayerVictoryConditions == VictoryConditions.rulerKilled)
        {
          // Win the game via poisoning the ruler
          Console.WriteLine("\nYou succeeded where so many others failed. The Dark Lords' reign of terror is over.");
        }
        else if (CurrentPlayer.PlayerVictoryConditions == VictoryConditions.prisonerFreed)
        {
          // Win the game via freeing the prisoner
          Console.WriteLine("\nYou freed an old man held captive! The Dark Lord's rule continues, but you can try to assassinate him again some other night.");
        }
      }

      // Allow player to play again
      Console.WriteLine("\nPlay again?");
      string response = Console.ReadLine();
      if (response.ToLower() == "y" || response.ToLower() == "yes")
      {
        Reset();
      }
      else
      {
        Console.WriteLine("Thanks for playing. Goodbye.");
      }
    }

    public void Setup() // Instantiate player & rooms, and show initial game-text
    {
      CurrentPlayer = new Player();
      Rooms = SetupRooms();
      CurrentRoom = Rooms["Hallway"];
      Console.Clear();
      Console.WriteLine("Welcome to the 'Castle Grimtol' console game. This is a modified version of Jake's stock-plot game from the assignment's original repo.\n");
      Console.WriteLine("The evil ruler of Castle Grimtol has imposed a reign of terror on the surrounding lands. You, a rebel from a nearby village, have sneaked into the castle with two goals in mind: 1) Find and free an old man from your village who was recently taken captive by the evil ruler. 2) End the reign of terror by killing the ruler and his council.\n");
      Console.WriteLine("At each stage in the game, you can type the following commands:\n'help' prints a list of commands available to you at the current time.\n'look' (re)prints the description of the room you're currently in.\n'go <direction>' takes you through one of the current room's exits into a neighboring room. (Your exit options can always be found by typing 'help'.)\n'take <item>' allows you to place a found item in your player inventory\n'inventory' prints a list of items currently in your possession.\n'use <item>' allows you to use an item -- either a found item or an item carried in your inventory.\n'quit' ends the game.");
      Console.WriteLine("\n\nFOR GRADING:\nSample 1st way to die: 'go east' at first step.\n\nSample 2nd way to die: 'go north', 'take guard uniform', 'use guard uniform', 'go south', 'go east', 'go south, then east', 'take hammer', 'go north', 'use hammer', 'take dungeon lock', 'go south', 'go west', 'use dungeon lock', 'take throne room key', 'go east, then north', 'use throne room key', 'go north'.\n\nThe original stock-plot was modified to allow for 2 mutually-exclusive, alternative ways to win:\n\n1st way to win: 'go north', 'take guard uniform', 'use guard uniform', 'go south', 'go east', 'go south, then east', 'take hammer', 'go north', 'use hammer', 'take dungeon lock', 'go south', 'go west, then north', 'go north, then east', 'take messenger overcoat', 'go west, then south', 'go south, then east', 'go north', 'use messenger overcoat'.\n\n2nd way to win: 'go north', 'take guard uniform', 'use guard uniform', 'go south', 'go east', 'go south, then east', 'take hammer', 'go north', 'use hammer', 'take dungeon lock', 'go south', 'go west', 'use dungeon lock', 'take via', 'go east, then north', 'go north, then east', 'go north', 'use vial'.\n\n ");
      Console.WriteLine("Good luck!\n");
    }

    public void UseItem(string itemName) // Handle a player's 'use' of an item
    {
      // If the item is NOT an item in the player's inventory, get the item (otherwise this will be NULL)
      Item foundItem = CurrentRoom.Items.Find(roomItem => roomItem.Name.ToLower() == itemName);
      // If the item IS an item in the player's inventory, get the item (otherwise this will be NULL)
      Item inventoryItem = CurrentPlayer.Inventory.Find(invItem => invItem.Name.ToLower() == itemName);

      // To use an item found in a room (without taking it first), it must not be takeable, it must be useable in the current room, and it can't have 'locked' = TRUE
      if (foundItem != null && foundItem.Useable)
      {
        if (!foundItem.Takeable && foundItem.Locked == false
            && (foundItem.UseLocation == null || foundItem.UseLocation == CurrentRoom))
        {
          Console.WriteLine(foundItem.UseDescription); // Print the paragraph associated with the item's use
          CurrentPlayer.Status = foundItem.EffectOnPlayer; // If using this item causes the player's status to change to 'won' or 'lost', record that change
          CurrentPlayer.PlayerVictoryConditions = foundItem.EffectOnVicConds; // Record any changes to the player's victory conditions associated with this item's use
          foundItem.InUse = true;
          if (foundItem.EffectOnOtherItem != null) // Account for this item's effect on another item (if any)
          {
            foreach (KeyValuePair<Item, bool> item in foundItem.EffectOnOtherItem)
            {
              item.Key.Locked = item.Value; // The effect (if any) will be to lock or unlock another item
            }
          }
        }
        else
        {
          Console.WriteLine("Sorry, you cannot 'use' this item yet.");
        }
      }
      // To use an inventory item, it must have already been taken (because that's how it got added to the inventory), it must be useable in the current room, and it can't have 'locked' = TRUE
      else if (inventoryItem != null)
      {
        if ((inventoryItem.UseLocation == null || inventoryItem.UseLocation == CurrentRoom) && inventoryItem.Locked == false)
        {
          Console.WriteLine(inventoryItem.UseDescription); // Print the paragraph associated with the item's use
          CurrentPlayer.Status = inventoryItem.EffectOnPlayer; // If using this item causes the player's status to change to 'won' or 'lost', record that change
          CurrentPlayer.PlayerVictoryConditions = inventoryItem.EffectOnVicConds; // Record any changes to the player's victory conditions associated with this item's use
          inventoryItem.InUse = true;
          if (inventoryItem.EffectOnOtherItem != null) // Account for this item's effect on another item (if any)
          {
            foreach (KeyValuePair<Item, bool> item in inventoryItem.EffectOnOtherItem)
            {
              item.Key.Locked = item.Value; // The effect (if any) will be to lock or unlock another item
            }
          }
        }
        else
        {
          Console.WriteLine("Sorry, you cannot 'use' this item yet.");
        }
      }
    }

    public void PrintOptions() // Print to the console a list of commands available to the user at the current time
    {
      string options = "'help'   'quit'   'look'   'inventory'   ";
      foreach (KeyValuePair<string, Room> exitDirection in CurrentRoom.Exits) // List the directions in which the player can 'go'
      {
        options += "'go " + exitDirection.Key + "'   ";
      }
      foreach (Item item in CurrentRoom.Items) // List the items that can be 'taken'
      {
        // If the item must be taken to be used:
        if (item.Takeable && CurrentPlayer.Inventory.Find(itm => itm.Name == item.Name) == null) // (Don't include items that have already been 'taken')
        {
          options += "'take " + item.Name + "'   ";
        }
        // If the item does not need to be taken to be used:
        if (item.Useable) // List the items found in the current room that can be 'used'
        {
          options += "'use " + item.Name + "'   ";
        }
      }
      foreach (Item item in CurrentPlayer.Inventory) // List the items in the player's inventory that can be 'used'
      {
        options += "'use " + item.Name + "'   ";
      }
      Console.WriteLine($"Your options include the following: {options}");
    }

    public void TakeTurn() // Handle a game-play 'turn'
    {
      // Launch any events that should occur
      if (CurrentRoom.Events != null && CurrentRoom.Events.Count > 0) // Check to see if this room has associated event(s)
      {
        foreach (Event evt in CurrentRoom.Events)
        {
          if (evt.ShouldFire(CurrentPlayer)) // Check to make sure this event should be allowed to fire
          {
            Console.WriteLine(evt.Description); // Print out the paragraph associated with this event
            CurrentPlayer.Status = evt.Effect; // Record any change to the player's status ('won'/'lost') associated with the event
          }
        }
      }

      // Turn 'loop'
      bool validOption = false;
      while (!validOption && CurrentPlayer.Status == PlayerStatus.playing) // Continue looping until user gives a valid command or the player wins/loses the game
      {
        Console.WriteLine("What do you do now?");

        string action = Console.ReadLine();

        // Respond to 'help' command
        if (action.ToLower() == "help")
        {
          PrintOptions();
        }

        // Respond to 'quit' command
        else if (action.ToLower() == "quit")
        {
          validOption = true;
          CurrentPlayer.Status = PlayerStatus.quit;
        }

        // Respond to 'inventory' command
        else if (action.ToLower() == "inventory")
        {
          validOption = true;
          CurrentPlayer.PrintInventory();
        }

        // Respond to 'go <direction>' command
        else if (action.Length >= 4 && action.ToLower().Substring(0, 3) == "go ")
        {
          string direction = action.ToLower().Substring(3); // Read the direction-name string
          if (CurrentRoom.Exit(direction)) // Make sure the given direction corresponds to one of this room's exit directions
          {
            Room nextRoom = CurrentRoom.Exits[direction]; // The room corresponding to the given exit direction
            if (!nextRoom.Door.Locked) // Make sure the door to the next room is not locked
            {
              validOption = true;
              CurrentRoom = CurrentRoom.Exits[direction]; // Go to the desired room
              Console.WriteLine($"\n[[ {CurrentRoom.Name} ]]\n"); // Print a hint showing the name of the room just entered
              Console.WriteLine(CurrentRoom.Description); // Print the room's description
            }
            else
            {
              Console.WriteLine("That door is locked!");
            }
          }
          else
          {
            Console.WriteLine("That option is invalid. Try again.");
          }
        }

        // Respond to 'look' command
        else if (action.ToLower() == "look")
        {
          CurrentRoom.PrintDescription();
        }

        // Respond to 'take <item>' command
        else if (action.Length >= 6 && action.ToLower().Substring(0, 5) == "take ")
        {
          string itemName = action.ToLower().Substring(5); // Get the item-name string
          Item item = CurrentRoom.Items.Find(roomItem => roomItem.Name.ToLower() == itemName); // Get the item
          if (item != null) // Make sure the item exists
          {
            validOption = true;
            CurrentPlayer.Take(item); // Put the item in the player's inventory
          }
          else
          {
            Console.WriteLine("That option is invalid. Try again.");
          }
        }

        // Respond to a 'use <item>' command
        else if (action.Length >= 5 && action.ToLower().Substring(0, 4) == "use ")
        {
          string itemName = action.ToLower().Substring(4); // Get the action-name string

          Item foundItem = CurrentRoom.Items.Find(roomItem => roomItem.Name.ToLower() == itemName); // Get the item if it's an item found in the current room (or null)
          Item inventoryItem = CurrentPlayer.Inventory.Find(invItem => invItem.Name.ToLower() == itemName); // Get the item if it's an item in the player's inventory (or null)
          if (foundItem != null || inventoryItem != null) // Make sure the item exists
          {
            validOption = true;
            UseItem(itemName); // Handle the effect(s) of 'using' the item
          }
          else
          {
            Console.WriteLine("That option is invalid. Try again.");
          }
        }

        // Respond to any invalid command not already caught
        else
        {
          Console.WriteLine("That option is invalid. Try again.");
        }

        // Keep track of which rooms the player has already visited.
        if (!CurrentPlayer.VisitedRooms.Contains(CurrentRoom))
        {
          CurrentPlayer.VisitedRooms.Add(CurrentRoom);
        }
        else
        {
          if (CurrentRoom.Events != null && CurrentRoom.Events.Count > 0)
          {
            foreach (Event evt in CurrentRoom.Events)
            {
              evt.Deactivated = true; // Deactivate default events for any room a player has already visited
            }
          }
        }
      }
    }

    // Instantiate the game's rooms, along with each one's associated items and events
    public Dictionary<string, Room> SetupRooms()
    {
      // CREATE CLASS INSTANCES
      Room hallway = new Room();
      Room barracks = new Room();
      Room castleCourtyard = new Room();
      Room captainsQuarters = new Room();
      Room guardRoom = new Room();
      Room dungeon = new Room();
      Room squireTower = new Room();
      Room warRoom = new Room();
      Room throneRoom = new Room();
      Item guardUniform = new Item();
      Item wideBed = new Item();
      Item narrowBed = new Item();
      Item throneRoomKey = new Item();
      Item note = new Item();
      Item vial = new Item();
      Item hammer = new Item();
      Item dungeonLock = new Item();
      Item messengerOvercoat = new Item();
      Item window = new Item();
      Item throneRoomDoor = new Item();
      Event courtyardEvt1 = new Event();
      Event courtyardEvt2 = new Event();
      Event guardRoomEvt1 = new Event();
      Event captQuartEvt1 = new Event();
      Event throneRoomEvt1 = new Event();

      // ITEM DEFINITIONS
      guardUniform = SetupGuardUniform(guardUniform);
      wideBed = SetupWideBed(wideBed);
      wideBed.UseLocation = barracks;
      narrowBed = SetupNarrowBed(narrowBed);
      narrowBed.UseLocation = barracks;
      throneRoomKey = SetupThroneRoomKey(throneRoomKey);
      throneRoomKey.UseLocation = castleCourtyard; // Unlocks door to throne room
      note = SetupNote(note);
      note.UseLocation = squireTower; // Used to get messenger to leave squire tower
      vial = SetupVial(vial);
      vial.UseLocation = warRoom; // Used to poison cups in the war room
      hammer = SetupHammer(hammer);
      hammer.UseLocation = dungeon; // Used to break lock in dungeon (to free prisoner)
      dungeonLock = SetupDungeonLock(dungeonLock);
      dungeonLock.UseLocation = captainsQuarters; // Used to reveal throneRoomKey, note, and vial when presented to captain
      messengerOvercoat = SetupMessengerOvercoat(messengerOvercoat);
      messengerOvercoat.UseLocation = dungeon; // Used to disguise the freed prisoner
      throneRoomDoor = SetupThroneRoomDoor(throneRoomDoor);
      throneRoomDoor.UseLocation = castleCourtyard; // Used to enter throne room

      // EVENT DEFINITIONS
      courtyardEvt1 = SetupCourtyardEvt1(courtyardEvt1);
      courtyardEvt2 = SetupCourtyardEvt2(courtyardEvt2);
      guardRoomEvt1 = SetupGuardRoomEvt1(guardRoomEvt1);
      captQuartEvt1 = SetupCaptQuartEvt1(captQuartEvt1);
      throneRoomEvt1 = SetupThroneRoomEvt1(throneRoomEvt1);

      // ROOM DEFINITIONS
      hallway = SetupHallway(hallway);
      hallway.Items = new List<Item>();
      hallway.Exits = new Dictionary<string, Room>() { { "north", barracks }, { "east", castleCourtyard } };

      barracks = SetupBarracks(barracks);
      barracks.Items = new List<Item>() { guardUniform, wideBed, narrowBed };
      barracks.Exits = new Dictionary<string, Room>() { { "south", hallway } };

      captainsQuarters = SetupCaptainsQuarters(captainsQuarters);
      captainsQuarters.Items = new List<Item>() { throneRoomKey, note, vial };
      captainsQuarters.Events = new List<Event>() { captQuartEvt1 };
      captainsQuarters.Exits = new Dictionary<string, Room>() { { "east, then north", castleCourtyard }, { "east", guardRoom } };

      castleCourtyard = SetupCastleCourtyard(castleCourtyard);
      castleCourtyard.Items = new List<Item>() { throneRoomDoor };
      castleCourtyard.Events = new List<Event>() { courtyardEvt1, courtyardEvt2 };
      castleCourtyard.Exits = new Dictionary<string, Room>() { { "north", throneRoom }, { "north, then east", squireTower }, { "west", hallway }, { "south, then west", captainsQuarters }, { "south, then east", guardRoom } };

      guardRoom = SetupGuardRoom(guardRoom);
      guardRoom.Items = new List<Item>() { hammer };
      guardRoom.Events = new List<Event>() { guardRoomEvt1 };
      guardRoom.Exits = new Dictionary<string, Room>() { { "west", captainsQuarters }, { "west, then north", castleCourtyard }, { "north", dungeon } };

      dungeon = SetupDungeon(dungeon);
      dungeon.Items = new List<Item>() { dungeonLock };
      dungeon.Exits = new Dictionary<string, Room>() { { "south", guardRoom } };

      squireTower = SetupSquireTower(squireTower);
      squireTower.Items = new List<Item>() { messengerOvercoat };
      squireTower.Exits = new Dictionary<string, Room>() { { "west, then south", castleCourtyard }, { "west, then north", throneRoom }, { "north", warRoom } };

      warRoom = SetupWarRoom(warRoom);
      warRoom.Items = new List<Item>();
      warRoom.Exits = new Dictionary<string, Room>() { { "south", squireTower } };

      throneRoom = SetupThroneRoom(throneRoom);
      throneRoom.Door = throneRoomDoor;
      throneRoom.Door.Locked = true;
      throneRoom.Items = new List<Item>();
      throneRoom.Events = new List<Event>() { throneRoomEvt1 };
      throneRoom.Exits = new Dictionary<string, Room>() { { "south", castleCourtyard }, { "south, then west", squireTower } };

      // DEFINE ITEM EFFECTS ON OTHER ITEMS
      throneRoomKey.EffectOnOtherItem = // (Unlocks door to throne room)
        new Dictionary<Item, bool>() {
          {throneRoom.Door, false}
        };
      hammer.EffectOnOtherItem = // (Unlocks lock in dungeon)
        new Dictionary<Item, bool>() {
          {dungeon.Items.Find(item => item.Name == "Dungeon Lock"), false}
        };
      dungeonLock.EffectOnOtherItem =
        new Dictionary<Item, bool>() {
          {captainsQuarters.Items.Find(item => item.Name == "Throne Room Key"), false},
          {captainsQuarters.Items.Find(item => item.Name == "Note"), false},
          {captainsQuarters.Items.Find(item => item.Name == "Vial"), false}
        };

      // CONSTRUCT THIS.ROOMS OBJECT
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

    public Room SetupHallway(Room hallway)
    {
      hallway.Name = "Hallway";
      hallway.Description = "You find yourself in a small hall. There doesn't appear to be anything of interest here. Your exits are a passage to the NORTH and a door to the EAST.";
      hallway.Items = new List<Item>();
      hallway.Door = new Item();
      return hallway;
    }

    public Room SetupBarracks(Room barracks)
    {
      barracks.Name = "Barracks";
      barracks.Description = "You enter a dark room in which castle guards lie asleep.\nYour only exit is SOUTH.\nNear the entrance, a wide bed on your right seems to be empty. A narrow bed on your left also seems unoccupied. You notice a guard uniform hanging on a hook.\nSuddenly, you think you hear someone approaching from the south hallway.";
      barracks.Door = new Item();
      return barracks;
    }

    public Room SetupCaptainsQuarters(Room captainsQuarters)
    {
      captainsQuarters.Name = "Captain's Quarters";
      captainsQuarters.Description = "In the Captain's quaters, you see a few small tables with maps of the countryside spread out. Your exits are EAST or EAST, THEN NORTH to get back to the courtyard.";
      captainsQuarters.Door = new Item();
      return captainsQuarters;
    }

    public Room SetupCastleCourtyard(Room castleCourtyard)
    {
      castleCourtyard.Name = "Castle Courtyard";
      castleCourtyard.Description = "You see a large courtyard with a fountain in its center and castle guards on patrol. Exits include: 1) a door to the NORTH, 2) a passage leading NORTH, THEN EAST, 3) a passage leading SOUTH, THEN WEST, 4) a passage leading SOUTH, THEN EAST.";
      castleCourtyard.Door = new Item();
      return castleCourtyard;
    }

    public Room SetupGuardRoom(Room guardRoom)
    {
      guardRoom.Name = "Guard Room";
      guardRoom.Description = "The guard room is empty. Your exits are WEST and NORTH.";
      guardRoom.Door = new Item();
      return guardRoom;
    }

    public Room SetupDungeon(Room dungeon)
    {
      dungeon.Name = "Dungeon";
      dungeon.Description = "Dark stairs lead you downward. At the bottom, you notice a harsh chill to the air. You see a prison cell with its door secured by a large but severly corroded padlock. Your only exit is SOUTH.";
      dungeon.Door = new Item();
      return dungeon;
    }

    public Room SetupSquireTower(Room squireTower)
    {
      squireTower.Name = "Squire Tower";
      squireTower.Description = "You follow a flight of stairs leading to the squire tower. Inside, you see a messenger asleep in a bed. His overcoat hangs from the bed post. Your exits are a door to the NORTH, the stairway leading back WEST, THEN SOUTH, and the possibility of following the stairs WEST, THEN NORTH to the room at the north of the courtyard.";
      squireTower.Door = new Item();
      return squireTower;
    }

    public Room SetupWarRoom(Room warRoom)
    {
      warRoom.Name = "War Room";
      warRoom.Description = "In this room, you see tables arranged for a feast, with hot food waiting the guests, and an ornate cup at each chair, filled with wine. Each cup is labeled with name of the ruler or one of his evil council. It's apparent that a meeting of the ruler and his war council is imminent. Your only exit is SOUTH.";
      warRoom.Door = new Item();
      return warRoom;
    }

    public Room SetupThroneRoom(Room throneRoom)
    {
      throneRoom.Name = "Throne Room";
      throneRoom.Description = "Your only exit is SOUTH.";
      return throneRoom;
    }

    public Item SetupGuardUniform(Item guardUniform)
    {
      guardUniform.Name = "Guard Uniform";
      guardUniform.Description = "Guard Uniform";
      guardUniform.Takeable = true;
      guardUniform.Useable = false;
      guardUniform.UseLocation = null;
      guardUniform.Locked = false;
      guardUniform.UseDescription = "You are now disguised as a guard.";
      guardUniform.EffectOnPlayer = PlayerStatus.playing;
      guardUniform.EffectOnOtherItem = null;
      guardUniform.InUse = false;
      return guardUniform;
    }

    public Item SetupWideBed(Item wideBed)
    {
      wideBed.Name = "Wide Bed";
      wideBed.Description = "Wide Bed";
      wideBed.Takeable = false;
      wideBed.Useable = true;
      wideBed.Locked = false;
      wideBed.UseDescription = "You climb into the bed and pretend to be asleep. A few minutes later several guards walk into the room. One approaches you to wake you... (GUARD) 'Hey Get Up! it's your turn for watch, Go relieve Shigeru in the Guard Room!' Quickly, you climb out of the bed.";
      wideBed.EffectOnPlayer = PlayerStatus.playing;
      wideBed.EffectOnOtherItem = null;
      wideBed.InUse = false;
      return wideBed;
    }

    public Item SetupNarrowBed(Item narrowBed)
    {
      narrowBed.Name = "Narrow Bed";
      narrowBed.Description = "Narrow Bed";
      narrowBed.Takeable = false;
      narrowBed.Useable = true;
      narrowBed.Locked = false;
      narrowBed.UseDescription = "(GUARD) 'What do you think you're doing? Hey, you're not Leroy! Quick, Jenkins, sieze him...' Jenkins, a bit over-zelous, swings his sword, cleaving you in half.";
      narrowBed.EffectOnPlayer = PlayerStatus.lost;
      narrowBed.EffectOnOtherItem = null;
      narrowBed.InUse = false;
      return narrowBed;
    }

    public Item SetupThroneRoomKey(Item throneRoomKey)
    {
      throneRoomKey.Name = "Throne Room Key";
      throneRoomKey.Description = "Throne Room Key";
      throneRoomKey.Takeable = true;
      throneRoomKey.Useable = true;
      throneRoomKey.Locked = true; // Unlocked when use dungeon lock
      throneRoomKey.UseDescription = "You approach the door and slide the throneRoomKey into the lock. It clicks, The door can now be opened.";
      throneRoomKey.EffectOnPlayer = PlayerStatus.playing;
      throneRoomKey.InUse = false;
      return throneRoomKey;
    }

    public Item SetupNote(Item note)
    {
      note.Name = "Note";
      note.Description = "Note";
      note.Takeable = true;
      note.Useable = true;
      note.Locked = true; // Unlocked when use dungeon lock
      note.UseDescription = "Shouting at the messenger, you kick his bed and demand he take your note to the gate captain. The messenger quickly shakes off his sleep runs from the room with your note.";
      note.EffectOnPlayer = PlayerStatus.playing;
      note.EffectOnOtherItem = null;
      note.InUse = false;
      return note;
    }

    public Item SetupVial(Item vial)
    {
      vial.Name = "Vial";
      vial.Description = "Vial";
      vial.Takeable = true;
      vial.Useable = true;
      vial.Locked = true; // Unlocked when use dungeon lock
      vial.UseDescription = "You realize the green vial you stole from the captain's quarters is a deadly poisen. You drop some of its contents into each of the feast cups. Within minutes, you now know, the evil ruler and his henchmen will all die. Silently, you sneak from the room and escape Castle Grimtol.";
      vial.EffectOnVicConds = VictoryConditions.rulerKilled;
      vial.EffectOnPlayer = PlayerStatus.won;
      vial.EffectOnOtherItem = null;
      vial.InUse = false;
      return vial;
    }

    public Item SetupHammer(Item hammer)
    {
      hammer.Name = "Hammer";
      hammer.Description = "Hammer";
      hammer.Takeable = true;
      hammer.Useable = true;
      hammer.Locked = false;
      hammer.UseDescription = "At a blow from your heavy hammer, the old dungeon lock cracks apart. You are able to wrench open the cell door and free the old village man imprisoned within. (PRISONER) 'You have freed me! But how will I escape without a disguise?'";
      hammer.EffectOnPlayer = PlayerStatus.playing;
      hammer.InUse = false;
      return hammer;
    }

    public Item SetupDungeonLock(Item dungeonLock)
    {
      dungeonLock.Name = "Dungeon Lock";
      dungeonLock.Description = "Dungeon Lock";
      dungeonLock.Takeable = true;
      dungeonLock.Useable = true;
      dungeonLock.Locked = true; // Unlocked by using hammer (to break it)
      dungeonLock.UseDescription = "(CAPTAIN) 'That broken lock can only mean that a prisoner has escaped! Quick, take this! [He slams a silver key down on the table and writes a note]. Go fetch a messenger boy for me and have him take this note to Captain Ezio. I'll go rouse the guards in the barracks!' He rushes from the room, leaving behind the note and key. You also notice a green vial partly concealed by one of the maps.";
      dungeonLock.EffectOnPlayer = PlayerStatus.playing;
      dungeonLock.EffectOnOtherItem = null;
      dungeonLock.InUse = false;
      return dungeonLock;
    }

    public Item SetupMessengerOvercoat(Item messengerOvercoat)
    {
      messengerOvercoat.Name = "Messenger Overcoat";
      messengerOvercoat.Description = "Messenger Overcoat";
      messengerOvercoat.Takeable = true;
      messengerOvercoat.Useable = true;
      messengerOvercoat.Locked = false;
      messengerOvercoat.UseDescription = "The freed prisoner lurks near the door of his open cell. You give him the overcoat stolen from the messenger, and the old man puts them on. (FREED PRISONER) 'With this disguise I'll be able to walk out of this place easily. Thank you!' Together, you and the old man sneak out of the dungeon and escape Castle Grimtol.";
      messengerOvercoat.EffectOnVicConds = VictoryConditions.prisonerFreed;
      messengerOvercoat.EffectOnPlayer = PlayerStatus.won;
      messengerOvercoat.EffectOnOtherItem = null;
      messengerOvercoat.InUse = false;
      return messengerOvercoat;
    }

    public Item SetupThroneRoomDoor(Item throneRoomDoor)
    {
      throneRoomDoor.Name = "Throne Room Door";
      throneRoomDoor.Description = "Throne Room Door";
      throneRoomDoor.Takeable = false;
      throneRoomDoor.Useable = true;
      throneRoomDoor.Locked = true; // Unlocked when the throneRoomKey from captains quarters is used
      throneRoomDoor.UseDescription = "You enter the Throne Room.";
      throneRoomDoor.EffectOnPlayer = PlayerStatus.playing;
      throneRoomDoor.EffectOnOtherItem = null;
      throneRoomDoor.InUse = false;
      return throneRoomDoor;
    }

    public Event SetupCourtyardEvt1(Event courtyardEvt1)
    {
      courtyardEvt1.Description = "A nearby guard sees you enter the courtyard. Fooled by your disguise, he greets you, thinking you are a fellow soldier: 'Oi, long night tonight! I wish I was in my bed. If you're just getting on shift, you should go talk to the captain.'";
      courtyardEvt1.RequiredItemName = "Guard Uniform";
      courtyardEvt1.ForbiddenItemName = null;
      courtyardEvt1.Effect = PlayerStatus.playing;
      return courtyardEvt1;
    }

    public Event SetupCourtyardEvt2(Event courtyardEvt2)
    {
      courtyardEvt2.Description = "One of the patrolling guards approaches. (GUARD) 'Wat? Who the blazes are you?' he shouts. He raises the alarm and several bowmen turn to fire on you. As an arrow slams into your body, you realize you have made a grave mistake.";
      courtyardEvt2.RequiredItemName = null;
      courtyardEvt2.ForbiddenItemName = "Guard Uniform";
      courtyardEvt2.Effect = PlayerStatus.lost;
      return courtyardEvt2;
    }

    public Event SetupGuardRoomEvt1(Event guardRoomEvt1)
    {
      if (CurrentPlayer.Inventory.Find(item => item.Name == "Hammer") == null)
      {
        guardRoomEvt1.Description = "You see a few tools lying in a corner of the room, including a large hammer.";
      }
      else
      {
        guardRoomEvt1.Description = "";
      }
      guardRoomEvt1.RequiredItemName = null;
      guardRoomEvt1.ForbiddenItemName = null;
      guardRoomEvt1.Effect = PlayerStatus.playing;
      return guardRoomEvt1;
    }

    public Event SetupCaptQuartEvt1(Event captQuartEvt1)
    {
      captQuartEvt1.Description = "The captain on shift greets you. (CAPTAIN) 'New recruit, huh? Well, lets stick you in the guard room. You can't screw things up there. Go relieve [He pauses to glance at his reports] private Miyamoto.'";
      captQuartEvt1.RequiredItemName = null;
      captQuartEvt1.ForbiddenItemName = null;
      captQuartEvt1.Effect = PlayerStatus.playing;
      return captQuartEvt1;
    }

    public Event SetupThroneRoomEvt1(Event throneRoomEvt1)
    {
      throneRoomEvt1.Description = "As you unlock the door and swing it wide you see an enormous hall stretching out before you. At the opposite end of the hall sitting on his throne you see the dark lord. The Dark Lord shouts at you demanding why you dared to interrupt him during his Ritual of Evil Summoning... Dumbfounded you mutter an incoherent response. Becoming more enraged the Dark Lord complains that you just ruined his concentration and he will now have to start the ritual over... Quickly striding towards you he smirks at least I know have a sacrificial volunteer. Plunging his jewel encrusted dagger into your heart your world slowly fades away.";
      throneRoomEvt1.RequiredItemName = null;
      throneRoomEvt1.ForbiddenItemName = null;
      throneRoomEvt1.Effect = PlayerStatus.lost;
      return throneRoomEvt1;
    }

  }
}
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

      // IN PROGRESS -- YOU SHOULD NOT BE ABLE TO USE THE HAMMER IN GUARD ROOM, AND YOU SHOULD NOT BE ABLE TO USE IT AT ALL UNTIL IT HAS 1ST BEEN TAKEN
      // DEBUG --
      // Console.WriteLine($"    {itemName} is foundItem: {foundItem != null}");
      // Console.WriteLine($"    {itemName} is Useable: {foundItem.Useable}");
      // Console.WriteLine($"    {itemName} is Takeable: {foundItem.Takeable}");
      // Console.WriteLine($"    {itemName} is Locked: {foundItem.Locked}");
      // string s = foundItem.UseLocation != null ? foundItem.UseLocation.Name : null;
      // Console.WriteLine($"    {itemName} UseLocation: {s}");
      // Console.WriteLine($"    {itemName} is inventoryItem: {inventoryItem != null}");
      // Console.WriteLine($"    {itemName} is Useable: {inventoryItem.Useable}");
      // Console.WriteLine($"    {itemName} is Takeable: {inventoryItem.Takeable}");
      // Console.WriteLine($"    {itemName} is Locked: {inventoryItem.Locked}");
      // string s = inventoryItem.UseLocation != null ? inventoryItem.UseLocation.Name : null;
      // Console.WriteLine($"    {itemName} UseLocation: {inventoryItem.UseLocation.Name ?? null}");

      // To use an item found in a room (without taking it first), it must not be takeable, it must be useable in the current room, and it can't have 'locked' = TRUE
      if (foundItem != null && foundItem.Useable)
      {
        if (!foundItem.Takeable && foundItem.Locked == false
            && (foundItem.UseLocation == null || foundItem.UseLocation == this.CurrentRoom))
        {
          Console.WriteLine(foundItem.UseDescription);
          this.CurrentPlayer.Status = foundItem.EffectOnPlayer;
          foundItem.InUse = true;
          if (foundItem.EffectOnOtherItem != null) // Account for this item's effect on another item (if any)
          {
            foreach (KeyValuePair<Item, bool> item in foundItem.EffectOnOtherItem)
            {
              item.Key.Locked = item.Value;
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
        if ((inventoryItem.UseLocation == null || inventoryItem.UseLocation == this.CurrentRoom) && inventoryItem.Locked == false)
        {
          Console.WriteLine(inventoryItem.UseDescription);
          this.CurrentPlayer.Status = inventoryItem.EffectOnPlayer;
          inventoryItem.InUse = true;
          if (inventoryItem.EffectOnOtherItem != null) // Account for this item's effect on another item (if any)
          {
            foreach (KeyValuePair<Item, bool> item in inventoryItem.EffectOnOtherItem)
            {
              item.Key.Locked = item.Value;
            }
          }
        }
        else
        {
          Console.WriteLine("Sorry, you cannot 'use' this item yet.");
        }
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
        // If the item must be taken to be used:
        if (item.Takeable && CurrentPlayer.Inventory.Find(itm => itm.Name == item.Name) == null) // (Don't include items that have already been 'taken')
        {
          options += "'take " + item.Name + "'   ";
        }
        // If the item does not need to be taken to be used:
        if (item.Useable)
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
      // DEBUG --
      // Console.WriteLine($"    this.CurrentRoom.Name: {this.CurrentRoom.Name}");

      if (this.CurrentRoom.Events != null && this.CurrentRoom.Events.Count > 0)
      {
        foreach (Event evt in this.CurrentRoom.Events)
        {
          if (evt.ShouldFire(this.CurrentPlayer))
          {
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
            Room nextRoom = this.CurrentRoom.Exits[direction];
            if (!nextRoom.Door.Locked)
            {
              validOption = true;
              this.CurrentRoom = this.CurrentRoom.Exits[direction];
              Console.WriteLine($"\n[[ You have entered the {CurrentRoom.Name}. Type 'look', 'inventory', or 'help' for more options. ]]\n");
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

        if (!CurrentPlayer.VisitedRooms.Contains(CurrentRoom))
        {
          CurrentPlayer.VisitedRooms.Add(CurrentRoom);
        }
        else
        {
          if (this.CurrentRoom.Events != null && this.CurrentRoom.Events.Count > 0)
          {
            foreach (Event evt in CurrentRoom.Events)
            {
              evt.Deactivated = true;
            }
          }
        }

      }
    }

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
      Event captQuartEvt1 = new Event();
      // Event captQuartEvt2 = new Event();
      Event dungeonEvt1 = new Event();
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
      window = SetupWindow(window);
      window.UseLocation = warRoom; // Used to escape at end of game
      throneRoomDoor = SetupThroneRoomDoor(throneRoomDoor);
      throneRoomDoor.UseLocation = castleCourtyard; // Used to enter throne room

      // EVENT DEFINITIONS
      courtyardEvt1 = SetupCourtyardEvt1(courtyardEvt1);
      courtyardEvt2 = SetupCourtyardEvt2(courtyardEvt2);
      captQuartEvt1 = SetupCaptQuartEvt1(captQuartEvt1);
      // captQuartEvt2 = SetupCaptQuartEvt2(captQuartEvt2);
      dungeonEvt1 = SetupDungeonEvt1(dungeonEvt1);
      throneRoomEvt1 = SetupThroneRoomEvt1(throneRoomEvt1);

      // ROOM DEFINITIONS
      hallway = SetupHallway(hallway);
      hallway.Exits = new Dictionary<string, Room>() { { "north", barracks }, { "east", castleCourtyard } };

      barracks = SetupBarracks(barracks);
      barracks.Items = new List<Item>() { guardUniform, wideBed, narrowBed };
      barracks.Exits = new Dictionary<string, Room>() { { "south", hallway } };

      captainsQuarters = SetupCaptainsQuarters(captainsQuarters);
      captainsQuarters.Items = new List<Item>() { throneRoomKey, note, vial };
      captainsQuarters.Events = new List<Event>() { captQuartEvt1 };
      // captainsQuarters.Events = new List<Event>() { captQuartEvt1, captQuartEvt2 };
      captainsQuarters.Exits = new Dictionary<string, Room>() { { "east, then north", castleCourtyard }, { "east", guardRoom } };

      castleCourtyard = SetupCastleCourtyard(castleCourtyard);
      castleCourtyard.Items = new List<Item>() { throneRoomDoor };
      castleCourtyard.Events = new List<Event>() { courtyardEvt1, courtyardEvt2 };
      castleCourtyard.Exits = new Dictionary<string, Room>() { { "north", throneRoom }, { "north, then east", squireTower }, { "west", hallway }, { "south, then west", captainsQuarters }, { "south, then east", guardRoom } };

      guardRoom = SetupGuardRoom(guardRoom);
      guardRoom.Items = new List<Item>() { hammer };
      guardRoom.Exits = new Dictionary<string, Room>() { { "west", captainsQuarters }, { "west, then north", castleCourtyard }, { "north", dungeon } };

      dungeon = SetupDungeon(dungeon);
      dungeon.Items = new List<Item>() { dungeonLock };
      dungeon.Events = new List<Event>() { dungeonEvt1 };
      dungeon.Exits = new Dictionary<string, Room>() { { "south", guardRoom } };

      squireTower = SetupSquireTower(squireTower);
      squireTower.Items = new List<Item>() { messengerOvercoat };
      squireTower.Exits = new Dictionary<string, Room>() { { "west, then south", castleCourtyard }, { "west, then north", throneRoom }, { "north", warRoom } };

      warRoom = SetupWarRoom(warRoom);
      warRoom.Items = new List<Item>() { window };
      warRoom.Exits = new Dictionary<string, Room>() { { "south", squireTower } };

      throneRoom = SetupThroneRoom(throneRoom);
      throneRoom.Door = throneRoomDoor;
      throneRoom.Door.Locked = true;
      throneRoom.Items = new List<Item>() { throneRoom.Door };
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
      hallway.Description = "You find yourself in a small hall there doesnt appear to be anything of interest here. You see a passages to the north and south, and a door to the west.";
      hallway.Items = new List<Item>();
      hallway.Door = new Item();
      return hallway;
    }

    public Room SetupBarracks(Room barracks)
    {
      barracks.Name = "Barracks";
      barracks.Description = "You see a room with several sleeping guards. The room smells of sweaty men. Near you are a wide bed and a narrow bed; both appear empty. There are several uniforms tossed about.";
      barracks.Door = new Item();
      return barracks;
    }

    public Room SetupCaptainsQuarters(Room captainsQuarters)
    {
      captainsQuarters.Name = "Captain's Quarters";
      captainsQuarters.Description = "As you approach the captains Quarters you swallow hard and notice your lips are dry, Stepping into the room you see a few small tables and maps of the countryside sprawled out.";
      captainsQuarters.Door = new Item();
      return captainsQuarters;
    }

    public Room SetupCastleCourtyard(Room castleCourtyard)
    {
      castleCourtyard.Name = "Castle Courtyard";
      castleCourtyard.Description = "You step into the large castle courtyard there is a flowing fountain in the middle of the grounds and a few guards patrolling the area.";
      castleCourtyard.Door = new Item();
      return castleCourtyard;
    }

    public Room SetupGuardRoom(Room guardRoom)
    {
      guardRoom.Name = "Guard Room";
      guardRoom.Description = "Pushing open the door of the guard room you look around and notice the room is empty, There are a few small tools in the corner and a chair propped against the wall near the that likely leads to the dungeon.";
      guardRoom.Door = new Item();
      return guardRoom;
    }

    public Room SetupDungeon(Room dungeon)
    {
      dungeon.Name = "Dungeon";
      dungeon.Description = "As you descend the stairs to the dungeon you notice a harsh chill to the air. Landing a the base of the stairs you see what the remains of a previous prisoner.";
      dungeon.Door = new Item();
      return dungeon;
    }

    public Room SetupSquireTower(Room squireTower)
    {
      squireTower.Name = "Squire Tower";
      squireTower.Description = "As you finish climbing the stairs to the squire tower you see a messenger nestled in his bed. His messenger overcoat is hanging from his bed post.";
      squireTower.Door = new Item();
      return squireTower;
    }

    public Room SetupWarRoom(Room warRoom)
    {
      warRoom.Name = "War Room";
      warRoom.Description = "Steping into the war room you see several maps spread across tables. On the maps many of the villages have been marked for purification. You also notice several dishes of prepared food to the side perhaps the war council will be meeting soon.";
      warRoom.Door = new Item();
      return warRoom;
    }

    public Room SetupThroneRoom(Room throneRoom)
    {
      throneRoom.Name = "Throne Room";
      throneRoom.Description = "You see a dark throne room.";
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
      narrowBed.UseDescription = "(GUARD) 'What do you think you're doing? Hey you're not Leroy! Quick, Jenkins, sieze him...' Jenkins, a bit over-zelous, swings his sword, cleaving you in half...";
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
      throneRoomKey.Locked = true; // Unlocked when use broken lock
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
      note.Locked = true; // Unlocked when use broken lock
      note.UseDescription = "Shouting at the messenger you kick his bed and demand he take your note to the gate captain. The messenger quickly shakes off his sleep runs from the room with your note.";
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
      vial.Locked = true; // Unlocked when use broken lock
      vial.UseDescription = "As you are staring around the room you realize the vial is likely the deadly poisen that the guards have been putting on their arrowheads. Looking for the most ornate cups you drain the vial into the cup then toss the vial out the window.";
      vial.EffectOnPlayer = PlayerStatus.playing;
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
      hammer.UseDescription = "Quickly you explain your plan to the man and a new light of hope dances across his face. (PRISONER) 'This is wonderful news but how am I going to get out of here?'";
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
      dungeonLock.UseDescription = "What an escaped prisoner... When did this happen? Quick take this (He slams a silver throneRoomKey on the table and jots down a quick note) go fetch a messenger boy for me and have him take this note the Gate Captain Ezio, but keep this quite. If That prisoner is really has escaped it will be both our heads. (CAPTAIN) I'll go rouse the guards, (The captain runs to the door north heading for the Barracks)";
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
      messengerOvercoat.UseDescription = "You hand off the clothes and the old man puts them on. With this disguise I'll be able to walk out of this place tomorrow. Thank you.";
      messengerOvercoat.EffectOnPlayer = PlayerStatus.playing;
      messengerOvercoat.EffectOnOtherItem = null;
      messengerOvercoat.InUse = false;
      return messengerOvercoat;
    }

    public Item SetupWindow(Item window)
    {
      window.Name = "Window";
      window.Description = "Window";
      window.Takeable = false;
      window.Useable = true;
      window.Locked = true; // Unlocked when the vial is used to poison cups in the war room
      window.UseDescription = "You escape out the window. You win.";
      window.EffectOnPlayer = PlayerStatus.won;
      window.EffectOnOtherItem = null;
      window.InUse = false;
      return window;
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
      courtyardEvt1.Description = "Oi, long night tonight I wish I was in my bed. If your just getting on shift your should go talk to the captain.";
      courtyardEvt1.RequiredItemName = "Guard Uniform";
      courtyardEvt1.ForbiddenItemName = null;
      courtyardEvt1.Effect = PlayerStatus.playing;
      return courtyardEvt1;
    }

    public Event SetupCourtyardEvt2(Event courtyardEvt2)
    {
      courtyardEvt2.Description = "To your left you see a guard approaching you. (GUARD) 'Wat? Who the blazes are you?' Quickly he raises the alarm and several of the crossbow men turn and fire on you. You realize you have made a grave mistake as a bolt slams into your body...";
      courtyardEvt2.RequiredItemName = null;
      courtyardEvt2.ForbiddenItemName = "Guard Uniform";
      courtyardEvt2.Effect = PlayerStatus.lost;
      return courtyardEvt2;
    }

    public Event SetupCaptQuartEvt1(Event captQuartEvt1)
    {
      captQuartEvt1.Description = "The captain on shift greets you (CAPTAIN) 'New recruit huh. Well lets stick you in the guard room you can't screw things up there. Go relieve (He pauses and glancing at his reports) 'private Miyamoto.";
      captQuartEvt1.RequiredItemName = null;
      captQuartEvt1.ForbiddenItemName = null;
      captQuartEvt1.Effect = PlayerStatus.playing;
      return captQuartEvt1;
    }

    // public Event SetupCaptQuartEvt2(Event captQuartEvt2)
    // {
    //   captQuartEvt2.Description = "(CAPTAIN) What are you doing here? Go stay in the Guard Room.";
    //   captQuartEvt2.RequiredItemName = null;
    //   captQuartEvt2.ForbiddenItemName = "Dungeon Lock";
    //   captQuartEvt2.Effect = PlayerStatus.playing;
    //   return captQuartEvt2;
    // }

    public Event SetupDungeonEvt1(Event dungeonEvt1)
    {
      dungeonEvt1.Description = "You also notice a man sitting in shackles. As you approach him you notice a small lock binding him to the wall with chains. As you near the prisoner his face turns to a deep frown.... (PRISONER) You look familiar, Hey thats right I know you from the village. Have you seriously turned your back on us and joined this squad of goons, He sighs defeated...";
      dungeonEvt1.RequiredItemName = null;
      dungeonEvt1.ForbiddenItemName = null;
      dungeonEvt1.Effect = PlayerStatus.playing;
      return dungeonEvt1;
    }

    public Event SetupThroneRoomEvt1(Event throneRoomEvt1)
    {
      throneRoomEvt1.Description = "As you unlock the door and swing it wide you see an enormous hall stretching out before you. At the opposite end of the hall sitting on his throne you see the dark lord. The Dark Lord shouts at you demanding why you dared to interrupt him during his Ritual of Evil Summoning... Dumbfounded you mutter an incoherent response. Becoming more enraged the Dark Lord complains that you just ruined his concentration and he will now have to start the ritual over... Quickly striding towards you he smirks at least I know have a sacrificial volunteer. Plunging his jewel encrusted dagger into your heart your world slowly fades away. You have died. Sorry.";
      throneRoomEvt1.RequiredItemName = null;
      throneRoomEvt1.ForbiddenItemName = null;
      throneRoomEvt1.Effect = PlayerStatus.lost;
      return throneRoomEvt1;
    }

  }
}
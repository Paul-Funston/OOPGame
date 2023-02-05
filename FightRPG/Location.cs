﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FightRPG
{
    public class Location : GameObject
    {

        //protected Dictionary<string, Action> _actionsAvailable = new();
        //public Dictionary<string, Action> GetActionsAvailable { get { return new Dictionary<string, Action>(_actionsAvailable); } }

        protected HashSet<Location> _locationsAvailable = new();
        private Location[] _locationsAvailableArray { get { return _locationsAvailable.ToArray(); } }

        private Location? _previousLocation = null;
        public Location? PreviousLocation { get { return _previousLocation; } }

        public int AddConnection(Location l)
        {
            _locationsAvailable.Add(l);
            return _locationsAvailable.Count();
        }

        /* MOVED UP TO GameObject
        protected string[] ActionsAsStrings()
        {
            return _actionsAvailable.Keys.ToArray();
        }

        public void ChooseAction()
        {
            string choice = Game.PlayerChoosesString(ActionsAsStrings());
            _actionsAvailable[choice].Invoke();
        }
        */

        protected void Travel()
        {
            if (_locationsAvailable.Count == 0)
            {
                if (_previousLocation != null)
                {

                    Game.MoveLocation(_previousLocation, this);
                    _previousLocation = null;
                } else
                {
                    throw new Exception("No locations found to travel to.");
                }

            } else if(_locationsAvailable.Count == 1) 
            {
                Game.MoveLocation(_locationsAvailable.First(), this);
            } else
            {
                Location choice = Game.PlayerChooseLocation(_locationsAvailableArray);
                Game.MoveLocation(choice, this);

            }
        }

        public Location TravelTo(Location travelingFrom)
        {
            _previousLocation = travelingFrom;
            return this;
        }


        /*
        public int ShowTravelOptions()
        {
            if (_locationsAvailable.Count > 8) { throw new Exception("Too many options."); }

            for (int i = 1; i <= _locationsAvailable.Count(); i++ )
            {
                Console.WriteLine($"{i}: {_locationsAvailableArray[i - 1].Name}");
            }

            Console.WriteLine($"0: Return to last location");
            return _locationsAvailable.Count();
        }
 
        public Location GetLocationAtOption(int option)
        {
            return _locationsAvailableArray[option - 1];
        }



        public void DoLocationAction()
        {
            // shops purchase items
            // dungeon get into fight
            // 
        }

        public void OpenMenu()
        {
            if (this.Name != "Menu")
            {
                Assets.Menu.TravelHere(this);

            }

        }
        */

        public Location(string name) : base(name)
        {


            _actionsAvailable.Add("Travel", Travel);
            _actionsAvailable.Add("View Stats", Game.DisplayStats );
            _actionsAvailable.Add("Inventory", Game.OpenInventory );
            Assets.AddLocation(Id, this);
        }



        public class Shop : Location
        {
            private HashSet<Item> _inventory = new();
            private Item[] InventoryArray { get { return _inventory.ToArray(); } }
            private string[] GetItemNames()
            {
                List<string> itemNames = new List<string>();
                foreach (Item i in _inventory)
                {
                    itemNames.Add(i.Name);
                }

                return itemNames.ToArray();
            }
            public int AddItem(Item item)
            {
                _inventory.Add(item);
                return _inventory.Count();
            }

            private void Purchase()
            {
                Console.WriteLine("Coming Soon.");
            }

            private void SellItems()
            {
                Console.WriteLine("Coming Soon.");
            }

            
            public Shop(string name) : base(name)
            {
                _actionsAvailable.Add("Buy", Purchase);
                _actionsAvailable.Add("Sell", SellItems);
                _actionsAvailable.Remove("Travel");
                _actionsAvailable.Add("Leave", Travel);
                Assets.AddShop(Id, this);
            }
        }

        public class Dungeon : Location
        {
            private int _fightsHere = 0;
            private Dictionary<Action, int> _inhabitants = new();
            private List<int> _newEnemyIds = new();

   
            private int GetNumberOfEnemies()
            {
                int n = new Random().Next(10);
                switch (n)
                {
                    case < 2:
                        n = 1;
                        break;

                    case < 6:
                        n = 2;
                        break;
                    case < 9:
                        n = 3;
                        break;
                    default:
                        n = 4;
                        break;
                }
                return n;
            }
            private void GetFight()
            {
                _fightsHere++;

                // Logic if boss fight or make a random encounter

                Fight newFight = MakeRandomEncounter();


                StartFight(newFight);
                
            }

            private Fight MakeRandomEncounter()
            {
                int n = GetNumberOfEnemies();

                Action[] _spawners = _inhabitants.Keys.ToArray();
                for (int i = 0; i < n; i++)
                {
                    int randomNum = new Random().Next() % _spawners.Length;
                    if (--_inhabitants[_spawners[randomNum]] >= 0)
                    {
                        _spawners[randomNum].Invoke();
                    }
                }

                if (_newEnemyIds.Count == 0)
                {
                    _newEnemyIds.Add(new Monster("Treasure Goblin", 1, 1, 1, 1, 9999).Id);
                    Console.WriteLine("Wow a rare enemy!");
                }

                Fight newFight = CreateFight(Game.GetActiveParty(), _newEnemyIds.ToArray());
                _newEnemyIds = new();

                return newFight;
            }

            private void StartFight(Fight fight)
            {
                // Special Travel option
                Game.MoveLocation(fight, this);
            }
            private Fight CreateFight(int[] partyIds, int[] enemyIds)
            {
                return new Fight(partyIds, enemyIds);

            }

            private void Goblin()
            {
                int id = Assets.CreateGoblin();
                _newEnemyIds.Add(id);
            }
            private void Bear()
            {
                int id = Assets.CreateBear();
                _newEnemyIds.Add(id);
            }
            private void Snake()
            {
                int id = Assets.CreateSnake();
                _newEnemyIds.Add(id);
            }
            private void Wolf()
            {
                int id = Assets.CreateWolf();
                _newEnemyIds.Add(id);
            }
            private void Treant()
            {
                int id = Assets.CreateTreant();
                _newEnemyIds.Add(id);
            }

            public Dungeon(string name) : base(name)
            {
                _actionsAvailable.Add("Fight", GetFight);
                _inhabitants.Add(Goblin, 99);
                _inhabitants.Add(Bear, 5);
                _inhabitants.Add(Snake, 20);
                _inhabitants.Add(Wolf, 20);
                _inhabitants.Add(Treant, 5);

                Assets.AddDungeon(Id, this);
            }
        }

    }
}

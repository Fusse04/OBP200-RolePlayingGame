using System.Text;

namespace OBP200_RolePlayingGame;


class Program
{
    // ======= Globalt tillstånd  =======

    // Spelarens "databas": alla värden som strängar
    // index: 0 Name, 1 Class, 2 HP, 3 MaxHP, 4 ATK, 5 DEF, 6 GOLD, 7 XP, 8 LEVEL, 9 POTIONS, 10 INVENTORY (semicolon-sep)
    static Player player;

    // Rum: [type, label]
    // types: battle, treasure, shop, rest, boss
    static List<string[]> Rooms = new List<string[]>();

    // Fiendemallar: [type, name, HP, ATK, DEF, XPReward, GoldReward]
    private static List<Enemy> EnemyTemplates = new List<Enemy>();

    // Status för kartan
    static int CurrentRoomIndex = 0;

    // Random
    static Random Rng = new Random();

    // ======= Main =======

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        InitEnemyTemplates();

        while (true)
        {
            ShowMainMenu();
            Console.Write("Välj: ");
            var choice = (Console.ReadLine() ?? "").Trim();

            if (choice == "1")
            {
                StartNewGame();
                RunGameLoop();
            }
            else if (choice == "2")
            {
                Console.WriteLine("Avslutar...");
                return;
            }
            else
            {
                Console.WriteLine("Ogiltigt val.");
            }

            Console.WriteLine();
        }
    }

    // ======= Meny & Init =======

    static void ShowMainMenu()
    {
        Console.WriteLine("=== Text-RPG ===");
        Console.WriteLine("1. Nytt spel");
        Console.WriteLine("2. Avsluta");
    }

    static void StartNewGame()
    {
        Console.Write("Ange namn: ");
        var name = (Console.ReadLine() ?? "").Trim();
        if (string.IsNullOrWhiteSpace(name)) name = "Namnlös";

        Console.WriteLine("Välj klass: 1) Warrior  2) Mage  3) Rogue");
        Console.Write("Val: ");
        var k = (Console.ReadLine() ?? "").Trim();

        string cls = "Warrior";
        int hp = 0, maxhp = 0, atk = 0, def = 0;
        int potions = 0, gold = 0;
        int xp = 0;
        int lvl = 1;
        
        switch (k)
        {
            case "1": // Warrior: tankig
                cls = "Warrior";
                maxhp = 40; hp = 40; atk = 7; def = 5; potions = 2; gold = 15; 
                player = new Warrior(name, hp, maxhp, atk, def, cls, gold, xp, lvl, potions);
                break;
            case "2": // Mage: hög damage, låg def
                cls = "Mage";
                maxhp = 28; hp = 28; atk = 10; def = 2; potions = 2; gold = 15;
                player = new Mage(name, hp, maxhp, atk, def, cls, gold, xp, lvl, potions);
                break;
            case "3": // Rogue: krit-chans
                cls = "Rogue";
                maxhp = 32; hp = 32; atk = 8; def = 3; potions = 3; gold = 20;
                player = new Rogue(name, hp, maxhp, atk, def, cls, gold, xp, lvl, potions);
                break;
            default:
                cls = "Warrior";
                maxhp = 40; hp = 40; atk = 7; def = 5; potions = 2; gold = 15;
                player = new Warrior(name, hp, maxhp, atk, def, cls, gold, xp, lvl, potions);
                break;
        }

   
        // Initiera karta (linjärt äventyr)
        Rooms.Clear();
        Rooms.Add(new[] { "battle", "Skogsstig" });
        Rooms.Add(new[] { "treasure", "Gammal kista" });
        Rooms.Add(new[] { "shop", "Vandrande köpman" });
        Rooms.Add(new[] { "battle", "Grottans mynning" });
        Rooms.Add(new[] { "rest", "Lägereld" });
        Rooms.Add(new[] { "battle", "Grottans djup" });
        Rooms.Add(new[] { "boss", "Urdraken" });

        CurrentRoomIndex = 0;

        Console.WriteLine($"Välkommen, {name} the {cls}!");
        player.ShowStatus();
    }

    static void RunGameLoop()
    {
        while (true)
        {
            var room = Rooms[CurrentRoomIndex];
            Console.WriteLine($"--- Rum {CurrentRoomIndex + 1}/{Rooms.Count}: {room[1]} ({room[0]}) ---");

            bool continueAdventure = EnterRoom(room[0]);
            
            if (!player.IsAlive())
            {
                Console.WriteLine("Du har stupat... Spelet över.");
                break;
            }
            
            if (!continueAdventure)
            {
                Console.WriteLine("Du lämnar äventyret för nu.");
                break;
            }

            CurrentRoomIndex++;
            
            if (CurrentRoomIndex >= Rooms.Count)
            {
                Console.WriteLine();
                Console.WriteLine("Du har klarat äventyret!");
                break;
            }
            
            Console.WriteLine();
            Console.WriteLine("[C] Fortsätt     [Q] Avsluta till huvudmeny");
            Console.Write("Val: ");
            var post = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();

            if (post == "Q")
            {
                Console.WriteLine("Tillbaka till huvudmenyn.");
                break;
            }

            Console.WriteLine();
        }
    }

    // ======= Rumshantering =======

    static bool EnterRoom(string type)
    {
        switch ((type ?? "battle").Trim())
        {
            case "battle":
                return DoBattle(isBoss: false);
            case "boss":
                return DoBattle(isBoss: true);
            case "treasure":
                return DoTreasure();
            case "shop":
                return DoShop();
            case "rest":
                return DoRest();
            default:
                Console.WriteLine("Du vandrar vidare...");
                return true;
        }
    }

    // ======= Strid =======

    static bool DoBattle(bool isBoss)
    {
        Enemy enemy = GenerateEnemy(isBoss);
        enemy.Appered();



        while (enemy.Hp > 0 && player.IsAlive())
        {
            Console.WriteLine();
            player.ShowStatus();
            Console.WriteLine($"Fiende: {enemy.Name} HP={enemy.Hp}");
            Console.WriteLine("[A] Attack   [X] Special   [P] Dryck   [R] Fly");
            if (isBoss) Console.WriteLine("(Du kan inte fly från en boss!)");
            Console.Write("Val: ");

            var cmd = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();

            if (cmd == "A")
            {
                int damage = player.CalculateDamage(enemy.Defense, Rng);
                enemy.TakeDamage(damage);
                Console.WriteLine($"Du slog {enemy.Name} för {damage} skada.");
            }
            else if (cmd == "X")
            {
                int special = player.UseClassSpecial(enemy.Defense, isBoss, Rng);
                enemy.TakeDamage(special);
                Console.WriteLine($"Special! {enemy.Name} tar {special} skada.");
            }
            else if (cmd == "P")
            {
                player.UsePotion();
            }
            else if (cmd == "R" && !isBoss)
            {
                if (player.TryRunAway(Rng))
                {
                    Console.WriteLine("Du flydde!");
                    return true; // fortsätt äventyr
                }
                else
                {
                    Console.WriteLine("Misslyckad flykt!");
                }
            }
            else
            {
                Console.WriteLine("Du tvekar...");
            }

            if (enemy.Hp <= 0)
            {
                enemy.ResetHp();
                break;
            }

            // Fiendens tur
            int enemyDamage = enemy.CalculateDamage(player.Defense,Rng);
            player.TakeDamage(enemyDamage);
            Console.WriteLine($"{enemy.Name} anfaller och gör {enemyDamage} skada!");
        }

        if (!player.IsAlive())
        {
            return false; // avsluta äventyr
        }

        // Vinstrapporter, XP, guld, loot
        player.AddXP(enemy.XPReward);
        player.AddGold(enemy.GoldReward);

        Console.WriteLine($"Seger! +{enemy.XPReward} XP, +{enemy.GoldReward} guld.");
        IInventoryAdd PlayerInventory = player;
        enemy.MaybeGivePlayerLoot(Rng, PlayerInventory);

        return true;
    }

    

        // Enemytemplate med all info som enemy behöver
    static void InitEnemyTemplates() 
    {
        EnemyTemplates.Clear();
        EnemyTemplates.Add(new Enemy (  "beast",  "Vildsvin",    18,   18,   4,   1,   6,   4, false ));
        EnemyTemplates.Add(new Enemy (  "undead",  "Skelett",    20,   20,   5,   2,   7,   5, false ));
        EnemyTemplates.Add(new Enemy (  "bandit",  "Bandit",     16,   16,   6,   1,   8,   6, false ));
        EnemyTemplates.Add(new Enemy (  "slime",   "Geléslem",   14,   14,   3,   0,   5,   3, false ));
    }

    // ======= Rumshändelser =======

    static bool DoTreasure()
    {
        Console.WriteLine("Du hittar en gammal kista...");
        if (Rng.NextDouble() < 0.5)
        {
            int gold = Rng.Next(8, 15);
            player.AddGold(gold);
            Console.WriteLine($"Kistan innehåller {gold} guld!");
        }
        else
        {
            var items = new[] { "Iron Dagger", "Oak Staff", "Leather Vest", "Healing Herb" };
            string found = items[Rng.Next(items.Length)];
            player.AddToInventory(found);
            Console.WriteLine($"Du plockar upp: {found}");
        }
        return true;
    }

    static bool DoShop()
    {
        Console.WriteLine("En vandrande köpman erbjuder sina varor:");
        while (true)
        {
            Console.WriteLine($"Guld: {player.Gold} | Drycker: {player.Potions}");
            Console.WriteLine("1) Köp dryck (10 guld)");
            Console.WriteLine("2) Köp vapen (+2 ATK) (25 guld)");
            Console.WriteLine("3) Köp rustning (+2 DEF) (25 guld)");
            Console.WriteLine("4) Sälj alla 'Minor Gem' (+5 guld/st)");
            Console.WriteLine("5) Lämna butiken");
            Console.Write("Val: ");
            var val = (Console.ReadLine() ?? "").Trim();

            if (val == "1")
            {
                player.TryBuy(10, ShopItem.Potion, "Du köper en dryck.");
            }
            else if (val == "2")
            {
                player.TryBuy(25, ShopItem.Weapon, "Du köper ett bättre vapen.");
            }
            else if (val == "3")
            {
                player.TryBuy(25, ShopItem.Armor, "Du köper bättre rustning.");
            }
            else if (val == "4")
            {
                SellMinorGems();
            }
            else if (val == "5")
            {
                Console.WriteLine("Du säger adjö till köpmannen.");
                break;
            }
            else
            {
                Console.WriteLine("Köpmannen förstår inte ditt val.");
            }
        }
        return true;
    }

    static void SellMinorGems()
    {
        if (player.Inventory.Count == 0)
        {
            Console.WriteLine("Du har inga föremål att sälja.");
            return;
        }

        int count = 0;
        foreach (var item in player.Inventory)
        {
            if (item == "Minor Gem")
            {
               count++; 
               player.Inventory.Remove(item);
            }
        }
        
        if (count == 0)
        {
            Console.WriteLine("Inga 'Minor Gem' i väskan.");
            return;
        }
        player.AddGold(count*5);
        Console.WriteLine($"Du säljer {count} st Minor Gem för {count * 5} guld.");
    }

    static bool DoRest()
    {
        Console.WriteLine("Du slår läger och vilar.");
        player.Rest();
        Console.WriteLine("HP återställt till max.");
        return true;
    }

    // ======= Status =======
    
    
    // ======= Hjälpmetoder =======

    static int ParseInt(string s, int fallback)
    {
        try
        {
            int value = Convert.ToInt32(s);
            return value;
        }
        catch (Exception e)
        {
            return fallback;
        }
    }
    public static Enemy GenerateEnemy(bool isBoss)
    {
        
        if (isBoss)
        {
            // Boss-mall
            return new Enemy (  "boss", "Urdraken", 55, 55, 9, 4, 30, 50, true);
        }
        
        var enemy = EnemyTemplates[Rng.Next(EnemyTemplates.Count)];
        enemy.RandomizeStats(Rng);
        return enemy;
    }
}

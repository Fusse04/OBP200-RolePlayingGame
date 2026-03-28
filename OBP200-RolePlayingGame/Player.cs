namespace OBP200_RolePlayingGame;

public abstract class Player : Character , IInventoryAdd, IAttack
{
    public string ClassType { get; private set; }
    public int Gold { get; protected set; }
    public int XP { get; private set; }
    public int Level { get; protected set; }
    public int Potions { get; private set; }
    public List<string> Inventory { get; private set; } = new List<string>();


    public Player(string name, int hp, int maxHp, int attack, int defense, string classtype, int gold, int xp,
        int level, int potions) : base(name, hp, maxHp, attack, defense)
    {
        ClassType = classtype;
        Gold = gold;
        XP = xp;
        Level = level;
        Potions = potions;
    }

    public void AddToInventory(string item)
    {
        if (!string.IsNullOrWhiteSpace(item))
        {
            Inventory.Add(item);
        }
    }
    
    public void AddXP(int amount)
    {
        XP += Math.Max(0, amount);
        MaybelevelUp();
    }
    
    public void AddGold(int amount)
    {
        Gold += Math.Max(0, amount);
    }

    public int NextLevelThreshold()
    { 
        return Level == 1 ? 10 : (Level == 2 ? 25 : (Level == 3 ? 45 : Level * 20));
    }

    protected abstract void MaybelevelUp();
    

    public abstract int CalculateDamage(int enemyDefence, Random random);

    public abstract int UseClassSpecial(int enemyDefensive, bool vsBoss, Random random);
    

    public bool TryRunAway(Random random)
    {
        double chance = 0.25;   // Warrior      
        if (ClassType == "Rogue") chance = 0.5;   
        if (ClassType == "Mage") chance = 0.35;
        return random.NextDouble() < chance;
    }

    public void ShowStatus()
    {
        Console.WriteLine($"[{Name} | {ClassType}]  HP {Hp}/{MaxHp}  ATK {Attack}  DEF {Defense}  LVL {Level}  XP {XP}  Guld {Gold}  Drycker {Potions}");
        if (Inventory.Count > 0)
            Console.WriteLine($"Väska: {string.Join(", ", Inventory)}");
    }
    
    public void UsePotion()
    {
        if (Potions <= 0)
        {
            Console.WriteLine("Du har inga drycker kvar.");
            return;
        }

        // Helning av spelaren
        int heal = 12;
        int newHp = Math.Min(MaxHp, Hp + heal);
        Potions = (Potions - 1);
        Console.WriteLine($"Du dricker en dryck och återfår {newHp - Hp} HP.");
        Hp = newHp;
        
    }

    public void Rest()
    {
        Hp = MaxHp;
    }
    public void TryBuy(int cost, ShopItem item, string successMsg)
    {
        
        if (Gold >= cost)
        {

            Gold = (Gold - cost);
            switch (item)
            {
                case ShopItem.Potion :
                    Potions++;
                    break;
                case ShopItem.Armor :
                    Defense += 2;
                    break;
                case ShopItem.Weapon :
                    Attack += 2;
                    break;
            }
            Console.WriteLine(successMsg);
        }
        else
        {
            Console.WriteLine("Du har inte råd.");
        }
    }

}
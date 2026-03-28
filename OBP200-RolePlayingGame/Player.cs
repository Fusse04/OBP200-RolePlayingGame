namespace OBP200_RolePlayingGame;

public class Player : Character , IInventoryAdd
{
    public string ClassType { get; private set; }
    public int Gold { get; private set; }
    public int XP { get; private set; }
    public int Level { get; private set; }
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

    private void MaybelevelUp()
    { 
        int nextThreshold = Level == 1 ? 10 : (Level == 2 ? 25 : (Level == 3 ? 45 : Level * 20));
        if (XP >= nextThreshold)
        {
            Level++;
        }
        switch (ClassType)
        {
            case "Warrior":
                MaxHp += 6; Attack += 2; Defense += 2;
                break;
            case "Mage":
                MaxHp += 4; Attack += 4; Defense += 1;
                break;
            case "Rogue":
                MaxHp += 5; Attack += 3; Defense += 1;
                break;
            default:
                MaxHp += 4; Attack += 3; Defense += 1;
                break;
        }
        Hp = MaxHp; // full heal vid level up
        Console.WriteLine($"Du når nivå {Level}! Värden ökade och HP återställd.");
    }
    
     public int CalculatePlayerDamage(int enemyDefence , Random random)
    {


        // Beräkna grundskada
        int baseDmg = Math.Max(1, Attack - (enemyDefence / 2));
        int roll = random.Next(0, 3); // liten variation

        switch (ClassType)
        {
            case "Warrior":
                baseDmg += 1; // warrior buff
                break;
            case "Mage":
                baseDmg += 2; // mage buff
                break;
            case "Rogue":
                baseDmg += (random.NextDouble() < 0.2) ? 4 : 0; // rogue crit-chans
                break;
            default:
                baseDmg += 0;
                break;
        }

        return Math.Max(1, baseDmg + roll);
    }
    
    public int UseClassSpecial(int enemyDefensive, bool vsBoss, Random random)
    {
        int specialDmg = 0;
        // Hantering av specialförmågor
        switch (ClassType)
        {
            // Heavy Strike: hög skada men självskada
            case "Warrior":
                Console.WriteLine("Warrior använder Heavy Strike!");
                specialDmg = Math.Max(2, Attack + 3 - enemyDefensive);
                TakeDamage(2);
                break;
            // Fireball: stor skada, kostar guld
            case "Mage":
                if (Gold >= 3)
                {
                    Console.WriteLine("Mage kastar Fireball!");
                    Gold -= 3;
                    specialDmg = Math.Max(3, Attack + 5 - (enemyDefensive / 2));
                }
                else
                {
                    Console.WriteLine("Inte tillräckligt med guld för Fireball (kostar 3).");
                }

                break;
            case "Rogue":
                if (random.NextDouble() < 0.5)
                {
                    Console.WriteLine("Rogue utför en lyckad Backstab!");
                    specialDmg = Math.Max(4, Attack + 6);
                }
                else
                {
                    Console.WriteLine("Backstab misslyckades!");
                    specialDmg = 1;
                }

                break;

        }

        if (vsBoss)
            specialDmg = (int)Math.Round(specialDmg * 0.8);
        return Math.Max(0, specialDmg);
    }

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
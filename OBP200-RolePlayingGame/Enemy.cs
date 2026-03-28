namespace OBP200_RolePlayingGame;

public class Enemy : Character, IAttack
{
    public string Type { get; private set; }
    public int XPReward { get; private set; }
    public int GoldReward { get; private set; }
    public bool IsBoss { get; private set; }

    public Enemy(string type, string name, int hp, int maxHp, int attack, int defense, int xpReward, int goldReward,
        bool isBoss) : base(name, hp, maxHp, attack, defense)
    {
        Type = type;
        XPReward = xpReward;
        GoldReward = goldReward;
        IsBoss = isBoss;
    }

    public void Appered()
    {
        Console.WriteLine($"En {Name} dyker upp! (HP {Hp}, ATK {Attack}, DEF {Defense})");
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;
    }

    public void ResetHp()
    {
        Hp = MaxHp;
    }
    
    public int CalculateDamage(int playerDefense, Random random)
    {
        int roll = random.Next(0, 3);
        int dmg = Math.Max(1, Attack - (playerDefense / 2)) + roll;

        return dmg;
    }

    public void RandomizeStats(Random random)
    {
        Hp = MaxHp + random.Next(-1, 3);
        Attack += random.Next(0, 2);
        Defense += random.Next(0, 2);
        XPReward += random.Next(0, 3);
        GoldReward += random.Next(0, 3);
    }

    public void MaybeGivePlayerLoot(Random rng, IInventoryAdd inventoryAdd)
    {
        if (rng.NextDouble() < 0.35)
        {
            string item = IsBoss ? "Dragon Scale" : "Minor Gem";
            inventoryAdd.AddToInventory(item);
        }
    }
}
namespace OBP200_RolePlayingGame;

public class Warrior : Player
{

    public Warrior(string name, int hp, int maxHp, int attack, int defense, string classtype, int gold, int xp,
        int level, int potions) : base(name, hp, maxHp, attack, defense, classtype, gold, xp, level, potions)
    {
        
    }
    
    public override int CalculateDamage(int enemyDefence, Random random)
    {
        int baseDmg = Math.Max(1, Attack - (enemyDefence / 2));
        int roll = random.Next(0, 3);
        baseDmg += 1; // warrior buff
        return Math.Max(1, baseDmg + roll);
    }
    
    public override int UseClassSpecial(int enemyDefens, bool vsBoss, Random random)
    {
        Console.WriteLine("Warrior använder Heavy Strike!");
        int specialDmg = Math.Max(2, Attack + 3 - enemyDefens);
        TakeDamage(2); // självskada
        if (vsBoss) specialDmg = (int)Math.Round(specialDmg * 0.8);
        return Math.Max(0, specialDmg);
    }
    
    protected override void MaybelevelUp()
    {
        if (XP >= NextLevelThreshold())
        {
            Level++;
            MaxHp += 6; 
            Attack += 2; 
            Defense += 2;
            Hp = MaxHp; // full heal vid level up
            Console.WriteLine($"Du når nivå {Level}! Värden ökade och HP återställd.");
        }
        
    }
    
}
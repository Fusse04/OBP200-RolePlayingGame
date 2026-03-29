namespace OBP200_RolePlayingGame;

public class Rogue : Player
{
    public Rogue(string name, int hp, int maxHp, int attack, int defense, string classtype, int gold, int xp,
        int level, int potions) : base(name, hp, maxHp, attack, defense, classtype, gold, xp, level, potions)
    {
        
    }
    
    public override int CalculateDamage(int enemyDefence, Random random)
    {
        int baseDmg = Math.Max(1, Attack - (enemyDefence / 2));
        int roll = random.Next(0, 3);
        baseDmg += (random.NextDouble() < 0.2) ? 4 : 0; // crit-chans
        return Math.Max(1, baseDmg + roll);
    }

    public override int UseClassSpecial(int enemyDefens, bool vsBoss, Random random)
    {
        int specialDmg;
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
        if (vsBoss) specialDmg = (int)Math.Round(specialDmg * 0.8);
        return Math.Max(0, specialDmg);
    } 
    
    protected override void MaybelevelUp()
    {
        if (XP >= NextLevelThreshold())
        {
            Level++;
            MaxHp += 5; 
            Attack += 3; 
            Defense += 1;
            Hp = MaxHp; // full heal vid level up
            Console.WriteLine($"Du når nivå {Level}! Värden ökade och HP återställd.");
        }
        
    }
}
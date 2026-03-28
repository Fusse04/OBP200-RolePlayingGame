namespace OBP200_RolePlayingGame;

public class Mage : Player
{

    public Mage(string name, int hp, int maxHp, int attack, int defense, string classtype, int gold, int xp,
        int level, int potions) : base( name, hp, maxHp, attack, defense, classtype,  gold,  xp, level,  potions)
    {
    }
    
    
    public override int CalculateDamage(int enemyDefence, Random random)
    {
        int baseDmg = Math.Max(1, Attack - (enemyDefence / 2));
        int roll = random.Next(0, 3); // liten variation
        baseDmg += 2; // mage buff
        return Math.Max(1, baseDmg + roll);
    }

    public override int UseClassSpecial(int enemyDefensive, bool vsBoss, Random random)
    {
        int specialDmg = 0;
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
        if (vsBoss)
            specialDmg = (int)Math.Round(specialDmg * 0.8);
        return Math.Max(0, specialDmg);
    }

    protected override void MaybelevelUp()
    {
        if (XP >= NextLevelThreshold())
        {
            Level++;
            MaxHp += 4;
            Attack += 4;
            Defense += 1;
            Hp = MaxHp; // full heal vid level up
            Console.WriteLine($"Du når nivå {Level}! Värden ökade och HP återställd.");
        }
        
    }
}
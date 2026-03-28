namespace OBP200_RolePlayingGame;

public abstract class Character
{
    public string Name { get; protected set; }
    public int Hp { get; protected set; }
    public int MaxHp { get;  protected set; }
    public int Attack { get; protected set; }
    public int Defense { get; protected set; }

    public bool IsAlive() => Hp > 0;

    public Character(string name, int hp, int maxHp, int attack, int defense)
    {
        Name = name;
        Hp = hp;
        MaxHp = maxHp;
        Attack = attack;
        Defense = defense;
    }

    public void TakeDamage(int damage)
    {
       Hp = Math.Max(0, Hp - Math.Max(0, damage)); 
    }
}
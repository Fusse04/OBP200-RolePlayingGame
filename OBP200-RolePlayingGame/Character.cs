namespace OBP200_RolePlayingGame;

abstract class Character
{
    private string Name { get; set; }
    public int Hp { get; set; }
    public int MaxHp { get;  set; }
    public int Attack { get; set; }
    public int Defense { get; set; }

    public bool IsAlive() => Hp >= 0;

    public void TakeDamage(int damage)
    {
       Hp = Math.Max(0, Hp - Math.Max(0, damage)); 
    }

    public void UsePotion(int potions)
    {
        if (potions > 0)
        {
            Console.WriteLine("Du har inga drycker kvar.");
            return;
        }

        int heal = 12;
        int newHealthPoints =Math.Min(MaxHp, Hp + heal);
        Console.WriteLine($"Du dricker en dryck och återfår {newHealthPoints - Hp} Health points.");
        Hp = newHealthPoints;
        potions--;
    }
}
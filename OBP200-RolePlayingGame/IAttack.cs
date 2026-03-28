namespace OBP200_RolePlayingGame;

public interface IAttack
{
    public int CalculateDamage(int playerDefense, Random random);
}
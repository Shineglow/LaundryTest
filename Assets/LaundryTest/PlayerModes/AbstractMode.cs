namespace LaundryTest.PlayerModes
{
    public abstract class AbstractMode
    {
        public abstract void Enter(PlayerModeContext context);
        public abstract EPlayerMode Update(PlayerModeContext context);
        public abstract void Exit(PlayerModeContext context);
    }
}
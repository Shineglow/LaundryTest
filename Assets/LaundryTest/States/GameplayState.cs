namespace LaundryTest
{
    public abstract class GameplayState
    {
        public virtual void Enter(){}
        public virtual void Update(){}
        public virtual void Exit(){}
    }
}
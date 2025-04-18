namespace LaundryTest.InputSystem
{
    public struct PlayerInput
    {
        public ButtonState<bool> InteractPerformed;
        public ButtonState<bool> CancelPerformed;
        public ButtonState<bool> Action1Performed;
        public ButtonState<bool> Action2Performed;
        public ButtonState<bool> Action3Performed;
    }

    public struct ButtonState<T>
    {
        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                WasChanged = !Equals(_value, value);
                _value = value;
            }
        }
        
        public bool WasChanged { get; private set; }
    }
}
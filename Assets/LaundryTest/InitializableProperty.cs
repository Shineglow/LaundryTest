namespace LaundryTest
{
    public struct InitializableProperty<T> : IResetable
    {
        private T _value;
        public bool IsInitialized { get; private set; }

        public T Value
        {
            get => _value;
            set
            {
                IsInitialized = true;
                _value = value;
            }
        }

        public void Reset()
        {
            _value = default;
            IsInitialized = false;
        }
    }
}
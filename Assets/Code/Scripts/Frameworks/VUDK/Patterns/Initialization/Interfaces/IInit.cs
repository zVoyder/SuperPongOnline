namespace VUDK.Patterns.Initialization.Interfaces
{
    public interface IInit
    {
        /// <summary>
        /// Initializes object.
        /// </summary>
        public void Init();

        /// <summary>
        /// Checks object correct initialization.
        /// </summary>
        public bool Check();
    }

    public interface IInit<T>
    {
        /// <summary>
        /// Initializes object with its arguments.
        /// </summary>
        public void Init(T arg);

        /// <summary>
        /// Checks object correct initialization.
        /// </summary>
        public bool Check();
    }

    public interface IInit<T1, T2>
    {
        /// <summary>
        /// Initializes object with its arguments.
        /// </summary>
        public void Init(T1 arg1, T2 arg2);

        /// <summary>
        /// Checks object correct initialization.
        /// </summary>
        public bool Check();
    }

    public interface IInit<T1, T2, T3>
    {
        /// <summary>
        /// Initializes object with its arguments.
        /// </summary>
        public void Init(T1 arg1, T2 arg2, T3 arg3);

        /// <summary>
        /// Checks object correct initialization.
        /// </summary>
        public bool Check();
    }
}
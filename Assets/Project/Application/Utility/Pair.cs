namespace Project.Application.Utility
{
    public class Pair<T, U>
    {
        public T First = default;
        public U Second = default;

        public Pair() {}

        public Pair(T first, U second)
        {
            First = first;
            Second = second;
        }

        public void Initialize(T first, U second)
        {
            First = first;
            Second = second;
        }

        public bool IsEmpty()
        {
            return First == null && Second == null;
        }
    }
}

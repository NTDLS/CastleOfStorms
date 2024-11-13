namespace Cos.Library
{
    public class CircularBuffer<T>
    {
        private readonly Queue<T> _queue;
        private readonly int _capacity;

        public CircularBuffer(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException("capacity", "Capacity must be greater than 0.");

            _capacity = capacity;
            _queue = new Queue<T>(capacity);
        }

        public void Push(T item)
        {
            if (_queue.Count == _capacity)
            {
                _queue.Dequeue();
            }

            _queue.Enqueue(item);
        }

        public int Count => _queue.Count;

        public IEnumerable<T> Items => _queue;
    }
}

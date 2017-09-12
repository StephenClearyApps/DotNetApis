namespace DotNetApis.Common
{
    public static class InstanceOf<TValue>
    {
        public sealed class For<TTag>
        {
            public For(TValue value)
            {
                Value = value;
            }

            public TValue Value { get; }
        }
    }
}

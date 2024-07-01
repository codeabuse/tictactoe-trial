using System;

namespace TicTacToe
{
    /// <summary>
    /// Represents optional result, useful to replace null return type or nullable value types,
    /// providing safe execution path when no value has been returned.
    /// Can be used in combination with Result.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public readonly struct Option<TValue>
    {
        public static Option<TValue> None => new(false);

        public bool HasValue => _hasValue;

        private readonly TValue _value;
        private readonly bool _hasValue;

        public Option(TValue value)
        {
            _value = value;
            _hasValue = true;
        }

        private Option(bool hasValue)
        {
            _value = default;
            _hasValue = hasValue;
        }

        public void Map(Action<TValue> some, Action none = null)
        {
            if (_hasValue)
            {
                some(_value);
            }
            else
            {
                none?.Invoke();
            }
        }

        public static implicit operator Option<TValue>(TValue value)
        {
            return value is {}?
                new Option<TValue>(value) :
                None;
        }
    }
    
    /// <summary>
    /// Represents optional result, useful to replace null return type or nullable value types,
    /// providing safe execution path when no value has been returned.
    /// Can be used in combination with Result.
    /// </summary>
    /// <typeparam name="TResult1"></typeparam>
    /// <typeparam name="TResult2"></typeparam>
    public readonly struct Option<TResult1, TResult2>
    {

        private readonly TResult1 _result1;
        private readonly TResult2 _result2;
        private readonly int _resultIndex;

        public Option(TResult1 result1)
        {
            _result1 = result1;
            _result2 = default;
            _resultIndex = 0;
        }

        private Option(TResult2 result2)
        {
            _result1 = default;
            _result2 = result2;
            _resultIndex = 1;
        }

        public void Map(Action<TResult1> onResult1, Action<TResult2> onResult2)
        {
            switch (_resultIndex)
            {
                case 0:
                    onResult1.Invoke(_result1);
                    break;
                case 1:
                    onResult2.Invoke(_result2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_resultIndex));
            }
        }

        public static implicit operator Option<TResult1, TResult2>(TResult1 value)
        {
            return new(value);
        }

        public static implicit operator Option<TResult1, TResult2>(TResult2 value)
        {
            return new(value);
        }
    }
}
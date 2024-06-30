using System;

namespace TicTacToe.Structures
{
    public readonly struct Result<TValue, TError>
    {
        private readonly bool _isFailed;
        private readonly TValue _value;
        private readonly TError _error;

        public bool IsFailed => _isFailed;

        private Result(TValue value)
        {
            _isFailed = false;
            _value = value;
            _error = default;
        }
        
        private Result(TError error)
        {
            _isFailed = true;
            _value = default;
            _error = error;
        }

        public void Map(Action<TValue> onSuccess, Action<TError> onFailed)
        {
            if (_isFailed)
                onFailed?.Invoke(_error);
            else
                onSuccess(_value);
        }

        public static implicit operator Result<TValue, TError>(TValue value) => 
                new (value);
        
        public static implicit operator Result<TValue, TError>(TError error) => 
                new (error);
    }
    
    public readonly struct ResultVoid<TError>
    {
        public static ResultVoid<TError> Success => new();

        private readonly bool _isFailed; 
        private readonly TError _error;
        public bool IsFailed => _isFailed;
        
        private ResultVoid(TError error)
        {
            _isFailed = true;
            _error = error;
        }
        
        public void Map(Action onSuccess, Action<TError> onFailed)
        {
            if (_isFailed)
                onFailed?.Invoke(_error);
            else
                onSuccess();
        }
        
        public static implicit operator ResultVoid<TError>(TError error) => 
                new (error);
    }
}
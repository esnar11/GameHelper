using System;
using System.Diagnostics;

namespace GameHelper.Interfaces
{
    [DebuggerDisplay("{Value} (min {Min}; max {Max})")]
    [Serializable]
    public class RangeF: ICloneable<RangeF>
    {
        private float _min;
        private float _value;
        private float _max;

        public float Min
        {
            get => _min;
            set
            {
                _min = value;
            }
        }

        /// <summary>
        /// Текущее значение равно минимуму
        /// </summary>
        public bool ValueEqMin => Value.Equals(Min);

        /// <summary>
        /// Текущее значение равно минимуму
        /// </summary>
        public bool ValueEqMax => Value.Equals(Max);

        public float NormalizedValue => (Value - Min) / (Max - Min);

        public float Max
        {
            get => _max;
            set
            {
                _max = value;
            }
        }

        public float Value
        {
            get => _value;
            set
            {
                var oldValue = _value;

                _value = value;
                if (_value < Min)
                    _value = Min;
                if (_value > Max)
                    _value = Max;

                if (!oldValue.Equals(_value))
                    ValueChanged?.Invoke(this, oldValue, _value);
            }
        }

        public event Action<RangeF, float, float> ValueChanged;

        public void SetToMin()
        {
            Value = Min;
        }

        public void SetToMax()
        {
            Value = Max;
        }

        public RangeF Clone()
        {
            return new RangeF
            {
                _min = _min,
                _max = _max,
                _value = _value
            };
        }

        public void CopyFrom(RangeF range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));

            _min = range.Min;
            _max = range.Max;
            _value = range.Value;
        }
    }
}

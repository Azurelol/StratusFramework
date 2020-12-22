using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
    public enum StratusValueSource
    {
        Invalid,
        Reference,
        Value
    }

    public class StratusValue<T>
    {
        public StratusValueSource source { get; private set; }
        public T value
        {
            get
            {
                switch (source)
                {
                    case StratusValueSource.Reference:
                        return _getter();
                    case StratusValueSource.Value:
                        return _value;
                }
                throw new Exception("No value source was set");
            }
        }

        private T _value;
        private Func<T> _getter;

        public StratusValue(Func<T> getter)
        {
            if (getter == null)
            {
                this.source = StratusValueSource.Invalid;
                return;
            }
            this._getter = getter;
            this.source = StratusValueSource.Reference;
        }

        public StratusValue(T value)
        {
            if (value == null)
            {
                this.source = StratusValueSource.Invalid;
                return;
            }
            this._value = value;
            this.source = StratusValueSource.Value;
        }

        //public static implicit operator T(StratusValue<T> value ) => value.value;
    }

}
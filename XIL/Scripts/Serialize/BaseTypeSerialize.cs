﻿using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace wxb
{
    class Serialize<T> : ITypeSerialize where T : System.IEquatable<T>
    {
        public Serialize(byte typeFlag, System.Action<T, IStream> writeTo, System.Func<IStream, T> mergeFrom)
        {
            this.writeTo = writeTo;
            this.mergeFrom = mergeFrom;
            typeFlag_ = typeFlag;
        }

        byte typeFlag_ = 0;
        byte ITypeSerialize.typeFlag { get { return typeFlag_; } } // 类型标识

        System.Action<T, IStream> writeTo;
        void ITypeSerialize.WriteTo(object obj, IStream ms)
        {
            T value = (T)obj;
            writeTo(value, ms);
        }

        System.Func<IStream, T> mergeFrom;
        void ITypeSerialize.MergeFrom(ref object value, IStream ms)
        {
            value = mergeFrom(ms);
        }

        // 判断两个值是否相等
        bool ITypeSerialize.IsEquals(object x, object y)
        {
            return ((System.IEquatable<T>)x).Equals((System.IEquatable<T>)y);
        }
    }
}

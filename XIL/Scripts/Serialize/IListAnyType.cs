using System.Collections;

namespace wxb
{
    abstract class IListAnyType : ITypeSerialize
    {
        protected ITypeSerialize elementTypeSerialize;

        protected bool isSmartType = false; // �����Ƿ񱣴�һ��������

        public IListAnyType(ITypeSerialize elementTypeSerialize)
        {
            this.elementTypeSerialize = elementTypeSerialize;
        }

        public IListAnyType()
        {

        }

        byte ITypeSerialize.typeFlag { get { return TypeFlags.arrayType; } } // �����ʶ

        void ITypeSerialize.WriteTo(object value, IStream stream)
        {
            IList array = (IList)value;
            if (array == null)
            {
                stream.WriteByte(0);
                return;
            }

            stream.WriteByte(1);
            stream.WriteByte(elementTypeSerialize.typeFlag);
            stream.WriteVarInt32(array.Count);
            object elementObj;
            for (int i = 0; i < array.Count; ++i)
            {
                elementObj = array[i];
                using (new RLStream(stream))
                {
                    elementTypeSerialize.WriteTo(elementObj, stream);
                }
            }
        }

        protected abstract IList Create(int lenght);

        void ITypeSerialize.MergeFrom(ref object value, IStream stream)
        {
            byte flag = stream.ReadByte();
            if (flag == 0)
            {
                value = null;
                return;
            }

            byte flagType = stream.ReadByte();
            int lenght = stream.ReadVarInt32();

            bool isTypeTrue = flagType == elementTypeSerialize.typeFlag; // �����Ƿ�һ��
            var array = value as IList;
            if (isTypeTrue && (array == null || array.Count != lenght))
            {
                array = Create(lenght);
                value = array;
            }

            if (isTypeTrue)
            {
                for (int i = 0; i < lenght; ++i)
                {
                    object v = array[i];
                    RLStream.MergeFrom(elementTypeSerialize, stream, ref v);
                    array[i] = v;
                }
            }
            else
            {
                for (int i = 0; i < lenght; ++i)
                {
                    RLStream.Next(stream);
                }
            }
        }

        // �ж�����ֵ�Ƿ����
        bool ITypeSerialize.IsEquals(object x, object y)
        {
            var xlist = (IList)x;
            var ylist = (IList)y;
            int count = xlist.Count;
            if (count != ylist.Count)
                return false;

            for (int i = 0; i < count; ++i)
            {
                if (!BinarySerializable.IsEquip(xlist[i], ylist[i]))
                    return false;
            }

            return true;
        }
    }
}
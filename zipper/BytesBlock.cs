using System;

namespace zipper
{
    [Serializable]
    public class BytesBlock
    {
        public int OrderNum { get; set; }

        public byte[] BytesArray { get; set; }

        public BytesBlock(int orderNum, byte[] bytesArray)
        {
            OrderNum = orderNum;
            BytesArray = bytesArray;
        }
    }
}
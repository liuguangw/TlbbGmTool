using System.Text;

namespace liuguang.Axp;

public class MathTable
{
    private readonly uint[] _mathDataBuf;

    public MathTable()
    {
        var mathDataBuf = new uint[0x400];
        uint seed = 0x0010_0001;
        for (var index1 = 0; index1 < 0x100; index1++)
        {
            var index2 = index1;
            for (var i = 0; i < 4; i++)
            {
                seed = (seed * 125 + 3) % 0x2AAAAB;
                var temp1 = (seed & 0xFFFF) << 0x10;

                seed = (seed * 125 + 3) % 0x2AAAAB;
                var temp2 = seed & 0xFFFF;

                mathDataBuf[index2] = (temp1 | temp2);
                index2 += 0x100;
            }
        }
        _mathDataBuf = mathDataBuf;
    }

    public uint Hash(HashType hashType, string plain)
    {
        uint seed1 = 0x7FED_7FED;
        uint seed2 = 0xEEEE_EEEE;
        var strEncoding = Encoding.GetEncoding("GB18030");
        var stringData = strEncoding.GetBytes(plain);
        foreach (var byteP in stringData)
        {
            uint ch = byteP;
            /*if (ch > 127)
            {
                ch = unchecked(ch - 256);
            }*/
            if ((ch & 0x80) != 0)
            {
                ch |= 0xFFFF_FF00;
            }
            uint bufIndex = (((uint)hashType) << 8) + ch;

            seed1 = _mathDataBuf[bufIndex] ^ (seed1 + seed2);
            seed2 = ch + seed1 + seed2 + (seed2 << 5) + 3;
        }
        return seed1;
    }
}

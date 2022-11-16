using System;
using System.Collections.Generic;


namespace LostEvoRewrite
{
    public class DecompressFile
    {
        public static byte[] Decompress(byte[] src)
        {
            var output = new List<byte>();
            byte[] buffer = new byte[4096];
            int r0, r2, r3, flagByte, curBuff, r6, bufferIdx, length, curResult;
            r0 = 0; r2 = 0;
            curBuff = 4078;
            r6 = r2;
            bufferIdx = r0;
            flagByte = 0;


            Array.Clear(buffer, flagByte, curBuff);
            r0 = BitConverter.ToInt32(src, bufferIdx);
            bufferIdx += 4;

            while (r0 > 0)
            {
                flagByte = (flagByte << 15) >> 16;

                if ((flagByte & 0x100) != 0x100)
                {
                    r0 = r0 - 1;
                    flagByte = (src[bufferIdx++] << 16 >> 16 | 0xFF00);
                    if (r0 < 0) break;
                }

                if ((flagByte & 0x1) == 0x1)
                {
                    r0 = r0 - 1;
                    if (r0 < 0) break;
                    r3 = (src[bufferIdx++] & 0xFF);
                    output.Add((byte)r3); 
                    buffer[r6 + curBuff] = (byte)r3;
                    curBuff = (curBuff + 1) & 4095;
                    continue;
                }

                r0 -= 2;
                if (r0 < 0) break;
            
                r2 = (src[bufferIdx + 1] & 0xF0) << 4;
                length = (src[bufferIdx + 1] & 0xF) + 2;
                curResult = src[bufferIdx] | r2;
                bufferIdx += 2;

                for (int i = 0; i <= length; i++)
                {
                    r2 = (curResult + i) & 4095;
                    r3 = buffer[r6 + r2];
                    output.Add((byte)r3);
                    buffer[r6 + curBuff] = (byte)r3;
                    curBuff = (curBuff + 1) & 4095;
                }
            }
            return output.ToArray();
        }

    }
}

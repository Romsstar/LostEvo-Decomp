    using System;
    using System.Collections.Generic;
using System.IO;

using System.Text;


namespace LostEvoRewrite
{
    public class Compress
    {
        public static byte[] text_buf = new byte[4113];
        public static int[] rson = new int[4353];
        public static int[] lson = new int[4097];
        public static int[] dad = new int[4097];
        public static int match_length, match_position;
        public static int BufferSize = 4096;

        public static void InsertNode(int r)
        {

            int cmp = 1;
            var p = BufferSize + 1 + text_buf[r];
            rson[r] = lson[r] = BufferSize;
            match_length = 0;

            while (true)
            {
                if (cmp >= 0)
                {
                    if (rson[p] != BufferSize)
                    {
                        p = rson[p];
                    }
                    else
                    {
                        rson[p] = r;
                        dad[r] = p;
                        return;
                    }
                }
                else
                {
                    if (lson[p] != BufferSize)
                    {
                        p = lson[p];
                    }
                    else
                    {
                        lson[p] = r;
                        dad[r] = p;
                        return;
                    }
                }
                int i;
                for (i = 1; i < 18; i++)
                {
                    if ((cmp = text_buf[r + i] - text_buf[p + i]) != 0)
                        break;
                }

                if (i > match_length)
                {
                    match_position = p;
                    if ((match_length = i) >= 18) break;
                }
            }

            dad[r] = dad[p];
            lson[r] = lson[p];
            rson[r] = rson[p];
            dad[lson[p]] = r;
            dad[rson[p]] = r;

            if (rson[dad[p]] == p)
                rson[dad[p]] = r;
            else
                lson[dad[p]] = r;

            dad[p] = BufferSize; /* remove p */
        }




        public static void DeleteNode(int p)
        {
            int q;
            if (dad[p] == 4096)
                return; // not in tree

            if (rson[p] == 4096)
            {
                q = lson[p];
            }
            else if (lson[p] == 4096)
            {
                q = rson[p];
            }
            else
            {
                q = lson[p];
                if (rson[q] != 4096)
                {
                    do
                    {
                        q = rson[q];
                    } while (rson[q] != 4096);

                    rson[dad[q]] = lson[q];
                    dad[lson[q]] = dad[q];
                    lson[q] = lson[p];
                    dad[lson[p]] = q;
                }

                rson[q] = rson[p];
                dad[rson[p]] = q;
            }

            dad[q] = dad[p];

            if (rson[dad[p]] == p)
                rson[dad[p]] = q;
            else
                lson[dad[p]] = q;
            dad[p] = 4096;
        }

        public static byte[] CompressFile(byte[] data)
        {
            int i,
            c,
            len,
            r,
            s,
            last_match_length,
            code_buf_ptr;
            var code_buf = new byte[17];

            var output = new List<byte>();
           
            var inputIdx = 0;

            for (i = 4097; i <= 4352; i++) rson[i] = 4096;
            for (i = 0; i < 4096; i++) dad[i] = 4096;
            code_buf[0] =
            0; // code_buf[1..16] saves eight units of code, and code_buf[0] works as eight flags, "1" representing that the unit is an unencoded letter (1 byte), "0" a position-and-length pair (2 bytes). Thus, eight units require at most 16 bytes of code.
            byte mask = 1;
            code_buf_ptr = mask;

            s = 0;
            r = 4078;
            Array.Clear(text_buf, 0, r);

            for (len = 0; len < 18 && inputIdx < data.Length; len++) text_buf[r + len] = data[inputIdx++];

            for
            (i = 1; i <= 18; i++)
                InsertNode(r -
                i); // Insert the 18 strings, each of which begins with one or more 'space' characters. Note the order in which these strings are inserted. This way, degenerate trees will be less likely to occur.

            InsertNode(r);

            do
            {
                if (match_length > len) match_length = len; // match_length may be spuriously long near the end of text.
                if (match_length <= 2)
                {
                    match_length = 1; // Not long enough match. Send one byte.
                    code_buf[0] |= mask; // 'send one byte' flag
                    code_buf[code_buf_ptr++] = text_buf[r]; // Send uncoded.
                }
                else
                {
                    code_buf[code_buf_ptr++] = (byte)match_position;
                    code_buf[code_buf_ptr++] =
                    (byte)(((match_position >> 4) & 0xf0) |
                    (match_length - 3)); //Send position and length pair. Note match_length > 2.
                }

                if ((mask <<= 1) == 0) /* Shift mask left one bit. */
                {
                    for (i = 0; i < code_buf_ptr; ++i)
                        output.Add(code_buf[i]);
                    code_buf[0] = 0;
                    code_buf_ptr = mask = 1;


                }

                last_match_length = match_length;
                for (i = 0; i < last_match_length && inputIdx < data.Length; i++)
                {
                    DeleteNode(s); /* Delete old strings and */
                    c = data[inputIdx++];
                    text_buf[s] = (byte)c; /* read new bytes */
                    if (s < 17)
                        text_buf[s + 4096] =
                        (byte)c; /* If the position is near the end of buffer, extend the buffer to make string comparison easier. */

                    s = (s + 1) & 4095;
                    r = (r + 1) & 4095;
                    /* Since this is a ring buffer, increment the position
                    modulo N. */
                    InsertNode(r); /* Register the string in text_buf[r..r+F-1] */
                }

                while (i++ < last_match_length)
                {
                    /* After the end of text, */
                    DeleteNode(s); /* no need to read, but */
                    s = (s + 1) & 4095;
                    r = (r + 1) & 4095;

                    if (--len != 0)
                        InsertNode(r); // buffer may not be empty.
                }
            } while (len > 0); /* until length of string to be processed is zero */

            if (code_buf_ptr > 1)
                /* Send remaining code. */
                for (i = 0; i < code_buf_ptr; ++i)
                    output.Add(code_buf[i]);

            return output.ToArray();
        }


        public static void CompressFiles(string PAKFile)
        {         
                var data = File.ReadAllBytes(PAKFile);
                byte[] compressed = CompressFile(data);
                using (var output = File.Open(Path.GetFileNameWithoutExtension(PAKFile) + ".enc", FileMode.Create))
                using (var bw = new BinaryWriter(output, Encoding.UTF8, false))
                {
                    bw.Write(compressed.Length);
                    bw.Write(compressed);
                }
            }
    }
}
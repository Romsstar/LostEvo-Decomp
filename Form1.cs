using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LostEvoRewrite
{

    public struct partition
    {
        public string filenum;
        public uint offset;
        public uint decsize;
        public uint encsize;
        public uint flag; //0x80000000 = uncompressed, 0x0 = compressed
    };

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public byte[] data;

        public BinaryReader br;
        public uint numPartitions;
        public byte[] curBytes;
        public List<partition> partitionList = new List<partition>();
        public static string dir = "extracted//";
        private static int[] text_buf = new int[4113];
        private static int match_position, match_length, lson, rson, dad;

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            {
                openFileDialog1.Filter = "PAK files|*.PAK|All Files|*.*";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string PAKFile = openFileDialog1.FileName;
                    FileStream fs = new FileStream(PAKFile, FileMode.Open);
                    br = new BinaryReader(fs);
                    numPartitions = br.ReadUInt32();
                    char[] sVersion = br.ReadChars(12);


                    partition partitionArray = new partition();
                    for (int i = 0; i < numPartitions; i++)
                    {
                        partitionArray.filenum = "" + i;
                        partitionArray.offset = br.ReadUInt32();
                        partitionArray.decsize = br.ReadUInt32();
                        partitionArray.encsize = br.ReadUInt32();
                        partitionArray.flag = br.ReadUInt32();
                        partitionList.Add(partitionArray);
                    }

                }
            }
        }



        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {

            foreach (var entry in partitionList)
            {
                br.BaseStream.Seek(entry.offset, SeekOrigin.Begin);

                data = br.ReadBytes((int)entry.encsize);

                File.WriteAllBytes(dir + (entry.filenum) + ".bin", data);
            }
        }
        int align(long pos, int align)
        {
            return (int)((align - (pos % align)) % align);
        }

        private byte[] Decompress(byte[] src)
        {
            var buffer = new List<byte>();
            byte[] buffer2 = new byte[4096];
            uint r0, curResult, r2, r3, flagByte, curBuff, r6, buferIdx, r9, curResult2, lr;
            r0 = 0;
            curResult = 0;
            r2 = 0;

            curBuff = 4078;
            r6 = r2;
            buferIdx = r0;
            flagByte = 0;


            Array.Clear(buffer2, (int)flagByte, (int)curBuff);

            r0 = BitConverter.ToUInt32(src, (int)buferIdx);
            buferIdx += 4;


            while (r0 > 0)
            {

                curResult = flagByte << 15;
                flagByte = curResult >> 16;

                if ((flagByte & 0x100) != 0x100)
                {
                    r0 = r0 - 1;
                    curResult = src[buferIdx++];
                    if ((int)r0 < 0) break;

                    curResult = curResult | 0xFF00;
                    curResult = curResult << 16;
                    flagByte = curResult >> 16;
                }


                if ((flagByte & 0x1) == 0x1)
                {
                    r0 = r0 - 1;
                    curResult = src[buferIdx++];
                    if ((int)r0 < 0) break;
                    r3 = curResult & 0xFF;
                    buffer.Add((byte)r3);
                    curResult = 4095;
                    r2 = curBuff + 1;
                    buffer2[r6 + curBuff] = (byte)r3;
                    curBuff = r2 & curResult;
                    continue;
                }

                r3 = src[buferIdx];
                curResult = r0 - 1;
                if ((int)curResult < 0) break;
                curResult = src[buferIdx + 1];
                buferIdx = buferIdx + 2;
                r0 = r0 - 2;
                if ((int)r0 < 0) break;
                r2 = curResult & 0xF0;
                curResult = curResult & 0xF;
                r9 = curResult + 2;
                curResult = 4095;
                curResult2 = r3 | (r2 << 4);
                lr = 0;

                do
                {
                    r2 = curResult2 + lr;
                    r2 = r2 & curResult;
                    r3 = buffer2[r6 + r2];
                    lr = lr + 1;
                    r2 = curBuff + 1;
                    buffer.Add((byte)r3);
                    buffer2[r6 + curBuff] = (byte)r3;
                    curBuff = r2 & curResult;
                } while (lr <= r9);
            }
            return buffer.ToArray();
        }





        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())

            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    List<byte[]> filedata = new List<byte[]>(); ;
                    var path = fbd.SelectedPath;
             
                    var inputFiles = Directory.EnumerateFiles(path).ToArray();
                    long padding = 0;
                    var i = 0;
                    
                    int offset = inputFiles.Length * 16 + 16;
                    using (var output = File.Open("SPR_NCGR.bin", FileMode.Create))
                    using (var bw = new BinaryWriter(output, Encoding.UTF8, false))
                    {
                        //PAK Header
                        bw.Write(inputFiles.Length);
                        bw.Write(0x31302e32);
                        bw.Write(padding);

                        foreach (var inputFile in inputFiles)
                        {
                            var currentBytes = File.ReadAllBytes(inputFile);
                            bw.Write(offset);
                            if (partitionList[i].flag == 0) //If File is compressed
                            {
                                bw.Write(Decompress(currentBytes).Length); //Write Decompressed Filesize in table
                            }
                            else
                            {
                                bw.Write(currentBytes.Length); //Else write its original size
                            }

                            bw.Write(currentBytes.Length);
                            bw.Write(partitionList[i].flag);
                            offset += (currentBytes.Length+align(currentBytes.Length, 0x04)); //Write Offset
                            filedata.Add(currentBytes);
                            
                            i++;
                        }  

                        for (int file_idx = 0; file_idx < partitionList.Count(); file_idx++)
                        {
                            var fileBytes = filedata[file_idx];
                            var flag = partitionList[file_idx].flag;
                            bw.Write(fileBytes, 0, fileBytes.Length);

                            if (flag == 0)
                            {
                                int pads_to_write = align(bw.BaseStream.Position, 0x4);
                                for (int y = 0; y < pads_to_write; ++y)
                                {
                                    bw.Write((byte)0);
                             
                                }

           
                            }  
                            i++;
                      

                        }
                    }
                }
            }
        }
    }
}






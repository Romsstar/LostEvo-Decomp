
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static LostEvoRewrite.DecompressFile;
using static LostEvoRewrite.Compress;
using System.Text.RegularExpressions;

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
        public static string filetableName;
        public List<partition> partitionList = new List<partition>();
        public static string dir = "extracted//";


        int align(long pos, int align)
        {
            return (int)((align - (pos % align)) % align);
        }

        
        public static string PadNumbers(string input, int maxStringLength)
        {
            return Regex.Replace(input, "[0-9]+", match => match.Value.PadLeft(maxStringLength, '0'));
        }

        //check if file doesn't exist
        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }
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
                    filetableName = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);

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

                    //if json doesn't exist
                    if (!FileExists(dir + filetableName + ".json"))
                    {
                        File.WriteAllText(Directory.CreateDirectory(filetableName + "\\") + filetableName + ".json",
                            JsonConvert.SerializeObject(partitionList, Formatting.Indented));
                    }
                    //free the stream
                    br.Close();
                    fs.Close();

                }
            }
        }


        private void compressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "PAK files|*.bin|All Files|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string PAKFile = openFileDialog1.FileName;
                CompressFiles(PAKFile);
            }
        }

        private void decompressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //File into Byte Array

            openFileDialog1.Filter = "PAK files|*.bin|All Files|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string PAKFile = openFileDialog1.FileName;
                FileStream fs = new FileStream(PAKFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                br = new BinaryReader(fs);
                byte[] data = File.ReadAllBytes(PAKFile);
                byte[] decompressed = Decompress(data);
                File.WriteAllBytes("miau" + ".decompressed", decompressed);

            }
        }

        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {

            foreach (var entry in partitionList)
            {
                br.BaseStream.Seek(entry.offset, SeekOrigin.Begin);

                data = br.ReadBytes((int)entry.encsize);
                File.WriteAllBytes(dir + PadNumbers(entry.filenum, 4) + ".bin", data);
            }
        }


        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())

            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    List<byte[]> filedata = new List<byte[]>();
                    ;
                    var path = fbd.SelectedPath + "\\extracted";

                    var inputFiles = Directory.EnumerateFiles(path).ToArray();
                    long padding = 0;
                    var i = 0;
                    string pathName = new DirectoryInfo(fbd.SelectedPath).Name;
                    string jsonFile = pathName + "\\" + pathName + ".json";
                    string json = File.ReadAllText(jsonFile);
                    partitionList = JsonConvert.DeserializeObject<List<partition>>(json);

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
                            offset += (currentBytes.Length + align(currentBytes.Length, 0x04)); //Write Offset+Padding
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

        private void compressFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())

            {

                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    var path = fbd.SelectedPath + "\\decompressed";
                    var inputFiles = Directory.EnumerateFiles(path).ToArray();
                   
             
                    //CompressFiles on all files in folder
                    foreach (var inputFile in inputFiles)
                    {
                        CompressFiles(inputFile);

                    }

                }
            }
        }
    }}
        
    






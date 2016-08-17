using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace ScannableDataEncoding.Core
{
    /// <summary>
    /// Color to represents 1's 
    /// </summary>
    public enum OnColorsOptions
    {
        Black = 1,
        Blue = 2,
        Red = 3
    }

    //Color to represent 0's 
    public enum OffColorsOptions
    {
        White = 1,
        Green = 2
    }

    /// <summary>
    /// Project: ScannableDataEncoding
    /// Authors:
    ///          Jordan Hook (jordan@jvprogramming.com)
    ///          Vinood Persad (vinood@jvprogramming.com) 
    /// Website: http://jvprogramming.com 
    /// Description:
    ///              The ScannableDataEncoding is a POE (Proof of concept) tool used to create simple images that be printed and scanned back into a computer. 
    ///              In order to acheive this data must be converted to Binary values and then out putted as colors in which should be easily identifyable on 
    ///              paper. This is a long process and thus this project is for research and educational purposes and should not be used in a real world, 
    ///              mission critical application! 
    ///              
    ///              The idea behind this project is to create an encoder that will created images based on encrypted data that can be store in real paper files 
    ///              and recovered back into the system when needed without having to manually type it out. E.G. you could save an encrypted source code in a set 
    ///              of paper files and load it back when needed. Burning paper is faster then formatting drives! 
    /// </summary>
    public class ScannableDataEncoding
    {
        /// <summary>
        /// The color in which all 1's will appear in the image
        /// </summary>
        public Color OnColor { get; private set; }

        /// <summary>
        /// The color in which all 0's will appear in the image
        /// </summary>
        public Color OffColor { get; private set; }

        //Undocumented test variables 
        public static int BlockSize { get; set; }
        public static int RowSize { get; set; }

        /// <summary>
        /// Creates a new instance of the encoder / decoder 
        /// </summary>
        /// <param name="on_color">The on color for the image or the color that represents 1's in the binary stream</param>
        /// <param name="off_color">The off color for the image or the color that represents the 0's in the binary stream</param>
        public ScannableDataEncoding(OnColorsOptions on_color, OffColorsOptions off_color)
        {
            //Based on input choose and set an on color 
            switch (on_color)
            {
                case OnColorsOptions.Black:
                    OnColor = Color.Black;
                    break;
                case OnColorsOptions.Blue:
                    OnColor = Color.Blue;
                    break;
                case OnColorsOptions.Red:
                    OnColor = Color.Red;
                    break;
                default: //invalid choice? 
                    throw new Exception("Invalid color choice for the on_color parameter. Please choose from the valid color list.");
            }

            //Based on input choose and set an off color 
            switch (off_color)
            {
                case OffColorsOptions.Green:
                    OffColor = Color.Green;
                    break;
                case OffColorsOptions.White:
                    OffColor = Color.White;
                    break;
                default:
                    throw new Exception("Invalid color choice for the off_color parameter. Please choose from the valid color list."); 
            }

            //set some defaults
            BlockSize = 2;
            RowSize = 8 * 32; 
        }

        /// <summary>
        /// Encodes data into an image format that is printer friendly 
        /// </summary>
        /// <param name="data">The data to format</param>
        /// <returns>Bitmap representation of the data</returns>
        public Bitmap Encode(byte[] data)
        {
            string binary = ByteArrayToString(data);

            //calculate the amount of rows of pixels we are going to use 
            int blockSize = BlockSize; //in pixels 

            //Create some sizes to work with 
            int rowSize = RowSize;

            //Create an image to draw to 
            Bitmap source = new Bitmap(binary.Length * blockSize, ((binary.Length / rowSize)  + 1) * blockSize);

            //create a drawable graphic to draw with 
            Graphics g = Graphics.FromImage(source);

            //Create brushes to use for binary values 
            Brush[] brushSet = new Brush[] { new SolidBrush(OffColor), new SolidBrush(OnColor) };

            //Create variables to count rows and collumns 
            int x = 0, y = 0, z = 0;

            //while there is data left to draw 
            while(z < binary.Length)
            {
                //get the value as a bit 
                int value = int.Parse(binary.Substring(z, 1));

                //draw the value based on current index 
                g.FillRectangle(brushSet[value], new Rectangle(x * blockSize, y * blockSize, blockSize, blockSize));

                //if we reached the column size 
                if(x == rowSize)
                {
                    //reset column count 
                    x = 0;
                    
                    //increase row count 
                    y++; 
                }
                else
                {
                    //otherwise increase column 
                    x++;
                }

                //increase index of data 
                z++;
            }

            //return the drawn image 
            return source; 
        }

        public byte[] Decode(Bitmap source)
        {
            //create a new string to store the binary data in 
            string binary = string.Empty;

            //create a decoder set to map colors and bits 
            Dictionary<int, int> decodeSet = new Dictionary<int, int>();

            //setup the decoder set 
            decodeSet.Add(this.OffColor.ToArgb(), 0);
            decodeSet.Add(this.OnColor.ToArgb(), 1);

            //Loop accross the height of the image 
            for (int y = 0; y < source.Height; y += BlockSize)
            {
                //loop accross the collumns of the image 
                for (int x = 0; x < source.Width; x += BlockSize)
                {
                    //get the current pixel 
                    Color current = source.GetPixel(x, y); 

                    //check if it's a valid color 
                    if(decodeSet.ContainsKey(current.ToArgb()))
                    {
                        //if so append to the string 
                        binary += decodeSet[current.ToArgb()].ToString();
                    }
                    else
                    {
                        //exit the for loop as we are no longer reading possible colours... we are probably at the end of the data 
                        break;
                    }

                }
            }

            //return the decoded data 
            return BinaryStringToByteArray(binary); 
        }

        /// <summary>
        /// Convert the data to a binary string (easier to loop through)... will stop using strings if the concept works... 
        /// </summary>
        /// <param name="raw">the data to convert to binary rep</param>
        /// <returns>The binary representation of the data</returns>
        public static string ByteArrayToString(byte[] raw)
        {
            return string.Concat(raw.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
        }

        /// <summary>
        /// Loop through each bit and convert  it back to bytes
        /// </summary>
        /// <param name="input">the input bit stream</param>
        /// <returns>byte array representation of the binary stream of bits</returns>
        public static byte[] BinaryStringToByteArray(string input)
        {
            int numOfBytes = input.Length / 8; //8 bits to a byte 
            byte[] bytes = new byte[numOfBytes];
            for (int i = 0; i < numOfBytes; ++i)
            {
                //convert each byte 
                bytes[i] = Convert.ToByte(input.Substring(8 * i, 8), 2);
            }
            return bytes; 
        }
    }
}

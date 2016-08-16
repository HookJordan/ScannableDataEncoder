using System;
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
        }

        public Bitmap Encode(byte[] data)
        {
            string binary = ByteArrayToString(data);

            //calculate the amount of rows of pixels we are going to use 
            int blockSize = 2; //in pixels 

            //Create some sizes to work with 
            int rowSize = 8 * 32;

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

        /// <summary>
        /// Convert the data to a binary string (easier to loop through)... will stop using strings if the concept works... 
        /// </summary>
        /// <param name="raw">the data to convert to binary rep</param>
        /// <returns>The binary representation of the data</returns>
        public static string ByteArrayToString(byte[] raw)
        {
            return string.Concat(raw.Select(b => Convert.ToString(b, 2)));
        }
    }
}

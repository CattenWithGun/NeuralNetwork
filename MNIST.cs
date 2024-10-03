using System.Runtime.InteropServices;

namespace MNIST
{
    public static class MNISTFileHandler
    {
        /*
		Array.Copy has parameters like this:
		Source array, Where to start at source array, New array, Where to start at new array, How many elements to copy
		It is used to get a small section of the files to read how many elements in the file there is, but it is unreadable
		*/

        /*
		Image File Format:
		[offset] [type]          [value]          [description]
		0000     32 bit integer  0x00000803(2051) magic number (shows what type of file it is)
		0004     32 bit integer  10000            number of images
		0008     32 bit integer  28               number of rows (height of each image)
		0012     32 bit integer  28               number of columns (width of each image)
		0016     unsigned byte   ??               pixel (0 to 255, 0 meaning white and 255 meaning black)
		0017     unsigned byte   ??               pixel
		0018     unsigned byte   ??               pixel
		........
		xxxx     unsigned byte   ??               pixel
		*/

        public static byte[,,] GetImages(string pathToImageFile)
        {
            //Gets data in the image file
            byte[] imageFile = File.ReadAllBytes(pathToImageFile);

            //Finds the number of images
            byte[] numberOfImagesArray = new byte[4];
            Array.Copy(imageFile, 4, numberOfImagesArray, 0, 4);
            int numberOfImages = ByteArrayToInt(numberOfImagesArray);

            //Finds the width of the images
            byte[] widthOfImagesArray = new byte[4];
            Array.Copy(imageFile, 12, widthOfImagesArray, 0, 4);
            int width = ByteArrayToInt(widthOfImagesArray);

            //Finds the height of the images
            byte[] heightOfImagesArray = new byte[4];
            Array.Copy(imageFile, 8, heightOfImagesArray, 0, 4);
            int height = ByteArrayToInt(heightOfImagesArray);

            //Puts all the images into a 3D array, like a picture book
            byte[,,] images = new byte[numberOfImages, height, width];
            for(int imagesIndex = 0; imagesIndex < numberOfImages; imagesIndex++)
            {
                for(int heightIndex = 0; heightIndex < height; heightIndex++)
                {
                    for(int widthIndex = 0; widthIndex < width; widthIndex++)
                    {
                        images[imagesIndex, heightIndex, widthIndex] = imageFile[784 * imagesIndex + 28 * heightIndex + widthIndex];
                    }
                }
            }
            //For some reason the file is storing the left half of the image on the right, so this method flips that
            images = SplitFlipAll(images);
            return images;
        }

        public static void WriteImage(int index, byte[,,] images)
        {
            string[] rows = new string[28];
            string placeholder = "";
            for(int heightIndex = 0; heightIndex < 28; heightIndex++)
            {
                for(int widthIndex = 0; widthIndex < 28; widthIndex++)
                {
                    placeholder += ConvertToPixel(Convert.ToString(images[index, heightIndex, widthIndex]));
                }
                rows[heightIndex] = placeholder;
                placeholder = "";
            }
            for(int i = 0; i < 28; i++)
            {
                Console.WriteLine(rows[i]);
            }
        }

        private static byte[,,] SplitFlipAll(byte[,,] images)
        {
            for(int imageIndex = 0; imageIndex < images.GetLength(0); imageIndex++)
            {
                for(int heightIndex = 0; heightIndex < 28; heightIndex++)
                {
                    byte[] row = new byte[28];
                    for(int widthIndex = 0; widthIndex < 28; widthIndex++)
                    {
                        row[widthIndex] = images[imageIndex, heightIndex, widthIndex];
                    }
                    row = SplitFlipRow(row);
                    for(int widthIndex = 0; widthIndex < 28; widthIndex++)
                    {
                        images[imageIndex, heightIndex, widthIndex] = row[widthIndex];
                    }
                }
            }
            return images;
        }

        private static byte[] SplitFlipRow(byte[] row)
        {
            byte[] leftSide = new byte[14];
            Array.Copy(row, 0, leftSide, 0, 14);
            byte[] rightSide = new byte[14];
            Array.Copy(row, 14, rightSide, 0, 14);
            byte[] finalArray = new byte[28];
            Array.Copy(rightSide, 0, finalArray, 0, 14);
            Array.Copy(leftSide, 0, finalArray, 14, 14);
            return finalArray;
        }

        public static byte[] ImageToByteArray(byte[,,] images, int index)
        {
            byte[] finalArray = new byte[784];
            for(int heightIndex = 0; heightIndex < 28; heightIndex++)
            {
                for(int widthIndex = 0; widthIndex < 28; widthIndex++)
                {
                    finalArray[heightIndex * 28 + widthIndex] = images[index, heightIndex, widthIndex];
                }
            }
            return finalArray;
        }

        private static string ConvertToPixel(string number)
        {
            byte shade = Convert.ToByte(number);
            if(shade < 51)
            {
                return "█";
            }
            else if(shade >= 51 && shade < 102)
            {
                return "▓";
            }
            else if(shade >= 102 && shade < 153)
            {
                return "▒";
            }
            else if(shade >= 153 && shade < 204)
            {
                return "░";
            }
            return " ";
        }

        /*
		Label File Format:
		[offset] [type]          [value]          [description]
		0000     32 bit integer  0x00000801(2049) magic number (shows what type of file it is)
		0004     32 bit integer  10000            number of labels
		0008     unsigned byte   ??               label (the number that the image represents, 0 to 9)
		0009     unsigned byte   ??               label
		0010     unsigned byte   ??               label
		........
		xxxx     unsigned byte   ??               label
		*/

        public static byte[] GetLabels(string pathToLabelFile)
        {
            //Gets data in the label file
            byte[] labelFile = File.ReadAllBytes(pathToLabelFile);

            //Finds the number of labels
            byte[] numberOfLabelsArray = new byte[4];
            Array.Copy(labelFile, 4, numberOfLabelsArray, 0, 4);
            int numberOfLabels = ByteArrayToInt(numberOfLabelsArray);

            //Puts the labels into an array
            byte[] labels = new byte[numberOfLabels];
            for(int i = 8; i < numberOfLabels + 8; i++)
            {
                labels[i - 8] = labelFile[i];
            }
            return labels;
        }

        //Used to read certain parts of the MNIST database files
        private static int ByteArrayToInt(byte[] bytes)
        {
            //If the computer uses Little Endian, change it to Big Endian because the image and label files are in Big Endian
            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToInt32(bytes, 0);
        }

        //Gets a row from a 2D array
        private static T[] GetRow<T>(this T[,] array, int row)
        {
            if(!typeof(T).IsPrimitive) throw new InvalidOperationException("Not supported for managed types.");
            if(array == null) throw new ArgumentNullException("array");
            int cols = array.GetUpperBound(1) + 1;
            T[] result = new T[cols];
            int size;
            if(typeof(T) == typeof(bool)) size = 1;
            else if(typeof(T) == typeof(char)) size = 2;
            else size = Marshal.SizeOf<T>();
            Buffer.BlockCopy(array, row * cols * size, result, 0, cols * size);
            return result;
        }

        //Gets the expectedValues array with a label
        public static double[] LabelToExpectedValues(byte label)
        {
            double[,] expectedValues =
            {
        {1,0,0,0,0,0,0,0,0,0},
        {0,1,0,0,0,0,0,0,0,0},
        {0,0,1,0,0,0,0,0,0,0},
        {0,0,0,1,0,0,0,0,0,0},
        {0,0,0,0,1,0,0,0,0,0},
        {0,0,0,0,0,1,0,0,0,0},
        {0,0,0,0,0,0,1,0,0,0},
        {0,0,0,0,0,0,0,1,0,0},
        {0,0,0,0,0,0,0,0,1,0},
        {0,0,0,0,0,0,0,0,0,1},
      };
            return GetRow<double>(expectedValues, Convert.ToInt32(label));
        }
    }
}

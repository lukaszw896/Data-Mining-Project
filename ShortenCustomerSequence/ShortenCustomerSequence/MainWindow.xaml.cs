using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShortenCustomerSequence
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShortenData();
        }
        public void ShortenData()
        {
            int numberOfitemsets = 15;
            string line;
            List<string> lines = new List<string>();
            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader("C:\\Users\\lukas\\Desktop\\Data mining\\pumsb.dat");
            Random rnd = new Random();
            int customerCounter = 0;
            while ((line = file.ReadLine()) != null && customerCounter<20000)
            {
                string[] numbers = line.Split(' ');
                int[] randomNumbers = new int[numberOfitemsets];
                string[] randomItemsets = new string[numberOfitemsets];
                for (int j = 0; j < numberOfitemsets; j++) { randomNumbers[j] = -1; }
                int counter = 0;
                while (counter < numberOfitemsets)
                {
                    int random = rnd.Next(0, numbers.Count());
                    int i;
                    for (i = 0; i < counter; i++)
                    {
                        if (randomNumbers[i] == random)
                        {
                            break;
                        }
                    }
                    if(i >= counter){
                        randomNumbers[counter] = random;
                        counter++;
                    }
                }
                for (int i = 0; i < numberOfitemsets-1; i++)
                {
                    for (int j = 0; j < numberOfitemsets-1; j++)
                    {
                        if (randomNumbers[j] > randomNumbers[j + 1])
                        {
                            int tmp = randomNumbers[j];
                            randomNumbers[j] = randomNumbers[j + 1];
                            randomNumbers[j + 1] = tmp;
                        }
                    }
                }
                for (int i = 0; i < numberOfitemsets; i++)
                {
                    randomItemsets[i] = numbers[randomNumbers[i]];
                }
                string customerSeq = " ";
                foreach (string s in randomItemsets)
                {
                    customerSeq = customerSeq + s + " ";
                }
                lines.Add(customerSeq);
                customerCounter++;
            }
            using (StreamWriter outputFile = new StreamWriter("C:\\Users\\lukas\\Desktop\\Data mining\\pumsbShort.dat"))
            {
                foreach (string lin in lines)
                    outputFile.WriteLine(lin);
            }

            file.Close();
        }
    }
}

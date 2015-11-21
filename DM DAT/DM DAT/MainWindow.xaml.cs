using System;
using System.Collections.Generic;
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

namespace DM_DAT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<List<List<int>>> customersSequences;
        public List<List<List<int>>> frequentItemListsOfDifferentLength;
        public double supportPercentage = 0.5;
        public int numberOfCustomersSupport;
        public MainWindow()
        {
            customersSequences = new List<List<List<int>>>();
            frequentItemListsOfDifferentLength = new List<List<List<int>>>();
            InitializeComponent();
            ReadData();
            GenerateFrequentItemsets();
        }

        public void ReadData(){
            int counter = 0;
            string line;

            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader("C:\\Users\\lukas\\Desktop\\Data mining\\pumsb.dat");
            while ((line = file.ReadLine()) != null)
            {
                List<List<int>> customerTransactions = new List<List<int>>();
            //    Console.WriteLine(line);
                string[] customerTransactionsTMP = line.Split(' ');
                foreach (string trans in customerTransactionsTMP)
                {
                    List<int> itemsSequence = new List<int>();
                    foreach (char a in trans)
                    {
                        itemsSequence.Add(int.Parse(a.ToString()));
                    }
                    customerTransactions.Add(itemsSequence);
                }
                customersSequences.Add(customerTransactions);
                counter++;
            }
            numberOfCustomersSupport = (int)((double)counter * supportPercentage);
            Console.WriteLine("Number of customers: " + counter);
            file.Close();
        }

        public void GenerateFrequentItemsets()
        {
            GenereteOneItemFrequentItemSets();
            int a = 0;
            a++;
        }

        public void GenereteOneItemFrequentItemSets()
        {
            frequentItemListsOfDifferentLength.Add(new List<List<int>>());
            for (int i = 0; i < 10; i++)
            {
                int counter = 0;
                foreach(List<List<int>> customerSeq in customersSequences){
                    bool stopCheckingCutomer = false;
                    if (stopCheckingCutomer == false)
                    {
                        foreach (List<int> itemSet in customerSeq)
                        {
                            if (stopCheckingCutomer == false)
                            {
                                foreach (int item in itemSet)
                                {
                                    if (item == i)
                                    {
                                        counter++;
                                        stopCheckingCutomer = true;
                                        break;
                                    }
                                }
                            }  
                        }
                    }
                }
                if (counter >= numberOfCustomersSupport)
                {
                    List<int> tmp = new List<int>();
                    tmp.Add(i);
                    frequentItemListsOfDifferentLength[0].Add(tmp);
                }
            }
        }

        public List<List<int>> Apriori_gen(int k)
        {
            int numberOfItemsets = frequentItemListsOfDifferentLength[k - 2].Count;
            List<List<int>> candidatesK = new List<List<int>>();
            //index is k-2 because we're starting from 0
            for (int i = 0; i < numberOfItemsets; i++)
            {
                for (int j = 0; j < numberOfItemsets; j++)
                {
                    List<int> candidate = new List<int>();
                    
                    //let's check first k-2 items
                    int l;
                    for (l = 0; l < k - 2; l++)
                    {
                        if(frequentItemListsOfDifferentLength[k - 2][i][l]==frequentItemListsOfDifferentLength[k - 2][j][l]){
                            candidate.Add(frequentItemListsOfDifferentLength[k - 2][i][l]);
                        }
                        else
                        {
                            break;
                        }     
                    }
                    if (l >= k - 2)
                    {
                        candidate.Add(frequentItemListsOfDifferentLength[k - 2][i][k - 2]);
                        candidate.Add(frequentItemListsOfDifferentLength[k - 2][j][k - 2]);
                        candidatesK.Add(candidate);
                    }
                }
            }

                CutCandidates(candidatesK, k);

                return candidatesK;
        }

        //function which reduce number of candidates by those which contain subset of items
        //which do not belong to frequent itemset k-1
        public void CutCandidates(List<List<int>> candidatesK, int k){
            foreach (List<int> itemset in candidatesK)
            {
                List<int[]> subsets = new List<int[]>();
                int[] subset = new int[k - 1];

                //I add manualy one subset which is k-1 last items
                for (int i = 0; i < k - 1; i++)
                {
                    subset[i] = itemset[i+1];
                }
                subsets.Add(subset);

                //here I add the rest of possible subsets
                for (int i = 1; i < k; i++)
                {
                    int index = 1;
                    subset = new int[k - 1];
                    subset[0] = itemset[0];
                    for (int j = 1; j < k; j++)
                    {
                        if (j != i)
                        {
                            subset[index] = itemset[j];
                            index++;
                        }
                    }
                    subsets.Add(subset);
                }

                //I've got all subsets. Now I'll check whether they are in the k-1 frequent
                //list
                bool areAllSubsetsFrequent = true;
                foreach (int[] ss in subsets)
                {
                    bool isFrequentSubset = false;
                    foreach (List<int> fs in frequentItemListsOfDifferentLength[k - 2])
                    {
                        int i;
                        for (i = 0; i < k - 1; i++)
                        {
                            if (fs[i] != ss[i])
                            {
                                break;
                            }
                        }
                        //if we didn't break the loop than value of i should equal to k-1
                        //which means that checked set was the same as some from frequent itemset
                        if (i == k - 1)
                        {
                            isFrequentSubset = true;
                            break;
                        }
                    }
                    if (isFrequentSubset == false)
                    {
                        areAllSubsetsFrequent = false;
                        break;
                    }
                }
                if (areAllSubsetsFrequent == false)
                {
                    candidatesK.Remove(itemset);
                }
            }
        }

        //public void Generate
    }
}

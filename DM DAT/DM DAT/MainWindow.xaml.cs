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
        public List<List<int>> frequentItemsetList;
        public List<List<List<int>>> largeSequences;
        public double supportPercentage = 0.8;
        public int numberOfCustomersSupport;
        public MainWindow()
        {
            customersSequences = new List<List<List<int>>>();
            frequentItemListsOfDifferentLength = new List<List<List<int>>>();
            frequentItemsetList = new List<List<int>>();
            largeSequences = new List<List<List<int>>>();
            InitializeComponent();
            ReadData();
            GenerateFrequentItemsets();
            
            Transform();

            largeSequences.Add(new List<List<int>>());
            for (int i = 0; i < frequentItemsetList.Count; i++)
            {
                List<int> lengthOneSequence = new List<int>();
                lengthOneSequence.Add(i);
                largeSequences[0].Add(lengthOneSequence);
            }

                

            GenerateFrequentSequences();

            int a = 0;
            a++;


        }

        public void ReadData(){
            int counter = 0;
            string line;

            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader("C:\\Users\\lukas\\Desktop\\Data mining\\mushroom.dat");
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
            int kSeq = 2;
            do
            {
                List<List<int>> candidates = Apriori_gen(kSeq);
                AddCandidatesToFrequentList(candidates, kSeq);
                kSeq++;
            } while (frequentItemListsOfDifferentLength[kSeq - 2].Count > 0);
        }

        //We may speed up here if it will be necessary
        // ---if checked sequence is longer than some item set we are not checking it
        // -----if number of items left is smaller than the number of letters we need - break
        public void AddCandidatesToFrequentList(List<List<int>> candidates, int k)
        {
            frequentItemListsOfDifferentLength.Add(new List<List<int>>());
            for (int i = 0; i < candidates.Count; i++)
            {
                int counter = 0;
                foreach (List<List<int>> customerSeq in customersSequences)
                {
                    bool stopCheckingCutomer = false;
                    if (stopCheckingCutomer == false)
                    {
                        foreach (List<int> itemSet in customerSeq)
                        {
                                int itemCounter = 0;
                                if (itemSet.Count >= k)
                                {
                                    for (int j = 0; j < itemSet.Count;j++)
                                    //    foreach (int item in itemSet)
                                        {
                                            if (itemCounter == k)
                                            {
                                                break;
                                            }
                                            if (itemSet[j] == candidates[i][itemCounter])
                                            {
                                                itemCounter++;
                                            }
                                            //if number of items left is smaller than the number of letters we need - break
                                            if (itemSet.Count - j < k - itemCounter)
                                            {
                                                break;
                                            }
                                        }
                                    if (itemCounter == k)
                                    {
                                        counter++;
                                        stopCheckingCutomer = true;
                                        break;
                                    }
                                }
                        }
                    }
                }
                if (counter >= numberOfCustomersSupport)
                {
                    List<int> tmp = new List<int>();
                    for (int s = 0; s < k; s++)
                    {
                        tmp.Add(candidates[i][s]);
                    }
                    frequentItemListsOfDifferentLength[k-1].Add(tmp);
                }
            }
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
            foreach (List<int> itemset in candidatesK.ToList())
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

        public void Transform()
        {
            MakeSingleItemList();

            for (int i = 0; i < customersSequences.Count; i++)
            {
                for (int j = 0; j < customersSequences[i].Count; j++)
                {
                    List<int> tmpTransaction = new List<int>(customersSequences[i][j]);
                    customersSequences[i][j].Clear();
                    for (int l = 0; l < frequentItemsetList.Count; l++)
                    {
                        int litemsetLength = frequentItemsetList[l].Count;
                        if (litemsetLength <= tmpTransaction.Count)
                        {
                            int litemsetCounter = 0;
                            foreach (int item in tmpTransaction)
                            {
                                if (item == frequentItemsetList[l][litemsetCounter])
                                {
                                    litemsetCounter++;
                                }
                                if (litemsetCounter == frequentItemsetList[l].Count)
                                {
                                    break;
                                }
                            }
                            if (litemsetCounter == frequentItemsetList[l].Count)
                            {
                                customersSequences[i][j].Add(l);
                            }
                        }
                    }
                    if (customersSequences[i][j].Count == 0)
                    {
                        customersSequences[i].Remove(customersSequences[i][j]);
                    }
                }
                if (customersSequences[i].Count == 0)
                {
                    customersSequences.Remove(customersSequences[i]);
                }
            }
            
        }

        public void MakeSingleItemList()
        {
            for (int i = 0; i < frequentItemListsOfDifferentLength.Count; i++)
            {
                for (int j = 0; j < frequentItemListsOfDifferentLength[i].Count; j++)
                {
                    frequentItemsetList.Add(frequentItemListsOfDifferentLength[i][j]);
                }
            }
        }

        /******************************************************************/
        /********** GENERATING SEQUENCES **********************************/
        /******************************************************************/

        public void GenerateFrequentSequences()
        {
            int kSeq = 2;
            do
            {
                List<List<int>> candidates = SeqCandidateGenerator(kSeq);

                AddSequenceCandidatesToFrequentList(candidates, kSeq);
               // AddCandidatesToFrequentList(candidates, kSeq);
                kSeq++;
            } while (largeSequences[kSeq - 2].Count > 0);
        }

        private List<List<int>> SeqCandidateGenerator(int k)
        {
            int numOfLargeSeq = largeSequences[k - 2].Count;
            List<List<int>> candidatesK = new List<List<int>>();
            //index is k-2 because we're starting from 0
            for (int i = 0; i < numOfLargeSeq; i++)
            {
                for (int j = 0; j < numOfLargeSeq; j++)
                {
                    List<int> candidate = new List<int>();

                    //let's check first k-2 items
                    int l;
                    for (l = 0; l < k - 2; l++)
                    {
                        if (largeSequences[k - 2][i][l] == largeSequences[k - 2][j][l])
                        {
                            candidate.Add(largeSequences[k - 2][i][l]);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (l >= k - 2)
                    {
                        candidate.Add(largeSequences[k - 2][i][k - 2]);
                        candidate.Add(largeSequences[k - 2][j][k - 2]);
                        candidatesK.Add(candidate);
                    }
                }
            }

            CutSequenceCandidates(candidatesK, k);

            return candidatesK;
        }

        private void CutSequenceCandidates(List<List<int>> candidatesK, int k)
        {
            foreach (List<int> itemset in candidatesK.ToList())
            {
                List<int[]> subsets = new List<int[]>();
                int[] subset = new int[k - 1];

                //I add manualy one subset which is k-1 last items
                for (int i = 0; i < k - 1; i++)
                {
                    subset[i] = itemset[i + 1];
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
                    foreach (List<int> fs in largeSequences[k - 2])
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

        public void AddSequenceCandidatesToFrequentList(List<List<int>> candidates, int k)
        {
             largeSequences.Add(new List<List<int>>());
             int counter = 0;
            for (int i = 0; i < candidates.Count; i++)
            {
                counter = 0;
                bool stopCheckingCustomer = false;
                foreach (List<List<int>> customerSeq in customersSequences)
                {
                    if (stopCheckingCustomer == false)
                    {
                        int itemCounter = 0;
                        foreach (List<int> setOfItemSets in customerSeq)
                        {
                            foreach (int itemSet in setOfItemSets)
                            {
                                if (itemSet == candidates[i][itemCounter])
                                {
                                    itemCounter++;
                                    break;
                                }
                            }
                            if (itemCounter == k)
                            {
                               // stopCheckingCustomer = true;
                                counter++;
                                break;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (counter >= numberOfCustomersSupport)
                {
                    List<int> tmp = new List<int>();
                    for (int s = 0; s < k; s++)
                    {
                        tmp.Add(candidates[i][s]);
                    }
                    largeSequences[k - 1].Add(tmp);
                }
            }
        }

    }
}

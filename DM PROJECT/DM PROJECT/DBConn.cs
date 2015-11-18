using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_PROJECT
{
    public static class  DBConn
    {
        public async static Task<int> GetCustomerCount()
        {
            return await Task.Run(() =>
            {
                DataClasses1DataContext ds = new DataClasses1DataContext();

                var counter = ds.Customers.Count();

                ds.Dispose();
                return counter;
            });
        }

        public async static Task<int> GetNumberOfProducts()
        {
            return await Task.Run(() =>
            {
                DataClasses1DataContext ds = new DataClasses1DataContext();

                var counter = ds.Products.Count();

                ds.Dispose();
                return counter;
            });
        }

        public async static Task<List<List<List<int>>>> GetAllData()
        {
            List<List<List<int>>> list = new List<List<List<int>>>();
            return await Task.Run(() =>
            {
                List<string> customersID = new List<string>();
                DataClasses1DataContext ds = new DataClasses1DataContext();

                customersID = (from a in ds.Customers select a.CustomerID).ToList();
                int ba = 3;
                foreach (string customer in customersID)
                {
                    List<List<int>> productsOfOrdersList = new List<List<int>>();
                    List<int> ordersID = new List<int>();
                    ordersID = (from a in ds.Orders where a.CustomerID == customer select a.OrderID).ToList();
                    foreach (int order in ordersID)
                    {
                        List<int> productsID = new List<int>();
                        productsID = (from a in ds.Order_Details where a.OrderID == order select a.ProductID).ToList();
                        productsOfOrdersList.Add(productsID);
                    }
                    list.Add(productsOfOrdersList);                  
                }              
                return list;
            });
        }

        public async static Task<List<List<List<int>>>> GetAllItemSets(List<List<List<int>>> list)
        {
            int numOfProd = await DBConn.GetNumberOfProducts();
            int numOfCustomers = list.Count;
            List<List<List<int>>> itemSets = new List<List<List<int>>>();
            for (int i = 0; i < 10; i++)
            {
                itemSets.Add(new List<List<int>>());
            }
                return await Task.Run(() =>
                {   
                    //SZUKAM 1-ITEMSET
                    //sprawdzam support dla każdego itemu
                    for (int i = 1; i < numOfProd+1; i++)
                    {
                        int supportCounter = 0;
                        //support Counter wzrasta gdy item pojawił się chociaż w jednej transakcji klienta
                        for (int j = 0; j < numOfCustomers; j++)
                        {
                            bool isStop = false;
                            //sprawdzam każdy order. Jeżeli już w którymś obrocie dałem supportCounter ++ to chcę przerwać pętlę
                            for (int k = 0; k < list[j].Count; k++)
                            {
                                //sprawdzam każdy item
                                foreach (int item in list[j][k])
                                {
                                    if (item == i)
                                    {
                                        supportCounter++;
                                        isStop = true;
                                        break;
                                    }
                                }
                                if (isStop)
                                {
                                    break;
                                }
                            }
                            if (supportCounter > 1)
                            {
                                List<int> tmpList = new List<int>();
                                tmpList.Add(i);
                                itemSets[0].Add(tmpList);
                                break;
                            }
                        }
                    }
                    //SZUKAM 2-ITEMSET
                    for (int i = 0; i < itemSets[0].Count; i++)
                    {
                        //j = i + because I dont want to check the same pairs like 1,2 - 2,1
                        for (int j = i+1; j < itemSets[0].Count; j++)
                        {
                            int[] twoItems = new int[2];
                            int supportCounter = 0;
                            //support Counter wzrasta gdy item pojawił się chociaż w jednej transakcji klienta
                            for (int k = 0; k < numOfCustomers; k++)
                            {
                                bool isStop = false;
                                //sprawdzam każdy order. Jeżeli już w którymś obrocie dałem supportCounter ++ to chcę przerwać pętlę
                                for (int l = 0; l < list[k].Count; l++)
                                {
                                    twoItems[0] = i;
                                    twoItems[1] = j;
                                    //sprawdzam każdy item
                                    foreach (int item in list[k][l])
                                    {
                                        if (twoItems[0] == item)
                                        {
                                            twoItems[0] = -1;
                                        }
                                        else if (twoItems[1] == item)
                                        {
                                            twoItems[1] = -1;
                                        }
                                        if (twoItems[0] == -1 && twoItems[1] == -1)
                                        {
                                            supportCounter++;
                                            isStop = true;
                                            break;
                                        }
                                    }
                                    if (isStop)
                                    {
                                        break;
                                    }
                                }
                                if (supportCounter >= 1)
                                {
                                    List<int> tmpList = new List<int>();
                                    tmpList.Add(i);
                                    tmpList.Add(j);
                                    itemSets[1].Add(tmpList);
                                    break;
                                }
                            }
                        }
                    }

                    for (int i = 2; i < itemSets.Count; i++)
                    {
                        //liczę dla każdego itemsetu o długości 2
                        for (int j = 0; j < itemSets[i-1].Count; j++)
                        {
                            
                            //sprawdzam po kolei każdy element k-1 setu czy zaczyna się sekwencją powyżej dopóki któryś element jej nie posiada.
                            for (int m = j + 1; m < itemSets[i - 1].Count; m++)
                            {
                                List<int> firstElementsOfItemSet = new List<int>();
                                for (int k = 0; k < i - 1; k++)
                                {
                                    firstElementsOfItemSet.Add(itemSets[i - 1][j][k]);
                                }
                                bool endOfChecking = false;
                                //for each element in beginning sequence:
                                for (int n = 0; n < i - 1; n++)
                                {
                                    if (itemSets[i - 1][m][n] == firstElementsOfItemSet[n])
                                    {
                                        endOfChecking = true;
                                        break;
                                    }
                                }
                                //nie chcę dalej sprawdzać danego item setu jeżeli natrafiłem na item set bez tej samej poczatkowej sekwencji
                                if (endOfChecking)
                                {
                                    break;
                                }
                                //dodam końcowe elementy obu setów do listy która trzymała mi początkąwą sekwencję
                                firstElementsOfItemSet.Add(itemSets[i - 1][j][i - 1]);
                                firstElementsOfItemSet.Add(itemSets[i - 1][m][i - 1]);
                                for (int s1 = 0; s1 < 3; s1++)
                                {
                                    if (firstElementsOfItemSet[s1] == 28)
                                    {
                                        for (int s2 = 0; s2 < 3; s2++)
                                        {
                                            if (firstElementsOfItemSet[s2] == 39)
                                            {
                                                for (int s3 = 0; s3 < 3; s3++)
                                                {
                                                    if (firstElementsOfItemSet[s3] == 46)
                                                    {
                                                        s3++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                ////////////////////////////////////////////////////////
                                ////             itemset checking                   ////
                                ////////////////////////////////////////////////////////
                              /*  if (firstElementsOfItemSet.find == 11 && firstElementsOfItemSet[1] == 42 && firstElementsOfItemSet[2] == 72)
                                {
                                    break;
                                }*/
                                int supportCounter = 0;
                                //support Counter wzrasta gdy item pojawił się chociaż w jednej transakcji klienta
                                for (int o = 0; o < numOfCustomers; o++)
                                {
                                    List<int> tmpItemList = new List<int>(firstElementsOfItemSet);
                                    bool isStop = true ;
                                    //sprawdzam każdy order. Jeżeli już w którymś obrocie dałem supportCounter ++ to chcę przerwać pętlę
                                    for (int p = 0; p < list[o].Count; p++)
                                    {
                                        //sprawdzam każdy item
                                        foreach (int item in list[o][p])
                                        {
                                            for (int r = 0; r < tmpItemList.Count; r++)
                                            {
                                                if (tmpItemList[r] == item)
                                                {
                                                    tmpItemList[r] = -1;
                                                }
                                            }

                                            for (int r = 0; r < tmpItemList.Count; r++)
                                            {
                                                if (tmpItemList[r] != -1)
                                                {
                                                    isStop = false;
                                                    break;
                                                }
                                            }
                                            if (isStop)
                                            {
                                                supportCounter++;
                                                break;
                                            }
    
                                        }
                                        if (isStop)
                                        {
                                            break;
                                        }
                                    }
                                    if (supportCounter >= 1)
                                    {
                                        List<int> tmpList = new List<int>();
                                        for (int r = 0; r < firstElementsOfItemSet.Count; r++)
                                        {
                                            tmpList.Add(firstElementsOfItemSet[r]);
                                        }
                                        itemSets[i].Add(tmpList);
                                        break;
                                    }
                                }


                                ////////////////////////////////////////////////////////
                                ////////////////////////////////////////////////////////

                            }
                        }
                    }

                        return itemSets;
                });
        }
    }
}

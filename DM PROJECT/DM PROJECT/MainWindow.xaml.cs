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

namespace DM_PROJECT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Calculate();
        }
        public async void Calculate()
        {
            int count = await DBConn.GetCustomerCount();
            List<List<List<int>>> list = await DBConn.GetAllData();
            List<List<List<int>>> itemSets = await DBConn.GetAllItemSets(list);
            count = count + 1;
        }
    }
}

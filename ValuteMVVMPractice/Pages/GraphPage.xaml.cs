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
using ValuteMVVMPractice.ViewModels;
using OxyPlot;
using OxyPlot.Series;


namespace ValuteMVVMPractice.Pages
{
    /// <summary>
    /// Interaction logic for GraphPage.xaml
    /// </summary>
    public partial class GraphPage : Page
    {
        public static DynamicValCursViewModel dvcvm;
        public GraphPage()
        {
            InitializeComponent();
            dvcvm = new DynamicValCursViewModel(Convert.ToDateTime(DateTime.Now.Subtract(new TimeSpan(30,0,0,0)).ToString("dd/MM/yyyy")), Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy")), "R01235"); //По умолчанию доллар
            DataContext = dvcvm;
            ValuteBox.ItemsSource = MainWindow.cvm.Valute;
            ValuteBox.DisplayMemberPath = "CharCode";
            ValuteBox.SelectedValuePath = "ID";

            CurrencyPlot.Model = dvcvm.GetPlot();


        }

        private void ValuteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrencyPlot.Model = dvcvm.GetPlot();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dvcvm.DateRange1 < dvcvm.DateRange2)
            {
                CurrencyPlot.Model = dvcvm.GetPlot();
            }
            else
            {
                MessageBox.Show("Дата начала не может быть позже дата окончания", "", MessageBoxButton.OK, MessageBoxImage.Stop);
            }

        }
    }
}

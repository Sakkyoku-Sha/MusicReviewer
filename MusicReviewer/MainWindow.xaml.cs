
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
using DatabaseManagement;

namespace MusicReviewerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public LocalDataManager LocalDataManager;
  
        public MainWindow()
        {           
            InitializeComponent();
            LocalDataManager = new LocalDataManager();         
        }
        private void MainWindow_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void ReviewClick(object sender, RoutedEventArgs e)
        {
            Main.Content = LocalDataManager.RequestPage(LocalDataManager.PageTypes.ReviewPage);
        }

        private void SortButtonClk(object sender, RoutedEventArgs e)
        {
            Main.Content = LocalDataManager.RequestPage(LocalDataManager.PageTypes.SortPage);           
        }

        private void TagBtnClk(object sender, RoutedEventArgs e)
        {
            Main.Content = LocalDataManager.RequestPage(LocalDataManager.PageTypes.TagPage); 
        }

        private void StatBtnClk(object sender, RoutedEventArgs e)
        {
            Main.Content = LocalDataManager.RequestPage(LocalDataManager.PageTypes.StatPage); 
        }

        private void ExitBtnClk(object sender, RoutedEventArgs e)
        {
            this.MainWindow1.Close();
        }     
    }
}

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
using Microsoft.Win32;
using DatabaseManagement;

namespace MusicReviewerApp
{
    /// <summary>
    /// Interaction logic for SearchPage.xaml
    /// </summary>
    public partial class SortPage : Page
    {
        public List<Review_Reference> Local_Reviews;

        private LocalDataManager LocalData;
        private string Current_Sort;
        public SortPage(LocalDataManager LocalData)
        {
            InitializeComponent();
            this.LocalData = LocalData;
            this.Local_Reviews = this.LocalData.DBManager.GetAllReviews();

            this.Local_Reviews.Sort((r1, r2) => r1.Title.CompareTo(r2.Title));//default organization is by title. 
            Current_Sort = "Title";

            this.TableView.ItemsSource = this.Local_Reviews;
        }
        private void TableView_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView View = (sender as ListView);

            if(View.SelectedItem is Review_Reference)
            {
                ReviewObject Review_To_Edit = this.LocalData.DBManager.GetReview((View.SelectedItem as Review_Reference));
                Review_To_Edit.setTags(LocalData.DBManager.GetTagsForReview(View.SelectedItem as Review_Reference));

                if (Review_To_Edit != null) //This can happen if you double on the main page but not on a row of the table.
                {
                    Edit_Window Edit_Window = new Edit_Window(this.LocalData, Review_To_Edit);
                    Edit_Window.ShowDialog();

                    Edit_Window = null;
                    GC.Collect();
                    GC.WaitForFullGCComplete();
                }

                Review_To_Edit = null;
            }        
        }
        private void TableView_Header_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader Header_Clicked = e.OriginalSource as GridViewColumnHeader; 
            if(Header_Clicked != null)
            {
                if((Header_Clicked.Content as string).Equals(Current_Sort)) //Reverse the list if selected again
                {        
                    Local_Reviews.Reverse();
                    this.TableView.Items.Refresh(); //Since the memory reference to the
                }
                else
                {
                    switch (Header_Clicked.Content as string)
                    {
                        case "Title":
                            Current_Sort = "Title";
                            Local_Reviews.Sort((r1, r2) => r1.Title.CompareTo(r2.Title));                                                  
                            break;

                        case "Album":
                            Current_Sort = "Album";
                            Local_Reviews.Sort((r1, r2) => r1.Album.CompareTo(r2.Album));
                            break;

                        case "Rating":
                            Current_Sort = "Rating";
                            Local_Reviews.Sort((r1, r2) => r1.Rating.CompareTo(r2.Rating));                           
                            break;

                        case "Artist":
                            Current_Sort = "Artist";
                            Local_Reviews.Sort((r1, r2) => r1.Artist.CompareTo(r2.Artist));
                            break;

                        case "Release Date":
                            Current_Sort = "Release Date";
                            Local_Reviews.Sort((r1, r2) => r1.Release_Date.CompareTo(r2.Release_Date));
                            break;

                        case "Review Date":
                            Current_Sort = "Review Date";
                            Local_Reviews.Sort((r1, r2) => r1.Review_Date.CompareTo(r2.Review_Date));
                            break;

                    }
                }

                this.TableView.Items.Refresh();                  
            }         
        }
        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {    
            Review_Reference selectedReview = this.TableView.SelectedItem as Review_Reference;

            if(selectedReview == null) { return; }

            this.Local_Reviews.Remove(selectedReview);
            this.LocalData.DBManager.deleteReview(selectedReview);

            this.TableView.Items.Refresh();

            selectedReview = null;
        }

        private void AdvanceSearch_Button_Click(object sender, RoutedEventArgs e)
        {
            AdvanceSearchWindow advWindow = new AdvanceSearchWindow(this.LocalData, this);         
            advWindow.ShowDialog();
            advWindow.Close();
            advWindow = null;

            this.TableView.ItemsSource = null;

            GC.Collect(); //It just annoys me that the information iisn't immediately collected so I force it to be as it closes.
            GC.WaitForFullGCComplete();
          
            this.TableView.ItemsSource = this.Local_Reviews;
            this.TableView.Items.Refresh();
        }
        private void Refresh_Button_Click(object sender, RoutedEventArgs e)
        {
            this.TableView.Items.Refresh();
         
        }       
    }
}

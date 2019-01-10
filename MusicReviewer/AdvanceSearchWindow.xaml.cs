using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DatabaseManagement;

namespace MusicReviewerApp
{
    /// <summary>
    /// Interaction logic for AdvanceSearch.xaml
    /// </summary>
    /// 

    public partial class AdvanceSearchWindow : Window
    {
        private SortPage Parent_Page;
        private SQL_Review_REQUEST Request;
        private List<List<TagObject>> IncludedTagRequest;
        private List<List<TagObject>> ExcludedTagRequest;
        StringBuilder SB;
        LocalDataManager LocalData;

        public AdvanceSearchWindow(LocalDataManager Data, SortPage sortPage)
        {
            this.Parent_Page = sortPage;
            InitializeComponent();

            LocalData = Data;
            Request = new SQL_Review_REQUEST();
       
            IncludedTagRequest = new List<List<TagObject>>();
            ExcludedTagRequest = new List<List<TagObject>>();

            IncludedTagRequest.Add(null);
            ExcludedTagRequest.Add(null);

            SB = new StringBuilder();
        }
        public void AddToIncludedTags(List<TagObject> Tags){
         
            IncludedTagRequest[IncludedTagRequest.Count - 1] = Tags;

            Update_Request_Page();
        }
        public void AddToExcludedTags(List<TagObject> Tags){
           
            ExcludedTagRequest[ExcludedTagRequest.Count - 1] = Tags;

            Update_Request_Page();
        }
       
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (validForm())
            {
                if (!Release_Date_Min_Box.Text.Equals("")) { Request.ReleaseYearMax = int.Parse(Release_Date_Min_Box.Text); }
                if (!Release_Date_Max_Box.Text.Equals("")) { Request.ReleaseYearMin = int.Parse(Release_Date_Max_Box.Text); }
                if (!Review_Date_Min_Box.Text.Equals(""))  { Request.ReviewYearMax = int.Parse(Review_Date_Min_Box.Text); }
                if (!Review_Date_Max_Box.Text.Equals(""))  { Request.ReviewYearMin = int.Parse(Review_Date_Max_Box.Text); }
                if (!Rating_Min_Box.Text.Equals(""))       { Request.RatingMax = int.Parse(Rating_Min_Box.Text); } 
                if (!Rating_Max_Box.Text.Equals(""))       { Request.RatingMin = int.Parse(Rating_Max_Box.Text); }

                Request.Album = Album_Box.Text;
                Request.Artist = Artist_Box.Text;

                Request.Included_Tags = IncludedTagRequest;
                Request.Excluded_Tags = ExcludedTagRequest;

                Parent_Page.Local_Reviews = null;
                Parent_Page.Local_Reviews = LocalData.DBManager.getReviews(Request);               
                
                this.Close();
            }
            else
            {
                System.Windows.MessageBox.Show("Invalid Form");
            }           
        }
        //Validates the form
        private bool validForm()
        {      
            int temp;
            if(!Release_Date_Min_Box.Text.Equals("") && !int.TryParse(Release_Date_Min_Box.Text, out temp))  {return false;}
            if(!Release_Date_Max_Box.Text.Equals("") && !int.TryParse(Release_Date_Max_Box.Text, out temp))  {return false;}
            if(!Review_Date_Min_Box.Text.Equals("")  && !int.TryParse(Review_Date_Min_Box.Text, out temp))   {return false;}
            if(!Review_Date_Max_Box.Text.Equals("")  && !int.TryParse(Review_Date_Max_Box.Text, out temp))   {return false;}
            if(!Rating_Min_Box.Text.Equals("")       && !int.TryParse(Rating_Min_Box.Text, out temp))        {return false;}
            if(!Rating_Max_Box.Text.Equals("")       && !int.TryParse(Rating_Max_Box.Text, out temp))        {return false;}

            return true;
        }
        private void Return_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Text = "";
        }
        private void IncludeTagButton_Click(object sender, RoutedEventArgs e)
        {
            TagginWindow newWindow = new TagginWindow(LocalData, this, IncludedTagRequest[IncludedTagRequest.Count-1], true);           
            newWindow.ShowDialog();

            newWindow = null;
            GC.Collect();
            GC.WaitForFullGCComplete();

            Update_Request_Page();
        }
        private void ExcludeTagButton_Click(object sender, RoutedEventArgs e)
        {           
            TagginWindow newWindow = new TagginWindow(LocalData, this, ExcludedTagRequest[ExcludedTagRequest.Count-1], false);
            newWindow.ShowDialog();

            newWindow = null;
            GC.Collect();
            GC.WaitForFullGCComplete();

            Update_Request_Page();
        }
        private void Update_Request_Page()
        {
            Request_Box.Clear();
            SB.Clear();

            bool HasInclude = false;
            
            for (int x = 0; x < IncludedTagRequest.Count; x++)
            {
                if (x > 0){SB.Append("\nOR\n");}

                if(IncludedTagRequest[x] != null)
                {
                    HasInclude = true;
                    SB.Append("Has All: [");

                    foreach(TagObject Tag in IncludedTagRequest[x]) { SB.Append(Tag.Name + ", "); }
                    SB.Remove(SB.Length-2, 2);

                    SB.Append("]");
                }
                if(ExcludedTagRequest[x] != null)
                {
                    if (HasInclude) { SB.Append(" AND "); }
                    SB.Append("Does not have all of: [");
                    foreach (TagObject Tag in ExcludedTagRequest[x]) { SB.Append(Tag.Name + ", "); }
                    SB.Remove(SB.Length-2, 2);
                    SB.Append("]");
                }
                HasInclude = false;             
            }
            Request_Box.Text = SB.ToString();
           
        }
        public void SetArtist(string artist)
        {
            Artist_Box.Text = artist; 
        }   
        public void SetAlbum(string album)
        {
            Album_Box.Text = album;
        }
        private void Or_Button_Click(object sender, RoutedEventArgs e)
        {
            if(IncludedTagRequest[IncludedTagRequest.Count-1] == null && ExcludedTagRequest[ExcludedTagRequest.Count-1] == null) { return;}

            IncludedTagRequest.Add(null);
            ExcludedTagRequest.Add(null);

            SB.Append("\nOR ");
            Request_Box.Text = SB.ToString();
        }
        private void Artist_Button_Click(object sender, RoutedEventArgs e)
        {
            Window AddArtistWindow = new ElementListWindow(this.LocalData, Review_Fields.Artist, this)
            {
                Title = "Artist Names"
            };
            AddArtistWindow.ShowDialog();
            AddArtistWindow.Close();
            AddArtistWindow = null;
            GC.Collect();
            GC.WaitForFullGCComplete();
        }
        private void Album_Button_Click(object sender, RoutedEventArgs e)
        {
            Window AddAlbumWindow = new ElementListWindow(this.LocalData, Review_Fields.Album, this)
            {
                Title = "Album Titles"
            };
            AddAlbumWindow.ShowDialog();
            AddAlbumWindow.Close();
            AddAlbumWindow = null;
            GC.Collect();
            GC.WaitForFullGCComplete();
        }      
    }
}

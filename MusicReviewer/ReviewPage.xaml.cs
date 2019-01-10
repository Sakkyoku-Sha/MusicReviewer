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
using System.Drawing;
using TagLib;
using DatabaseManagement;

namespace MusicReviewerApp
{
    /// <summary>
    /// Interaction logic for ReviewPage.xaml
    /// </summary>DatebaseAccess
    public partial class ReviewPage : Page
    {
        public ReviewObject currentReview;
        public ReviewObject oldReview;

        private LocalDataManager LocalData;
        private Edit_Window Parent_Window;

        public ReviewPage(LocalDataManager LocalData)
        { 
            InitializeComponent();
            this.LocalData = LocalData;          
            Submit_Button.Visibility = Visibility.Visible;
            Update_Button.Visibility = Visibility.Hidden;
          
            oldReview = new ReviewObject();
            currentReview = new ReviewObject();

            
        }

        //For the case that you are selecting a review to edit in the sort page.
        public ReviewPage(LocalDataManager LocalData, ReviewObject Review_To_Edit, Edit_Window Edit_Window)
        {
            InitializeComponent();
            this.LocalData = LocalData;
            Parent_Window = Edit_Window;
            
            //Make the right buuttons visible.
            Submit_Button.Visibility = Visibility.Hidden;
            Update_Button.Visibility = Visibility.Visible;

            //Since this is the update constructor we need to store old info.         
            oldReview = new ReviewObject(Review_To_Edit);
            currentReview = Review_To_Edit;
        
            //Set the field values ot be equual to the review so you are "viewing" the contents of the review.
            TitleBox.Text = Review_To_Edit.Title;
            AlbumName.Text = Review_To_Edit.Album;
            YearBox.Text = Review_To_Edit.Release_Date.ToString();
            Artist_NameBox.Text = Review_To_Edit.Artist;
            ReviewBox.Text = Review_To_Edit.Review;
            FileNameLabel.Content = Review_To_Edit.File_Path;
            ScoreBox.Text = Review_To_Edit.Rating.ToString();

            //Update the Tags list;
            UpdateReviewTags();
        }       
        private void Submit(object sender, RoutedEventArgs e)
        {           
            if (this.ValidForm())
            {       
                //Set the fields from the form to the review
                SetCurrentReview();

                //Submit the Review.
                try  {this.LocalData.AddReview(this.currentReview);}
                catch(SQL_Exception Exception){Handle_Exception(Exception); return; }

                ClearReviewPage();

                SearchError.Foreground = System.Windows.Media.Brushes.Green;
                SearchError.Content = "Successfully Submitted Review";
            }
            else
            {
                SearchError.Foreground = System.Windows.Media.Brushes.Red;
                SearchError.Content = "Lacking key information or score is ill formatted";
            }
        }

        private void Handle_Exception(SQL_Exception exception)
        {
            switch (exception.Error_Type)
            {
                case SQL_Exception_Type.Unique_Failed:
                    SearchError.Foreground = System.Windows.Media.Brushes.Red;
                    SearchError.Content = "Review for this song already exists in the database";
                    break;
            }
        }

        private void Update()
        {
            if (ValidForm())
            {
                SearchError.Foreground = System.Windows.Media.Brushes.Green;
                SearchError.Content = "Review Updated";
                UpdateReviewInDataBase(); ///TODO -- This should be a boolean function 
            }
            else
            {
                SearchError.Foreground = System.Windows.Media.Brushes.Red;
                SearchError.Content = "Lacking key information or score is ill formatted";
            }
        }
        private void UpdateReviewInDataBase()
        {
            SetCurrentReview();
            this.LocalData.DBManager.UpdateReview(this.oldReview, this.currentReview);
        }

        private bool ValidForm()
        {
            float f = 0;

            //Title Cannot be Empty
            if (TitleBox.Text == "") return false;

            //Rating cannot be empty
            if (ScoreBox.Text == "") return false;

            //Score must be a float/Number 
            if (!(float.TryParse(ScoreBox.Text, out f))) return false;

            //Score must be between 0-100
            f = float.Parse(ScoreBox.Text);
            if ((f > 100 || f < 0)) return false;

            return true;
        }
        private void ClearReviewPage()
        {
            this.currentReview.Clear();

            GenresBox.Document.Blocks.Clear();
            InstrumentsBox.Document.Blocks.Clear();
            LanguagesBox.Document.Blocks.Clear();
          
            TitleBox.Text = "";
            YearBox.SelectedIndex = 0;
            SearchError.Content = "";
            ReviewBox.Text = "";
            Artist_NameBox.Text = "";
            AlbumName.Text = "";
            FileNameLabel.Content = "";
            ScoreBox.Text = "";
        }

        private void YearBox_Initialized(object sender, EventArgs e)
        {
            int currentYear = System.DateTime.Today.Year; 

            for(int x = currentYear; x > (currentYear-100); x--)
            {
                YearBox.Items.Add(x.ToString());
            }

            YearBox.Text = currentYear.ToString();
        }

        private void DropBorder_DragEnter(object sender, DragEventArgs e)
        {
            string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);


            if (droppedFiles[0].EndsWith(".mp3")) //Limits what can be dropped
            {
                FileNameLabel.Content = (droppedFiles[0]);

                //populate enteries if they are empty based on mp3 meta data. 
                var SongFile = TagLib.File.Create(droppedFiles[0]);

                PopulateFromFile(droppedFiles[0]);
               
            }     
        }
        private void DropBorder_Drop(object sender, DragEventArgs e)
        {
            DropBorder_DragEnter(sender, e);
        }

        private void PopulateFromFile(string SongFilePath)
        {
            var SongFile = TagLib.File.Create(SongFilePath);

            if (TitleBox.Text == "") {

                if (SongFile.Tag.Title != null)
                {
                    TitleBox.Text = SongFile.Tag.Title;
                }          
            }

            YearBox.Text = SongFile.Tag.Year.ToString(); 

            if (Artist_NameBox.Text == "") {

                if (SongFile.Tag.Performers.Length > 0)
                {
                    string Artists = SongFile.Tag.Performers[0];

                    for (int x = 1; x < SongFile.Tag.Performers.Length; x++) { Artists += ", " + SongFile.Tag.Performers[x]; }

                    Artist_NameBox.Text = Artists;
                }
            }
        
            if (AlbumName.Text == "") {

                if (SongFile.Tag.Album != null)
                {
                    AlbumName.Text = SongFile.Tag.Album;
                }
            }
        }
        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                //Get the operation from the string "if it does not lead with '-' it is assumed to be a plus               
                char operation = '+';
                string Search = SearchBox.Text;

                if(Search[0] == '+' || Search[0] == '-')
                {
                    operation = Search[0];
                    Search = Search.TrimStart('-');
                    Search = Search.TrimStart('+');
                }

                //If the entry is empty.
                if(SearchBox.Text.Length == 0){

                    SearchError.Foreground = System.Windows.Media.Brushes.Red;
                    SearchError.Content = "Empty request";
                    return;
                }

                //Check to see if the tag exists             
                if (!LocalData.IsATag(Search))
                {
                    SearchError.Foreground = System.Windows.Media.Brushes.Red;
                    SearchError.Content = "Provided Tag does not exist";
                    return;
                }

                //If trying to remove check to see if the current review has the tag
                if(operation == '-' && !currentReview.HasTag(Search))
                {
                    SearchError.Foreground = System.Windows.Media.Brushes.Red;
                    SearchError.Content = "Provided Tag cannot be removed";
                    return;
                }

                //If trying to remove check to see if the current review has the tag
                if (operation == '+' && currentReview.HasTag(Search))
                {
                    SearchError.Foreground = System.Windows.Media.Brushes.Red;
                    SearchError.Content = "Provided Tag has already been added";
                    return;
                }

                //Else it shold have been successfull
                SearchError.Foreground = System.Windows.Media.Brushes.Green;
                SearchError.Content = "Successfullly Added Tag";

                
                TagObject newTag = LocalData.Tags.Find(x => x.Name.Equals(Search));
                if (operation == '+') { currentReview.AddTag(newTag); }
                else if (operation == '-') { currentReview.RemoveTag(newTag); }
                UpdateReviewTags();
                SearchBox.Text = "";                
            }
            
        }

        public void UpdateReviewTags()
        {
            //Update Language Tags 
            LanguagesBox.Document.Blocks.Clear();
            InstrumentsBox.Document.Blocks.Clear();
            GenresBox.Document.Blocks.Clear();

            //currentReview = newReviewObject;

            foreach (TagObject Tag in currentReview.getTags())
            {
                switch (Tag.Type)
                {
                    case TagType.Language:
                        LanguagesBox.AppendText(Tag.Name + " , ");
                        break;
                    case TagType.Genre:
                        GenresBox.AppendText(Tag.Name + " , ");
                        break;
                    case TagType.Instrument:
                        InstrumentsBox.AppendText(Tag.Name + " , ");
                        break;
                }
            }
        }

        private void SelectTags_Click(object sender, RoutedEventArgs e)
        {
            TagginWindow TaggingWindow = new TagginWindow(LocalData,currentReview,this);
            
            TaggingWindow.ShowDialog();
            TaggingWindow.Close();
            TaggingWindow = null;
            GC.Collect(); //It just annoys me that the information iisn't immediately collected so I force it to be as it closes.
            GC.WaitForFullGCComplete();


            UpdateReviewTags();
        }
        private void Update_Button_Click(object sender, RoutedEventArgs e)
        {
            Update();
            System.Windows.MessageBox.Show("Successfully udpated Review");
            this.Parent_Window.Close();
        }

        public void SetCurrentReview()
        {
            //Fill in Review Object with details filled into the page. 
            currentReview.Title = TitleBox.Text;
            currentReview.Release_Date = int.Parse(YearBox.Text);
            currentReview.Rating = float.Parse(ScoreBox.Text);
            currentReview.Review = ReviewBox.Text;
            currentReview.File_Path = (string)FileNameLabel.Content;
            currentReview.Artist = Artist_NameBox.Text;
            currentReview.Album = AlbumName.Text;

            currentReview.Review_Date = System.DateTime.Now.Year;
        }

        private void AddArtist_Click(object sender, RoutedEventArgs e)
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

            Artist_NameBox.Text = currentReview.Artist;
        }

        private void AddAlbum_Click(object sender, RoutedEventArgs e)
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

            AlbumName.Text = currentReview.Album;
        }
    }
}

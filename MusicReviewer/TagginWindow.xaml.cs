using System.Collections.Generic;
using System.Windows;
using DatabaseManagement;

namespace MusicReviewerApp
{
    public partial class TagginWindow : Window
    {
        private LocalDataManager LocalData;   
        public List<TagObject> SelectedTags;

        private ReviewPage Parent_Page;
        private AdvanceSearchWindow AdvanceSearchWindow;
        private bool Including;

        public TagginWindow(LocalDataManager LocalData, ReviewObject givenReview, ReviewPage Parent_Page)
        {
            InitializeComponent();

            //Keep a list of all tags that are selected to be "submitted" to the review later.
            SelectedTags = new List<TagObject>(givenReview.getTags());

            this.Parent_Page = Parent_Page;
            this.LocalData = LocalData;
           
            AdvanceSearchWindow = null;

            CreateTagBoxes(givenReview.getTags());          
        }
        public TagginWindow(LocalDataManager LocalData, AdvanceSearchWindow AdvanceSearchWindow, List<TagObject> GivenTags, bool Including)
        {
            InitializeComponent();

            this.Including = Including;
            this.AdvanceSearchWindow = AdvanceSearchWindow;
            this.LocalData = LocalData;

            if (GivenTags == null) { SelectedTags = new List<TagObject>(); }
            else { SelectedTags = GivenTags; }
            
            Parent_Page = null;
            CreateTagBoxes(SelectedTags);
        }
        private void CreateTagBoxes(List<TagObject> AlreadySelectedTags)
        {         
            //Create and Add handlers for the Tag Buttons handlers to the buttons.
            //This is done because with redundancy because I want to make sure each of the 
            //Lists are sorted alphabetically within their category.
            foreach (TagObject Tag in LocalData.GenreTags)
            {
                TagBox box = new TagBox(Tag);
                box.Click += new RoutedEventHandler(CheckBox_Click);
                if (AlreadySelectedTags.Contains(Tag)) { box.IsChecked = true; }

                GenresTagList.Items.Add(box);
            }
            foreach (TagObject Tag in LocalData.InstrumentTags)
            {
                TagBox box = new TagBox(Tag);
                box.Click += new RoutedEventHandler(CheckBox_Click);
                if (AlreadySelectedTags.Contains(Tag)) { box.IsChecked = true; }

                InstrumentsTagList.Items.Add(box);
            }
            foreach (TagObject Tag in LocalData.LanguageTags)
            {
                TagBox box = new TagBox(Tag);
                box.Click += new RoutedEventHandler(CheckBox_Click);
                if (AlreadySelectedTags.Contains(Tag)) { box.IsChecked = true; }

                LanguagesTagList.Items.Add(box);
            }
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            TagBox clickedBox = ((TagBox)sender);

            if (clickedBox.IsChecked == true)
            {
                SelectedTags.Add(clickedBox.TagData);
            }
            else
            {
                SelectedTags.Remove(clickedBox.TagData);                       
            } 
        }
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if(Parent_Page != null)
            {
                Parent_Page.currentReview.setTags(SelectedTags);
            }
            else //ADV_Window case.
            {
                if (Including)
                {
                    AdvanceSearchWindow.AddToIncludedTags(SelectedTags);
                }
                else
                {
                    AdvanceSearchWindow.AddToExcludedTags(SelectedTags);
                }
            }
            this.Close();
        }
        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();      
        }      
        
    }
}

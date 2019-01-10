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
using System.Windows.Shapes;
using DatabaseManagement;
using static MusicReviewerApp.LocalDataManager;

namespace MusicReviewerApp
{
    public partial class TagginWindow : Window
    {
        private LocalDataManager LocalData;   
        public List<TagObject> SelectedTags;
        private List<TagBox> TagBoxes;
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
            CreateTagBoxes(LocalData.Tags);       
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
            CreateTagBoxes(GivenTags);
        }
        private void CreateTagBoxes(List<TagObject> AlreadySelectedTags)
        {          
            this.TagBoxes = new List<TagBox>();
            //Create and Add handlers for the Tag Buttons handlers to the buttons.
            foreach (TagObject Tag in LocalData.Tags)
            {
                TagBox newBox = new TagBox(Tag);
                //Add TagWindow Specific handelers 
                newBox.Click += new RoutedEventHandler(CheckBox_Click);
                TagBoxes.Add(newBox);

                if (SelectedTags.Contains(newBox.TagData)) { newBox.IsChecked = true; }

                //Make the buttons visible
                switch (newBox.TagData.Type)
                {
                    case TagType.Genre:
                        GenresTagList.Items.Add(newBox);
                        break;
                    case TagType.Instrument:
                        InstrumentsTagList.Items.Add(newBox);
                        break;
                    case TagType.Language:
                        LanguagesTagList.Items.Add(newBox);
                        break;
                }
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

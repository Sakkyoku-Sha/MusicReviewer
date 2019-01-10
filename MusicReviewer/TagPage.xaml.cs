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
using static MusicReviewerApp.LocalDataManager;

namespace MusicReviewerApp
{
    public class TagTypeButton : RadioButton
    {
        public TagType type { get; set; }
 
    }
    public partial class TagPage : Page
    {
        List<TagBox> SelectedTags; 

        private LocalDataManager LocalData;
        private TagType SelectedTypeToAdd;
        private List<TagBox> TagBoxes;

        public TagPage(LocalDataManager LocalData)
        {
            InitializeComponent();
            this.LocalData = LocalData;
            this.SelectedTags = new List<TagBox>();
            this.SelectedTypeToAdd = TagType.Language;
            this.LanguageSelect.IsChecked = true;

            this.TagBoxes = new List<TagBox>();

            //Add handlers to the buttons.
            foreach (TagObject Tag in this.LocalData.Tags)
            {
                TagBox box = new TagBox(Tag);
        
                //Add TagWindow Specific handelers 
                box.Checked += new RoutedEventHandler(CheckBox_Check);
                box.Unchecked += new RoutedEventHandler(CheckBox_Uncheck);

                TagBoxes.Add(box);

                //Make the buttons visible
                switch (box.TagData.Type)
                {
                    case TagType.Genre:
                        GenreTagBox.Items.Add(box);
                        break;
                    case TagType.Instrument:
                        InstrumentTagBox.Items.Add(box);
                        break;
                    case TagType.Language:
                        LanguageTagBox.Items.Add(box);
                        break;
                }
            }
        }

        private void CheckBox_Check(object sender, RoutedEventArgs e)
        {
            SelectedTags.Add((TagBox)sender);
        }
        private void CheckBox_Uncheck(object sender, RoutedEventArgs e)
        { 
            SelectedTags.Remove((TagBox)sender);
        }

        private void AddButton_Checked(object sender, RoutedEventArgs e)
        {
            this.AddButton.IsChecked = true;
            this.RemoveButton.IsChecked = false;
            TagSubmissionBox.Visibility = Visibility.Visible;
            TagSubmit.Content = "Submit Tag";

            LanguageSelect.Visibility = Visibility.Visible;
            GenreSelect.Visibility = Visibility.Visible;
            InstrumentSelect.Visibility = Visibility.Visible;

            TagSubmit.IsEnabled = true;
            TagSubmissionBox.IsEnabled = true;
        }

        private void RemoveButton_Checked(object sender, RoutedEventArgs e)
        {
            this.AddButton.IsChecked = false;
            this.RemoveButton.IsChecked = true;
            TagSubmissionBox.Visibility = Visibility.Hidden;
            TagSubmit.Content = "Remove Tags";

            LanguageSelect.Visibility = Visibility.Hidden;
            GenreSelect.Visibility = Visibility.Hidden;
            InstrumentSelect.Visibility = Visibility.Hidden;

            TagSubmit.IsEnabled = true;
            TagSubmissionBox.IsEnabled = false;
        }

        private void TagTypeSelect_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton sentFrom = (RadioButton)sender;

            if (sentFrom.Content.Equals(InstrumentSelect))
            {
                SelectedTypeToAdd = TagType.Instrument;
            }
            else if (sentFrom.Equals(GenreSelect))
            {
                SelectedTypeToAdd = TagType.Genre;
            }
            else
            {
                SelectedTypeToAdd = TagType.Language;
            }
        }
        /**
         * This will only occur when add is already active, so when this is clicked 
         * the selected tag should be added to tag list. 
         * 
         * Might be lazy, instead of updating the actualy whole taglist, probably just 
         * create a checkbox and add it to the appropriate box once submit is clicked. 
         */
        private void TagSubmit_Click(object sender, RoutedEventArgs e)
        {
            if(AddButton.IsChecked == true) 
            {
                if (TagSubmissionBox.Text == "" || (LanguageSelect.IsChecked == false && GenreSelect.IsChecked == false && InstrumentSelect.IsChecked == false)) {

                    ErrorLabel.Content = "Invalid Request!";
                    ErrorLabel.Foreground = Brushes.Red;
                    return;
                }

                if (this.LocalData.IsATag(TagSubmissionBox.Text))
                {
                    ErrorLabel.Content = "Tag already exists";
                    ErrorLabel.Foreground = Brushes.Red;
                    return;
                }

                TagObject newTag = new TagObject(TagSubmissionBox.Text, SelectedTypeToAdd);
                TagBox newTagBox = new TagBox(newTag);

                LocalData.AddTag(newTag);
                this.TagBoxes.Add(newTagBox);

                switch (SelectedTypeToAdd)
                {
                    case TagType.Genre:
                        GenreTagBox.Items.Add(newTagBox);
                        GenreTagBox.Items.Refresh();
                        break;
                    case TagType.Instrument:
                        InstrumentTagBox.Items.Add(newTagBox);
                        InstrumentTagBox.Items.Refresh();
                        break;
                    case TagType.Language:
                        LanguageTagBox.Items.Add(newTagBox);
                        LanguageTagBox.Items.Refresh();
                        break;
                }

                this.TagSubmissionBox.Text = "";
            }
            else
            {
                foreach(TagBox box in this.SelectedTags)
                {
                    LocalData.RemoveTag(box.TagData);
                    this.TagBoxes.Remove(box);

                    switch (box.TagData.Type)
                    {
                        case TagType.Genre:
                            GenreTagBox.Items.Remove(box);
                            GenreTagBox.Items.Refresh();
                            break;
                        case TagType.Instrument:
                            InstrumentTagBox.Items.Remove(box);
                            InstrumentTagBox.Items.Refresh();
                            break;
                        case TagType.Language:
                            LanguageTagBox.Items.Remove(box);
                            LanguageTagBox.Items.Refresh();
                            break;
                    }
                }            
            }

          
        }      
    }
}

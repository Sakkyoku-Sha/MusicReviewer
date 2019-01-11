using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DatabaseManagement;

namespace MusicReviewerApp
{
    /// <summary>
    /// Interaction logic for ElementList 
    /// </summary>
    public partial class ElementListWindow : Window
    {
        Review_Fields ListField;      
        LocalDataManager LocalData;
        Object SentFrom;
        bool SomethingSelected;
        public ElementListWindow(LocalDataManager LocalData, Review_Fields ListField, Object SentFrom)
        {
            SomethingSelected = false;
            this.SentFrom = SentFrom;
            this.ListField = ListField;
            this.LocalData = LocalData;
            InitializeComponent();
            PopulatePage();          
        }
        private void PopulatePage()
        {
            switch (this.ListField)
            {
                case Review_Fields.Artist:
                    List<string> AllArtists = this.LocalData.DBManager.GetAllArtists();
                    foreach (string Artist in AllArtists) { if (!Artist.Equals("")) { this.ElementList.Items.Add(Artist); } }
                    break;
                case Review_Fields.Album:
                    List<string> AllAlbums = this.LocalData.DBManager.GetAllAlbums();
                    foreach(string Album in AllAlbums) { if (!Album.Equals("")) { this.ElementList.Items.Add(Album); } }
                    break;
            }      
        }

        private void ElementList_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox box = sender as ListBox;
            string Clicked_On = box.SelectedItem as string; 

            if (SentFrom is ReviewPage)
            {
                if(ListField == Review_Fields.Artist){ (SentFrom as ReviewPage).currentReview.Artist = box.SelectedItem as string; }
                else if(ListField == Review_Fields.Album){ (SentFrom as ReviewPage).currentReview.Album = box.SelectedItem as string; }
            }
            else if (SentFrom is AdvanceSearchWindow)
            {
                if(ListField == Review_Fields.Artist){ (SentFrom as AdvanceSearchWindow).SetArtist(box.SelectedItem as string); }
                else if(ListField == Review_Fields.Album){(SentFrom as AdvanceSearchWindow).SetAlbum(box.SelectedItem as string);}
            }

            SomethingSelected = true;
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //if the window is closed empty the box 

            if (!SomethingSelected)
            {
                if (SentFrom is ReviewPage)
                {
                    if (ListField == Review_Fields.Artist) { (SentFrom as ReviewPage).currentReview.Artist = ""; }
                    else if (ListField == Review_Fields.Album) { (SentFrom as ReviewPage).currentReview.Album = ""; }
                }
                else if (SentFrom is AdvanceSearchWindow)
                {
                    if (ListField == Review_Fields.Artist) { (SentFrom as AdvanceSearchWindow).SetArtist(""); }
                    else if (ListField == Review_Fields.Album) { (SentFrom as AdvanceSearchWindow).SetAlbum(""); }
                }
            }
        }
    }
}

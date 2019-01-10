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
using System.IO;
using DatabaseManagement;
using Microsoft.Win32;

namespace MusicReviewerApp
{
    /// <summary>
    /// Interaction logic for StatPage.xaml
    /// </summary>
    /// 

    class Table_Entry
    {
        public string Type { get; set; }
        public string Rating { get; set; }

        public Table_Entry(string Type, string Rating)
        {
            this.Type = Type;
            this.Rating = Rating;
        }

    }
    public partial class StatPage : Page
    {
        const string BackUpFileName = "BackUp.db";
        LocalDataManager LocalData;

        List<Table_Entry> Instruments;
        List<Table_Entry> Genres;
        List<Table_Entry> Languages;
        List<Table_Entry> Release_Dates;
        List<Table_Entry> Artists;
        List<Table_Entry> Albums;

        public StatPage(LocalDataManager LocalData)
        {
            this.LocalData = LocalData; 
            InitializeComponent();

            Instruments = new List<Table_Entry>();
            Genres = new List<Table_Entry>();
            Languages = new List<Table_Entry>();
            Release_Dates = new List<Table_Entry>();
            Artists = new List<Table_Entry>();
            Albums = new List<Table_Entry>();

            AlbumView.ItemsSource = Albums;
            ArtistView.ItemsSource = Artists;
            YearView.ItemsSource = Release_Dates;
            GenreView.ItemsSource = Genres;
            InstrumentView.ItemsSource = Instruments;
            LanguageView.ItemsSource = Languages;
        }

        private void BackUp_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(BackUpFileName))
            {
                File.Delete(BackUpFileName);    
            }

            File.Copy(DatabaseManager.DatabaseName, BackUpFileName);
            System.Windows.MessageBox.Show("BackUp Made");
        }
        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            Instruments.Clear();
            Genres.Clear();
            Languages.Clear();
            Release_Dates.Clear();
            Albums.Clear();
            Artists.Clear();

            Instruments.Clear();
            Genres.Clear();
            Languages.Clear();
            Release_Dates.Clear();
            Artists.Clear();
            Albums.Clear();

            //basic stats gen. 
            string[] results;
            List<string[]> ListResults;

            //If there is nothing.

            string avg = LocalData.DBManager.GetAvgRating();
            if (avg != null)
            {
                AverageRating.Content = null;
                AverageRating.Content = "Average Rating\n" + LocalData.DBManager.GetAvgRating();
            }
            else return;
           
            results = LocalData.DBManager.GetHighestRated(Review_Fields.Artist);
            if (results.Length > 0)
            {
                HighestArtist.Content = null;
                HighestArtist.Content = "Highest Rated Artist\n" + results[0] + " | " + results[1];
            }
            results = LocalData.DBManager.GetHighestRated(Review_Fields.Album);
            if (results.Length > 0)
            {
                HighestAlbum.Content = null;
                HighestAlbum.Content = "Highest Rated Album\n" + results[0] + " | " + results[1];
            }
            results = LocalData.DBManager.GetHighestRated(Review_Fields.Release_Date);
            if (results.Length > 0)
            {
                HighestYear.Content = null;
                HighestYear.Content = "Highest Rated Year\n" + results[0] + " | " + results[1];
            }

            //Go through all reviews and tally relvant data into a map. Then calcculate relevant data at the end. 
            
            //Albums 
            ListResults = LocalData.DBManager.GetAvgRatings(Review_Fields.Album);
            if (ListResults.Count > 0)
            {
                foreach (string[] row in ListResults) { Albums.Add(new Table_Entry(row[1], row[0])); }
            }
            //Artists 
            ListResults = LocalData.DBManager.GetAvgRatings(Review_Fields.Artist);
            if (ListResults.Count > 0)
            {
                foreach (string[] row in ListResults) { Artists.Add(new Table_Entry(row[1], row[0])); }
            }
            //Release_Dates 
            ListResults = LocalData.DBManager.GetAvgRatings(Review_Fields.Release_Date);
            if (ListResults.Count > 0)
            {
                foreach (string[] row in ListResults) { Release_Dates.Add(new Table_Entry(row[1], row[0])); }
            }
            //Languages
            ListResults = LocalData.DBManager.GetAvgRatings(TagType.Language);
            if (ListResults.Count > 0)
            {
                foreach (string[] row in ListResults) { Languages.Add(new Table_Entry(row[1], row[0])); }
            }
            //Genres
            ListResults = LocalData.DBManager.GetAvgRatings(TagType.Genre);
            if (ListResults.Count > 0)
            {
                foreach (string[] row in ListResults) { Genres.Add(new Table_Entry(row[1], row[0])); }
            }
            //Instruments
            ListResults = LocalData.DBManager.GetAvgRatings(TagType.Instrument);
            if (ListResults.Count > 0)
            {
                foreach (string[] row in ListResults) { Instruments.Add(new Table_Entry(row[1], row[0])); }
            }

            AlbumView.Items.Refresh();
            ArtistView.Items.Refresh();
            YearView.Items.Refresh();
            GenreView.Items.Refresh();
            InstrumentView.Items.Refresh();
            LanguageView.Items.Refresh();

            GC.Collect();
        }

        private void Import_DataBase_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();

            //Make a backup of the oldDB;
            FileStream copyFile = new FileStream("importedDBbackup", FileMode.Create, FileAccess.ReadWrite);
            FileStream originalDb = new FileStream(DatabaseManager.DatabaseName, FileMode.Open, FileAccess.Read);
            originalDb.CopyTo(copyFile);

            originalDb.Close(); 
            copyFile.Close();

            //Ask the user for the file
            openFile.Multiselect = false;
            openFile.Filter = "Database Files(*.db)|*.db|All files (*.*)|*.*";

            if (openFile.ShowDialog() == true)
            {
                string fileName = openFile.FileName;

                //Get file
                FileStream importedFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                //Delete Old DB file to overwrite;              
                File.Delete(DatabaseManager.DatabaseName);

                //Create new File. 
                FileStream newFile = new FileStream(DatabaseManager.DatabaseName, FileMode.CreateNew, FileAccess.ReadWrite);

                //Copy the file
                importedFile.CopyTo(newFile);

                LocalData.DBManager.ReplaceDataBase(fileName);

                newFile.Close();
                importedFile.Close();
               
            }

            openFile = null;
            GC.Collect(); //It just annoys me that the information iisn't immediately collected so I force it to be as it closes.
            GC.WaitForFullGCComplete();
        }
    }
}

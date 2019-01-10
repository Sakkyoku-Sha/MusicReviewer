using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace DatabaseManagement
{
    public enum SQL_Exception_Type
    {
        Unique_Failed
    };

    public partial class SQL_Exception : Exception
    {
        public SQL_Exception_Type Error_Type;
        
        public SQL_Exception(SQL_Exception_Type Type)
        {
            this.Error_Type = Type;
        }
    } 

    public class DatabaseManager
    {
        public string InstallLocation = Path.GetDirectoryName(Assembly.GetAssembly(typeof(DatabaseManager)).CodeBase);
        public static string DatabaseName;
        public static string TagFileName; 
    
        private SQLiteConnection DatebaseConnection;

        public DatabaseManager()
        {
            DatabaseName = /*InstallLocation +*/  "MusicReviews.db";
            TagFileName = /*InstallLocation +*/ "Tags.ini";

            bool firstTime = false;
            if (!File.Exists(DatabaseName)) { SQLiteConnection.CreateFile(DatabaseName); firstTime = true; }
            this.DatebaseConnection = new SQLiteConnection("Data Source=" + DatabaseName + ";Version=3;");

            if(firstTime == true) { ResetDataBase(); }

            this.DatebaseConnection.Close();
        }
        public void ResetDataBase()
        {
            //Delete the old database.
            this.DatebaseConnection.Close();
            File.Delete(DatabaseName);

            //Connect/Create new database. 
            this.ConnectToDatabase();
            this.InitializeDataBase();
            this.InitializeTags();
            this.CloseConnection();
        }
        public void ConnectToDatabase()
        {
            if (this.DatebaseConnection.State == System.Data.ConnectionState.Open) { return; }

            if (!File.Exists("./" + DatabaseName))
            {
                SQLiteConnection.CreateFile(DatabaseName);
            }
            this.DatebaseConnection.Open();
        }
        public void CloseConnection()
        {
            if (this.DatebaseConnection.State == System.Data.ConnectionState.Closed) { return; }

            this.DatebaseConnection.Close();
        }
        private bool ExecuteNoResponseSQL(string SQL_Statement) 
        {
            this.ConnectToDatabase();

            try
            {
                //Console.WriteLine("Attempting To Execute " + SQL_Statement + "\n\n\n");
                SQLiteCommand SqlCmd = this.DatebaseConnection.CreateCommand();
                SqlCmd.CommandText = SQL_Statement;
                SqlCmd.ExecuteNonQuery();
            }
            catch (System.Data.SQLite.SQLiteException e)
            {
                HandleSQLite_Exception(e);              
            }

            this.CloseConnection();

            return true;
        }

        private void HandleSQLite_Exception(SQLiteException e)
        {
            switch (e.ResultCode)
            {
                case SQLiteErrorCode.Constraint:
                    throw new SQL_Exception(SQL_Exception_Type.Unique_Failed);
            }
        }

        /*  Generic Query Function that will return all values to a query in a string array list.
*  
*  The length of the inner strng array is the number of colums in the queried table.
* 
*/
        private List<string[]> Query(string SQL_Statement)
        {
            List<string[]> results = new List<string[]>();
            this.ConnectToDatabase();

            SQLiteCommand SqlCmd = this.DatebaseConnection.CreateCommand();
            SqlCmd.CommandText = SQL_Statement;
            SQLiteDataReader Reader = SqlCmd.ExecuteReader();


            while (Reader.Read())
            {
                string[] row = new string[Reader.FieldCount];
               
                for (int x = 0; x < Reader.FieldCount; x++) {

                    var i = Reader.GetValue(x);

                    if (i.GetType() == typeof(string))
                    {
                        row[x] = Reader.GetString(x);
                    }
                    else if (i.GetType() == typeof(double) || i.GetType() == typeof(float))
                    {
                        row[x] = Reader.GetDouble(x).ToString("F3"); //To float of 3 decimal points 
                    }
                    else if(i.GetType() == typeof(int))
                    {
                        row[x] = i.ToString();
                    }
                    else if(i == null)
                    {
                        row[x] = null;
                    }
                    else
                    {
                        row[x] = Reader.GetValue(x).ToString();
                    }
                }

                results.Add(row);
            }
            this.CloseConnection();
            return results;

        }

        public TagType GetTagType(string TagName)
        {
            //Length of inner array is 3, 1 = ID, 2 = Name, 3 = TagType
            List<string[]> results = Query(SqlStatements.GetTagTypeFromTag(TagName));

            return TagObject.ConvertToTagType(results[0][2][0]); //takes the first char 
        }
        public int GetTagID(string TagName)
        {
            List<string[]> results = Query(SqlStatements.GetTagTypeFromTag(TagName));

            return int.Parse(results[0][0]);
        }
        public TagObject GetTag(string TagName)
        {
            List<string[]> results = Query(SqlStatements.GetTagTypeFromTag(TagName));

            string Name = results[0][1];
            TagType Type = TagObject.ConvertToTagType(results[0][2][0]); //takes the first char 

            TagObject tag = new TagObject(Name, Type);

            return tag;
        }
        public List<TagObject> GetAllTags()
        {
            List<TagObject> allTags = new List<TagObject>();
            List<string[]> results = Query(SqlStatements.GetAllTags);

            foreach (string[] row in results)
            {
                allTags.Add(new TagObject(row[1], TagObject.ConvertToTagType(row[2][0])));
            }
            return allTags;
        }
        public List<string> GetAllArtists()
        {
            List<string> AllArtist = new List<string>();
            List<string[]> results = Query(SqlStatements.GetAllArtist);
            
            foreach(string[] row in results)
            {
                AllArtist.Add(row[0]);
            }

            return AllArtist;
        }
        public List<string> GetAllAlbums()
        {
            List<string> AllAlbums = new List<string>();
            List<string[]> results = Query(SqlStatements.GetAllArtist);

            foreach (string[] row in results)
            {
                AllAlbums.Add(row[0]);
            }

            return AllAlbums;
        }
        public int GetReviewID(string title, string artist, int releaseDate)
        {
            List<string[]> results = Query(SqlStatements.GetIDFromReview(title, artist, releaseDate));

            return int.Parse(results[0][0]);
        }
        public ReviewObject GetReview(string title, string artist, int releaseDate)
        {
            List<string[]> results = Query(SqlStatements.GetIDFromReview(title, artist, releaseDate));

            return new ReviewObject
            {
                Title = results[0][1],
                Release_Date = int.Parse(results[0][2]),
                Review_Date = int.Parse(results[0][3]),
                Album = results[0][4],
                Artist = results[0][5],
                Rating = float.Parse(results[0][6]),
                Review = results[0][7],
                File_Path = results[0][8]
            };

        }
        public ReviewObject GetReview(Review_Reference RR)
        {
            List<string[]> results = Query(SqlStatements.GetReview(RR));

            return new ReviewObject
            {
                Title = results[0][1],
                Release_Date = int.Parse(results[0][2]),
                Review_Date = int.Parse(results[0][3]),
                Album = results[0][4],
                Artist = results[0][5],
                Rating = float.Parse(results[0][6]),
                Review = results[0][7],
                File_Path = results[0][8]
            };

        }
        public List<Review_Reference> getReviews(SQL_Review_REQUEST request)
        {
            List<string[]> results = Query(SqlStatements.GetMusicReviews_Builder(request));
            List<Review_Reference> reviews = new List<Review_Reference>();

            //Each row will be a review build them here and return them.
            foreach(string[] row in results)
            {
                reviews.Add(new Review_Reference(row));
            }

            return reviews;
        }
        public List<Review_Reference> GetAllReviews()
        {
            List<Review_Reference> allReviews = new List<Review_Reference>();
            List<string[]> results = Query(SqlStatements.GetAllReviewReferences);

            foreach (string[] row in results)
            {
                allReviews.Add(new Review_Reference(row));
               
            }
            return allReviews;
        }

        //Get All the Tags assosiated with a review object
        //Does not set the Review OBject with tags. 
        public List<TagObject> GetTagsForReview(Review_Reference ro)
        {
            List<TagObject> ReviewTags = new List<TagObject>();
            List<string[]> results = Query(SqlStatements.GetTagsForReview(ro));

            foreach (string[] row in results)
            {
                ReviewTags.Add(new TagObject(row[0], TagObject.ConvertToTagType(row[1][0])));
            }

            return ReviewTags;
        }
        //Adding stuff to the database
        public void AddReview(ReviewObject rvw){ExecuteNoResponseSQL(SqlStatements.AddReview(rvw));}
        public void AddTag(TagObject tag) { ExecuteNoResponseSQL(SqlStatements.AddTag(tag)); }

        public void deleteReview(ReviewObject ro) { ExecuteNoResponseSQL(SqlStatements.DeleteReview(ro)); }
        public void deleteReview(Review_Reference ro) { ExecuteNoResponseSQL(SqlStatements.DeleteReview(ro)); }

        public void deleteTag(TagObject tag) { ExecuteNoResponseSQL(SqlStatements.DeleteTag(tag)); }

        public void UpdateReview(ReviewObject old_ro, ReviewObject new_ro) { ExecuteNoResponseSQL(SqlStatements.UpdateReview(old_ro, new_ro)); }

        
        private void InitializeDataBase()
        {
            ExecuteNoResponseSQL(SqlStatements.createReviews);
            ExecuteNoResponseSQL(SqlStatements.createReviewTags);
            ExecuteNoResponseSQL(SqlStatements.createTagsTable);
        }
        private void InitializeTags()
        {
            try
            {
                FileStream tagFile = new FileStream(TagFileName, FileMode.Open, FileAccess.ReadWrite);
                StreamReader reader = new StreamReader(tagFile);
                StringBuilder TagStrings = new StringBuilder();
                List<TagObject> TagsToAdd = new List<TagObject>();

                while(reader.EndOfStream == false)
                {
                    readThroughWhiteSpace();
                    if(reader.EndOfStream == true) { return; }

                    TagType nextTagType = TagObject.ConvertToTagType((char)reader.Read());
                    if (reader.EndOfStream == true) { return; }

                    if ((char)reader.Read() != '<'){
                        throw new Exception("Parse Error");
                    }
                    if (reader.EndOfStream == true) { return; }

                    char nextToAdd = (char)reader.Read();
                    if (reader.EndOfStream == true) { return; }

                    while (nextToAdd != '>')
                    {
                        TagStrings.Append(nextToAdd);
                        nextToAdd = (char)reader.Read();
                        if (nextToAdd != '>' && reader.EndOfStream == true) { throw new Exception("Parse Error");}
                    }
                 
                    TagsToAdd.Add(new TagObject(TagStrings.ToString(), nextTagType));
                    TagStrings.Clear();
                }

                System.Console.WriteLine(SqlStatements.AddTags(TagsToAdd));
                ExecuteNoResponseSQL(SqlStatements.AddTags(TagsToAdd));

                void readThroughWhiteSpace()
                {               
                    while (reader.EndOfStream == false && char.IsWhiteSpace((char)reader.Peek()))
                    {
                        reader.Read(); //read it in.
                    }
                }
                
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("Error in Project set up, tag ini files not found");
            }
        }

        //Statistic Query Functions;
        public string GetAvgRating()
        {
            List<string[]> results = Query(SqlStatements.Avg_Rating);
            if (results.Count == 0) return null;

            return results[0][0];
        }
        public List<string[]> GetAvgRatings(TagType field)
        {
            switch (field)
            {
                case TagType.Genre:
                    return Query(SqlStatements.Avg_Genres);                  
                case TagType.Instrument:
                    return Query(SqlStatements.Avg_Instruments);                 
                case TagType.Language:
                    return Query(SqlStatements.Avg_Language);                               
            }

            return null;
        }
        public List<string[]> GetAvgRatings(Review_Fields field)
        {
            switch (field)
            {
                case Review_Fields.Artist:
                    return Query(SqlStatements.Avg_Artist);
                case Review_Fields.Album:
                    return Query(SqlStatements.Avg_Album);
                case Review_Fields.Release_Date:
                    return Query(SqlStatements.Avg_Release_Date);
            }

            return null;

        }
        public string[] GetHighestRated(Review_Fields field)
        {
            switch (field)
            {
                case Review_Fields.Album:
                    return Query(SqlStatements.Highest_Rated_Album)[0];
                case Review_Fields.Artist:
                    return Query(SqlStatements.Highest_Rated_Artist)[0];
                case Review_Fields.Release_Date:
                    return Query(SqlStatements.Highest_Rated_Year)[0];

            }

            return null;
        }

        public void ReplaceDataBase(string fileName)
        {
            this.DatebaseConnection = new SQLiteConnection("Data Source=" + DatabaseName + ";Version=3;");
            this.DatebaseConnection.Close();
        }
    }
}

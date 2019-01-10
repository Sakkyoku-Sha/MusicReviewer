using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DatabaseManagement
{
    //Class that Specifies how the reviews can be interacted with. 
    public class SQL_Review_REQUEST
    {
        public int RatingMax,RatingMin,ReleaseYearMax,ReleaseYearMin,ReviewYearMax,ReviewYearMin;
        public List<List<TagObject>> Included_Tags, Excluded_Tags;
        public string Artist, Album;

        public SQL_Review_REQUEST()
        {
            RatingMax = -1;
            RatingMin= -1;

            ReleaseYearMax = -1;
            ReleaseYearMin = -1;

            ReviewYearMax = -1;
            ReviewYearMin = -1;

            Included_Tags = new List<List<TagObject>>();
            Excluded_Tags = new List<List<TagObject>>();
        }
    }
    public class SqlStatements
    {
        private const string ReviewReference = "(Title,Release_Date,Review_Date,Album,Artist,Rating)";

        public static string ReviewTable = "MusicReview";
        public const string ReviewTagTable = "ReviewTags";
        public const string TagTable = "Tags";
        
        //Note that SQLite has differen't ways that it can accept input
        public static string createReviews = @"CREATE TABLE IF NOT EXISTS
             MusicReview (" +
            "ID INTEGER PRIMARY KEY AUTOINCREMENT," +
            "Title varchar(255) NOT NULL," +
            "Release_Date int," + //going to have to  make our own text/ it will probably be YYYY/MM format 
            "Review_Date int," +
            "Album varchar(255)," +
            "Artist varchar(255)," +
            "Rating float," +
            "Review varchar(65536)," +
            "File_Path varchar(65536)," +
            "CONSTRAINT title_artist UNIQUE (Title, Artist, Release_Date)" +
            ");";

        public static string createReviewTags = "CREATE TABLE IF NOT EXISTS ReviewTags(" +
            "ReviewID int NOT NULL," +
            "TagID  int NOT NULL," +
            "PRIMARY KEY (ReviewID, TagID)" +
            ");";


        public static string createTagsTable = "CREATE TABLE IF NOT EXISTS Tags(" +
            "TagID INTEGER PRIMARY KEY AUTOINCREMENT," +
            "Name varchar(255) NOT NULL," +
            "TagType varchar(255) NOT NULL," +
            "CONSTRAINT TagUnique UNIQUE(Name))";

        public static string GetAllTags = "Select * From Tags";
        public static string GetAllReviewReferences = "Select Title,Release_Date,Review_Date,Album,Artist,Rating from MusicReview";
        public static string GetAllArtist = "select distinct Artist from MusicReview";
        public static string GetAllAlbums = "select distinct Album from MusicReview";

        //Statistic Based Functionss

        private static string TagInnerJoinReviews = "MusicReview inner join ReviewTags on MusicReview.ID = ReviewTags.ReviewID inner join Tags on ReviewTags.TagID = Tags.TagID";

        public static string Avg_Language = "select Avg(Rating) as Rating, Name from " + TagInnerJoinReviews  +" where TagType = 'L' group by Tags.TagID order by Rating desc";
        public static string Avg_Genres = "select Avg(Rating) as Rating, Name from " + TagInnerJoinReviews + " where TagType = 'G' group by Tags.TagID order by Rating desc";
        public static string Avg_Instruments = "select Avg(Rating) as Rating, Name from " + TagInnerJoinReviews + " where TagType = 'I' group by Tags.TagID order by Rating desc";
        public static string Avg_Release_Date = "select Avg(Rating) as Rating, Release_Date from MusicReview group by Release_Date order by Rating desc";
        public static string Avg_Artist = "select Avg(Rating) as Rating, Artist from MusicReview group by Artist order by Rating desc";
        public static string Avg_Album = "select Avg(Rating) as Rating, Album from MusicReview group by Album order by Rating desc";
        public static string Avg_Rating = "select Avg(Rating) from MusicReview";

        public static string Highest_Rated_Artist = "select Artist, Max(Rated) From (SELECT Artist, Avg(Rating) as Rated FROM MusicReview Group BY Artist) Where Artist != ''";
        public static string Highest_Rated_Year = "Select Release_Date, Rating From(Select Release_Date, MAX(Rating) as Rating from (SELECT Release_Date, Avg(Rating) as Rating From MusicReview Group by Release_Date))";
        public static string Highest_Rated_Album = "select Album, Rating from (select Album, Max(Rated) as Rating From (SELECT Album, Avg(Rating) as Rated FROM MusicReview Group BY Album) where Album != '')";
   
        public static string GetIDFromReview(string title, string artist, int release_date)
        {
            return "Select ID from MusicReview Where Title = '" + title + "' And Artist = '" + artist + "' And Release_Date = " + release_date.ToString(); 
        }

        //Get the ID for the given Tag
        public static string GetIDFromTag(string tag)
        {
            return "Select TagID from Tags Where Name = '" + tag + "'";      
        }

        //Get the id tag type table, i.e id of the genre type 
        public static string GetTagTypeFromTag(string tag)
        {
            return "Select TagType from Tags Where Name = '" + tag + "'"; 
        }

        public static string GetTagsForReview(Review_Reference ro)
        {
            StringBuilder SQL_Statement = new StringBuilder();     
            
            if (ro.Valid())
            {
                SQL_Statement.Append("Select Name, TagType from MusicReview inner join ReviewTags on MusicReview.ID = ReviewTags.ReviewID inner join Tags on ReviewTags.TagID = Tags.TagID where ");
                SQL_Statement.Append("Title = '" + ro.Title + "' AND ");
                SQL_Statement.Append("Artist = '" + ro.Artist + "' AND ");
                SQL_Statement.Append("Release_Date = " + ro.Release_Date.ToString());
            }

            return SQL_Statement.ToString();
        }


        //Add Tag to the table
        public static string AddTag(TagObject tag)
        {
            return "Insert into Tags(Name,TagType) VALUES('" + tag.Name + "','" + (char)tag.Type + "')";    
        }

        //Add List of Tags 
        public static string AddTags(List<TagObject> tags)
        {
            StringBuilder SQL_Statement = new StringBuilder();

            SQL_Statement.Append("Insert into Tags(Name,TagType) VALUES ");

            foreach(TagObject tag in tags)
            {
                SQL_Statement.Append("('" + tag.Name + "','" + (char)tag.Type + "'),");
            }

            SQL_Statement.Remove(SQL_Statement.Length - 1, 1);
            return SQL_Statement.ToString();
        }

        //Add Review to table
        public static string AddReview(ReviewObject review)
        {
            return NonNullFields(review) + FieldData(review) + "; " + AddTagsToReview(review);

            string NonNullFields(ReviewObject ro)
            {
                StringBuilder SQL_Statement = new StringBuilder();
                SQL_Statement.Append("Insert into MusicReview(");

                SQL_Statement.Append("Title,");
                if(ro.Release_Date != -1) { SQL_Statement.Append("Release_Date,"); }
                SQL_Statement.Append("Review_Date,"); 
                if(!ro.Album.Equals("")) { SQL_Statement.Append("Album,"); }
                SQL_Statement.Append("Artist,"); 
                if(ro.Rating != (float)-1.0) { SQL_Statement.Append("Rating,"); }
                if(!ro.Review.Equals("")) { SQL_Statement.Append("Review,"); }
                if(!ro.File_Path.Equals("")) { SQL_Statement.Append("File_Path,"); }

                SQL_Statement.Remove(SQL_Statement.Length - 1, 1); //Remove the uneeeded "," 

                SQL_Statement.Append(") ");
             
                return SQL_Statement.ToString();
            }
            string FieldData(ReviewObject ro)
            {
                StringBuilder SQL_Statement = new StringBuilder();
                SQL_Statement.Append(" VALUES(");

                SQL_Statement.Append('\'' + ro.Title + "\',"); 
                if (ro.Release_Date != -1) { SQL_Statement.Append(ro.Release_Date.ToString() + ','); }
                SQL_Statement.Append(ro.Review_Date.ToString() + ','); 
                if (!ro.Album.Equals("")) { SQL_Statement.Append('\'' + ro.Album + "\',"); }
                SQL_Statement.Append('\'' + ro.Artist + "\',"); 
                if (ro.Rating != (float)-1.0) { SQL_Statement.Append(ro.Rating.ToString() + ','); }
                if (!ro.Review.Equals("")) { SQL_Statement.Append('\'' + ro.Review.Replace('\'','"') + "\',"); } //Really LAZY sanitaztion this could be made much more robust.
                if (!ro.File_Path.Equals("")) { SQL_Statement.Append('\'' + ro.File_Path + "\',"); }

                SQL_Statement.Remove(SQL_Statement.Length - 1, 1); //Remove the uneeeded "," 

                SQL_Statement.Append(")");

                return SQL_Statement.ToString();
            }
        }

        public static string GetReview(Review_Reference ro)
        {
            return "Select * from MusicReview Where Title = '" + ro.Title + "' And Artist = '" + ro.Artist + "' And Release_Date = " + ro.Release_Date.ToString();
        }

        //Add Tags to Review
        public static string AddTagsToReview(ReviewObject review)
        {
            List<TagObject> tags = review.getTags();

            return AddTagsToReview(review, tags);          
        }

        public static string AddTagsToReview(ReviewObject review, List<TagObject> Tags)
        {          
            if (Tags.Count < 1) { return ""; }

            StringBuilder SQL_Statement = new StringBuilder();
            //Insert into ReviewTagsSelect ID as ReviewID, TagID from MusicReview join Tags where Title = 'test' and Tags.Name = "action"
            SQL_Statement.Append("Insert into ReviewTags(ReviewID,TagID) Select ID as ReviewID, TagID from MusicReview join Tags " +
                "where Title = '" + review.Title + "' " +
                "and Artist = '" + review.Artist + "' " +
                "and Release_Date = " + review.Release_Date + " " +
                "and ");
            
            SQL_Statement.Append("(Name = '" + Tags[0].Name + "' ");
            for (int x = 1; x < Tags.Count; x++)
            {
                SQL_Statement.Append(" OR Name = '" + Tags[x].Name + "'");
            }

            SQL_Statement.Append(")");

            return SQL_Statement.ToString();
        }

        //Delete Tag from Database
        public static string DeleteTag(TagObject tag)
        {
            StringBuilder SQL_Statement = new StringBuilder();

            SQL_Statement.Append("Delete from ReviewTags where TagID in (Select TagID from Tags where Name ='" + tag.Name + "' and TagType = '" + (char)tag.Type + "'); ");
            SQL_Statement.Append("Delete from Tags where Name ='" + tag.Name + "' and TagType = '" + (char)tag.Type + "';");
            
            return SQL_Statement.ToString();
        }

        //Delete Review From Database
        public static string DeleteReview(ReviewObject ro)
        {
            StringBuilder SQL_Statement = new StringBuilder();

            SQL_Statement.Append("Delete from ReviewTags where ReviewID in (Select ID as ReviewID from MusicReview where Title = '" + ro.Title + "' and " + "Artist = '" + ro.Artist + "'" + " and Release_Date = " + ro.Release_Date.ToString() + "); ");
            SQL_Statement.Append("Delete from MusicReview where Title = '" + ro.Title + "' and " + "Artist = '" + ro.Artist + "'" + " and Release_Date = " + ro.Release_Date.ToString());

            return SQL_Statement.ToString();
        }
        public static string DeleteReview(Review_Reference ro)
        {
            StringBuilder SQL_Statement = new StringBuilder();

            SQL_Statement.Append("Delete from ReviewTags where ReviewID in (Select ID as ReviewID from MusicReview where Title = '" + ro.Title + "' and " + "Artist = '" + ro.Artist + "'" + " and Release_Date = " + ro.Release_Date.ToString() + "); ");
            SQL_Statement.Append("Delete from MusicReview where Title = '" + ro.Title + "' and " + "Artist = '" + ro.Artist + "'" + " and Release_Date = " + ro.Release_Date.ToString());

            return SQL_Statement.ToString();
        }

        //Update a review entry;
        public static string UpdateReview(ReviewObject old_ro, ReviewObject new_ro)
        {
            StringBuilder SQL_Statement = new StringBuilder();

            SQL_Statement.Append("Update MusicReview Set ");

            if (new_ro.Title != null) { SQL_Statement.Append("Title = '" + new_ro.Title + "', "); }
            if (new_ro.Release_Date != -1) { SQL_Statement.Append("Release_Date = " + new_ro.Release_Date + ", "); }
            if (new_ro.Review_Date != -1) { SQL_Statement.Append("Review_Date = " + new_ro.Review_Date +  ", "); }
            if (new_ro.Album != null) { SQL_Statement.Append("Album = '" +  new_ro.Album + "', "); }
            if (new_ro.Artist != null) { SQL_Statement.Append("Artist = '" + new_ro.Artist + "', "); }
            if (new_ro.Rating != (float)-1.0) { SQL_Statement.Append("Rating = " + new_ro.Rating.ToString() + ", "); }
            if (new_ro.Review != null) { SQL_Statement.Append("Review = '" + new_ro.Review.Replace('\'', '"') + "', "); }
            if (new_ro.File_Path != null) { SQL_Statement.Append("File_Path = '" + new_ro.File_Path + "', "); }

            SQL_Statement.Remove(SQL_Statement.Length - 2, 2); //Remove the uneeeded "," 

            SQL_Statement.Append(" Where Title = '" + old_ro.Title + "' AND Artist = '" + old_ro.Artist + "' AND Release_Date = " + old_ro.Release_Date.ToString() + "; ");

            //Update the Tags.
            List<TagObject> TagsToAdd = new List<TagObject>();
            List<TagObject> TagsToRemove = new List<TagObject>();

            List<TagObject> New_Tags = new_ro.getTags();
            List<TagObject> Old_Tags = old_ro.getTags();

            foreach(TagObject Tag in New_Tags)
            {
                if (!Old_Tags.Contains(Tag)) { TagsToAdd.Add(Tag); Old_Tags.Remove(Tag); }
               
            }
            foreach(TagObject Tag in Old_Tags)
            {
                if(!New_Tags.Contains(Tag)) { TagsToRemove.Add(Tag); }
            }

            //Insert the new tags, remove the old tags.
            SQL_Statement.Append(AddTagsToReview(new_ro, TagsToAdd) + "; ");
            SQL_Statement.Append(RemoveTagsFromReview(new_ro, TagsToRemove) + "; ");
                        
            return SQL_Statement.ToString();
          
        }

        private static string RemoveTagsFromReview(ReviewObject new_ro, List<TagObject> tagsToRemove)
        {
            if(tagsToRemove.Count < 1) { return ""; }

            StringBuilder SQL_Statement = new StringBuilder();

            SQL_Statement.Append("Delete from ReviewTags where ReviewID in (Select ID as ReviewID from MusicReview where Title = '" + new_ro.Title + "' AND Artist = '" + new_ro.Artist + "' AND Release_Date = " + new_ro.Release_Date + ") " +
                " AND TagID in (Select TagID from Tags where Name = '" + tagsToRemove[0].Name + "'");

            for(int x = 1; x < tagsToRemove.Count; x++)
            {
                SQL_Statement.Append(" OR Name = '" + tagsToRemove[x].Name + "'");
            }

            return SQL_Statement.ToString() + ")";
        }

        //Get the SQL statement corresponding with a set of included strings
        public static string GetMusicReviews_Builder(SQL_Review_REQUEST request)
        {
            StringBuilder SQLRequest = new StringBuilder();
            bool needsAnd = false;
            bool somethingAdded = false; 

            //Returns "Review Reference" not a review;
            SQLRequest.Append("Select Title,Release_Date,Review_Date,Album,Artist,Rating From MusicReview Where ");

            if (!request.Artist.Equals(""))
            {
                somethingAdded = true;
                SQLRequest.Append("Artist = '" + request.Artist + "'");
                needsAnd = true;
            }
            if (!request.Album.Equals(""))
            {
                somethingAdded = true;
                if (needsAnd) { SQLRequest.Append(" AND Album = '" + request.Album + "'"); }
                else SQLRequest.Append("Album = '" + request.Album + "'");
            }
            //Check Comparative Statements 
            ComparatorAdd("Rating", '>', request.RatingMax);
            ComparatorAdd("Rating", '<', request.RatingMin);
            ComparatorAdd("Release_Date", '>', request.ReleaseYearMax);
            ComparatorAdd("Release_Date", '<', request.ReleaseYearMin);
            ComparatorAdd("Review_Date", '>', request.ReviewYearMax);
            ComparatorAdd("Review_Date", '<', request.ReviewYearMin);

            string join_tags = "Select ID from MusicReview inner join ReviewTags on ID = ReviewID inner join Tags on ReviewTags.TagID = Tags.TagID where Name in";

            //Add the Included Tags 
            if (!AllNull(request.Included_Tags))
            {
                somethingAdded = true;
                if (needsAnd)
                {
                    SQLRequest.Append(" AND ID in (" + join_tags);
                }
                else
                {
                    SQLRequest.Append("ID in (" + join_tags);
                }
               
                foreach (List<TagObject> Request in request.Included_Tags.Where(n=> n != null))
                {                  
                    SQLRequest.Append("(");
                    foreach(TagObject Tag in Request)
                    {
                        SQLRequest.Append("'" + Tag.Name + "',");
                                    
                    }
                    SQLRequest.Remove(SQLRequest.Length - 1, 1);
                    SQLRequest.Append(") group by ID having count(ID) = " + Request.Count + " union " + join_tags);
                }

                SQLRequest.Remove(SQLRequest.Length - (join_tags.Length + 6), (join_tags.Length+6));
                needsAnd = true;
                SQLRequest.Append(")");
            }

            //Exclude the excluded tags
            if(!AllNull(request.Excluded_Tags))
            {
                somethingAdded = true;
                if (needsAnd)
                {
                    SQLRequest.Append(" AND ID not in (" + join_tags);
                }
                else
                {
                    SQLRequest.Append("ID not in (" + join_tags);
                }

                //Select ID from(MusicReview inner join ReviewTags on ID = ReviewID inner join Tags on ReviewTags.TagID = Tags.TagID) where Name in ('english', 'fake') group by ID having count(ID) = 2
                foreach (List<TagObject> Request in request.Excluded_Tags.Where(n => n != null))
                {
                    SQLRequest.Append("(");
                    foreach (TagObject Tag in Request)
                    {
                        SQLRequest.Append("'" + Tag.Name + "',");

                    }
                    SQLRequest.Remove(SQLRequest.Length - 1, 1);
                    SQLRequest.Append(") group by ID having count(ID) = " + Request.Count + " union " + join_tags);
                }

                SQLRequest.Remove(SQLRequest.Length - (join_tags.Length + 6), (join_tags.Length + 6));
                needsAnd = true;
                SQLRequest.Append(")");
            }

            void ComparatorAdd(string field, char symbol, int value)
            {
                if (value >= 0)
                {
                    somethingAdded = true;
                    addAndRequest(field + ' ' + symbol + ' ' + value);
                    needsAnd = true;
                }
            }                    
            void addAndRequest(string Request)
            {
                if (needsAnd) { SQLRequest.Append(" AND " + Request); }
                else 
                    SQLRequest.Append(Request);
            }          
            
            if(somethingAdded == false){ return "Select Title,Release_Date,Review_Date,Album,Artist,Rating From MusicReview"; }

            return SQLRequest.ToString();  
        }

        private static bool AllNull<T>(List<T> List)
        {
            foreach(var temp in List)
            {
                if(temp != null) { return false; }
            }

            return true;
        }

        public static string GetMusicReviews_Builder(SQL_Review_REQUEST including, SQL_Review_REQUEST excluding)
        {
            string SQLRequest = "";


            return SQLRequest;
        }

        

    }
}

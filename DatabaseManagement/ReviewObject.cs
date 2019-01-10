using System.Collections.Generic;

namespace DatabaseManagement
{
    //Only store the info that will be displayed on the sort page no tags etc.. 
    //this is done for memory purposes
    public class Review_Reference
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public float Rating { get; set; }
        public int Release_Date { get; set; }
        public int Review_Date { get; set; }
        public string Album { get; set; }

        public Review_Reference(ReviewObject ro)
        {
            Title = ro.Title;
            Artist = ro.Artist;
            Rating = ro.Rating;
            Release_Date = ro.Release_Date;
            Review_Date = ro.Review_Date;
            Album = ro.Album;
        }
        
        public Review_Reference(string[] row)
        {
            Title = row[0];
            Release_Date = int.Parse(row[1]);
            Review_Date = int.Parse(row[2]);
            Album = row[3];
            Artist = row[4];
            Rating = float.Parse(row[5]);
        }

        public bool Valid()
        {
            if (this.Title != null && this.Release_Date != -1 && this.Artist != null) { return true; }
            return false;
        }

        //We need to make this, add SQL fuuncton to get ReviewOBject from ReviewRefernece -DBmanager / SQL
        //We need a function to get all Review_References in the DB. -DBManager /SQL
    }

    public enum Review_Fields
    {
        Title,
        Artist,
        Rating,
        Release_Date,
        Review_Date,
        Album,
        Review,
        File_Path
    };
    
    public class ReviewObject
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public float Rating { get; set; }
        public int Release_Date { get; set; }
        public int Review_Date { get; set; } // may not be used 
        public string Album { get; set; }
        public string Review { get; set; }
        public string File_Path { get; set; }

        private List<TagObject> Tags;        

        public ReviewObject()
        {
            this.Review_Date = -1; //Check values to see if anything has been entered or not 
            this.Release_Date = -1;
            this.Rating = -1;

            Title = Artist = Album = Review = File_Path = null;
            Release_Date = Review_Date = -1;
            Rating = (float)-1.0;

            Tags = new List<TagObject>();
        }

        public ReviewObject(string[] row)
        {
            Title = row[1];
            Release_Date = int.Parse(row[2]);
            Review_Date = int.Parse(row[3]);
            Album = row[4];
            Artist = row[5];
            Rating = float.Parse(row[6]);
            Review = row[7];
            File_Path = row[8];            
        }

        public ReviewObject(ReviewObject givenReview)
        {
            this.Title = givenReview.Title;
            this.Release_Date = givenReview.Release_Date;
            this.Review_Date = givenReview.Review_Date;
            this.Album = givenReview.Album;
            this.Artist = givenReview.Artist;
            this.Rating = givenReview.Rating;
            this.Review = givenReview.Review;
            this.File_Path = givenReview.File_Path;

            this.Tags = givenReview.Tags;
        }

        public void setTags(List<TagObject> Tags){this.Tags = Tags;}

        public List<TagObject> getTags(){return this.Tags; }

        public void EmptyTags(){this.Tags.Clear();}

        public bool HasTag(TagObject tag) { return this.Tags.Contains(tag); }

        public bool HasTag(string TagName)
        {
            foreach(TagObject tag in this.Tags)
            {
                if (tag.Name.Equals(TagName)) { return true; }
            }

            return false;
        }
      
        public bool Valid()
        {
            if (this.Title != null && this.Release_Date != -1 && this.Artist != null) { return true; }
            return false;
        }

        public void Clear()
        {
            this.Tags.Clear();
            this.Review_Date = -1; //Check values to see if anything has been entered or not 
            this.Release_Date = -1;
            this.Rating = -1;

            Title = Artist = Album = Review = File_Path = null;
            Release_Date = Review_Date = -1;
            Rating = (float)-1.0;
        }

        public void AddTag(TagObject Tag)
        {
            this.Tags.Add(Tag);
        }

        public void RemoveTag(TagObject newTag)
        {
            this.Tags.Remove(newTag);
        }
    }
}

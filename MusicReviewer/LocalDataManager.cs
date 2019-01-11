using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DatabaseManagement;

namespace MusicReviewerApp
{
    public partial class TagBox : CheckBox
    {
        public TagObject TagData;

        public TagBox(TagObject tag)
        {
            this.TagData = tag;
            this.Name = tag.Name.Replace(' ','_');
            this.Content = tag.Name;
        }
    }   
    public class LocalDataManager
    {
        public enum PageTypes
        {
            ReviewPage,
            SortPage,
            StatPage,
            TagPage,
            EditReviewPage
        };

        //Manage the Database 
        public DatabaseManager DBManager;

        //Always manages all Tags
        public List<TagObject> Tags;
        public List<TagObject> LanguageTags;
        public List<TagObject> GenreTags;
        public List<TagObject> InstrumentTags;

        //Maintain pages between clicks so "new" pages are always being created.
        //Windows are generated when needed.
        private ReviewPage ReviewPage;
        private SortPage SortPage;
        private StatPage StatPage;
        private TagPage TagPage;

        public LocalDataManager()
        {
            ReviewPage = null;
            SortPage = null;
            StatPage = null;
            TagPage = null;

            //Generate all of the tags upon construction.
            DBManager = new DatabaseManager();

            InstrumentTags = new List<TagObject>();
            LanguageTags = new List<TagObject>();
            GenreTags = new List<TagObject>();

            Tags = DBManager.GetAllTags();

            foreach(TagObject Tag in Tags)
            {
                switch (Tag.Type)
                {
                    case TagType.Genre:
                        GenreTags.Add(Tag);
                        break;
                    case TagType.Instrument:
                        InstrumentTags.Add(Tag);
                        break;
                    case TagType.Language:
                        LanguageTags.Add(Tag);
                        break;
                }
            }

            //Sort in Alphabetical Order: 
            GenreTags.Sort((r1, r2) => r1.Name.CompareTo(r2.Name));
            LanguageTags.Sort((r1, r2) => r1.Name.CompareTo(r2.Name));
            InstrumentTags.Sort((r1, r2) => r1.Name.CompareTo(r2.Name));
        }      
        public bool IsATag(string givenTag)
        {
            foreach (TagObject tag in Tags)
            {
                if (tag.Name.Equals(givenTag)) { return true; }
            }
            return false;
        }
        public Page RequestPage(PageTypes type)
        {
            switch (type)
            {
                case PageTypes.TagPage:
                    if (TagPage == null) { return createNewPage(PageTypes.TagPage); }
                    else return TagPage;
                case PageTypes.ReviewPage:
                    if (ReviewPage == null) { return createNewPage(PageTypes.ReviewPage); }
                    else return ReviewPage;
                case PageTypes.SortPage:
                    if (SortPage == null) { return createNewPage(PageTypes.SortPage); }
                    else return SortPage;
                case PageTypes.StatPage:
                    if (StatPage == null) { return createNewPage(PageTypes.StatPage); }
                    else return StatPage;               
            }

            return null;
        }

        public void AddReview(ReviewObject givenReview)
        {
            this.DBManager.AddReview(givenReview);

            if (this.SortPage != null)
            {
                this.SortPage.Local_Reviews.Add(new Review_Reference(givenReview));
                this.SortPage.TableView.Items.Refresh();
            }
        }
        private Page createNewPage(PageTypes type)
        {
            switch (type)
            {
                case PageTypes.ReviewPage:
                    this.ReviewPage = new ReviewPage(this);
                    return this.ReviewPage;
                case PageTypes.SortPage:
                    this.SortPage = new SortPage(this);
                    return this.SortPage;
                case PageTypes.StatPage:
                    this.StatPage = new StatPage(this);
                    return this.StatPage;
                case PageTypes.TagPage:
                    this.TagPage = new TagPage(this);
                    return this.TagPage;            
            }

            return null;
        }     

        public void LoadTags(List<TagObject> Tags)
        {
            this.Tags = Tags;
        }     
        public List<Review_Reference> GetReviewsFromRequest(SQL_Review_REQUEST searchRequest)
        {
            return this.DBManager.getReviews(searchRequest);
        }
        public void AddTag(TagObject Tag)
        {
            this.DBManager.AddTag(Tag);
            this.Tags.Add(Tag);
                      
            switch (Tag.Type)
            {
                case TagType.Genre:
                    GenreTags.Add(Tag);
                    GenreTags.Sort((r1, r2) => r1.Name.CompareTo(r2.Name));
                    break;
                case TagType.Instrument:
                    InstrumentTags.Add(Tag);
                    InstrumentTags.Sort((r1, r2) => r1.Name.CompareTo(r2.Name));
                    break;
                case TagType.Language:
                    LanguageTags.Add(Tag);
                    LanguageTags.Sort((r1, r2) => r1.Name.CompareTo(r2.Name));
                    break;
            }
        }
        public void RemoveTag(TagObject Tag)
        {
            //If the tag is in the current review (odd case, but kind of needs to be done)'
            if (ReviewPage.currentReview.getTags().Contains(Tag))
            {
                ReviewPage.currentReview.EmptyTags();
                ReviewPage.ClearReviewPage();
            }

            this.DBManager.deleteTag(Tag);
            this.Tags.Remove(Tag);
            switch (Tag.Type)
            {
                case TagType.Genre:
                    GenreTags.Remove(Tag);
                    break;
                case TagType.Instrument:
                    InstrumentTags.Remove(Tag);
                    break;
                case TagType.Language:
                    LanguageTags.Remove(Tag);                   
                    break;
            }
        }                  
    }
}
using System.Windows;
using DatabaseManagement;

namespace MusicReviewerApp
{
    public partial class Edit_Window : Window
    {
        public Edit_Window(LocalDataManager LocalData, ReviewObject SentReview)
        {
            InitializeComponent();
            this.Content = new ReviewPage(LocalData,SentReview,this);
        }
    }
}

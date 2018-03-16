using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace CoolBooks.Models
{
    public class HomeBookViewModel
    {        
        public Books RandomBook { get; set; }
        public List<Books> LatestBooks { get; set; }
    }

    public class DetailsBooksViewModel
    {
        public Books Book { get; set; }
        public Reviews NewBookReivew { get; set; }
        public List<Reviews> UserReviews { get; set; }
        public int NrOfReviewPages { get; set; }
        public int CurrentReviewPage { get; set; } 
        public bool UserIsAdmin { get; set; }
        public string UserId { get; set; }
        public bool UserSignedIn { get; set; }
        public PagedList<Reviews> ReviewMenu { get; set; }
    }

    public class CreateBookViewModel
    {
        public string Title { get; set; }
        public int GenreId { get; set; }        
        public string AuthorFirstName { get; set; }
        public string AuthorLastName { get; set; }
        public string AlternativeTitle { get; set; }
        public short? Part { get; set; }
        public string Description { get; set; }        
        public string ISBN { get; set; }
        public DateTime? PublishDate { get; set; }
        public string ImagePath { get; set; }                                
    }
}
using BlogProject.BLL.Abstract;
using BlogProject.DAL.EntityFramework;
using BlogProject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogProject.BLL
{
    public class CategoryManager : ManagerBase<Category>
    {
        public override int Delete(Category cat)
        {
            BlogManager blogManager = new BlogManager();
            LikedManager likedManager = new LikedManager();
            CommentManager commentManager = new CommentManager();
            
            // kategori ile ilişkili blogların silinmesi gerekiyor.
            foreach (Blog blog in cat.Blogs.ToList())
            {
                //Bloglar ile ilişkili likeların silinmesi gerekiyor.
                foreach (Like like in blog.Likes.ToList())
                {
                    likedManager.Delete(like);
                }

                //Bloglar ile ilişkili commentların silinmesi gerekiyor.
                foreach (Comment comment in blog.Comments.ToList())
                {
                    commentManager.Delete(comment);
                }

                blogManager.Delete(blog);
            }
            return base.Delete(cat);
        }
    }
}

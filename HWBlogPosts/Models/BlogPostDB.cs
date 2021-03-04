using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HWBlogPosts.Models
{
    public class HomeViewModel
    {
        public List<BlogPost> BlogPosts { get; set; }
        public int Page { get; set; }
        public int AmountOfPages { get; set; }
    }
    public class IndividualPostViewModel
    {
        public BlogPost BlogPost { get; set; }
        public List<Comment> Comments { get; set; }        
        public string Name { get; set; }
        
    }
    public class BlogPost
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set;}
        public int Id { get; set; }
    }
    public class Comment
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }        
        public int BlogPostId { get; set; }
    }
    public class BlogPostDB
    {
        private readonly string _connectionString;
        public BlogPostDB(string connectionString)
        {
            _connectionString = connectionString;
        }                
        public BlogPost GetBlogPost(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM BlogPosts WHERE Id=@id";
            command.Parameters.AddWithValue("@id", id);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            return new BlogPost
            {
                Date = (DateTime)reader["Date"],
                Id = (int)reader["id"],
                Text = (string)reader["Text"],
                Title = (string)reader["Title"]
            };
        }
        public List<Comment> GetCommentsForPost(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Comments WHERE blogPostid = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<Comment> comments = new();
             while(reader.Read())
            {
                comments.Add(new Comment
                {
                    Date = (DateTime)reader["Date"],
                    BlogPostId=(int)reader["blogpostid"],
                    Content=(string)reader["Comment"],
                    Name=(string)reader["Name"]                   
                });
            }
            return comments;
        }
        public void AddBlogPost(BlogPost bp)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO BlogPosts(Title,Text,Date)
                                    VALUES (@title,@text,@date)";
            command.Parameters.AddWithValue("@title", bp.Title);
            command.Parameters.AddWithValue("@text", bp.Text);
            command.Parameters.AddWithValue("@date", DateTime.Now);                
            connection.Open();
            command.ExecuteNonQuery();
        }
        public void AddComment(Comment c)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Comments(BlogPostId,Name,Date,Comment)
                                    VALUES (@blogPostId,@name,@date,@comment)";
            command.Parameters.AddWithValue("@blogPostId", c.BlogPostId);
            command.Parameters.AddWithValue("@name",c.Name);
            command.Parameters.AddWithValue("@date", DateTime.Now);
            command.Parameters.AddWithValue("@comment", c.Content);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public int MostRecentBPId()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT TOP 1 Id FROM BlogPosts ORDER BY Date DESC";
            connection.Open();
            int id = (int)command.ExecuteScalar();
            return id;
        }
        public List<BlogPost> GetPostsPerPage(int page)
        {
            int offset = page * 5;
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Blogposts ORDER BY Date DESC OFFSET @offset ROWS FETCH NEXT 5 ROWS ONLY";
            command.Parameters.AddWithValue("@offset", offset);
            connection.Open();
            List<BlogPost> blogPosts = new();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {                
                blogPosts.Add(BuildInner(reader));
            }
            return blogPosts;
        }
        public BlogPost BuildInner(SqlDataReader reader)
        {
            string text = (string)reader["Text"];
            if (text.Length > 200)
            {
                text = text.Substring(0, 200);
                text += "...";
            }

            return new BlogPost
            {
                Date = (DateTime)reader["Date"],
                Id = (int)reader["id"],
                Text = text,
                Title = (string)reader["Title"]
            };
        }
        public int AmountOfPages()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM BlogPosts";
            connection.Open();
            int amountOfBlogposts = (int)command.ExecuteScalar();
            int pages = amountOfBlogposts / 5;
            if (amountOfBlogposts % 5 == 0)
            {
                return pages;
            }
            return (pages + 1);

        }
    }
}

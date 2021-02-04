using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ForumAPI.Services.BoardService.BoardDataContext
{
    internal sealed class BoardContext : DbContext
    {
        public DbSet<Board> Boards { get; set; }
        
        public DbSet<Topic> Topics { get; set; }
        
        public DbSet<Post> Posts { get; set; }
        
        public DbSet<AdditionalPostInfo> AdditionalPostInfos { get; set; }

        public BoardContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
    }

    internal sealed class Board
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Alias { get; set; }
        
        public IList<Topic> Topics { get; set; }
    }

    internal sealed class Topic
    {
        public Guid Id { get; set; }
        
        public Guid BoardId { get; set; }
        
        public Board Board { get; set; }
        
        public Guid CreaterId { get; set; }
        
        public string TopicHeader { get; set; }
        
        public IList<Post> Posts { get; set; }
    }

    internal sealed class Post
    {
        public Guid Id { get; set; }
        
        public Guid TopicId { get; set; }
        
        public Topic Topic { get; set; }
        
        public Guid PosterId { get; set; }
        
        public string Text { get; set; }
        
        public IList<AdditionalPostInfo> AdditionalPostInfos { get; set; }
    }

    internal sealed class AdditionalPostInfo
    {
        public Guid Id { get; set; }
        
        public Guid PostId { get; set; }
        
        public Post Post { get; set; }
        
        public string ContentURL { get; set; }
    }
}
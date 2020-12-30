using System;
using System.Collections.Generic;
using System.Linq;
using ForumAPI.Services.BoardService.BoardDataContext;
using Microsoft.EntityFrameworkCore;

namespace ForumAPI.Services.BoardService
{
    public class BoardService
    {
        private BoardContext boardContext;

        public BoardService(BoardContext boardContext)
        {
            this.boardContext = boardContext;
        }

        public IList<BoardModel> GetBoards()
        {
            var boards = boardContext.Boards.Include(brd => brd.Topics).ThenInclude(top => top.Posts)
                .ThenInclude(pst => pst.AdditionalPostInfos);
            var boardModels = boards.Select(brd => MapToModel(brd)).ToList();

            return boardModels;
        }

        private BoardModel MapToModel(Board board)
        {
            return new BoardModel
            {
                Id = board.Id,
                Name = board.Name,
                Topics = board.Topics.Select(top => new TopicModel
                {
                    Id = top.Id,
                    CreaterId = top.CreaterId,
                    TopicHeader = top.TopicHeader,
                    Posts = top.Posts.Select(pst => new PostModel
                    {
                        Id = pst.Id,
                        PosterId = pst.PosterId,
                        Text = pst.Text,
                        AdditionalPostInfos = pst.AdditionalPostInfos.Select(inf => new AdditionalPostInfoModel
                        {
                            Id = inf.Id,
                            ContentURL = inf.ContentURL
                        }).ToList()
                    }).ToList()
                }).ToList()
            };
        }

        private Board MapFromModel(BoardModel model)
        {
            return new Board
            {
                Id = model.Id,
                Name = model.Name,
                Topics = model.Topics.Select(top => new Topic
                {
                    Id = top.Id,
                    BoardId = model.Id,
                    CreaterId = top.CreaterId,
                    TopicHeader = top.TopicHeader,
                    Posts = top.Posts.Select(pst => new Post
                    {
                        Id = pst.Id,
                        TopicId = top.Id,
                        PosterId = pst.PosterId,
                        Text = pst.Text,
                        AdditionalPostInfos = pst.AdditionalPostInfos.Select(inf => new AdditionalPostInfo
                        {
                            Id = inf.Id,
                            PostId = pst.Id,
                            ContentURL = inf.ContentURL
                        }).ToList()
                    }).ToList()
                }).ToList()
            };
        }
    }

    public class BoardModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IList<TopicModel> Topics { get; set; }
    }

    public class TopicModel
    {
        public Guid Id { get; set; }

        public Guid CreaterId { get; set; }

        public string TopicHeader { get; set; }

        public IList<PostModel> Posts { get; set; }
    }

    public class PostModel
    {
        public Guid Id { get; set; }

        public Guid PosterId { get; set; }

        public string Text { get; set; }

        public IList<AdditionalPostInfoModel> AdditionalPostInfos { get; set; }
    }

    public class AdditionalPostInfoModel
    {
        public Guid Id { get; set; }

        public string ContentURL { get; set; }
    }
}
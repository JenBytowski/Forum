using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForumAPI.Services.BoardService.BoardDataContext;
using Microsoft.EntityFrameworkCore;

namespace ForumAPI.Services.BoardService
{
    public interface IBoardService
    {
        IEnumerable<BoardModel> GetBoards();

        Task<BoardModel> GetBoardByName(string board);

        Task<TopicModel> GetTopic(GetTopicRequest request);

        Task CreateTopic(CreateTopicRequest request);
    }

    internal sealed class BoardService : IBoardService
    {
        private BoardContext boardContext;

        public BoardService(BoardContext boardContext)
        {
            this.boardContext = boardContext;
        }

        public IEnumerable<BoardModel> GetBoards()
        {
            this.boardContext.Boards.Load();
            var boards = this.boardContext.Boards.Local.Select(brd => BoardModel.MapToModel(brd));

            return boards;
        }

        public async Task<BoardModel> GetBoardByName(string boardAlias)
        {
            var boards = this.boardContext.Boards.Include(brd => brd.Topics).ThenInclude(top => top.Posts);
            var board = await boards.SingleAsync(brd => brd.Alias == boardAlias);

            return BoardModel.MapToModel(board);
        }

        public async Task<TopicModel> GetTopic(GetTopicRequest request)
        {
            await this.boardContext.Boards.Where(brd => brd.Alias == request.BoardAlias).LoadAsync();
            var board = this.boardContext.Boards.Local.Single(brd => brd.Alias == request.BoardAlias);
            await this.boardContext.Entry(board).Collection(brd => brd.Topics).LoadAsync();
            var topic = board.Topics.Single(tp => tp.Id == request.TopicId);
            await this.boardContext.Entry(topic).Collection(tp => tp.Posts).LoadAsync();

            return TopicModel.MapToModel(topic);
        }

        //TODO fix this crap
        public async Task CreateTopic(CreateTopicRequest request)
        {
            if (request.BoardId == default && request.BoardAlias == default)
            {
                throw new InvalidOperationException("board id and alias are null");
            }

            await this.boardContext.Boards.Where(brd => brd.Id == request.BoardId || brd.Alias == request.BoardAlias)
                .LoadAsync();

            var board = request.BoardId != default
                ? this.boardContext.Boards.Local.Single(brd => brd.Id == request.BoardId)
                : this.boardContext.Boards.Local.Single(brd => brd.Alias == request.BoardAlias);

            if ((request.BoardId != default && request.BoardId != board.Id) ||
                (request.BoardAlias != default && request.BoardAlias != board.Alias))
            {
                throw new InvalidOperationException("board id and alias are mismatch");
            }
            
            await this.boardContext.Entry(board).Collection(brd => brd.Topics).LoadAsync();

            if (board.Topics == default)
            {
                throw new InvalidOperationException($"{board.Name} topics are null");
            }

            if (request.BoardId == default)
            {
                request.BoardId = board.Id;
            }
            
            //some shit
            var newTopicModel = CreateTopicRequest.MapToModel(request);
            var newTopic = TopicModel.MapFromModel(newTopicModel);

            board.Topics.Add(newTopic);
            await this.boardContext.SaveChangesAsync();
        }
    }

    public sealed class GetTopicRequest
    {
        public string BoardAlias { get; set; }

        public Guid TopicId { get; set; }
    }

    public sealed class CreateTopicRequest
    {
        public Guid? BoardId { get; set; }

        public string BoardAlias { get; set; }

        public Guid? CreaterId { get; set; }

        public string TopicHeader { get; set; }

        public string Text { get; set; }

        public IEnumerable<string> AdditionalPostInfos { get; set; }

        public static TopicModel MapToModel(CreateTopicRequest request)
        {
            return new TopicModel
            {
                BoardId = request.BoardId.Value,
                CreaterId = request.CreaterId != default ? request.CreaterId.Value : Guid.NewGuid(),
                TopicHeader = request.TopicHeader,
                Posts = new List<PostModel>
                {
                    new PostModel
                    {
                        PosterId = Guid.NewGuid(),
                        Text = request.Text,
                        AdditionalPostInfos = request.AdditionalPostInfos.Select(inf => new AdditionalPostInfoModel
                        {
                            ContentURL = inf
                        }).ToList()
                    }
                }
            };
        }
    }

    public sealed class BoardModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public IList<TopicModel> Topics { get; set; }

        internal static BoardModel MapToModel(Board board)
        {
            return new BoardModel
            {
                Id = board.Id,
                Name = board.Name,
                Alias = board.Alias,
                Topics = board.Topics?.Select(top => TopicModel.MapToModel(top)).ToList()
            };
        }

        internal static Board MapFromModel(BoardModel model)
        {
            return new Board
            {
                Id = model.Id,
                Name = model.Name,
                Topics = model.Topics.Select(top => TopicModel.MapFromModel(top)).ToList()
            };
        }
    }

    public sealed class TopicModel
    {
        public Guid Id { get; set; }

        public Guid BoardId { get; set; }

        public Guid CreaterId { get; set; }

        public string TopicHeader { get; set; }

        public IList<PostModel> Posts { get; set; }

        internal static TopicModel MapToModel(Topic topic)
        {
            return new TopicModel()
            {
                Id = topic.Id,
                BoardId = topic.BoardId,
                TopicHeader = topic.TopicHeader,
                CreaterId = topic.CreaterId,
                Posts = topic.Posts?.Select(pst => PostModel.MapToModel(pst)).ToList()
            };
        }

        internal static Topic MapFromModel(TopicModel model)
        {
            return new Topic
            {
                Id = model.Id,
                BoardId = model.BoardId,
                TopicHeader = model.TopicHeader,
                CreaterId = model.CreaterId,
                Posts = model.Posts?.Select(pst => PostModel.MapFromModel(pst)).ToList()
            };
        }
    }

    public sealed class PostModel
    {
        public Guid Id { get; set; }

        public Guid TopicId { get; set; }

        public Guid PosterId { get; set; }

        public string Text { get; set; }

        public IList<AdditionalPostInfoModel> AdditionalPostInfos { get; set; }

        internal static PostModel MapToModel(Post post)
        {
            return new PostModel
            {
                Id = post.Id,
                TopicId = post.TopicId,
                PosterId = post.PosterId,
                Text = post.Text,
                AdditionalPostInfos = post.AdditionalPostInfos?.Select(inf => AdditionalPostInfoModel.MapToModel(inf))
                    .ToList()
            };
        }

        internal static Post MapFromModel(PostModel model)
        {
            return new Post
            {
                Id = model.Id,
                TopicId = model.TopicId,
                PosterId = model.PosterId,
                Text = model.Text,
                AdditionalPostInfos = model.AdditionalPostInfos
                    ?.Select(inf => AdditionalPostInfoModel.MapFromModel(inf)).ToList()
            };
        }
    }

    public sealed class AdditionalPostInfoModel
    {
        public Guid Id { get; set; }

        public Guid PostId { get; set; }

        public string ContentURL { get; set; }

        internal static AdditionalPostInfoModel MapToModel(AdditionalPostInfo additionalPostInfo)
        {
            return new AdditionalPostInfoModel
            {
                Id = additionalPostInfo.Id,
                PostId = additionalPostInfo.PostId,
                ContentURL = additionalPostInfo.ContentURL
            };
        }

        internal static AdditionalPostInfo MapFromModel(AdditionalPostInfoModel additionalPostInfo)
        {
            return new AdditionalPostInfo
            {
                Id = additionalPostInfo.Id,
                PostId = additionalPostInfo.PostId,
                ContentURL = additionalPostInfo.ContentURL
            };
        }
    }
}
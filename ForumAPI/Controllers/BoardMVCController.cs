using System;
using System.Net;
using System.Threading.Tasks;
using ForumAPI.Services.BoardService;
using Microsoft.AspNetCore.Mvc;

namespace ForumAPI.Controllers
{
    [Route("boards")]
    [Controller]
    public sealed class BoardMVCController : Controller
    {
        private readonly IBoardService boardService;

        public BoardMVCController(IBoardService boardService)
        {
            this.boardService = boardService;
        }

        [HttpGet]
        [ApiExplorerSettings(GroupName = "boards")]
        public IActionResult GetBoards()
        {
            var boards = boardService.GetBoards();

            return View(boards);
        }

        [HttpGet]
        [Route("{boardAlias}")]
        [ApiExplorerSettings(GroupName = "boards")]
        public async Task<IActionResult> GetBoard([FromRoute] string boardAlias)
        {
            try
            {
                var board = await boardService.GetBoardByName(boardAlias);

                return View(board);
            }
            catch (InvalidOperationException e)
            {
                return RedirectToAction(nameof(this.GetBoards));
            }
        }

        [HttpGet]
        [Route("{boardAlias}/{topicId}")]
        [ApiExplorerSettings(GroupName = "topics")]
        public async Task<IActionResult> GetTopic(string boardAlias, Guid topicId)
        {
            try
            {
                var topic = await this.boardService.GetTopic(new GetTopicRequest
                    {BoardAlias = boardAlias, TopicId = topicId});

                return View(topic);
            }
            catch (InvalidOperationException e)
            {
                return RedirectToAction(nameof(this.GetBoard), routeValues: boardAlias);
            }
        }

        [HttpPost]
        [Route("{boardAlias}")]
        [ApiExplorerSettings(GroupName = "topics")]
        public async Task<IActionResult> CreateTopic([FromRoute] string boardAlias,
            [FromForm] CreateTopicRequest request)
        {
            request.BoardAlias = boardAlias;
            await this.boardService.CreateTopic(request);

            return RedirectToAction(nameof(this.GetBoards));
        }

        [HttpPost]
        [Route("{boardAlias}/{topicId}")]
        [ApiExplorerSettings(GroupName = "posts")]
        public async Task<IActionResult> CreatePost([FromRoute] string boardAlias, [FromForm] CreatePostRequest request)
        {
            await this.boardService.CreatePost(request);

            return RedirectToAction(nameof(this.GetTopic), new {boardAlias = boardAlias, topicId = request.TopicId});
        }
        
        [HttpPut]
        [Route("{boardAlias}/{topicId}")]
        [ApiExplorerSettings(GroupName = "posts")]
        public async Task<IActionResult> RefactorPost([FromRoute] string boardAlias, [FromRoute] Guid topicId,
            [FromForm] RefactorPostRequest request)
        {
            await this.boardService.RefactorPost(request);

            return StatusCode((int) HttpStatusCode.NoContent);
        }

        [HttpDelete]
        [Route("{boardAlias}/{topicId}/{postId}")]
        [ApiExplorerSettings(GroupName = "posts")]
        public async Task<IActionResult> DeletePost([FromRoute] string boardAlias, [FromRoute] Guid topicId,
            [FromRoute] Guid postId)
        {
            await this.boardService.RemovePost(postId);

            return RedirectToAction(nameof(this.GetTopic), new {boardAlias = boardAlias, topicId = topicId});
        }
    }
}
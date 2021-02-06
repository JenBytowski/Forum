using System;
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
        public IActionResult GetBoards()
        {
            var boards = boardService.GetBoards();

            return View(boards);
        }

        [HttpGet]
        [Route("{boardAlias}")]
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
        public async Task<IActionResult> CreateTopic([FromRoute]string boardAlias, [FromForm]CreateTopicRequest request)
        {
            request.BoardAlias = boardAlias;
            await this.boardService.CreateTopic(request);

            return RedirectToAction(nameof(this.GetBoards));
        }
    }
}
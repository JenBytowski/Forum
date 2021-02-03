using System;
using System.Threading.Tasks;
using ForumAPI.Services.BoardService;
using Microsoft.AspNetCore.Mvc;

namespace ForumAPI.Controllers
{
    [Route("topics")]
    [Controller]
    public sealed class TopicMVCController : Controller
    {
        private readonly IBoardService boardService;

        public TopicMVCController(IBoardService boardService)
        {
            this.boardService = boardService;
        }
        
        [HttpGet]
        [Route("{board}")]
        public async Task<IActionResult> GetTopics([FromRoute]string board)
        {
            var topics= await boardService.GetTopicsByBoard(board);

            return View(topics);
        }
    }
}
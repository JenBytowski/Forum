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
    }
}
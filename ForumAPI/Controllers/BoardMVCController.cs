using ForumAPI.Services.BoardService;
using Microsoft.AspNetCore.Mvc;

namespace ForumAPI.Controllers
{
    [Route("boards")]
    public sealed class BoardMVCController : Controller
    {
        private readonly BoardService boardService;
        
        public BoardMVCController(BoardService boardService)
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
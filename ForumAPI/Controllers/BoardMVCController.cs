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
        public async Task<IActionResult> GetBoard([FromRoute]string boardAlias)
        {
            var board = await boardService.GetBoardByName(boardAlias);

            return View(board);
        }
    }
}
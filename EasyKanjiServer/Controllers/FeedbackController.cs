using EasyKanjiServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyKanjiServer.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EasyKanjiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly DBContext _db;

        public FeedbackController(DBContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedback()
        {
            return await _db.Feedback.ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Feedback>> GetFeedback(int id)
        {
            var feedback = await _db.Feedback.FirstOrDefaultAsync(r => r.Id == id);

            if (feedback == null)
            {
                return NotFound();
            }

            return feedback;
        }

        [HttpPost]
        public async Task<ActionResult<FeedbackDTO>> PostFeedback(FeedbackDTO feedbackDTO)
        {
            var user = await _db.Users.FirstOrDefaultAsync(i => i.Username == feedbackDTO.Username);
            var feedback = new Feedback { Body = feedbackDTO.Body, Email = feedbackDTO.Email, User = user };
            await _db.Feedback.AddAsync(feedback);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFeedback), new { id = feedback.Id }, feedbackDTO);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Feedback>> DeleteFeedback(int id)
        {
            var feedback = await _db.Feedback.FirstOrDefaultAsync(i => i.Id == id);

            if (feedback == null)
                return NotFound();

            _db.Feedback.Remove(feedback);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}

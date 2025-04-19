using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Vet.DataAccess.Repository.IRepository;
using Vet.Models;

namespace VetApi.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class PostController(IUnitOfWork unitOfWork) : ControllerBase {
        // GET: api/Post
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts() {
            var posts = await unitOfWork.Post.GetAll(include: "Veterinarian");
            return Ok(posts);
        }

        // GET: api/Post/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Post>> GetPost(int id) {
            var post = await unitOfWork.Post.Get(
                filter: p => p.Id == id,
                include: "Veterinarian"
            );

            if (post == null) {
                return NotFound();
            }

            return Ok(post);
        }

        // GET: api/Post/vet/5
        [HttpGet("vet/{vetId}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPostsByVet(string vetId) {
            var posts = await unitOfWork.Post.GetAll(
                filter: p => p.VeterinarianId == vetId,
                include: "Veterinarian"
            );

            return Ok(posts);
        }

        // POST: api/Post
        [HttpPost]
        [Authorize(Roles = nameof(Veterinarian))]
        public async Task<ActionResult<Post>> CreatePost(Post post) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) {
                return Unauthorized("User not found.");
            }
            post.VeterinarianId = userId;
            await unitOfWork.Post.AddAsync(post);
            await unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        // PUT: api/Post/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Veterinarian")]
        public async Task<IActionResult> UpdatePost(int id, Post post) {
            if (id != post.Id) {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingPost = await unitOfWork.Post.Get(p => p.Id == id);

            if (existingPost == null) {
                return NotFound();
            }

            if (userId != existingPost.VeterinarianId) {
                return Forbid("You can only update your own posts.");
            }

            // Update only allowed fields
            existingPost.Title = post.Title;
            existingPost.Content = post.Content;
            existingPost.ImageUrl = post.ImageUrl;

            try {
                unitOfWork.Post.Update(existingPost);
                await unitOfWork.SaveAsync();
            }
            catch (DbUpdateConcurrencyException) {
                var exists = await unitOfWork.Post.Get(p => p.Id == id) != null;
                if (!exists) {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // DELETE: api/Post/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Veterinarian")]
        public async Task<IActionResult> DeletePost(int id) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = await unitOfWork.Post.Get(p => p.Id == id);

            if (post == null) {
                return NotFound();
            }

            if (userId != post.VeterinarianId) {
                return Forbid("You can only delete your own posts.");
            }

            unitOfWork.Post.Remove(post);
            await unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}
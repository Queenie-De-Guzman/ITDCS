using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static MyApp.Data.MusicContext;

namespace MyApp.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly MusicDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PlaylistController(MusicDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Playlist
        public async Task<IActionResult> Index()
        {
            var playlists = await _context.Playlists.Include(p => p.Music).ToListAsync();
            return View(playlists);
        }

        // GET: Playlist/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlaylistModel playlist)
        {
            if (ModelState.IsValid)
            {
                if (playlist.ImageFile != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(playlist.ImageFile.FileName);
                    string uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "images", fileName);

                    using (var fileStream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await playlist.ImageFile.CopyToAsync(fileStream);
                    }

                    playlist.ImagePath = "/images/" + fileName;
                }

                _context.Playlists.Add(playlist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(playlist);
        }

        // GET: Playlist/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist == null) return NotFound();

            return View(playlist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PlaylistModel playlist)
        {
            if (id != playlist.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (playlist.ImageFile != null)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(playlist.ImageFile.FileName);
                        string uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "images", fileName);

                        using (var fileStream = new FileStream(uploadPath, FileMode.Create))
                        {
                            await playlist.ImageFile.CopyToAsync(fileStream);
                        }

                        playlist.ImagePath = "/images/" + fileName;
                    }

                    _context.Update(playlist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaylistExists(playlist.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(playlist);
        }

        // GET: Playlist/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var playlist = await _context.Playlists.Include(p => p.Music).FirstOrDefaultAsync(m => m.Id == id);
            if (playlist == null) return NotFound();

            return View(playlist);
        }

        // GET: Playlist/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var playlist = await _context.Playlists.FirstOrDefaultAsync(m => m.Id == id);
            if (playlist == null) return NotFound();

            return View(playlist);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist != null)
            {
                // Delete image file
                if (!string.IsNullOrEmpty(playlist.ImagePath))
                {
                    string filePath = Path.Combine(_hostEnvironment.WebRootPath, playlist.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Playlists.Remove(playlist);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PlaylistExists(int id)
        {
            return _context.Playlists.Any(e => e.Id == id);
        }
    }
}

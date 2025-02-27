using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Models;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using static MyApp.Data.MusicContext;

namespace MyApp.Controllers
{
    public class MusicController : Controller
    {
        private readonly MusicDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public MusicController(MusicDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Music
        public async Task<IActionResult> Index()
        {
            var musicList = await _context.Music.Include(m => m.Playlist).ToListAsync();
            return View("Index", musicList);
        }

        // GET: Music/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var music = await _context.Music.Include(m => m.Playlist).FirstOrDefaultAsync(m => m.Id == id);
            if (music == null) return NotFound();

            return View("Details", music);
        }

        // GET: Music/Create
        public IActionResult Create()
        {
            ViewBag.Playlists = new SelectList(_context.Playlists, "Id", "Name");
            return View("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Artist,Album,Genre,Year,ImageFile,AudioFile,PlaylistId")] MusicModel music)
        {
            if (ModelState.IsValid)
            {
                // Handle Image Upload
                if (music.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);
                    string uniqueImageName = Guid.NewGuid().ToString() + Path.GetExtension(music.ImageFile.FileName);
                    string imagePath = Path.Combine(uploadsFolder, uniqueImageName);
                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await music.ImageFile.CopyToAsync(fileStream);
                    }
                    music.ImagePath = "/uploads/" + uniqueImageName;
                }

                // Handle Audio Upload
                if (music.AudioFile != null)
                {
                    string audioFolder = Path.Combine(_hostEnvironment.WebRootPath, "music");
                    Directory.CreateDirectory(audioFolder);
                    string uniqueAudioName = Guid.NewGuid().ToString() + Path.GetExtension(music.AudioFile.FileName);
                    string audioPath = Path.Combine(audioFolder, uniqueAudioName);
                    using (var fileStream = new FileStream(audioPath, FileMode.Create))
                    {
                        await music.AudioFile.CopyToAsync(fileStream);
                    }
                    music.AudioPath = "/music/" + uniqueAudioName;
                }

                _context.Music.Add(music);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Playlists = new SelectList(_context.Playlists, "Id", "Name", music.PlaylistId);
            return View("Create", music);
        }

        // GET: Music/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var music = await _context.Music.FindAsync(id);
            if (music == null) return NotFound();

            ViewBag.Playlists = new SelectList(_context.Playlists, "Id", "Name", music.PlaylistId);
            return View("Edit", music);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Artist,Album,Genre,Year,ImageFile,AudioFile,PlaylistId")] MusicModel music)
        {
            if (id != music.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingMusic = await _context.Music.FindAsync(id);
                    if (existingMusic == null) return NotFound();

                    existingMusic.Title = music.Title;
                    existingMusic.Artist = music.Artist;
                    existingMusic.Album = music.Album;
                    existingMusic.Genre = music.Genre;
                    existingMusic.Year = music.Year;
                    existingMusic.PlaylistId = music.PlaylistId;

                    // Handle new image upload
                    if (music.ImageFile != null)
                    {
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                        Directory.CreateDirectory(uploadsFolder);
                        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(music.ImageFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await music.ImageFile.CopyToAsync(fileStream);
                        }

                        // Delete old image
                        if (!string.IsNullOrEmpty(existingMusic.ImagePath))
                        {
                            string oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, existingMusic.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
                        }

                        existingMusic.ImagePath = "/uploads/" + uniqueFileName;
                    }

                    // Handle new audio upload
                    if (music.AudioFile != null)
                    {
                        string audioFolder = Path.Combine(_hostEnvironment.WebRootPath, "music");
                        Directory.CreateDirectory(audioFolder);
                        string uniqueAudioName = Guid.NewGuid().ToString() + Path.GetExtension(music.AudioFile.FileName);
                        string audioPath = Path.Combine(audioFolder, uniqueAudioName);
                        using (var fileStream = new FileStream(audioPath, FileMode.Create))
                        {
                            await music.AudioFile.CopyToAsync(fileStream);
                        }

                        // Delete old audio file
                        if (!string.IsNullOrEmpty(existingMusic.AudioPath))
                        {
                            string oldAudioPath = Path.Combine(_hostEnvironment.WebRootPath, existingMusic.AudioPath.TrimStart('/'));
                            if (System.IO.File.Exists(oldAudioPath)) System.IO.File.Delete(oldAudioPath);
                        }

                        existingMusic.AudioPath = "/music/" + uniqueAudioName;
                    }

                    _context.Update(existingMusic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MusicExists(music.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Playlists = new SelectList(_context.Playlists, "Id", "Name", music.PlaylistId);
            return View("Edit", music);
        }

        // GET: Music/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var music = await _context.Music.FirstOrDefaultAsync(m => m.Id == id);
            if (music == null) return NotFound();

            return View("Delete", music);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var music = await _context.Music.FindAsync(id);
            if (music != null)
            {
                // Delete image file if exists
                if (!string.IsNullOrEmpty(music.ImagePath))
                {
                    string filePath = Path.Combine(_hostEnvironment.WebRootPath, music.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                }

                // Delete audio file if exists
                if (!string.IsNullOrEmpty(music.AudioPath))
                {
                    string audioPath = Path.Combine(_hostEnvironment.WebRootPath, music.AudioPath.TrimStart('/'));
                    if (System.IO.File.Exists(audioPath)) System.IO.File.Delete(audioPath);
                }

                _context.Music.Remove(music);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MusicExists(int id)
        {
            return _context.Music.Any(e => e.Id == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcMusicStore_v10.Models;
using System.Data.Entity;
using System.IO;


namespace MvcMusicStore_v10.Controllers
{
    public class StoreManagerController : Controller
    {
        MusicModel storeDB = new MusicModel();
        
        public ActionResult Index()
        {
            var albums = storeDB.Albums.Include(a => a.Genre).Include(a => a.Artist);
            return View(albums.ToList());
        }
        public ActionResult Create()
        {
            ViewBag.GenreId = new SelectList(storeDB.Genres, "GenreId", "Name");
            ViewBag.ArtistId = new SelectList(storeDB.Artists, "ArtistId", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult Create(Album album)
        {
            if (ModelState.IsValid)
            {
                storeDB.Albums.Add(album);
                storeDB.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GenreId = new SelectList(storeDB.Genres, "GenreId", "Name", album.GenreId);
            ViewBag.ArtistId = new SelectList(storeDB.Artists, "ArtistId", "Name", album.ArtistId);
            return View(album);
        }
        public ActionResult Details(int id)
        {
            Album album = storeDB.Albums.Find(id);
            return View(album);
        }
        public ActionResult Edit(int id)
        {
            Album album = storeDB.Albums.Find(id);
            ViewBag.GenreId = new SelectList(storeDB.Genres, "GenreId", "Name");
            ViewBag.ArtistId = new SelectList(storeDB.Artists, "ArtistId", "Name");
            return View(album);
        }
        [HttpPost]
        public ActionResult Edit(Album album)
        {
            if (ModelState.IsValid)
            {
                storeDB.Entry(album).State = EntityState.Modified;
                storeDB.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GenreId = new SelectList(storeDB.Genres, "GenreId", "Name", album.GenreId);
            ViewBag.ArtistId = new SelectList(storeDB.Artists, "ArtistId", "Name", album.ArtistId);
            return View(album);
        }
        public ActionResult UploadImage()
        {
            Album album = new Album();
            return View(album);
        }

        [HttpPost]
        public ActionResult UploadImage(Album album, HttpPostedFileBase image1)
        {
            if (ModelState.IsValid)
            {
                //Set the model's properties and save to db
                album.AlbumArtUrl = Path.GetFileName(image1.FileName);             
                storeDB.Albums.Add(album);
                storeDB.SaveChanges();
                //Check if the file already exists or not null.
                if (image1 != null && image1.ContentLength > 0)
                {
                    try
                    {
                        string path = Path.Combine(Server.MapPath("~/uploads/"), Path.GetFileName(image1.FileName));
                        if (System.IO.File.Exists(path))
                        {
                            ViewBag.Message = "The File Already Exists in System";
                        }
                        else
                        {
                            image1.SaveAs(path);
                        }
                    }
                    catch (Exception exception)
                    {
                        ViewBag.Message = "ERROR:" + exception.Message.ToString();
                    }
                }
                else
                {
                    ViewBag.Message = "Specify the file";
                }
                //Return to Index where ImageCarousel should display new image
                return RedirectToAction("Home", "Index");
            }
            return View(album);
        }

    }
}
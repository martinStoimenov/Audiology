using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audiology.Web.ViewModels.Songs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Audiology.Web.Controllers
{
    public class SongsController : Controller
    {
        // GET: Songs
        public ActionResult Index()
        {
            return this.View();
        }

        // GET: Songs/Details/5
        public ActionResult Details(int id)
        {
            return this.View();
        }

        // GET: Songs/Create
        public ActionResult Upload()
        {
            return this.View();
        }

        // POST: Songs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(SongUploadViewModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            return this.Json(input);
            //return this.Content("Successfull!!");

            /*try
            {
                // TODO: Add insert logic here

                return this.RedirectToAction(nameof(this.Index));
            }
            catch
            {
                return this.View(input);
            }*/
        }

        // GET: Songs/Edit/5
        public ActionResult Edit(int id)
        {
            return this.View();
        }

        // POST: Songs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return this.RedirectToAction(nameof(this.Index));
            }
            catch
            {
                return this.View();
            }
        }

        // GET: Songs/Delete/5
        public ActionResult Delete(int id)
        {
            return this.View();
        }

        // POST: Songs/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
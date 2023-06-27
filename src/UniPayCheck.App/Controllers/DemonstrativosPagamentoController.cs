using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UniPayCheck.App.Controllers
{
    public class DemonstrativosPagamentoController : Controller
    {
        // GET: DemonstrativosPagamentoController
        public ActionResult Index()
        {
            return View();
        }

        // GET: DemonstrativosPagamentoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DemonstrativosPagamentoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DemonstrativosPagamentoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DemonstrativosPagamentoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DemonstrativosPagamentoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DemonstrativosPagamentoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DemonstrativosPagamentoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

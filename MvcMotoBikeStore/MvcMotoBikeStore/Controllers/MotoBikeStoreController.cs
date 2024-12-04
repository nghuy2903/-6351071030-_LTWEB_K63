using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using MvcMotoBikeStore.Models;
using PagedList;

namespace MvcMotoBikeStore.Controllers
{
    public class MotoBikeStoreController : Controller
    {
        private readonly QLBanXeGanMayEntities db;

        public MotoBikeStoreController()
        {
            db = new QLBanXeGanMayEntities();
        }

        // GET: MotoBikeStore
        private List<XEGANMAY> Layxemoi(int skip, int count)
        {
            return db.XEGANMAYs.OrderByDescending(a => a.Ngaycapnhat).Skip(skip).Take(count).ToList();
        }

        public ActionResult Index(int? page)
        {
            int pageSize = 3; 
            int pageNumber = page ?? 1;
            int skip = (pageNumber - 1) * pageSize;

            var xeMoi = Layxemoi(skip, pageSize);
            var pagedXeMoi = new StaticPagedList<XEGANMAY>(xeMoi, pageNumber, pageSize, db.XEGANMAYs.Count());

            return View(pagedXeMoi);
        }

        public ActionResult Mota() 
        {
            

            var mota = db.HANGSANXUATs.ToList();

            //var pagedMota = mota.ToPagedList(pageNumber, pageSize);

            return PartialView(mota);
        }

        public ActionResult Loaixe() 
        {
            var loaixe = from lx in db.LOAIXEs select lx;
            return PartialView(loaixe);
        }

        public ActionResult SPTheoChuDe(int id, int? page)
        {
            ViewBag.CurrentCategoryId = id;
            int pageSize = 3;
            int pageNumber = page ?? 1;

            var products = from xe in db.XEGANMAYs
                           join sx in db.SANXUATXEs on xe.MaXe equals sx.MaXe
                           join hsx in db.HANGSANXUATs on sx.MaHSX equals hsx.MaHSX
                           where hsx.MaHSX == id
                           select new SPTheoHangVM
                           {
                               HangSanXuat = hsx,
                               XeGanMay = xe,
                               SanXuatXe = sx
                           };

            var sortedProducts = products.OrderBy(p => p.XeGanMay.TenXe);  // Example sorting by product name

            // Paginate the result
            var pagedProducts = sortedProducts.ToPagedList(pageNumber, pageSize);

            return View(pagedProducts);
        }

        public ActionResult SPTheoLoaiXe(int id, int? page)
        {
            ViewBag.CurrentCategoryId = id;
            int pageSize = 3;
            int pageNumber = page ?? 1;

            var products = from xe in db.XEGANMAYs
                           join lx in db.LOAIXEs on xe.MaLX equals lx.MaLX
                           where lx.MaLX == id
                           select new SPTheoLoaiXeVM
                           {
                               XeGanMay = xe,
                               LoaiXe = lx
                           };

            var sortedProducts = products.OrderBy(p => p.XeGanMay.TenXe);  

            
            var pagedProducts = sortedProducts.ToPagedList(pageNumber, pageSize);

            return View(pagedProducts);
        }

        public ActionResult Details(int id)
        {
            var xe = from s in db.XEGANMAYs
                     where s.MaXe == id
                     select s;

            return View(xe.Single());
        }
    }
}
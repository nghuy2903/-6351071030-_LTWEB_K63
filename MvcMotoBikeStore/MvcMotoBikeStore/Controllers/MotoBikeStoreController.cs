using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcMotoBikeStore.Models;

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
        private List<XEGANMAY> Layxemoi(int count)
        {
            return db.XEGANMAYs.OrderByDescending(a => a.Ngaycapnhat).Take(count).ToList();
        }

        public ActionResult Index()
        {
            var xemoi = Layxemoi(5);
            return View(xemoi);
        }

        public ActionResult Mota() 
        {
            var mota = db.HANGSANXUATs.ToList();

            return PartialView(mota);
        }

        public ActionResult Loaixe() 
        {
            var loaixe = from lx in db.LOAIXEs select lx;
            return PartialView(loaixe);
        }

        public ActionResult SPTheoChuDe(int id)
        {
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

            return View(products.ToList());
        }

        public ActionResult SPTheoLoaiXe(int id)
        {
            var products = from xe in db.XEGANMAYs
                           join lx in db.LOAIXEs on xe.MaLX equals lx.MaLX
                           where lx.MaLX == id
                           select new SPTheoLoaiXeVM
                           {
                               XeGanMay = xe,
                               LoaiXe = lx
                           };

            return View(products.ToList());
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
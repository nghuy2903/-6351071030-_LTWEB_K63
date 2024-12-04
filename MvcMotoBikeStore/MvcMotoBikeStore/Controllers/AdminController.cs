using Microsoft.Ajax.Utilities;
using MvcMotoBikeStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Data.Entity;
using System.Data;

namespace MvcMotoBikeStore.Controllers
{
    public class AdminController : Controller
    {

        QLBanXeGanMayEntities db = new QLBanXeGanMayEntities();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Xe(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 5;
            var xe = db.XEGANMAYs.ToList().OrderBy(n => n.MaXe).ToPagedList(pageNumber, pageSize);
            return View(xe);
        }

        [HttpGet]
        public ActionResult CreateMoto()
        {
            ViewBag.MaLX = new SelectList(db.LOAIXEs.OrderBy(n => n.TenLoaiXe), "MaLX", "TenLoaiXe");
            ViewBag.MaNPP = new SelectList(db.NHAPHANPHOIs.OrderBy(n => n.TenNPP), "MaNPP", "TenNPP");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateMoto(XEGANMAY xe, HttpPostedFileBase fileUpload)
        {
            if (fileUpload == null || fileUpload.ContentLength == 0)
            {
                ViewBag.Thongbao = "Vui lòng chọn hình ảnh.";
                ViewBag.MaLX = new SelectList(db.LOAIXEs.OrderBy(n => n.TenLoaiXe), "MaLX", "TenLoaiXe");
                ViewBag.MaNPP = new SelectList(db.NHAPHANPHOIs.OrderBy(n => n.TenNPP), "MaNPP", "TenNPP");
                return View();
            }

            var fileName = Path.GetFileName(fileUpload.FileName);
            var path = Path.Combine(Server.MapPath("~/images"), fileName);

            if (System.IO.File.Exists(path))
            {
                ViewBag.Thongbao = "Hình ảnh đã tồn tại.";
            }
            else
            {
                xe.Ngaycapnhat = xe.Ngaycapnhat ?? DateTime.Now;
                fileUpload.SaveAs(path);
                xe.Anhbia = fileName; // Lưu tên file vào thuộc tính Anhbia của XEGANMAY
                db.XEGANMAYs.Add(xe);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaLX = new SelectList(db.LOAIXEs.OrderBy(n => n.TenLoaiXe), "MaLX", "TenLoaiXe");
            ViewBag.MaNPP = new SelectList(db.NHAPHANPHOIs.OrderBy(n => n.TenNPP), "MaNPP", "TenNPP");
            return View("Xe");
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {

            // Gán các giá trị người dùng nhập liệu cho các biến
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {

                //Gần giá trị cho đối tượng được tạo mới (ad)
                ADMIN ad = db.ADMINs.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau); if (ad != null)
                {
                    // ViewBag.Thongbao = "Chúc mừng đăng nhập thành công”; Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }

        public ActionResult Detail(int id)
        {
            XEGANMAY xe = db.XEGANMAYs.SingleOrDefault(n => n.MaXe == id);
            ViewBag.MaXe = xe.MaXe;
            if (xe == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(xe);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            XEGANMAY xe = db.XEGANMAYs.SingleOrDefault(n => n.MaXe == id);
            ViewBag.MaXe = xe.MaXe;
            if (xe == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(xe);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {

            XEGANMAY xe = db.XEGANMAYs.SingleOrDefault(n => n.MaXe == id);


            if (xe == null)
            {
                return HttpNotFound();
            }


            db.XEGANMAYs.Remove(xe);
            db.SaveChanges();


            return RedirectToAction("Xe");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            XEGANMAY xe = db.XEGANMAYs.SingleOrDefault(n => n.MaXe == id);
            
            if (xe == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaLX = new SelectList(db.LOAIXEs.OrderBy(n => n.TenLoaiXe), "MaLX", "TenLoaiXe", xe.MaLX);
            ViewBag.MaNPP = new SelectList(db.NHAPHANPHOIs.OrderBy(n => n.TenNPP), "MaNPP", "TenNPP", xe.MaNPP);
            return View(xe);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(XEGANMAY xe, HttpPostedFileBase fileUpload)
        {
            //Dua du lieu vao dropdownload
            ViewBag.MaLX = new SelectList(db.LOAIXEs.OrderBy(n => n.TenLoaiXe), "MaLX", "TenLoaiXe");
            ViewBag.MaNPP = new SelectList(db.NHAPHANPHOIs.OrderBy(n => n.TenNPP), "MaNPP", "TenNPP");
            //Kiem tra duong dan file
            if (fileUpload == null)

            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View(xe);
            }
            //Them vao CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //Luu ten fie, luu y bo sung thu vien using System.IO;
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //Luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/images"), fileName); //Kiem tra hình anh ton tai chua?
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //Lưu hình anh vao duong dan
                        fileUpload.SaveAs(path);
                    }

                    xe.Anhbia = fileName;
                    //Lưu vào CSDL
                    db.Entry(xe).State = EntityState.Modified;
                    db.SaveChanges();

                }

                return RedirectToAction("Xe");
            }
        }

        public  ActionResult Statistics()
        {
            var data = db.XEGANMAYs
                 .GroupBy(x => x.MaLX)
                 .Select(g => new
                 {
                     Category = g.FirstOrDefault().LOAIXE.TenLoaiXe,
                     Count = g.Count()
                 })
                 .ToList();

            
            ViewBag.Categories = data.Select(d => d.Category).ToList();
            ViewBag.Counts = data.Select(d => d.Count).ToList();

            return View();
        }
    }

    
}
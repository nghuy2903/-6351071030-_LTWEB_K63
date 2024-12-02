using MvcMotoBikeStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcMotoBikeStore.Controllers
{
    public class GioHangController : Controller
    {
        QLBanXeGanMayEntities data = new QLBanXeGanMayEntities();

        public List<Giohang> Laygiohang()
        {
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if(lstGiohang == null)
            {
                lstGiohang= new List<Giohang>();
                Session["GioHang"] = lstGiohang;
            }

            return lstGiohang;
        }

        public ActionResult ThemGioHang(int iMaXe, string strURL) 
        { 
            List<Giohang> lstGioHang = Laygiohang();
            Giohang sanpham = lstGioHang.Find(n => n.iMaXe == iMaXe);
            if (sanpham == null) 
            {
                sanpham = new Giohang(iMaXe);
                lstGioHang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }

        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> lstGioHang = Session["GioHang"] as List<Giohang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoluong);
            }

            return iTongSoLuong;
        }

        private double TongTien()
        {
            double iTongTien = 0;
            List<Giohang> lstGioHang = Session["GioHang"] as List<Giohang>;
            if (lstGioHang != null)
            {
                iTongTien = lstGioHang.Sum(n => n.dThanhtien);
            }

            return iTongTien;
        } 

        public ActionResult GioHang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            if(lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "MotoBikeStore");
            }

            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();  
            return View(lstGiohang);
        }

        public ActionResult XoaGioHang(int iMaSp)
        {
            List<Giohang> lstGioHang = Laygiohang();

            Giohang sanpham = lstGioHang.SingleOrDefault(n => n.iMaXe == iMaSp);
            if(sanpham != null)
            {
                lstGioHang.RemoveAll(n => n.iMaXe == iMaSp);
                return RedirectToAction("GioHang");
            }

            if(lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "MotoBikeStore");
            }

            return RedirectToAction("GioHang");
        }

        public ActionResult CapNhapGioHang(int iMaSp, FormCollection f)
        {
            List<Giohang> lstGioHang = Laygiohang();

            Giohang sanpham = lstGioHang.SingleOrDefault(n => n.iMaXe == iMaSp);

            if(sanpham != null)
            {
                sanpham.iSoluong = int.Parse(f["txtSoLuong"].ToString());
            }

            return RedirectToAction("GioHang");
        }

        public ActionResult XoaTatCaSanPham()
        {
            List<Giohang> lstGioHang = Laygiohang();
            lstGioHang.Clear();
            return RedirectToAction("Index", "MotoBikeStore");
        }

        [HttpGet]
        public ActionResult DatHang()
        {

            //Kiem tra dang nhap
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "Nguoidung");
            }
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "MotoBikeStore");
            }
            //Lay gio hang tu Session

            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);

        }

        public ActionResult DatHang(FormCollection collection)
        {

            //Them Don hang
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            List<Giohang> gh = Laygiohang();
            ddh.MaKH =  kh.MaKH;
            ddh.Ngaydat = DateTime.Now;
            string ngaygiao = collection["Ngaygiao"];

            // Kiểm tra chuỗi và chuyển đổi
            if (!string.IsNullOrEmpty(ngaygiao) && DateTime.TryParse(ngaygiao, out DateTime parsedNgaygiao))
            {
                ddh.Ngaygiao = parsedNgaygiao;
            }
            ddh.Tinhtranggiaohang = false;
            ddh.Dathanhtoan = false;
            data.DONDATHANGs.Add(ddh);
            data.SaveChanges();
            //Them chi tiet don hang
            foreach (var item in gh)
            {
                CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.MaXe = item.iMaXe;
                ctdh.Soluong =item.iSoluong;
                ctdh.Dongia =(decimal)item.dDongia;
                data.CHITIETDONTHANGs.Add(ctdh);
            }
            data.SaveChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang");

        }

        public ActionResult XacNhanDonHang()
        {
            return View();
        }
        // GET: GioHang
        public ActionResult Index()
        {
            return View();
        }
    }
}
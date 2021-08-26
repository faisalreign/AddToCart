using addtocart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace addtocart.Controllers
{
    public class HomeController : Controller
    {
        
        db_ecommerceEntities2 db = new db_ecommerceEntities2();

        public ActionResult Index()
        {
            Session["u_id"] = 1;
            if (TempData["cart"] != null)
            {
                float x = 0;
                List<cart> li2 = TempData["cart"] as List<cart>;
                foreach (var item in li2)
                {
                    x += item.bill;

                }

                TempData["total"] = x;
            }
            TempData.Keep();

            return View(db.tbl_product.OrderByDescending(x=>x.pro_id).ToList());
        }

        public ActionResult Adtocart(int? Id)
        {

            tbl_product p = db.tbl_product.Where(x => x.pro_id == Id).SingleOrDefault();
            return View(p);
        }

        List<cart> li = new List<cart>();
        [HttpPost]
        public ActionResult Adtocart(tbl_product pi, string qty, int Id)
        {
            tbl_product p = db.tbl_product.Where(x => x.pro_id == Id).SingleOrDefault();

            cart c = new cart();
            c.productid = p.pro_id;
            c.price = (float)p.pro_price;
            c.qty = Convert.ToInt32(qty);
            c.bill = c.price * c.qty;
            c.productname = p.pro_name;
            if (TempData["cart"] == null)
            {
                li.Add(c);
                TempData["cart"] = li;

            }
            else
            {
                List<cart> li2 = TempData["cart"] as List<cart>;
                int flag = 0;
                foreach(var item in li2)
                {
                    if(item.productid==c.productid)
                    {
                        item.qty += c.qty;
                        item.bill += c.bill;
                        flag = 1;
                    }
                }
                if(flag==0)
                {
                    li2.Add(c);
                    //item add new................
                }
                //li2.Add(c);
                TempData["cart"] = li2;
            }

            TempData.Keep();




            return RedirectToAction("Index");
        }

        public ActionResult remove(int? Id)
        {
            List<cart> li2 = TempData["cart"] as List<cart>;
            cart c = li2.Where(x => x.productid == Id).SingleOrDefault();
            li2.Remove(c);
            float h = 0;
            foreach(var item in li2)
            {
                h += item.bill;
            }
            TempData["total"] = h;
            return RedirectToAction("checkout");
        }

        public ActionResult checkout()
        {
            TempData.Keep();


            return View();
        }

        [HttpPost]
        public ActionResult checkout(tbl_order o)
        {
            List<cart> li = TempData["cart"] as List<cart>;
            tbl_invoice iv = new tbl_invoice();
            iv.in_fk_user= Convert.ToInt32(Session["u_id"].ToString());
            iv.in_date = System.DateTime.Now;
            
            db.tbl_invoice.Add(iv);
            db.SaveChanges();

            foreach(var item in li)
            {
                tbl_order od = new tbl_order();
                od.o_fk_pro = item.productid;
                od.o_fk_invoice = iv.in_id;
                od.o_date = System.DateTime.Now;
                od.o_qty = item.qty;
                od.o_unitprice = (int)item.price;
                od.o_bill = item.bill;
                db.tbl_order.Add(od);
                db.SaveChanges();
            }
            TempData.Remove("total");
            TempData.Remove("cart");

            TempData["msg"]="Transaction Completed.......";
            TempData.Keep();

            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
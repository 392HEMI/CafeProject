using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data.Entity;
using System.Data.Sql;

using CafeProject.MobileDataLevel.Contexts;
using CafeProject.MobileWebApplication;
using CafeProject.MobileWebApplication.Models;
using CafeProject.MobileDataLevel.Entities;

using System.Net.Mail;
using System.Threading.Tasks;

namespace CafeProject.MobileWebApplication.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Nearest()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Menu(int? id)
        {
            var context = DatabaseContext.Create();
            int? i = context.ObjectMenuItems.Where(a => a.ObjectID == id).Select(a => a.FoodTypeID).FirstOrDefault();
            var obj = ShowMenuList("Show", id, i);
            return View("~/Views/Home/Menu.cshtml", obj);
        }
        
        [HttpPost]
        public ActionResult Menu(string command, int? objID, int? ftID)
        {
            var obj = ShowMenuList(command, objID, ftID);
            return View("~/Views/Home/Menu.cshtml", obj);
        }

        [HttpGet]
        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignIn(SignIn model)
        {
            var context = DatabaseContext.Create();
            if (ModelState.IsValid)
            {
                var user = context.Users.Find(model.Login, model.Password);
                if (user != null)
                {
                    if (user.ConfirmEmail == true)
                    {
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email не подтвержден.");
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Неверный логин или пароль");
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
                return HttpNotFound();

            var context = DatabaseContext.Create();

            var model = context
                .Objects
                .Where(obj => obj.ID == id.Value && obj.IsWork && !obj.Deleted)
                .Select(obj => new DetailsModel()
                {
                    ObjectID = obj.ID,
                    ObjectCaption = obj.Caption,
                    ObjectType = obj.Type.Title,
                    ObjectIcon = obj.Icon,
                    ObjectAddresses = obj
                                        .Addresses
                                        .Select(address => "ул. " + address.Street.Title + " дом " + address.HouseNumber)
                                        .ToList(),
                    ObjectPhoneNumber = obj.PhoneNumber,
                    ObjectOptions = obj
                                        .Options
                                        .Select(option => new CafeProject.MobileWebApplication.Models.ObjectOption()
                                        {
                                            ID = option.ID,
                                            Title = option.Option,
                                            Icon = option.Icon
                                        }).ToList()
                }).SingleOrDefault();

            if (model == null)
                return HttpNotFound();

            var stat = context
                .Objects
                .Where(obj => obj.ID == id.Value && obj.IsWork && !obj.Deleted)
                .Select(obj => new 
                {
                    LikeCooking = obj.Statistics.Average(s => s.LikeCooking),
                    LikeInterior = obj.Statistics.Average(s => s.LikeInterior),
                    LikeService = obj.Statistics.Average(s => s.LikeService),
                    LikePrice = obj.Statistics.Average(s => s.LikePrice)
                }).SingleOrDefault();

            model.Statistics = new double[4] { stat.LikeCooking, stat.LikeInterior, stat.LikeService, stat.LikePrice };

            return View("~/Views/Home/Details.cshtml", model);
        }

        [HttpGet]
        public ActionResult GetNearest(float? latitude, float? longitude)
        {
            if (!latitude.HasValue || !longitude.HasValue)
                return this.HttpBadRequest();

            var context = DatabaseContext.Create();
            var model = new NearestModel();

            model.Objects = context.Objects
                .Where(obj => obj.IsWork && !obj.Deleted)
                .Select(obj => new CafeProject.MobileWebApplication.Models.GeneralObject()
                {
                    ID = obj.ID,
                    Caption = obj.Caption,
                    Type = obj.Type.Title,
                    Icon = obj.Icon
                }).ToArray();

            if (Request.IsAjaxRequest())
                return PartialView("~/Views/Home/_NearestList.cshtml", model);

            return View("~/Views/Home/NearestList.cshtml", model);
        }

        //Функция для Меню
        public Object ShowMenuList(string command, int? objID, int? ftID)
        {
            if (!objID.HasValue)
                return HttpNotFound();
            var context = DatabaseContext.Create();
            var obj = context.Objects.
                Where(b => b.ID == objID).
                Select(m => new MenuModel
                {
                    ObjectID = m.ID,
                    ObjectCaption = m.Caption,
                    ObjectType = m.Type.Title,
                    ObjectIcon = m.Icon,
                    ObjectAddress = m.Addresses
                                        .Select(a => "ул." + a.Street.Title + " дом " + a.HouseNumber)
                                        .ToList(),
                    ObjectPhoneNumber = m.PhoneNumber,

                    ObjectFoodTypes = m.MenuItems
                    .Select(i => new CafeProject.MobileWebApplication.Models.FoodType() { ID = i.FoodType.ID, Type = i.FoodType.Title })
                    .Distinct()
                    .ToList(),

                    ObjectFoods = m.MenuItems.Where(f => f.FoodTypeID == ftID).Select(f => new Food()
                    {
                        ID = f.ID,
                        Name = f.FoodName,
                        Photo = f.FoodIcon,
                        Consist = f.FoodConsist,
                        Price = f.FoodPrice
                    }).ToList()
                }).SingleOrDefault();

            if (obj == null)
                return HttpNotFound();
            return obj;
        }

        [AjaxOnly]
        [HttpGet]
        public ActionResult GetStatistics(int? id)
        {
            if (!id.HasValue)
                return HttpNotFound();
            return null;
        }
    }
}

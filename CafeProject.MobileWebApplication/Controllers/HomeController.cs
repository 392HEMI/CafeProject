﻿using System;
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
        private void setStatistics(ObjectStatistic statistics, int? key, byte? value)
        {
            switch (key)
            {
                case 1:
                    {
                        statistics.LikeCooking = value.Value;
                        break;
                    }
                case 2:
                    {
                        statistics.LikeInterior = value.Value;
                        break;
                    }
                case 3:
                    {
                        statistics.LikeService = value.Value;
                        break;
                    }
                case 4:
                    {
                        statistics.LikePrice = value.Value;
                        break;
                    }
            }
        }

        private ActionResult HttpBadRequest(string message)
        {
            Response.StatusCode = 400;
            return Content(message);
        }

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

        // -------------------------------------- Statistics ----------------------------------------
        [AjaxOnly]
        [HttpGet]
        public ActionResult GetStatistics(int? id)
        {
            if (!id.HasValue)
                return HttpNotFound();
            DatabaseContext context = DatabaseContext.Create();

            StatisticsModel model;
            if (!context.ObjectStatistics.Any(s => s.ObjectID == id))
            {
                model = new StatisticsModel()
                {
                    Cooking = 0,
                    Interior = 0,
                    Service = 0,
                    Price = 0
                };
            }
            else
                model = context
                    .Objects
                    .Where(obj => obj.ID == id.Value && obj.IsWork && !obj.Deleted)
                    .Select(obj => new StatisticsModel()
                    {
                        Cooking = obj.Statistics.Average(s => (double)s.LikeCooking),
                        Interior = obj.Statistics.Average(s => (double)s.LikeInterior),
                        Service = obj.Statistics.Average(s => (double)s.LikeService),
                        Price = obj.Statistics.Average(s => (double)s.LikePrice)
                    }).SingleOrDefault();
            if (model == null)
                return HttpNotFound();
            return PartialView("~/Views/Home/_Statistics.cshtml", model);
        }

        [AjaxOnly]
        [HttpGet]
        //[Authorize(Roles="")]
        public ActionResult GetStatisticsEditor(int? id)
        {
            if (!id.HasValue)
                return HttpNotFound();
            DatabaseContext context = DatabaseContext.Create();
            string username = "shamin_alexander";//User.Identity.Name;
            StatisticsEditorModel model = context.ObjectStatistics
                .Where(s => s.User.Login == username)
                .Where(s => s.ObjectID == id)
                .Select(s => new StatisticsEditorModel()
                {
                    Cooking = s.LikeCooking ?? 0,
                    Interior = s.LikeInterior ?? 0,
                    Service = s.LikeService ?? 0,
                    Price = s.LikePrice ?? 0
                }).SingleOrDefault();
            if (model == null)
                model = new StatisticsEditorModel();
            return PartialView("~/Views/Home/_StatisticsEditor.cshtml", model);
        }

        [AjaxOnly]
        [HttpPost]
        // [Authorize]
        public ActionResult SetStatistics(int? id, byte? key, byte? value)
        {
            if (!key.HasValue || !key.HasValue ||
                (key < 1 || key > 4) ||
                (value < 1) || (value > 10))
                return HttpBadRequest("invalid data");

            string username = "shamin_alexander";
            DatabaseContext context = DatabaseContext.Create();
            var statistics = context
                .ObjectStatistics
                .Where(s => s.ObjectID == id)
                .SingleOrDefault(s => s.User.Login == username);


            // Этот код нужно тестировать
            if (statistics == null)
            {
                var user = context
                    .Users
                    .Where(u => u.Login == username)
                    .Select(u => new
                    {
                        ID = u.ID
                    }).SingleOrDefault();

                if (user == null)
                {
                    // SignOut();
                    return Json(new { status = "user not found" });
                }

                if (!context.Objects.Any(o => o.ID == id))
                    return Json(new { status = "object not found" });

                statistics = new ObjectStatistic()
                {
                    LikeCooking = null,
                    LikeInterior = null,
                    LikeService = null,
                    LikePrice = null,
                    UserID = user.ID,
                    ObjectID = id.Value
                };
                context.ObjectStatistics.Add(statistics);
            }
            setStatistics(statistics, key, value);
            context.SaveChanges();
            return Json(new { status = "success" });
        }
    }
}
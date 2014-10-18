using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data.Sql;
using System.Data.Entity;
using System.Security.Cryptography;
using CafeProject.MobileDataLevel.Entities;
using CafeProject.MobileDataLevel.Contexts;
using CafeProject.MobileWebApplication.Models;


using System.Net.Mail;
using System.Threading.Tasks;


namespace CafeProject.MobileWebApplication.Controllers
{
    public class RegistrationController : Controller
    {
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(CafeProject.MobileWebApplication.Models.User user)
        {
            if (ModelState.IsValid)
            {
                byte b = 0;
                var context = DatabaseContext.Create();
                switch (user.Gender)
                {
                    case "Муж.":
                        b = 0;
                        break;
                    case "Жен.":
                        b = 1;
                        break;
                    default:
                        break;
                }
                byte[] ps = Sha(user.Password);
                //Сохранение полученных данных
                CafeProject.MobileDataLevel.Entities.User us = new CafeProject.MobileDataLevel.Entities.User()
                {
                    ID = Guid.NewGuid(),
                    FirstName = user.Name,
                    MiddleName = user.Surname,
                    Login = user.Login,
                    Gender = b,
                    Email = user.Email,
                    //Подтверждение майла
                    ConfirmEmail = false
                };
                us.Password = new Password() { PasswordHash = ps };
                context.Users.Add(us);
                context.SaveChangesAsync();
                //От кого письмо с заголовком
                MailAddress from = new MailAddress("timson7@yandex.ru", "Регистрация");
                //Кому будет послано
                MailAddress to = new MailAddress(user.Email);
                //Объект сообщения
                MailMessage mess = new MailMessage(from, to);
                mess.Subject = "Подтверждение регистрации";
                mess.Body = string.Format("Для завершения регистрации перейдите по ссылке:" +
                        "<a href=\"{0}\" title=\"Подтвердить регистрацию\">{0}</a>",
                        Request.Url.Host + ":" + Request.Url.Port + ("/ConfirmEmail/" + us.ID + "/Email = " + us.Email));
                //Подтвержение что тело сообщения содержит информацию
                mess.IsBodyHtml = true;
                //адрес smtp-сервера, с которого будет посылаться ссообщение
                SmtpClient smpt = new System.Net.Mail.SmtpClient("smtp.yandex.ru", 25);
                //логин и пароль
                smpt.Credentials = new System.Net.NetworkCredential("timson7@yandex.ru", "2240800");
                //шифрование подключения. у разных сервером иногда бывают разные требования по шифрования
                smpt.EnableSsl = true;
                //Отправка письма
                smpt.Send(mess);
                return RedirectToAction("Confirm", new { Email = user.Email });
            }
            else
                return View(user);
        }

        //Хэш пароля
        public byte[] Sha(string password)
        {
            HashAlgorithm sha = new SHA512Managed();
            byte[] pass = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] result = sha.ComputeHash(pass);
            sha.Dispose();
            return result;
        }

        //Для эл.адреса
        public ActionResult Confirm(string Email)
        {
            ViewBag.Message = "На ваш почтовый адрес: " + Email + " Вам высланы дальнейщие инструкции по завершению регистрации.";
            return View();
        }

        //Подтверждение эл.адреса
        public ActionResult ConfirmEmail(string ID, string Email)
        {
            CafeProject.MobileDataLevel.Entities.User us = new CafeProject.MobileDataLevel.Entities.User();
            var context = DatabaseContext.Create();
            var u = context.Users.Where(i => i.ID == new Guid(ID)).SingleOrDefault();
            if (u != null)
            {
                var e = context.Users.Where(i => i.Email == Email).SingleOrDefault();
                if (e != null)
                {
                    us.ConfirmEmail = true;
                    context.SaveChanges();
                }
                else
                {
                    ViewBag.Message = "Регистрация не пройдена в связи не верного Email адреса";
                }
            }
            else
            {
                ViewBag.Message = "Пользователь не найден";
            }
            return Content("Подтвержение прошло успешно!");
        }
    }
}

﻿using System;
using System.Web.Mvc;
using BusinessTripApplication.Models;
using BusinessTripApplication.Repository;
using BusinessTripApplication.ViewModels;

namespace BusinessTripApplication.Controllers
{
    [RequireHttps]
    public class UserController : Controller
    {

        IUserService UserService;

        public UserController(IUserService repo)
        {
            UserService = repo;
        }

        public UserController()
        {
            UserService = new UserService(new UserRepository());
        }

        // GET: User
        [HttpGet]
        public ActionResult Registration()
        {
            RegistrationViewModel model = new RegistrationViewModel();
            return View(model);
        }

        [HttpGet]
        public ActionResult LogIn()
        {
            LogInViewModel model = new LogInViewModel();
            return View(model);
        }
        public ActionResult Dashboard(int id = 0)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn([Bind(Exclude = "Name,IsEmailVerified,ActivationCode")] User user)
        {
            return View(user.Email);
            /*
            try
            {
                var model = new LogInViewModel(ModelState.IsValid, user, UserService);
                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToRoute("~/Shared/Error");
            }
            */

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified,ActivationCode")] User user)
        {
            try
            {
                var model = new RegistrationViewModel(ModelState.IsValid, user,UserService);
                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToRoute("~/Shared/Error");
            }
            
        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool result = UserService.VerifyAccount(id);
            ViewBag.Status = result;

            if(!result)
                ViewBag.Message = "Invalid Request";
            
            return View();
        }


    }
}
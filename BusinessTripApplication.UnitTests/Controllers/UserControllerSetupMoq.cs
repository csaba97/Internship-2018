﻿using System;
using System.Net;
using BusinessTripApplication.Models;
using BusinessTripApplication.Repository;
using BusinessTripApplication.ViewModels;
using Moq;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Security;

namespace BusinessTripApplication.UnitTests.Controllers
{
    public static class UserControllerSetupMoq
    {
        public static void SendVerificationLinkEmail(Mock<IRegistrationViewModel> MockRegistrationViewModel, MailMessage message)
        {
            MockRegistrationViewModel.Setup(mock => mock.SendVerificationLinkEmail(It.IsAny<string>(), It.IsAny<string>())).Callback(
               (string email, string activation) =>
               {
                   message = new MailMessage(new MailAddress("businesstripapplication@gmail.com", "Registration"), new MailAddress(email))
                   {
                       Subject = "Activation link",
                       Body = activation,
                       IsBodyHtml = true
                   };
               });
        }

        public static void CheckUser(Mock<IRegistrationViewModel> MockRegistrationViewModel)
        {
            MockRegistrationViewModel.Setup(mock => mock.CheckUser(It.IsAny<IUserService>(), It.IsAny<User>())).Returns(
                (IUserService userService, User user) =>
                {
                    bool emailExists = userService.EmailExists(user.Email);

                    if (emailExists)
                        return false;

                    User addedUser = userService.Add(user);
                    addedUser.Password = "";

                    MockRegistrationViewModel.Object.SendVerificationLinkEmail(user.Email, user.ActivationCode.ToString());

                    return true;
                });
        }

        public static void CheckUser(Mock<ILogInViewModel> MockLoginViewModel)
        {
                MockLoginViewModel.Setup(mock => mock.CheckUser(It.IsAny<IUserService>(), It.IsAny<User>())).Returns(
                (IUserService userService, User user) =>
                {
                    bool emailExists = userService.EmailExists(user.Email);

                    if (emailExists==false)
                        return false;

                    User addedUser = userService.GetUserByEmail(user.Email);
                    Console.WriteLine("0");
                    if ((addedUser.Password == user.Password)==false)
                        return false;
                    Console.WriteLine("1");
                   
                    bool rememberMe = false;
                    //MockLoginViewModel.Object.
                    MockLoginViewModel.Object.SetCookie(user.Email, rememberMe);

                    return true;
                });
        }

        public static void SetCookie(Mock<ILogInViewModel> MockLoginViewModel, HttpCookie cookie)
        {
            MockLoginViewModel.Setup(mock => mock.SetCookie(It.IsAny<string>(), It.IsAny<bool>())).Callback(
                (string email, bool rememberMe) =>
                {
                    int timeout = rememberMe ? 525600 : 20; // 525600 min = 1 year
                    var ticket = new FormsAuthenticationTicket(email, rememberMe, timeout);
                    string encrypted = FormsAuthentication.Encrypt(ticket);
                    cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                    cookie.Expires = DateTime.Now.AddMinutes(timeout);
                    cookie.HttpOnly = true;
                });
        }

    }
}
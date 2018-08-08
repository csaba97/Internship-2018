﻿using BusinessTripApplication.Models;
using System;
using System.Collections.Generic;
using BusinessTripModels.Models;

namespace BusinessTripApplication.Repository
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public User Add(User addedUser)
        {
            return userRepository.Add(addedUser);
        }

        public bool EmailExists(string email)
        {
            return userRepository.FindByEmail(email) != null;
        }

        public IList<User> FindAll()
        {
            return userRepository.FindAll();
        }

        public bool IsEmailVerified(string email)
        {
            return userRepository.FindByEmail(email).IsEmailVerified == true;
        }

        public User FindByEmail(string email)
        {
            return userRepository.FindByEmail(email);
        }

        public bool VerifyAccount(string id)
        {
            User user = userRepository.FindByActivationCode(new Guid(id));
            if (user.ActivationCodeExpireDate < DateTime.Now)
                return false;
            if (user != null && !user.IsEmailVerified)
            {
                user.IsEmailVerified = true;
                userRepository.UpdateIsEmailVerified(user);
                return true;
            }
            return false;
        }

        public User FindByActivationCode(Guid activationCode)
        {
            return userRepository.FindByActivationCode(activationCode);
        }
    }
}

using System.Collections.Generic;
using Server;
using EFDataManager;
using LogicForUI.Interfaces;
using Refit;
using User = Entities.User;

namespace LogicForUI
{
    public class UIUserSupplier
    {
        private readonly IUserWebInterface _userController;

        public UIUserSupplier()
        {
            var connection =
                System.Configuration.ConfigurationManager.
                    ConnectionStrings["ServerConnection"].ConnectionString;
            _userController = RestService.For<IUserWebInterface>(connection);
        }

        public User GetUserByName(string userName)
        {
            return _userController.GetUserByName(userName).Result;
        }

        public void CreateUserIfNotExists(string userName)
        {
            _userController.CreateUser(userName);
        }

        public IReadOnlyList<string> GetAllUsers()
        {
            return _userController.GetAllUsers().Result;
        }
    }
}
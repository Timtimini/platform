using System;
using System.Collections.Generic;
using Otito.Services.HelperModel.User;
using Otito.Services.Model;

namespace Otito.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        User AuthenticateSocial(string email, string firstName, string lastName);
        int SaveUser(string username, string password);
        Profile GetProfile(int userId);
        int ChangePassword(string previous, string newPassword, int userId);
        UserStatistics GetStatistics(DateTime from, DateTime to);
        List<string> UserList();
        bool IfEmailExist(string email);
    }
}
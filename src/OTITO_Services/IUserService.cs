using System;
using System.Collections.Generic;
using OTITO_Services.HelperModel.User;
using OTITO_Services.Model;

namespace OTITO_Services
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
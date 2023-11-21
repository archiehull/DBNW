using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using whois;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using MySqlConnector;



namespace whois
{
    // Class is used as a singleton so that all all database info can be stored and interacted with in a Dictionary
    public class User
    {
        // Create public dictionary to store LoginID's and their corresponding UserInfo
        public static Dictionary<String, User> Users = new Dictionary<string, User>();

        private static User _Instance;

        // When accessing the fields of "UserInfo", "GetInstance()" will only call a single instance of "User()"
        public static User GetInstance()
        {
            // If no user has been created, create one
            if(_Instance == null)
            {
                _Instance = new User();
            }
            return _Instance;
        }

        // Private instance of User, to be used for singleton
        private User(){}

        // Used to access the Dictionary that stores LoginID & UserInfo
        public static Dictionary<string, User> GetUsers()
        {
            return Users;
        }
        
        public String UserID { get; set; }
        public String Surname { get; set; }
        public String Fornames { get; set; }
        public String Title { get; set; } 
        public String Position { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }
        public String Location { get; set; }
        
        public User(
             String userID,
             String surname,
             String fornames,
             String title,
             String position,
             String phone,
             String email,
             String location)
        {
            UserID = userID;
            Surname = surname;
            Fornames = fornames;
            Title = title;
            Position = position;
            Phone = phone;
            Email = email;
            Location = location;

        }
    };
}


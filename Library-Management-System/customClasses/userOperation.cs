using Library_Management_System.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Library_Management_System
{
    internal static class userOperation
    {
        public enum userGettingOption
        {
            Error = -1,
            Mail = 0,
            Number = 1,
        }
        private static readonly dbLibraryEntities entities = new dbLibraryEntities();
        public static tblUser getUser(userGettingOption option, string strUsername)
        {
            List<tblUser> tblUsers = entities.tblUsers.ToList();
            if (option == userGettingOption.Mail)
            {
                return tblUsers.Find(p => p.usrMail == strUsername);
            }
            else
            {
                return tblUsers.Find(p => p.usrSchoolNumber == strUsername);
            }
        }
        public static tblUser getUser(int irID)
        {
            return new dbLibraryEntities().tblUsers.Find(irID);
        }
        public static tblUser[] getAllUsers()
        {
            return entities.tblUsers.ToArray();
        }
        public static viewUpgradedUser getUpgradedUser(int irID)
        {
            viewUpgradedUser result = (from item in getAllUsersWithWiev()
                                       where item.usrID == irID
                                       select item).First();
            return result;
        }
        public static viewUpgradedUser[] getAllUsersWithWiev()
        {
            return new dbLibraryEntities().viewUpgradedUsers.ToArray();
        }
        public static viewUpgradedUser[] userSearchOnView(string strInput)
        {
            viewUpgradedUser[] allRawUsers = entities.viewUpgradedUsers.ToArray();
            viewUpgradedUser[] result = (from item in allRawUsers
                                         where item.usrFullName.ToLower().Contains(strInput) || item.usrSchoolNumber.Contains(strInput) || item.usrMail.Contains(strInput)
                                         select item).ToArray();
            return result;
        }
        public static viewUpgradedUser[] getWaitingLecturer()
        {
            tblUser[] allRawUsers = entities.tblUsers.ToArray();
            viewUpgradedUser[] allUsers = new dbLibraryEntities().viewUpgradedUsers.ToArray();
            viewUpgradedUser[] vrResult = (from item in allUsers
                                           where (from subitem in allRawUsers where subitem.usrAuthorityLevel == "lecturer" && subitem.usrLecturerActivation == false
                                                  select subitem.usrID).Contains(item.usrID)
                                           select item).ToArray();
            return vrResult;
        }
        public static Tuple<bool, string> updateUser(tblUser tempUser)
        {
            try
            {
                if (!otherMethods.isMD5(tempUser.usrPassword))
                {
                    return new Tuple<bool, string>(false, "Your password doesnt look like MD5 format.");
                }

                tblUser found = entities.tblUsers.Find(tempUser.usrID);
                found.usrName = tempUser.usrName;
                found.usrSurname = tempUser.usrSurname;
                found.usrSchoolNumber = tempUser.usrSchoolNumber;
                found.usrMail = tempUser.usrMail;
                found.usrPassword = tempUser.usrPassword;
                found.usrAuthorityLevel = tempUser.usrAuthorityLevel;
                found.usrAccountActivation = tempUser.usrAccountActivation;
                found.usrLecturerActivation = tempUser.usrLecturerActivation;
                entities.SaveChanges();
                return new Tuple<bool, string>(true, "The user was updated successfully.");
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return new Tuple<bool, string>(false, "This E-Mail already registered.");
            }
        }
        public static Dictionary<string, int> getUserGeneral()
        {
            tblUser[] allUsers = getAllUsers();
            Dictionary<string, int> dictResult = new Dictionary<string, int>
            {
                { "student-active", entities.tblUsers.Count(p => p.usrAuthorityLevel == "student" && p.usrAccountActivation == true) },
                { "student-passive", entities.tblUsers.Count(p => p.usrAuthorityLevel == "student" && p.usrAccountActivation == false) },
                { "lecturer-active", entities.tblUsers.Count(p => p.usrAuthorityLevel == "lecturer" && p.usrAccountActivation == true) },
                { "lecturer-passive", entities.tblUsers.Count(p => p.usrAuthorityLevel == "lecturer" && p.usrAccountActivation == false) }
            };
            return dictResult;
        }
        private static XDocument getAllUsersWithXML()
        {
            tblUser[] users = getAllUsers();
            XDocument document = new XDocument();
            document.Add(new XElement("users", users.Select(p => new XElement("user_detail",
                new XAttribute("id", p.usrID),
                new XElement("name", p.usrName),
                new XElement("surname", p.usrSurname),
                new XElement("school_number", p.usrSchoolNumber),
                new XElement("mail", p.usrMail),
                new XElement("password", p.usrPassword),
                new XElement("authority_level", p.usrAuthorityLevel),
                new XElement("authority_activation", p.usrAccountActivation.ToString()),
                new XElement("lecturer_activation", p.usrLecturerActivation.ToString())
                                ))));
            return document;
        }
        public static void deleteUser(tblUser tempUser)
        {
            entities.tblUsers.Remove(tempUser);
            entities.SaveChanges();
        }
        public static void allUserstoXML(string strFileName)
        {
            getAllUsersWithXML().Save(strFileName);
        }
        public static void allUserstoJSON(string strFileName)
        {
            string strJsonResult = JsonConvert.SerializeXNode(getAllUsersWithXML(), Formatting.Indented);
            File.WriteAllText(strFileName, strJsonResult);
        }
    }
}

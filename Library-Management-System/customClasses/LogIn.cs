using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Library_Management_System.customClasses
{
    internal class LogIn : otherMethods
    {
        private string _username;
        private string _password;
        private userOperation.userGettingOption gettingOption;
        public Window wndwPrevious;
        public string strUsername
        {
            get => _username;
            set
            {
                _username = value.Trim().ToLower();
                if (otherMethods.isNumber(_username) && _username.Length == 9)
                {
                    gettingOption = userOperation.userGettingOption.Number;
                }
                else if (otherMethods.isMail(_username))
                {
                    gettingOption = userOperation.userGettingOption.Mail;
                }
                else
                {
                    gettingOption = userOperation.userGettingOption.Error;
                }
            }
        }
        public string strPassword
        {
            get => otherMethods.generateMD5(_password);
            set => _password = value.Trim();
        }
        public LogIn(string _strUsername, string _strPassword, Window _window)
        {
            strUsername = _strUsername;
            strPassword = _strPassword;
            wndwPrevious = _window;
        }
        public Tuple<bool, string> SignIn()
        {

            models.tblUser tempUser;
            if (gettingOption != userOperation.userGettingOption.Error)
            {
                tempUser = userOperation.getUser(gettingOption, strUsername);
                if (tempUser != null)
                {

                    if (tempUser.usrPassword == strPassword)
                    {
                        if (tempUser.usrAccountActivation)
                        {
                            if (tempUser.usrAuthorityLevel == "admin")
                            {
                                ManagementWindow management = new ManagementWindow(tempUser);
                                management.Show();
                                wndwPrevious.Close();
                            }
                            else
                            {
                                LibraryWindow library = new LibraryWindow(tempUser);
                                library.Show();
                                wndwPrevious.Close();
                            }
                        }
                        else
                        {
                            return new Tuple<bool, string>(false, "Your account was disabled.");
                        }
                    }
                    else
                    {
                        return new Tuple<bool, string>(false, "Your password is wrong.");
                    }
                }
                else
                {
                    return new Tuple<bool, string>(false, "You have to register the system.");
                }
            }
            else
            {
                return new Tuple<bool, string>(false, "You have to enter valid information for login.");
            }
            return new Tuple<bool, string>(true, "Succesfull");
        }        
    }
}

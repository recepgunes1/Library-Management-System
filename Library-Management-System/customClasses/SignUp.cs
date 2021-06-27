using Library_Management_System.models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Library_Management_System.customClasses
{
    internal class SignUp : otherMethods
    {
        //class definition
        private readonly dbLibraryEntities entities = new dbLibraryEntities();

        //other
        private Dictionary<string, bool> dictPasswordRequirements;
        //properties definition
        private string _name;
        private string _surname;
        private string _schoolnumber;
        private string _mail;
        private string _password;
        private string _authoirtylevel;
        private bool _acoountactivation;
        private Nullable<bool> _lectureractivation;
        public string strName { get => _name; set => _name = value.Trim().ToLower(); }
        public string strSurname { get => _surname; set => _surname = value.Trim().ToLower(); }
        public string strSchoolNumber { get => _schoolnumber; set => _schoolnumber = value; }
        public string strMail
        {
            get => _mail;
            set
            {
                if (isMail(value.Trim()))
                {
                    _mail = value.Trim();
                }
                else
                {
                    _mail = null;
                }
            }
        }
        public string strPassword
        {
            get => _password;
            set
            {
                if (isStrongPassword(value.Trim()).Values.All(p => p == true))
                {
                    _password = generateMD5(value.Trim());
                }
                else
                {
                    _password = null;
                    dictPasswordRequirements = isStrongPassword(value.Trim());
                }
            }
        }
        public string strAuthorityLevel { get => _authoirtylevel; set => _authoirtylevel = value.Trim().ToLower(); }
        public bool blAccountActivation { get => _acoountactivation; set => _acoountactivation = value; }
        public bool? blLecturerActivation { get => _lectureractivation; set => _lectureractivation = value; }
        public SignUp(string _name, string _surname, string _schoolnumber, string _mail, string _password, string _authoritylevel)
        {
            strName = _name;
            strSurname = _surname;
            strSchoolNumber = _schoolnumber;
            strMail = _mail;
            strPassword = _password;
            strAuthorityLevel = _authoritylevel;
            blAccountActivation = true;
        }
        public SignUp(string _name, string _surname, string _schoolnumber, string _mail, string _password, string _authoritylevel,
            bool _accountactivation, bool? _lectureractivation)
        {
            strName = _name;
            strSurname = _surname;
            strSchoolNumber = _schoolnumber;
            strMail = _mail;
            strPassword = _password;
            strAuthorityLevel = _authoritylevel;
            blAccountActivation = _accountactivation;
            blLecturerActivation = _lectureractivation;
        }
        public Tuple<bool, string> Register()
        {
            models.tblUser[] allUsers = entities.tblUsers.ToArray();
            models.tblUser tempUser = new models.tblUser()
            {
                usrName = strName,
                usrSurname = strSurname,
                usrSchoolNumber = strSchoolNumber,
                usrMail = strMail,
                usrPassword = strPassword,
                usrAuthorityLevel = strAuthorityLevel,
                usrAccountActivation = blAccountActivation
            };
            foreach (tblUser vrItem in allUsers)
            {
                if (vrItem.usrMail == tempUser.usrMail)
                {
                    return new Tuple<bool, string>(false, "This E-Mail has already registered system;");
                }
                else if (vrItem.usrSchoolNumber == tempUser.usrSchoolNumber)
                {
                    return new Tuple<bool, string>(false, "This School Number has already registered system;");
                }
            }
            if (tempUser.usrPassword == null)
            {
                string strMessage = $"Missing requirements for your password:{Environment.NewLine}";
                foreach (KeyValuePair<string, bool> vrItem in dictPasswordRequirements)
                {
                    if (vrItem.Value == false)
                    {
                        strMessage += $"-{vrItem.Key} {Environment.NewLine}";
                    }
                }
                return new Tuple<bool, string>(false, strMessage);
            }
            entities.tblUsers.Add(tempUser);
            entities.SaveChanges();
            return new Tuple<bool, string>(true, "Successfull");
        }
        public Tuple<bool, string> ExtentedRegister()
        {
            models.tblUser[] allUsers = entities.tblUsers.ToArray();
            models.tblUser tempUser = new models.tblUser()
            {
                usrName = strName,
                usrSurname = strSurname,
                usrSchoolNumber = strSchoolNumber,
                usrMail = strMail,
                usrPassword = strPassword,
                usrAuthorityLevel = strAuthorityLevel,
                usrAccountActivation = blAccountActivation,
                usrLecturerActivation = blLecturerActivation
            };
            foreach (tblUser vrItem in allUsers)
            {
                if (vrItem.usrMail == tempUser.usrMail)
                {
                    return new Tuple<bool, string>(false, "This E-Mail has already registered system;");
                }
                else if (vrItem.usrSchoolNumber == tempUser.usrSchoolNumber)
                {
                    return new Tuple<bool, string>(false, "This School Number has already registered system;");
                }
            }
            if (tempUser.usrPassword == null)
            {
                string strMessage = $"Missing requirements for your password:{Environment.NewLine}";
                foreach (KeyValuePair<string, bool> vrItem in dictPasswordRequirements)
                {
                    if (vrItem.Value == false)
                    {
                        strMessage += $"-{vrItem.Key} {Environment.NewLine}";
                    }
                }

                return new Tuple<bool, string>(false, strMessage);
            }
            entities.tblUsers.Add(tempUser);
            entities.SaveChanges();
            return new Tuple<bool, string>(true, "Successfull");
        }
    }
}

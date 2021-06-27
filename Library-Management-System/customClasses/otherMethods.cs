using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Controls;

namespace Library_Management_System
{
    public class otherMethods
    {
        public static bool isMail(string strMail)
        {
            return new EmailAddressAttribute().IsValid(strMail);
        }
        public static bool isNumber(string strNumber)
        {
            List<int> whiteList = Enumerable.Range(48, 10).ToList();
            return strNumber.All(p => whiteList.Contains(p));
        }
        public static bool isMD5(string strHashText)
        {
            List<int> whiteList = Enumerable.Range(48, 10).ToList();
            whiteList.AddRange(Enumerable.Range(97, 6).ToList());
            return strHashText.All(p => whiteList.Contains(p) && strHashText.Length == 32);
        }
        public static string generateMD5(string strPlainText)
        {
            MD5 md5 = MD5.Create();
            StringBuilder stringBuilder = new StringBuilder();
            byte[] plainTextBytes = Encoding.ASCII.GetBytes(strPlainText);
            byte[] hashTextBytes = md5.ComputeHash(plainTextBytes);
            foreach (byte vrItem in hashTextBytes)
            {
                stringBuilder.Append(vrItem.ToString("X2"));
            }
            return stringBuilder.ToString().ToLower();
        }
        public static Dictionary<string, bool> isStrongPassword(string strPassword)
        {
            Dictionary<string, bool> keyValuesResult = new Dictionary<string, bool>();
            List<int> lstUpperCase = Enumerable.Range(65, 26).ToList();
            List<int> lstLowerCase = Enumerable.Range(97, 26).ToList();
            List<int> lstNumbers = Enumerable.Range(48, 10).ToList();
            keyValuesResult.Add("Upper Case", strPassword.Any(p => lstUpperCase.Contains(p)));
            keyValuesResult.Add("Lower Case", strPassword.Any(p => lstLowerCase.Contains(p)));
            keyValuesResult.Add("Numbers", strPassword.Any(p => lstNumbers.Contains(p)));
            keyValuesResult.Add("Length(4 to 18)", 3 < strPassword.Length && strPassword.Length < 19);
            return keyValuesResult;
        }
        public static void textboxClear(params TextBox[] textBoxes)
        {
            foreach (TextBox vrItem in textBoxes)
            {
                vrItem.Clear();
            }
        }
        public static void passwordboxClear(params PasswordBox[] textBoxes)
        {
            foreach (PasswordBox vrItem in textBoxes)
            {
                vrItem.Clear();
            }
        }
        public static string makeTitle(string strRaw)
        {
            string[] arrLower = strRaw.Split(' ');
            string strResult = string.Empty;
            foreach (string vrItem in arrLower)
            {
                if (vrItem != " ")
                {
                    strResult += $" {vrItem.ToUpper()[0]}{vrItem.ToLower().Substring(1)}";
                }
            }
            return strResult.Trim();
        }
    }
}

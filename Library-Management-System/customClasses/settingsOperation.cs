using Library_Management_System.models;
using System;

namespace Library_Management_System.customClasses
{
    internal static class settingsOperation
    {
        public static byte getStudentMaxAmount()
        {
            byte irValue = Convert.ToByte(new dbLibraryEntities().tblSettings.Find(2).stgValue);
            return irValue;
        }
        public static byte getLecturerMaxAmount()
        {
            byte irValue = Convert.ToByte(new dbLibraryEntities().tblSettings.Find(4).stgValue);
            return irValue;
        }
        public static int getStudentBorrowTime()
        {
            int irValue = Convert.ToInt32(new dbLibraryEntities().tblSettings.Find(3).stgValue);
            return irValue;
        }
        public static int getLecturerBorrowTime()
        {
            int irValue = Convert.ToInt32(new dbLibraryEntities().tblSettings.Find(5).stgValue);
            return irValue;
        }
        public static string getSchoolName()
        {
            return new dbLibraryEntities().tblSettings.Find(1).stgValue;
        }
        public static string getSchoolIconPath()
        {
            return new dbLibraryEntities().tblSettings.Find(6).stgValue;
        }
        public static void updateSchoolName(string strSchoolName)
        {
            dbLibraryEntities entities = new dbLibraryEntities();
            tblSetting setting = entities.tblSettings.Find(1);
            setting.stgValue = otherMethods.makeTitle(strSchoolName);
            entities.SaveChanges();
        }
        public static void updateSchoolIconUpdate(string strIconPath)
        {
            dbLibraryEntities entities = new dbLibraryEntities();
            tblSetting setting = entities.tblSettings.Find(6);
            setting.stgValue = strIconPath;
            entities.SaveChanges();
        }
        public static void updateStudentBorrowTime(int irNewTime)
        {
            dbLibraryEntities entities = new dbLibraryEntities();
            tblSetting setting = entities.tblSettings.Find(3);
            setting.stgValue = irNewTime.ToString();
            entities.SaveChanges();
        }
        public static void updateLecturerBorrowTime(int irNewTime)
        {
            dbLibraryEntities entities = new dbLibraryEntities();
            tblSetting setting = entities.tblSettings.Find(5);
            setting.stgValue = irNewTime.ToString();
            entities.SaveChanges();
        }
        public static void updateStudentBookAmount(int irNewAmount)
        {
            dbLibraryEntities entities = new dbLibraryEntities();
            tblSetting setting = entities.tblSettings.Find(2);
            setting.stgValue = irNewAmount.ToString();
            entities.SaveChanges();
        }
        public static void updateLecturerBookAmount(int irNewAmount)
        {
            dbLibraryEntities entities = new dbLibraryEntities();
            tblSetting setting = entities.tblSettings.Find(4);
            setting.stgValue = irNewAmount.ToString();
            entities.SaveChanges();
        }
    }
}

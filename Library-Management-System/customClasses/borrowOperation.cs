using Library_Management_System.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Library_Management_System.customClasses
{
    internal static class borrowOperation
    {
        private static readonly dbLibraryEntities entities = new dbLibraryEntities();
        public static tblBorrowed getBorrowDetail(int irUserID, int irBookID)
        {
            return entities.tblBorroweds.ToArray().Where(p => p.brwUsrID == irUserID && p.brwBkID == irBookID && p.brwSitutation == null).First();
        }
        public static Queue<tblBorrowed> getCloseRedemptionDatesBook(int irUserID)
        {
            Queue<tblBorrowed> borrowedsQueue = new Queue<tblBorrowed>();
            tblBorrowed[] borroweds = new dbLibraryEntities().tblBorroweds.ToArray().Where(p => p.brwSitutation == null && p.brwUsrID == irUserID).ToArray();
            foreach (tblBorrowed vrItem in borroweds)
            {
                if (getLeftTime(vrItem.brwID) <= 2)
                {
                    borrowedsQueue.Enqueue(vrItem);
                }
            }
            return borrowedsQueue;
        }
        public static viewUpgradedBorrowed[] getBorrowedWithView()
        {
            return new dbLibraryEntities().viewUpgradedBorroweds.ToArray();
        }
        private static XDocument getAllBorrowedOperation()
        {
            viewUpgradedBorrowed[] borroweds = new dbLibraryEntities().viewUpgradedBorroweds.ToArray();
            XDocument document = new XDocument();
            document.Add(new XElement("borroweds", borroweds.Select(p => new XElement("detail",
                new XAttribute("id", p.brwID),
                new XElement("user_fullname", p.usrFullName),
                new XElement("book", p.bkName),
                new XElement("purchase_date", p.brwPurchaseDate.ToString("dd/MM/yyyy")),
                new XElement("redemption_date", p.brwRedemptionDate.ToString("dd/MM/yyyy")),
                new XElement("situtation", p.brwSitutation)
            ))));
            return document;
        }
        public static void allBorrowedOperationtoXML(string strFileName)
        {
            getAllBorrowedOperation().Save(strFileName);
        }
        public static void allBorrowedOperationtoJSON(string strFileName)
        {
            string strJsonResult = JsonConvert.SerializeXNode(getAllBorrowedOperation(), Formatting.Indented);
            File.WriteAllText(strFileName, strJsonResult);
        }
        public static bool doIHaveBook(int irUserID, int irBookID)
        {
            var result = entities.tblBorroweds.ToArray().Where(p => p.brwUsrID == irUserID && p.brwBkID == irBookID && p.brwSitutation is null).ToArray();
            if (result.Length == 0)
                return false;
            return true;
        }
        private static int getLeftTime(int irBorrowedID)
        {
            tblBorrowed borrowed = entities.tblBorroweds.Find(irBorrowedID);
            return (borrowed.brwRedemptionDate - DateTime.Now).Days;
        }
    }
}

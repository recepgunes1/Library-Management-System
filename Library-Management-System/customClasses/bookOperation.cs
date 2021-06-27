using Library_Management_System.models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Library_Management_System.customClasses
{
    internal static class bookOperation
    {
        private static readonly dbLibraryEntities entities = new dbLibraryEntities();
        public static tblBook getBooks(int irBookID)
        {
            return entities.tblBooks.Find(irBookID);
        }
        public static tblBook[] getAllBooks()
        {
            return entities.tblBooks.ToArray();
        }
        public static tblBook[] getMyBooks(int irID)
        {
            tblBorrowed[] rawBorrowed = entities.tblBorroweds.ToArray().Where(p => p.brwUsrID == irID).ToArray();
            tblBook[] allBooks = entities.tblBooks.ToArray();
            tblBook[] vrResult = (from item in allBooks
                                  where (from subitem in rawBorrowed where subitem.brwSitutation == null select subitem.brwBkID).Contains(item.bkID)
                                  select item).ToArray();
            return vrResult;
        }
        public static tblBook[] getMyBooks(int irID, string strInput)
        {
            tblBorrowed[] rawBorrowed = entities.tblBorroweds.ToArray().Where(p => p.brwUsrID == irID).ToArray();
            tblBook[] allBooks = entities.tblBooks.ToArray();
            tblBook[] vrResult = (from item in allBooks
                                  where (from subitem in rawBorrowed where subitem.brwSitutation == null select subitem.brwBkID).Contains(item.bkID)
                                  && item.bkName.Contains(strInput)
                                  select item).ToArray();
            return vrResult;
        }
        public static viewUpgradedBook[] searchBook(string strInput)
        {
            viewUpgradedBook[] allBookUnfiltred = getAllBooksWithWiev();
            var result = (from item in allBookUnfiltred
                         where item.bkAuthor.ToLower().Contains(strInput) ||
                         item.bkName.ToLower().Contains(strInput) ||
                         item.bkSummary.ToLower().Contains(strInput) ||
                         item.ctgName.ToLower().Contains(strInput)
                         select item).ToArray();
            return result;
        }
        public static viewUpgradedBook[] getAllBooksWithWiev()
        {
            return new dbLibraryEntities().viewUpgradedBooks.ToArray();
        }
        public static viewUpgradedBook[] bookFiletered(int irUserID)
        {
            viewUpgradedBook[] upgradedBooks = getAllBooksWithWiev();
            int[] myBooksID = (from item in getMyBooks(irUserID)
                               select item.bkID).ToArray();
            var result = (from item in upgradedBooks
                          where item.bkAmount > 0 &&
                                !myBooksID.Contains(item.bkID)
                          select item).ToArray();
            return result;
        }
        public static viewUpgradedBook[] bookFiletered(viewUpgradedBook[] upgradedBooks, string strInput)
        {
            var result = (from item in upgradedBooks
                          where item.bkName.ToLower().Contains(strInput) ||
                          item.bkAuthor.ToLower().Contains(strInput) ||
                          item.bkSummary.ToLower().Contains(strInput)
                          select item).ToArray();
            return result;
        }
        public static viewUpgradedBook[] bookFiletered(bool? blUnstocked, bool? blOwned, string strCategory, int irUserID)
        {
            viewUpgradedBook[] upgradedBooks = getAllBooksWithWiev();
            if (blUnstocked == false)
            {
                upgradedBooks = (from item in upgradedBooks
                                 where item.bkAmount > 0
                                 select item).ToArray();
            }
            if (blOwned == false)
            {
                int[] myBooksID = (from item in getMyBooks(irUserID)
                                   select item.bkID).ToArray();
                upgradedBooks = (from item in upgradedBooks
                                 where !myBooksID.Contains(item.bkID)
                                 select item).ToArray();
            }
            if (!string.IsNullOrEmpty(strCategory))
            {
                upgradedBooks = (from item in upgradedBooks
                                 where item.ctgName.ToLower() == strCategory
                                 select item).ToArray();
            }
            return upgradedBooks;
        }
        public static viewUpgradedUser[] getBookOwners(int irBookID)
        {
            tblBorrowed[] arrOwnerRawRows = entities.tblBorroweds.Where(p => p.brwBkID == irBookID).ToArray();
            viewUpgradedUser[] arrAllUsers = new dbLibraryEntities().viewUpgradedUsers.ToArray();
            viewUpgradedUser[] vrResult = (from item in arrAllUsers
                                           where (from subitem in arrOwnerRawRows where subitem.brwSitutation is null select subitem.brwUsrID).Contains(item.usrID)
                                           select item).ToArray();
            return vrResult;
        }
        public static viewUpgradedBorrowed[] getAllWaitingBooks()
        {
            return new dbLibraryEntities().viewUpgradedBorroweds.ToArray().Where(p => p.brwSitutation == false).ToArray();
        }
        private static XDocument getAllBooksWithXML()
        {
            viewUpgradedBook[] books = getAllBooksWithWiev();
            XDocument document = new XDocument();
            document.Add(new XElement("books", books.Select(p => new XElement("book_detail",
                new XAttribute("id", p.bkID),
                new XElement("name", p.bkName),
                new XElement("author", p.bkAuthor),
                new XElement("category", p.ctgName),
                new XElement("pages", p.bkPages),
                new XElement("published_date", p.bkPublishedDate.ToString("dd/MM/yyyy")),
                new XElement("summary", p.bkSummary)
                ))));
            return document;
        }
        public static int loanBook(int irBorrowedItemID)
        {
            return entities.giveBackBook(irBorrowedItemID);
        }
        public static void becomeWaitingBook(tblBorrowed brwData)
        {
            tblBorrowed brwTemp = entities.tblBorroweds.Find(brwData.brwID);
            brwTemp.brwSitutation = false;
            entities.SaveChanges();
        }
        public static void allBookstoXML(string strFileName)
        {
            getAllBooksWithXML().Save(strFileName);
        }
        public static void allBookstoJSON(string strFileName)
        {
            string strJsonResult = JsonConvert.SerializeXNode(getAllBooksWithXML(), Formatting.Indented);
            File.WriteAllText(strFileName, strJsonResult);
        }
        public static Tuple<bool, string> updateBook(tblBook tempBook)
        {
            try
            {
                tblBook selectedBook = entities.tblBooks.Find(tempBook.bkID);
                selectedBook.bkName = tempBook.bkName;
                selectedBook.bkAuthor = tempBook.bkAuthor;
                selectedBook.bkPublishedDate = tempBook.bkPublishedDate;
                selectedBook.bkPages = tempBook.bkPages;
                selectedBook.bkAmount = tempBook.bkAmount;
                selectedBook.bkSummary = tempBook.bkSummary;
                selectedBook.bkCategoryID = tempBook.bkCategoryID;
                entities.SaveChanges();
                return new Tuple<bool, string>(true, "The book was updated successfully.");
            }
            catch(Exception e)
            {
                return new Tuple<bool, string>(false, $"The error has been occurred:{System.Environment.NewLine}{e.Message}");
            }
        }
        public static Tuple<bool, string> createBook(string _strName, string _strAuthor, DateTime _dtPublishedDate,
            int _irPages, byte _irAmount, string _strSummary, int _irCategoryID)
        {
            if (!getAllBooks().Any(p => p.bkName == _strName.ToLower().Trim()))
            {
                try
                {
                    tblBook bookTemp = new tblBook
                    {
                        bkName = _strName.Trim().ToLower(),
                        bkAuthor = _strAuthor.Trim().ToLower(),
                        bkPublishedDate = _dtPublishedDate,
                        bkPages = _irPages,
                        bkAmount = _irAmount,
                        bkSummary = _strSummary.Trim().ToLower(),
                        bkCategoryID = _irCategoryID
                    };
                    entities.tblBooks.Add(bookTemp);
                    entities.SaveChanges();
                    return new Tuple<bool, string>(true, "The book was added successfully.");
                }
                catch (Exception e)
                {
                    return new Tuple<bool, string>(false, $"The error has been occurred:{System.Environment.NewLine}{e.Message}");
                }
            }
            else
            {
                return new Tuple<bool, string>(false, "This book has registered already.");
            }
        }
        public static Tuple<bool, string> borrowTheBook(int irUserID, int irBookID, int irWaitingTime)
        {
            try
            {
                tblBorrowed borrowed = new tblBorrowed() { brwUsrID = irUserID, brwBkID = irBookID, brwPurchaseDate = DateTime.Now, brwRedemptionDate = DateTime.Now.AddDays(irWaitingTime) };
                entities.tblBorroweds.Add(borrowed);
                entities.SaveChanges();
            }
            catch
            {
                return new Tuple<bool, string>(false, "You can't take this book");
            }
            return new Tuple<bool, string>(true, "You take the book");
        }
    }
}

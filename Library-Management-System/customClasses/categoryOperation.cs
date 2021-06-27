using Library_Management_System.models;
using System;
using System.Linq;

namespace Library_Management_System.customClasses
{
    internal static class categoryOperation
    {
        private static readonly dbLibraryEntities entities = new dbLibraryEntities();
        private static tblCategory[] allCategories;
        private static tblBook[] allBooks;
        private static tblCategory getCategory(string strPattern)
        {
            allCategories = getAllCategories();
            tblCategory vrResult = (from item in allCategories
                                    where item.ctgName == strPattern
                                    select item).First();
            return vrResult;
        }
        public static tblCategory[] getAllCategories()
        {
            return entities.tblCategories.ToArray();
        }
        public static Tuple<bool, string> createNewCategory(string strName)
        {
            allCategories = getAllCategories();
            if (!allCategories.Any(p => p.ctgName == strName))
            {
                tblCategory category = new tblCategory
                {
                    ctgName = strName,
                    ctgCreatedDate = DateTime.Now
                };
                entities.tblCategories.Add(category);
                entities.SaveChanges();
                return new Tuple<bool, string>(true, "New category was added system.");
            }
            return new Tuple<bool, string>(false, "This category has registered already.");
        }
        public static Tuple<bool, string> deleteCategory(string strName)
        {
            allCategories = getAllCategories();
            allBooks = bookOperation.getAllBooks();
            tblCategory test = getCategory(strName);
            if (!allBooks.Any(p => p.bkCategoryID == test.ctgID))
            {
                entities.tblCategories.Remove(test);
                entities.SaveChanges();
                return new Tuple<bool, string>(true, "You removed a category succesfully.");
            }
            return new Tuple<bool, string>(false, "You didn't delete this category because it is invalid.");
        }
    }
}

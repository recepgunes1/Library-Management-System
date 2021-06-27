using Library_Management_System.customClasses;
using Library_Management_System.models;
using System.Windows;

namespace Library_Management_System
{
    /// <summary>
    /// Interaction logic for BookDetailWindow.xaml
    /// </summary>
    public partial class BookDetailWindow : Window
    {
        private readonly tblUser selectedUser;
        private readonly viewUpgradedBook selectedBook;
        private int irBorrowTime;
        public BookDetailWindow(tblUser _user, viewUpgradedBook _book)
        {
            InitializeComponent();
            selectedUser = _user;
            selectedBook = _book;
        }
        private void wndwBookDetail_Loaded(object sender, RoutedEventArgs e)
        {
            txtBookName.Text = selectedBook.bkName;
            txtBookAuthor.Text = selectedBook.bkAuthor;
            dtBookPublishedDate.Text = selectedBook.bkPublishedDate.ToString("dd/MM/yyyy");
            txtBookPages.Text = selectedBook.bkPages.ToString();
            txtBookCategory.Text = selectedBook.ctgName;
            rctxtBookSummary.AppendText(selectedBook.bkSummary);
            if (selectedBook.bkAmount > 0 && !borrowOperation.doIHaveBook(selectedUser.usrID,selectedBook.bkID))
            {
                if (selectedUser.usrAuthorityLevel.ToLower() == "student" && selectedUser.usrAmontBooks < settingsOperation.getStudentMaxAmount())
                {
                    btnTakeIt.IsEnabled = true;
                    irBorrowTime = settingsOperation.getStudentBorrowTime();
                }
                if (selectedUser.usrAuthorityLevel.ToLower() == "lecturer")
                {
                    if (selectedUser.usrLecturerActivation == true)
                    {
                        if (selectedUser.usrAmontBooks < settingsOperation.getLecturerMaxAmount())
                        {
                            btnTakeIt.IsEnabled = true;
                            irBorrowTime = settingsOperation.getLecturerBorrowTime();
                        }
                    }
                    else
                    {
                        if (selectedUser.usrAmontBooks < settingsOperation.getStudentMaxAmount())
                        {
                            btnTakeIt.IsEnabled = true;
                            irBorrowTime = settingsOperation.getStudentBorrowTime();
                        }
                    }
                }
            }
        }
        private void btnTakeIt_Click(object sender, RoutedEventArgs e)
        {
            int irSelectedUserID = selectedUser.usrID;
            int irSeletectedBookID = selectedBook.bkID;
            bookOperation.borrowTheBook(irSelectedUserID, irSeletectedBookID, irBorrowTime);
            Close();
        }
    }
}

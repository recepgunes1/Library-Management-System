using Library_Management_System.customClasses;
using Library_Management_System.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Library_Management_System
{
    /// <summary>
    /// Interaction logic for LibraryWindow.xaml
    /// </summary>
    public partial class LibraryWindow : Window
    {
        private viewUpgradedBook[] allUpgradedBooks;
        private tblUser selectedUser;
        private viewUpgradedUser upgradedSelectedUser;
        private tblCategory[] allCategories;
        private tblBook selectedBook;
        public LibraryWindow(tblUser _tblUser)
        {
            InitializeComponent();
            selectedUser = userOperation.getUser(_tblUser.usrID);
        }
        //Window Loaded Events
        private void wndwLibrary_Loaded(object sender, RoutedEventArgs e)
        {
            Title = settingsData.getLibraryWindowName($"{selectedUser.usrName} {selectedUser.usrSurname}");
            txtWelcomeText.Text = settingsData.getWelcomeText($"{selectedUser.usrName} {selectedUser.usrSurname}");
            if (File.Exists(settingsOperation.getSchoolIconPath()))
            {
                imgSchoolIcon.Source = new BitmapImage(new Uri(settingsOperation.getSchoolIconPath()));
            }

            dgAllBooksUpdate();
            cmbCategoryUpdate();
            updateMyBooks();
            upgradedSelectedUser = userOperation.getUpgradedUser(selectedUser.usrID);
            txtFullName.Text = upgradedSelectedUser.usrFullName;
            txtSchoolNumber.Text = upgradedSelectedUser.usrSchoolNumber;
            txtAuthorityLevel.Text = upgradedSelectedUser.usrAuthorityLevel;
            txtEMail.Text = upgradedSelectedUser.usrMail;
            txtAuthorityLevel.Foreground = upgradedSelectedUser.usrAuthorityLevel == "lecturer" ?
                (selectedUser.usrLecturerActivation == true ? Brushes.DarkGreen : Brushes.DarkRed) : Brushes.DarkBlue;
            updateNotifications();
        }
        //Welcome Tab Events and methods
        private void updateNotifications()
        {
            lstNotifacations.Items.Clear();
            Queue<tblBorrowed> borrowedsQueue = borrowOperation.getCloseRedemptionDatesBook(selectedUser.usrID);
            while (borrowedsQueue.Count != 0)
            {
                tblBorrowed temp = borrowedsQueue.Dequeue();
                string strContent = $"Your redemption date is closer for{Environment.NewLine}{bookOperation.getBooks((int)temp.brwBkID).bkName.ToUpper()} anymore,{Environment.NewLine}please give it back.";
                ListBoxItem item = new ListBoxItem() { Content = strContent, Foreground = Brushes.Red };
                lstNotifacations.Items.Add(item);
            }
        }
        //Library Tab Events and Methods
        private void txtSearching_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strInput = txtSearching.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(strInput))
                dgAllBooksUpdate();
            else
                dtgrdAllBooks.ItemsSource = bookOperation.bookFiletered((viewUpgradedBook[])dtgrdAllBooks.ItemsSource,strInput);
        }
        private void dtgrdAllBooks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            viewUpgradedBook vrTemp = (viewUpgradedBook)dtgrdAllBooks.SelectedItem;
            if (vrTemp == null)
                return;
            selectedUser = userOperation.getUser(selectedUser.usrID);
            BookDetailWindow bookDetail = new BookDetailWindow(selectedUser, vrTemp);
            bookDetail.Show();
        }
        private void btnApplyFiltre_Click(object sender, RoutedEventArgs e)
        {
            txtSearching.Clear();
            bool? blUnstocked = checkShowUnstocked.IsChecked;
            bool? blOwned = checkShowOwned.IsChecked;
            string strSelectedCategory = cmbLibraryCategories.SelectedIndex == 0 ? null : cmbLibraryCategories.Text;
            dtgrdAllBooks.ItemsSource = bookOperation.bookFiletered(blUnstocked, blOwned, strSelectedCategory, selectedUser.usrID);
        }
        private void dgAllBooksUpdate()
        {
            allUpgradedBooks = bookOperation.bookFiletered(selectedUser.usrID);
            dtgrdAllBooks.ItemsSource = allUpgradedBooks;
        }
        private void cmbCategoryUpdate()
        {
            allCategories = categoryOperation.getAllCategories();
            cmbLibraryCategories.Items.Add("all of them");
            foreach (tblCategory vrItem in allCategories)
            {
                cmbLibraryCategories.Items.Add(vrItem.ctgName);
            }
        }
        //My Account Tab Events and Methods
        //My Account Tab -> User Information part 
        private void txtEMail_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strInput = txtEMail.Text.ToLower().Trim();
            if (otherMethods.isMail(strInput) && selectedUser.usrMail != strInput)
            {
                btnUpdateMail.IsEnabled = true;
            }
            else
            {
                btnUpdateMail.IsEnabled = false;
            }
        }
        private void btnUpdateMail_Click(object sender, RoutedEventArgs e)
        {
            string strInput = txtEMail.Text.ToLower().Trim();
            selectedUser.usrMail = strInput;
            userOperation.updateUser(selectedUser);
            selectedUser = userOperation.getUser(selectedUser.usrID);
            txtEMail.Clear();
            txtEMail.Text = selectedUser.usrMail;
        }
        private void btnUpdatePasswordActivator(object sender, RoutedEventArgs e)
        {
            string strOldPassword = pswdOldPassw.Password.Trim();
            string strNewPassword = pswdNewPassw.Password.Trim();
            string strNewPasswordAgain = pswdNewPasswAgain.Password.Trim();
            if ((otherMethods.generateMD5(strOldPassword) == selectedUser.usrPassword) && (strNewPassword == strNewPasswordAgain))
            {
                btnUpdatePassword.IsEnabled = true;
            }
            else
            {
                btnUpdatePassword.IsEnabled = false;
            }
        }
        private void btnUpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            string strNewPassword = pswdNewPassw.Password.Trim();
            selectedUser.usrPassword = otherMethods.generateMD5(strNewPassword);
            Tuple<bool, string> result = userOperation.updateUser(selectedUser);
            selectedUser = userOperation.getUser(selectedUser.usrID);
            otherMethods.passwordboxClear(pswdNewPassw, pswdNewPasswAgain, pswdOldPassw);
            MessageBox.Show(result.Item2, "INFORMATION", MessageBoxButton.OK);
        }
        //My Account Tab -> Owned Books part 
        private void updateMyBooks()
        {
            tblBook[] myBooks = bookOperation.getMyBooks(selectedUser.usrID);
            dtgrdOwnedBooks.ItemsSource = myBooks;
        }
        private void txtSearchMyBooks_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strInput = txtSearchMyBooks.Text.Trim().ToLower();
            tblBook[] myBooks = bookOperation.getMyBooks(selectedUser.usrID,strInput);
            dtgrdOwnedBooks.ItemsSource = myBooks;
        }
        private void dtgrdOwnedBooks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            selectedBook = (tblBook)dtgrdOwnedBooks.SelectedItem;
            if (selectedBook == null)
                return;
            txtSearchMyBooks.Clear();
            tblBorrowed vrTempBorrowed = borrowOperation.getBorrowDetail(selectedUser.usrID, selectedBook.bkID);
            txtBookName.Text = selectedBook.bkName;
            dtpcBarrowDate.Text = vrTempBorrowed.brwPurchaseDate.ToString("dd/MM/yyyy");
            txtLeftTime.Text = (vrTempBorrowed.brwRedemptionDate - vrTempBorrowed.brwPurchaseDate).Days.ToString();
            btnGiveBack.IsEnabled = true;

        }
        private void btnGiveBack_Click(object sender, RoutedEventArgs e)
        {
            tblBorrowed borrowed = borrowOperation.getBorrowDetail(selectedUser.usrID, selectedBook.bkID);
            bookOperation.becomeWaitingBook(borrowed);
            txtBookName.Clear();
            dtpcBarrowDate.Clear();
            txtLeftTime.Clear();
            btnGiveBack.IsEnabled = false;
            updateMyBooks();
            updateNotifications();
        }
        //Other methods and events
        private void btnExitSession_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
        //Main Tab Operation
        private void tbMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateNotifications();
            updateMyBooks();
            if (tbMain.SelectedIndex != 1)
                dgAllBooksUpdate();
        }
    }
}

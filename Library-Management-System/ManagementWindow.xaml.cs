using Library_Management_System.customClasses;
using Library_Management_System.models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Library_Management_System
{
    /// <summary>
    /// Interaction logic for ManagementWindow.xaml
    /// </summary>
    public partial class ManagementWindow : Window
    {
        public tblUser[] allUsers;
        public tblUser[] waitingLecturer;
        public tblBook[] allBooks;
        public tblCategory[] allCategories;
        public viewUpgradedUser[] upgradedAllUsers;
        public viewUpgradedBook[] upgradedAllBooks;
        public tblUser selectedUser;
        public tblUser LoggedInUser;
        public tblBook selectedBook;
        public ManagementWindow(tblUser _usradmin)
        {
            InitializeComponent();
            LoggedInUser = _usradmin;
        }
        private void wndwManagement_Loaded(object sender, RoutedEventArgs e)
        {
            Title = settingsData.getManagementWindowName($"{LoggedInUser.usrName} {LoggedInUser.usrSurname}");
            dataGridAllUserUpdate();
            listboxWaitingLecturerUpdate();
            updateUserGeneralInformations();
            dataGridAllBooksUpdate();
            comboBoxCategoriesUpdate();
            dtgrdWaitingVerifyBooks_SourceUpdate();
            updateBorrowed();
            txtSchoolIconLocation.Text = settingsOperation.getSchoolIconPath();
            txtSchoolName.Text = settingsOperation.getSchoolName();
            txtBookAmount.Text = "0";

        }
        //User Tab Operation
        private void dtgrdAllUser_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dtgrdAllUser.SelectedItems.Count == 1)
            {
                viewUpgradedUser temp = (viewUpgradedUser)dtgrdAllUser.SelectedItem;
                selectedUser = userOperation.getUser(temp.usrID);
                txtUserName.Text = selectedUser.usrName;
                txtUserSurname.Text = selectedUser.usrSurname;
                txtUserSchoolNumber.Text = selectedUser.usrSchoolNumber;
                txtUserMail.Text = selectedUser.usrMail;
                txtUserPassword.Text = selectedUser.usrPassword;
                cmbLecturerActivation.SelectedIndex = selectedUser.usrLecturerActivation == null ? -1 : (selectedUser.usrLecturerActivation == true ? 0 : 1);
                cmbAccountActivation.SelectedIndex = selectedUser.usrAccountActivation == true ? 0 : 1;
                foreach (object vrItem in cmbAuthorityLevel.Items)
                {
                    if (vrItem.ToString().ToLower().Contains(selectedUser.usrAuthorityLevel))
                    {
                        cmbAuthorityLevel.SelectedItem = vrItem;
                        break;
                    }
                }
                btnSaveItUser.Click -= userCreate;
                btnSaveItUser.Click += userUpdate;
            }
        }
        private void userUpdate(object sender, RoutedEventArgs e)
        {
            Tuple<bool, string> tblResult;
            tblUser tempUser = new tblUser
            {
                usrID = selectedUser.usrID,
                usrName = txtUserName.Text.Trim().ToLower(),
                usrSurname = txtUserSurname.Text.Trim().ToLower(),
                usrSchoolNumber = txtUserSchoolNumber.Text.Trim().ToLower(),
                usrMail = txtUserMail.Text.Trim().ToLower(),
                usrPassword = txtUserPassword.Text.Trim().ToLower(),
                usrAuthorityLevel = cmbAuthorityLevel.Text.ToLower(),
                usrAccountActivation = cmbAccountActivation.SelectedIndex == 0
            };
            if (cmbAuthorityLevel.Text.ToLower() == "lecturer")
            {
                tempUser.usrLecturerActivation = cmbLecturerActivation.SelectedIndex == 0;
            }
            else
            {
                tempUser.usrLecturerActivation = null;
            }

            tblResult = userOperation.updateUser(tempUser);
            MessageBoxGenerator(tblResult);
            dataGridAllUserUpdate();
            listboxWaitingLecturerUpdate();
            updateUserGeneralInformations();
            updateBorrowed();
            btnSaveItUser.Click -= userUpdate;
            btnSaveItUser.Click += userCreate;
            otherMethods.textboxClear(txtUserName, txtUserSurname, txtUserSchoolNumber, txtUserMail, txtUserPassword);
        }
        private void userCreate(object sender, RoutedEventArgs e)
        {
            bool? result;
            if (cmbAuthorityLevel.Text.ToLower() == "lecturer")
            {
                result = cmbLecturerActivation.SelectedIndex == 0;
            }
            else
            {
                result = null;
            }

            SignUp signUp = new SignUp(txtUserName.Text, txtUserSurname.Text, txtUserSchoolNumber.Text, txtUserMail.Text, txtUserPassword.Text,
                cmbAuthorityLevel.Text.ToLower(), cmbAccountActivation.SelectedIndex == 0, result);
            Tuple<bool, string> tblResult = signUp.ExtentedRegister();
            MessageBoxGenerator(tblResult);
            dataGridAllUserUpdate();
            listboxWaitingLecturerUpdate();
            updateUserGeneralInformations();
            otherMethods.textboxClear(txtUserName, txtUserSurname, txtUserSchoolNumber, txtUserMail, txtUserPassword);
        }
        private void btnSaveItUserActivator(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.Print(sender.ToString());
            if (txtUserName.Text.Trim().Length >= 2 && txtUserSurname.Text.Trim().Length >= 2 && txtUserSchoolNumber.Text.Trim().Length == 9
                && otherMethods.isMail(txtUserMail.Text.Trim()) && txtUserPassword.Text.Trim().Length > 3)
            {
                btnSaveItUser.IsEnabled = true;
            }
            else
            {
                btnSaveItUser.IsEnabled = false;
            }
        }
        private void txtUserSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strPattern = txtUserSearch.Text.Trim().ToLower();
            dtgrdAllUser.ItemsSource = userOperation.userSearchOnView(strPattern);
        }
        private void btnExitSession_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
        private void btnEnableAllLecturer_Click(object sender, RoutedEventArgs e)
        {
            foreach (viewUpgradedUser vrItem in userOperation.getWaitingLecturer())
            {
                tblUser vrTemp = userOperation.getUser(vrItem.usrID);
                vrTemp.usrLecturerActivation = true;
                userOperation.updateUser(vrTemp);
            }
            listboxWaitingLecturerUpdate();
        }
        private void btnEnableSelectedLecturer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int irUserID = ((viewUpgradedUser)dtgrdWaitingLecturer.SelectedItem).usrID;
                tblUser vrUser = userOperation.getUser(irUserID);
                vrUser.usrLecturerActivation = true;
                userOperation.updateUser(vrUser);
                listboxWaitingLecturerUpdate();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("You have to select an user.", "ERROR", MessageBoxButton.OK, MessageBoxImage.None);
            }
        }
        private void dataGridAllUserUpdate()
        {
            upgradedAllUsers = userOperation.getAllUsersWithWiev();
            dtgrdAllUser.ItemsSource = upgradedAllUsers;
        }
        private void listboxWaitingLecturerUpdate()
        {
            dtgrdWaitingLecturer.ItemsSource = userOperation.getWaitingLecturer();
        }
        private void updateUserGeneralInformations()
        {
            Dictionary<string, int> dictGeneral = userOperation.getUserGeneral();
            txtActiveStudent.Text = dictGeneral["student-active"].ToString();
            txtPassiveStudent.Text = dictGeneral["student-passive"].ToString();
            txtTotalStudent.Text = Convert.ToString(dictGeneral["student-active"] + dictGeneral["student-passive"]);
            txtActiveLecturer.Text = dictGeneral["lecturer-active"].ToString();
            txtPassiveLecturer.Text = dictGeneral["lecturer-passive"].ToString();
            txtTotalLecturer.Text = Convert.ToString(dictGeneral["lecturer-active"] + dictGeneral["lecturer-passive"]);
        }
        private void MessageBoxGenerator(Tuple<bool, string> tuple)
        {
            if (!tuple.Item1)
            {
                MessageBox.Show(tuple.Item2, "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                MessageBox.Show(tuple.Item2, "INFORMATION", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
        private void onlyNumbers(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        //Book Tab Operation
        private void dataGridAllBooksUpdate()
        {
            dtgrdAllBook.ItemsSource = null;
            upgradedAllBooks = bookOperation.getAllBooksWithWiev();
            dtgrdAllBook.ItemsSource = upgradedAllBooks;
        }
        private void txtBookSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strPattern = txtBookSearch.Text.Trim().ToLower();
            dtgrdAllBook.ItemsSource = bookOperation.searchBook(strPattern);
        }
        private void dtgrdAllBook_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            rctxtBookSummary.Document.Blocks.Clear();
            if (dtgrdAllBook.SelectedItems.Count == 1)
            {
                viewUpgradedBook temp = (viewUpgradedBook)dtgrdAllBook.SelectedItem;
                selectedBook = bookOperation.getBooks(temp.bkID);
                txtBookName.Text = selectedBook.bkName;
                txtBookAuthor.Text = selectedBook.bkAuthor;
                txtBookPages.Text = selectedBook.bkPages.ToString();
                dpPublishedDate.SelectedDate = selectedBook.bkPublishedDate;
                cmbCategories.Text = temp.ctgName;
                rctxtBookSummary.AppendText(selectedBook.bkSummary);
                txtBookAmount.Text = selectedBook.bkAmount.ToString();
                dtgrdBookOwners.ItemsSource = bookOperation.getBookOwners(selectedBook.bkID);
                btnSaveItBook.Click -= bookCreate;
                btnSaveItBook.Click += bookUpdate;
            }
        }
        private void btnSaveItBookActivator(object sender, RoutedEventArgs e)
        {
            string strSummary = new TextRange(rctxtBookSummary.Document.ContentStart, rctxtBookSummary.Document.ContentEnd).Text.ToLower().Replace(Environment.NewLine, string.Empty);
            if (txtBookName.Text.Trim().ToLower().Length > 0 && txtBookAuthor.Text.Trim().ToLower().Length > 0 && int.TryParse(txtBookPages.Text, out int irTemp)
                && dpPublishedDate.SelectedDate != null && cmbCategories.SelectedIndex != -1 && byte.TryParse(txtBookAmount.Text, out byte bytTemp) && strSummary.Length > 0)
            {
                btnSaveItBook.IsEnabled = true;
                btnUpBookAmount.IsEnabled = true;
                btnDownBookAmount.IsEnabled = true;
            }
            else
            {

                btnSaveItBook.IsEnabled = false;
                btnUpBookAmount.IsEnabled = false;
                btnDownBookAmount.IsEnabled = false;
            }
        }
        private void bookUpdate(object sender, RoutedEventArgs e)
        {
            if (selectedBook != null)
            {
                allCategories = categoryOperation.getAllCategories();
                Tuple<bool, string> tupleResult;
                tblBook tblBookTemp = new tblBook
                {
                    bkID = selectedBook.bkID,
                    bkName = txtBookName.Text.Trim().ToLower(),
                    bkAuthor = txtBookAuthor.Text.Trim().ToLower(),
                    bkPages = Convert.ToInt32(txtBookPages.Text.ToLower().Trim()),
                    bkPublishedDate = (DateTime)dpPublishedDate.SelectedDate,
                    bkCategoryID = (from item in allCategories where item.ctgName == cmbCategories.Text.ToLower().Trim() select item.ctgID).Single(),
                    bkSummary = new TextRange(rctxtBookSummary.Document.ContentStart, rctxtBookSummary.Document.ContentEnd)
                    .Text.ToLower().Replace(Environment.NewLine, string.Empty),
                    bkAmount = Convert.ToByte(txtBookAmount.Text)
                };
                tupleResult = bookOperation.updateBook(tblBookTemp);
                otherMethods.textboxClear(txtBookAuthor, txtBookName, txtBookPages);
                txtBookAmount.Text = "0";
                dpPublishedDate.SelectedDate = null;
                rctxtBookSummary.Document.Blocks.Clear();
                dataGridAllBooksUpdate();
                updateBorrowed();
                comboBoxCategoriesUpdate();
                MessageBoxGenerator(tupleResult);
                btnSaveItBook.Click -= bookUpdate;
                btnSaveItBook.Click += bookCreate;
                selectedBook = null;
                dtgrdBookOwners.ItemsSource = null;
            }
        }
        private void bookCreate(object sender, RoutedEventArgs e)
        {
            Tuple<bool, string> tupleResult;
            int irCategoryId = (from item in allCategories where item.ctgName == cmbCategories.Text.ToLower().Trim() select item.ctgID).Single();
            string strSummary = new TextRange(rctxtBookSummary.Document.ContentStart, rctxtBookSummary.Document.ContentEnd).Text.ToLower().Replace(Environment.NewLine, string.Empty);

            tupleResult = bookOperation.createBook(txtBookName.Text.Trim().ToLower(), txtBookAuthor.Text.Trim().ToLower(), (DateTime)dpPublishedDate.SelectedDate,
                Convert.ToInt32(txtBookPages.Text), Convert.ToByte(txtBookAmount.Text), strSummary, irCategoryId);

            otherMethods.textboxClear(txtBookAuthor, txtBookName, txtBookPages);
            txtBookAmount.Text = "0";
            dpPublishedDate.SelectedDate = null;
            dtgrdBookOwners.ItemsSource = null;
            rctxtBookSummary.Document.Blocks.Clear();
            dataGridAllBooksUpdate();
            comboBoxCategoriesUpdate();
            MessageBoxGenerator(tupleResult);
        }
        private void btnUpBookAmount_Click(object sender, RoutedEventArgs e)
        {
            byte bytAmount = Convert.ToByte(txtBookAmount.Text);
            if (bytAmount <= 255)
            {
                bytAmount++;
                txtBookAmount.Text = Convert.ToString(bytAmount);
            }
        }
        private void btnDownBookAmount_Click(object sender, RoutedEventArgs e)
        {
            byte bytAmount = Convert.ToByte(txtBookAmount.Text);
            if (bytAmount >= 0)
            {
                bytAmount--;
                txtBookAmount.Text = Convert.ToString(bytAmount);
            }

        }
        //Book Tab -> Category Operations
        private void comboBoxCategoriesUpdate()
        {
            cmbCategories.Items.Clear();
            cmbDeletedCategory.Items.Clear();
            allCategories = categoryOperation.getAllCategories();
            foreach (tblCategory vrItem in allCategories)
            {
                cmbCategories.Items.Add(otherMethods.makeTitle(vrItem.ctgName));
                cmbDeletedCategory.Items.Add(otherMethods.makeTitle(vrItem.ctgName));
            }
        }
        private void btnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            Tuple<bool, string> tupleResult;
            string strInput = cmbDeletedCategory.Text.ToLower();
            tupleResult = categoryOperation.deleteCategory(strInput);
            MessageBoxGenerator(tupleResult);
            comboBoxCategoriesUpdate();
        }
        private void btnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            Tuple<bool, string> tupleResult;
            string strInput = txtNewCategory.Text.Trim().ToLower();
            tupleResult = categoryOperation.createNewCategory(strInput);
            MessageBoxGenerator(tupleResult);
            comboBoxCategoriesUpdate();
            txtNewCategory.Clear();
        }
        //Book Tab -> Book verify
        private void dtgrdWaitingVerifyBooks_SourceUpdate()
        {
            dtgrdWaitingVerifyBooks.ItemsSource = bookOperation.getAllWaitingBooks();
        }
        private void btnVerifyAllBooks_Click(object sender, RoutedEventArgs e)
        {
            foreach (viewUpgradedBorrowed vwItem in dtgrdWaitingVerifyBooks.Items)
            {
                int irBorrowedID = vwItem.brwID;
                bookOperation.loanBook(irBorrowedID);
            }
            dtgrdWaitingVerifyBooks_SourceUpdate();
            updateBorrowed();
        }
        private void btnVerifySelectedBooks_Click(object sender, RoutedEventArgs e)
        {
            viewUpgradedBorrowed borrowed = (viewUpgradedBorrowed)dtgrdWaitingVerifyBooks.SelectedItem;
            if (borrowed == null)
                return;
            int irBorrowedID = borrowed.brwID;
            bookOperation.loanBook(irBorrowedID);
            dtgrdWaitingVerifyBooks_SourceUpdate();
            updateBorrowed();
        }
        //Settings Tab Operation
        //Settings Tab School Details
        private void btnSchoolIconUpdate_Click(object sender, RoutedEventArgs e)
        {
            string strNewIconPath = string.Empty;
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png",
                Multiselect = false
            };
            if (fileDialog.ShowDialog() == true)
            {
                strNewIconPath = fileDialog.FileName;
                settingsOperation.updateSchoolIconUpdate(strNewIconPath);
                txtSchoolIconLocation.Text = settingsOperation.getSchoolIconPath();
            }

        }
        private void btnSchoolNameUpdate_Click(object sender, RoutedEventArgs e)
        {
            string strSchoolName = txtSchoolName.Text.Trim().ToLower();
            settingsOperation.updateSchoolName(strSchoolName);
            txtSchoolName.Text = settingsOperation.getSchoolName();
            Title = settingsData.getManagementWindowName($"{LoggedInUser.usrName} {LoggedInUser.usrSurname}");
        }
        //Settings Tab Backup
        private void btnBackupBook_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog
            {
                Title = "(Book) Save your file",
                Filter = "Json Files (*.json)|*.json| XML Files (*.xml)|*.xml"
            };
            if (fileDialog.ShowDialog() == true)
            {
                string strFileName = fileDialog.FileName.ToLower();
                string vrExtension = Path.GetExtension(strFileName);
                switch (vrExtension)
                {
                    case ".xml":
                        bookOperation.allBookstoXML(strFileName);
                        break;
                    case ".json":
                        bookOperation.allBookstoJSON(strFileName);
                        break;
                }
            }

        }
        private void btnBackupUser_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog
            {
                Title = "(Book) Save your file",
                Filter = "Json Files (*.json)|*.json| XML Files (*.xml)|*.xml"
            };
            if (fileDialog.ShowDialog() == true)
            {
                string strFileName = fileDialog.FileName.ToLower();
                string vrExtension = Path.GetExtension(strFileName);
                switch (vrExtension)
                {
                    case ".xml":
                        userOperation.allUserstoXML(strFileName);
                        break;
                    case ".json":
                        userOperation.allUserstoJSON(strFileName);
                        break;
                }
            }
        }
        private void btnBackupBorrowOps_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog
            {
                Title = "(Book) Save your file",
                Filter = "Json Files (*.json)|*.json| XML Files (*.xml)|*.xml"
            };
            if (fileDialog.ShowDialog() == true)
            {
                string strFileName = fileDialog.FileName.ToLower();
                string vrExtension = Path.GetExtension(strFileName);
                switch (vrExtension)
                {
                    case ".xml":
                        borrowOperation.allBorrowedOperationtoXML(strFileName);
                        break;
                    case ".json":
                        borrowOperation.allBorrowedOperationtoJSON(strFileName);
                        break;
                }
            }
        }
        //Settings Tab Borrow and Amount
        private void cmbBorrowTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBorrowTime.SelectedIndex == 0)
            {
                txtBorrowTime.Text = settingsOperation.getStudentBorrowTime().ToString();
            }
            else if (cmbBorrowTime.SelectedIndex == 1)
            {
                txtBorrowTime.Text = settingsOperation.getLecturerBorrowTime().ToString();
            }
            btnUpBorrow.IsEnabled = true;
            btnDownBorrow.IsEnabled = true;
        }
        private void cmbAmountBook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbAmountBook.SelectedIndex == 0)
            {
                txtAmountBook.Text = settingsOperation.getStudentMaxAmount().ToString();
            }
            else if (cmbAmountBook.SelectedIndex == 1)
            {
                txtAmountBook.Text = settingsOperation.getLecturerMaxAmount().ToString();
            }
            btnAmountUp.IsEnabled = true;
            btnAmountDown.IsEnabled = true;
        }
        private void btnUpBorrow_Click(object sender, RoutedEventArgs e)
        {
            int irBorrowTime = Convert.ToInt32(txtBorrowTime.Text);
            irBorrowTime += 1;
            if (!(irBorrowTime > 0 && irBorrowTime < int.MaxValue))
            {
                return;
            }

            txtBorrowTime.Text = irBorrowTime.ToString();
            if (cmbBorrowTime.SelectedIndex == 0)
            {
                settingsOperation.updateStudentBorrowTime(irBorrowTime);
            }

            if (cmbBorrowTime.SelectedIndex == 1)
            {
                settingsOperation.updateLecturerBorrowTime(irBorrowTime);
            }
        }
        private void btnDownBorrow_Click(object sender, RoutedEventArgs e)
        {
            int irBorrowTime = Convert.ToInt32(txtBorrowTime.Text);
            irBorrowTime -= 1;
            if (!(irBorrowTime > 0 && irBorrowTime < int.MaxValue))
            {
                return;
            }

            txtBorrowTime.Text = irBorrowTime.ToString();
            if (cmbBorrowTime.SelectedIndex == 0)
            {
                settingsOperation.updateStudentBorrowTime(irBorrowTime);
            }

            if (cmbBorrowTime.SelectedIndex == 1)
            {
                settingsOperation.updateLecturerBorrowTime(irBorrowTime);
            }
        }
        private void btnAmountUp_Click(object sender, RoutedEventArgs e)
        {
            int irAmountBook = Convert.ToInt32(txtAmountBook.Text);
            irAmountBook += 1;
            if (!(irAmountBook > 0 && irAmountBook < int.MaxValue))
            {
                return;
            }

            txtAmountBook.Text = irAmountBook.ToString();
            if (cmbAmountBook.SelectedIndex == 0)
            {
                settingsOperation.updateStudentBookAmount(irAmountBook);
            }

            if (cmbAmountBook.SelectedIndex == 1)
            {
                settingsOperation.updateLecturerBookAmount(irAmountBook);
            }
        }
        private void btnAmountDown_Click(object sender, RoutedEventArgs e)
        {
            int irAmountBook = Convert.ToInt32(txtAmountBook.Text);
            irAmountBook -= 1;
            if (!(irAmountBook > 0 && irAmountBook < int.MaxValue))
            {
                return;
            }

            txtAmountBook.Text = irAmountBook.ToString();
            if (cmbAmountBook.SelectedIndex == 0)
            {
                settingsOperation.updateStudentBookAmount(irAmountBook);
            }

            if (cmbAmountBook.SelectedIndex == 1)
            {
                settingsOperation.updateLecturerBookAmount(irAmountBook);
            }
        }
        //Settings Tab Borrowed
        private void updateBorrowed()
        {
            dtgrdBorrowedBooks.ItemsSource = borrowOperation.getBorrowedWithView();
        }
    }
}

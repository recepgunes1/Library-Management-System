using Library_Management_System.customClasses;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Library_Management_System
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private int irTryCounter;
        public LoginWindow()
        {
            InitializeComponent();
            irTryCounter = 0;
        }
        //Main Window Events
        private void wndwLogin_Loaded(object sender, RoutedEventArgs e)
        {
            Title = settingsData.getLoginWindowName();
            if (File.Exists(settingsOperation.getSchoolIconPath()))
            {
                imgLoginIcon.Source = new BitmapImage(new Uri(settingsOperation.getSchoolIconPath()));
                imgRegistrIcon.ImageSource = new BitmapImage(new Uri(settingsOperation.getSchoolIconPath()));
            }
        }
        //Login Tab Events
        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            Tuple<bool, string> tplResult;
            LogIn logIn = new LogIn(txtLoginUsername.Text, pswdLoginPassword.Password, this);
            this.Dispatcher.Invoke(() =>
            {
                tplResult = logIn.SignIn();
                if (!tplResult.Item1)
                {
                    MessageBoxGenerator(tplResult);
                }
            });
            buttonBlocking();
        }
        private void btnSignInActivator(object sender, RoutedEventArgs e)
        {
            if (txtLoginUsername.Text.Trim().Length > 3 && pswdLoginPassword.Password.Trim().Length > 3)
            {
                btnSignIn.IsEnabled = true;
            }
            else
            {
                btnSignIn.IsEnabled = false;
            }
        }
        //Register Tab Events
        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            Tuple<bool, string> tplResult;
            SignUp signUp = new SignUp(txtRegisterName.Text, txtRegisterSurname.Text, txtRegisterSchoolNumber.Text,
                txtRegisterMail.Text, pswdRegisterPassword1.Password == pswdRegisterPassword2.Password ? pswdRegisterPassword1.Password : null,
                cmbAuthorityLevel.Text.ToLower().Trim());
            LogIn logIn;
            Dispatcher.Invoke(() =>
            {
                tplResult = signUp.Register();
                if (tplResult.Item1)
                {
                    logIn = new LogIn(txtRegisterSchoolNumber.Text.Trim(), pswdRegisterPassword1.Password.Trim(), this);
                    logIn.SignIn();
                }
                else
                {
                    MessageBoxGenerator(tplResult);
                }
                
            });
        }
        private void btnSignUpActivator(object sender, RoutedEventArgs e)
        {
            if (txtRegisterName.Text.Trim().Length >= 3 && txtRegisterSurname.Text.Trim().Length >= 3 && txtRegisterSchoolNumber.Text.Trim().Length == 9 &&
                otherMethods.isMail(txtRegisterMail.Text.Trim()) && pswdRegisterPassword1.Password.Trim() == pswdRegisterPassword2.Password.Trim()
                && pswdRegisterPassword1.Password.Length > 3)
            {
                btnSignUp.IsEnabled = true;
            }
            else
            {
                btnSignUp.IsEnabled = false;
            }
        }
        //Other Events
        private void onlyTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-ZğüşöçİĞÜŞÖÇ]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void onlyNumbers(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        //Other Methods
        private void MessageBoxGenerator(Tuple<bool, string> tuple)
        {
            if (tuple.Item1 == false)
            {
                if(tuple.Item2 == "Your password is wrong.")
                {
                    irTryCounter++;
                    if (irTryCounter == 3)
                    {
                        MessageBoxGenerator(new Tuple<bool, string>(false, $"You've tried wrong password 3 times,{Environment.NewLine}You was blocked for 10 seconds."));
                        return;
                    }
                }
                MessageBox.Show(tuple.Item2, "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        async void buttonBlocking()
        {
            if (irTryCounter == 3)
            {
                irTryCounter = 0;
                grdLogin.IsEnabled = false;
                pswdLoginPassword.Clear();
                await Task.Delay(10000);
                grdLogin.IsEnabled = true;
            }
        }
    }
}

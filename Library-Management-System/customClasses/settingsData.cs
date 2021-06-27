namespace Library_Management_System.customClasses
{
    internal static class settingsData
    {
        public static string getLoginWindowName()
        {
            if (!string.IsNullOrEmpty(settingsOperation.getSchoolName()))
            {
                return $"Login Window | {otherMethods.makeTitle(settingsOperation.getSchoolName())}";
            }
            else
            {
                return "Login Window";
            }
        }
        public static string getLibraryWindowName(string strName)
        {
            if (!string.IsNullOrEmpty(settingsOperation.getSchoolName()))
            {
                return $"Library of {otherMethods.makeTitle(settingsOperation.getSchoolName())} | {otherMethods.makeTitle(strName)}";
            }
            else
            {
                return $"Library Window | {otherMethods.makeTitle(strName)}";
            }
        }
        public static string getManagementWindowName(string strName)
        {
            if (!string.IsNullOrEmpty(settingsOperation.getSchoolName()))
            {
                return $"Management of {otherMethods.makeTitle(settingsOperation.getSchoolName())} | {otherMethods.makeTitle(strName)}";
            }
            else
            {
                return $"Management Window | {otherMethods.makeTitle(strName)}";
            }
        }
        public static string getWelcomeText(string strName)
        {
            if (!string.IsNullOrEmpty(settingsOperation.getSchoolName()))
            {
                return $"{otherMethods.makeTitle(strName)}, Welcome to {otherMethods.makeTitle(settingsOperation.getSchoolName())} Library System";
            }
            else
            {
                return $"{otherMethods.makeTitle(strName)}, Welcome to Library System";
            }
        }
    }
}

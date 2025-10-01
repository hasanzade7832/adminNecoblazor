namespace AdminNeco.Services
{
    public static class ApiConstants
    {
        public static class Auth
        {
            public const string WebLogin = "api/Login/LoginONew";
            public const string SendOtp = "api/SendOtp";
            public const string LoginWithOtp = "api/loginWithOtp";
            public const string TokenSetup = "api/tokenSetup";
            public const string WebLogout = "api/Login/Logout";
            public const string GetIdByUserToken = "api/user/GetByToken";
        }

        public static class File
        {
            public const string Upload = "api/File/Upload";
            public const string Insert = "api/File/Insert";
            public const string GetById = "api/File/GetById";
            public const string Download = "api/File/Download";
        }

        public static class ProgramType
        {
            public const string GetAll = "api/ProgramType/GetAll";
            public const string Insert = "api/ProgramType/Insert";
            public const string Update = "api/ProgramType/Update";
            public const string Delete = "api/ProgramType/Delete";
        }
    }
}

using System.Reflection;

namespace NineCafeApi.Authorization
{
    public static class Perm
    {
        // USER
        public const string USER_READ = "user.read";
        public const string USER_CREATE = "user.create";
        public const string USER_UPDATE = "user.update";
        public const string USER_DELETE = "user.delete";
        public const string USER_CHANGE_PASSWORD = "user.change_password";

        // ROLE
        public const string ROLE_READ = "role.read";
        public const string ROLE_CREATE = "role.create";
        public const string ROLE_UPDATE = "role.update";
        public const string ROLE_DELETE = "role.delete";
        public const string ROLE_ASSIGN_PERMISSION = "role.assign_permission";

        // PERMISSION
        public const string PERMISSION_READ = "permission.read";
        public const string PERMISSION_CREATE = "permission.create";
        public const string PERMISSION_UPDATE = "permission.update";
        public const string PERMISSION_DELETE = "permission.delete";

        // MODULE
        public const string MODULE_READ = "module.read";
        public const string MODULE_CREATE = "module.create";
        public const string MODULE_UPDATE = "module.update";
        public const string MODULE_DELETE = "module.delete";
        

        // Helper to retrieve all permission constants (useful for seeding DB)
        public static IReadOnlyList<string> GetAll()
        {
            return typeof(Perm)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(fi => (string)fi.GetValue(null)!)
                .ToList();
        }
    }
}
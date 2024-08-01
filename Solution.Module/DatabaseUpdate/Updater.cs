using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using Solution.Module.BusinessObjects;

namespace Solution.Module.DatabaseUpdate
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppUpdatingModuleUpdatertopic.aspx
    public class Updater : ModuleUpdater
    {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion)
        {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            //string name = "MyName";
            //DomainObject1 theObject = ObjectSpace.FindObject<DomainObject1>(CriteriaOperator.Parse("Name=?", name));
            //if(theObject == null) {
            //    theObject = ObjectSpace.CreateObject<DomainObject1>();
            //    theObject.Name = name;
            //}
            PermissionPolicyUser sampleUser = ObjectSpace.FindObject<PermissionPolicyUser>(new BinaryOperator("UserName", "User"));
            if (sampleUser == null)
            {
                sampleUser = ObjectSpace.CreateObject<PermissionPolicyUser>();
                sampleUser.UserName = "User";
                sampleUser.SetPassword("");
            }
            PermissionPolicyRole userRole = CreateRole("Users");
            userRole = ChangePermissions("Users");

            sampleUser.Roles.Add(userRole);

            PermissionPolicyUser userAdmin = ObjectSpace.FindObject<PermissionPolicyUser>(new BinaryOperator("UserName", "Admin"));
            if (userAdmin == null)
            {
                userAdmin = ObjectSpace.CreateObject<PermissionPolicyUser>();
                userAdmin.UserName = "Admin";
                // Set a password if the standard authentication type is used
                userAdmin.SetPassword("");
            }
            // If a role with the Administrators name doesn't exist in the database, create this role
            PermissionPolicyRole adminRole = CreateRole("Administrators");
            adminRole.IsAdministrative = true;
            userAdmin.Roles.Add(adminRole);
            ObjectSpace.CommitChanges(); //This line persists created object(s).
        }
        public override void UpdateDatabaseBeforeUpdateSchema()
        {
            base.UpdateDatabaseBeforeUpdateSchema();
            //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
            //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
            //}
        }

        private PermissionPolicyRole CreateRole(string _name)
        {
            PermissionPolicyRole role = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", _name));
            if (role == null)
            {
                role = ObjectSpace.CreateObject<PermissionPolicyRole>();
                role.Name = _name;
            }
            return role;
        }

        private PermissionPolicyRole ChangePermissions(string _name)
        {
            PermissionPolicyRole role = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", _name));

            // Разрешение на навигацию
            role.AddNavigationPermission(@"Application/NavigationItems/Items/Default/", SecurityPermissionState.Allow);
            role.AddNavigationPermission(@"Application/NavigationItems/Items/Reports/", SecurityPermissionState.Allow);
            role.AddNavigationPermission(@"Application/NavigationItems/Items/Directory/Items/Cargo_ListView", SecurityPermissionState.Allow);
            role.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Deny);
            role.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/PermissionPolicyRole_ListView", SecurityPermissionState.Deny);
            role.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/PermissionPolicyUser_ListView", SecurityPermissionState.Deny);

            // Разрешения на доступ к объектам и их свойствам
            role.AddTypePermissionsRecursively<PermissionPolicyUser>(SecurityOperations.FullAccess, SecurityPermissionState.Deny);
            role.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.FullAccess, SecurityPermissionState.Deny);

            // Разрешения для объектов Storage
            role.AddTypePermissionsRecursively<Storage>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);

            // Разрешения для объектов CargoPicket
            role.AddTypePermissionsRecursively<CargoPicket>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);

            // Разрешения для объектов Picket
            role.AddTypePermissionsRecursively<Picket>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);

            // Разрешения для объектов Platform
            role.AddTypePermissionsRecursively<Platform>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);

            // Разрешения для объектов Cargo
            role.AddTypePermissionsRecursively<Cargo>(SecurityOperations.Read, SecurityPermissionState.Allow);
            role.AddTypePermissionsRecursively<Cargo>(SecurityOperations.Write, SecurityPermissionState.Deny);
            role.AddTypePermissionsRecursively<Cargo>(SecurityOperations.Create, SecurityPermissionState.Deny);
            role.AddTypePermissionsRecursively<Cargo>(SecurityOperations.Delete, SecurityPermissionState.Deny);

            //// Разрешения для объектов PlatformAuditTrail
            //role.AddTypePermissionsRecursively<PlatformAuditTrail>(SecurityOperations.Read, SecurityPermissionState.Allow);
            //role.AddTypePermissionsRecursively<PlatformAuditTrail>(SecurityOperations.Write, SecurityPermissionState.Deny);
            //role.AddTypePermissionsRecursively<PlatformAuditTrail>(SecurityOperations.Create, SecurityPermissionState.Deny);
            //role.AddTypePermissionsRecursively<PlatformAuditTrail>(SecurityOperations.Delete, SecurityPermissionState.Deny);
            //
            //// Разрешения для объектов CargoAuditTrail
            //role.AddTypePermissionsRecursively<CargoAuditTrail>(SecurityOperations.Read, SecurityPermissionState.Allow);
            //role.AddTypePermissionsRecursively<CargoAuditTrail>(SecurityOperations.Write, SecurityPermissionState.Deny);
            //role.AddTypePermissionsRecursively<CargoAuditTrail>(SecurityOperations.Create, SecurityPermissionState.Deny);
            //role.AddTypePermissionsRecursively<CargoAuditTrail>(SecurityOperations.Delete, SecurityPermissionState.Deny);

            return role;
        }
    }
}

using System.Linq;
using OakLeaf.MM.Main.Collections;

namespace HWB.NETSCALE.BOEF.User
{
    /// <summary>
    /// Summary description for User.
    /// </summary>
    public partial class User
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        private string _accessLevel;


        public string AccessLevel
        {
            get { return _accessLevel; }
            set { _accessLevel = value; }
        }

        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
        }

        public UserEntity GetUserById(int? pk)
        {
            IQueryable<UserEntity> query = from u in ObjectContext.UserEntities
                                           where u.UserPK == pk
                                           select u;
            return GetEntity(query);
        }

        public UserEntity CheckLogin(string user, string pw)
        {
            IQueryable<UserEntity> query = from u in ObjectContext.UserEntities
                                           where u.UserID == user &&
                                                 u.Password == pw
                                           select u;

            return GetEntity(query);
        }

        public mmBindingList<UserEntity> GetAllUser()
        {
            IQueryable<UserEntity> query = from ul in ObjectContext.UserEntities
                                           orderby ul.LastName
                                           select ul;
            return GetEntityList(query);
        }
    }
}
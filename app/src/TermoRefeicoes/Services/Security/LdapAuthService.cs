using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using termoRefeicoes.Interfaces.Services;
using termoRefeicoes.Interfaces.Services.Security;
using termoRefeicoes.Models;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;

namespace termoRefeicoes.Services.Security
{
    public class LdapAuthService : IAuthentication
    {

        private readonly LdapConfig _config;
        protected readonly LdapSearchConstraints _ldapConnection;
        protected User _user;


        protected readonly IUserSenior _userSeniorService;

        public LdapAuthService(IOptions<LdapConfig> ldapConfig, IUserSenior userSeniorService)
        {
            _config = ldapConfig.Value;
            _userSeniorService = userSeniorService;

        }

        public async Task<User> Autenthicate(string username, string password)
        {
            string bindDN = "LDAP://" + _config.BindDn;
            LdapConnection connection = new LdapConnection();
            object isValidPassword = null;
            try
            {
                string userDn = $"{username}@{_config.Server}";
                connection.Connect(_config.Server, LdapConnection.DefaultPort);
                connection.Bind(userDn, password);
                isValidPassword = connection.Bound;
            }
            catch (Exception e)
            {
                var message = "";
                if (e.Message == "Invalid Credentials")
                {
                    message = "Nome de usuário ou senha incorretos.";
                }
                throw new ApplicationException(message);
            }
            List<User> listGroups = GetAllProps(username, password, connection);

            // bool groupsVerify = VerirfyGroup(listGroups[0].Groups);
            // if (!groupsVerify)
            // {
            //     throw new ApplicationException("O usuário não tem permissão para acessar este sistema!!!");
            // }

            // string nameOfUser = GetUserFullName(username);            
            var _userSenior = _userSeniorService.GetUsers(username);
            var matricula = _userSenior.Result.Where(index => index.NumCadastro != 0).Last();

            _user = new User
            {
                Matricula = matricula.NumCadastro,
                Username = username,
                Name = listGroups[0]?.Name,
                Groups = listGroups[0]?.Groups
            };


            return _user;
        }



        private LdapConnection ConnectLdap(string username, string password)
        {
            string bindDN = "LDAP://" + _config.BindDn;
            LdapConnection connection = new LdapConnection();
            try
            {
                string userDn = $"{username}@{_config.Server}";
                connection.Connect(_config.Server, LdapConnection.DefaultPort);
                connection.Bind(userDn, password);
                return connection;
            }
            catch (Exception e)
            {


                return connection;
            }

        }

        private List<User> GetAllProps(string username, string password, LdapConnection ldapConnection)
        {
            List<Group> listGroups = new List<Group>();
            var users = new List<User>();
            try
            {
                string userDn = $"CN={username},{_config.SearchBase}";
                string[] requiredAttributes = { "cn", "memberof", "samaccountname" };

                var filters = String.Format(_config.SearchFilter, username);

                LdapSearchResults lsc = (LdapSearchResults)ldapConnection.Search(_config.SearchBase, LdapConnection.ScopeSub, filters, null, false);
                while (lsc.HasMore())
                {
                    LdapEntry nexEntry;
                    try
                    {
                        nexEntry = lsc.Next();
                    }
                    catch (LdapException e)
                    {
                        Console.WriteLine("Error : " + e.LdapErrorMessage);
                        continue;
                    }

                    LdapAttributeSet attributeSet = nexEntry.GetAttributeSet();
                    System.Collections.IEnumerator ienum = attributeSet.GetEnumerator();
                    var nameUser = attributeSet.GetAttribute("sAMAccountName")?.StringValue;

                    if (nameUser.ToLower() == username.ToLower())
                    {
                        var array = attributeSet.GetAttribute("memberof")?.StringValueArray;

                        // cycle through objects in each field e.g. group membership  
                        // (for many fields there will only be one object such as name)  
                        foreach (Object myCollection in array)
                        {
                            string groupNameTmp = myCollection.ToString();
                            string[] groupNameParts = groupNameTmp.Split(',');
                            string[] groupNameParts2 = groupNameParts[0].Split('=');
                            string _groupName = groupNameParts2[1];


                            Group group = new Group
                            {
                                GroupName = _groupName
                            };

                            listGroups.Add(group);
                        }


                        var groupUser = listGroups;

                        User objUSer = new User
                        {
                            Name = attributeSet.GetAttribute("cn")?.StringValue,
                            Username = attributeSet.GetAttribute("sAMAccountName")?.StringValue,
                            Groups = groupUser

                        };

                        users.Add(objUSer);
                        return users;
                    }

                }
                return users;
            }
            catch (LdapException e)
            {
                return users;
            }
        }

        // private bool VerirfyGroup(List<Group> groups)
        // {
        //     List<Group> listGroups = new List<Group>();

        //     foreach (var ldapField in groups)
        //     {
        //         if (ldapField.GroupName == "G_RH" || ldapField.GroupName == "G_TI")
        //         {
        //             return true;
        //         }
        //         cycle through objects in each field e.g. group membership  
        //         (for many fields there will only be one object such as name)                  
        //     }
        //     return false;

        // }
        public bool BelongToGroup(string groupName)
        {

            Group group = _user.Groups.Where(u => u.GroupName.Contains(groupName)).FirstOrDefault();
            return group is Group;
        }
    }
}
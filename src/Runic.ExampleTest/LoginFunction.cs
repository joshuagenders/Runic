﻿using Runic.Attributes;
using Runic.Data;
using Runic.Orchestration;

namespace Runic.ExampleTest
{
    public class LoginFunction
    {
        [MinesRunes("AuthenticatedUser")]
        [MutableParameter("userDetails", typeof(UserDetails))]
        public async void Login(UserDetails userDetails = null)
        {
            if (userDetails == null)
            {
                userDetails = GetUserDetails();
            }
            var result = await new TimedAction("Login", () => DoLogin(userDetails)).Execute();
            var response = (ExampleResponse)result.ExecutionResult;
            var rune = new Rune()
            {
                Name = "AuthenticatedUser",
                Detail = result
            };
            rune.IndexedProperties[Ref.Indexes[(int)Ref.Keys.CustomerId]] = response.CustomerId;
            rune.IndexedProperties[Ref.Indexes[(int)Ref.Keys.AuthCookie]] = response.AuthCookie;

            Runes.Mine(rune);
        }

        public ExampleResponse DoLogin(UserDetails userDetails)
        {
            return new ExampleResponse();
        }

        public UserDetails GetUserDetails()
        {
            //get from file, db etc.
            //can pass run id to db so that client reservation works, create datastore function to manage core data
            //you can then set that as an input stream for a flow that you've constructed
            return new UserDetails()
            {
                UserName = "",
                Password = ""
            };
        }
        
    }

    public class UserDetails
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
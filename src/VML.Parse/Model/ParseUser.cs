﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ParseUser.cs" company="VML">
//   Copyright VML 2014. All rights reserved.
//  </copyright>
//  <created>01/09/2014 5:08 PM</created>
//  <updated>01/23/2014 2:32 PM by Ben Ramey</updated>
// --------------------------------------------------------------------------------------------------------------------

#region Usings

using System;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using RestSharp;
using VML.Parse.Attributes;
using VML.Parse.ResultSets;

#endregion

namespace VML.Parse.Model
{
    [DataContract]
    [ParseClassName("_User")]
    public class ParseUser : ParseObject
    {
        #region Public Properties

        [DataMember(Name = "email")]
        public string Email
        {
            get
            {
                return GetProperty<string>("email");
            }
            set
            {
                SetProperty("email", value);
            }
        }

        [DataMember(Name = "sessionToken")]
        public string SessionToken
        {
            get
            {
                return GetProperty<string>("sessionToken");
            }
            set
            {
                SetProperty("sessionToken", value);
            }
        }

        [DataMember(Name = "username")]
        public string Username
        {
            get
            {
                return GetProperty<string>("username");
            }
            set
            {
                SetProperty("username", value);
            }
        }

        #endregion

        #region Public Methods

        public static ParseUser GetByToken(string sessionToken)
        {
            RestRequest request = ParseContext.API.GetDefaultRequest("users/me");
            request.Method = Method.GET;
            return ExecuteUserRequest(request);
        }

        public static bool ResetPassword(string email)
        {
            RestRequest request = ParseContext.API.GetDefaultRequest("requestPasswordReset");
            request.Method = Method.POST;
            request.AddBody(new { email });

            IRestResponse<ParseBasicResponse> response = ParseContext.API.ExecuteRequest<ParseBasicResponse>(request);

            return string.IsNullOrWhiteSpace(response.Data.Error) && response.Data.Code == 0;
        }

        public static ParseUser SignIn(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("username");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("password");
            }

            RestRequest request = ParseContext.API.GetDefaultRequest("login");
            request.Method = Method.GET;
            request.AddParameter("username", username);
            request.AddParameter("password", password);

            return ExecuteUserRequest(request);
        }

        public static ParseUser SignUp(ParseUser newUser, string password)
        {
            if (newUser == null)
            {
                throw new ArgumentNullException("newUser");
            }

            newUser["password"] = password;
            newUser.Remove("createdAt");
            newUser.Remove("updatedAt");
            RestRequest request = ParseContext.API.GetDefaultRequest("users");
            request.Method = Method.POST;
            request.AddBody(newUser);

            IRestResponse<ParseUser> response = ParseContext.API.ExecuteRequest<ParseUser>(
                request, HttpStatusCode.Created);
            return response.Data;
        }

        public static bool ValidateSession(string sessionToken)
        {
            try
            {
                return GetByToken(sessionToken) != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Methods

        private static ParseUser ExecuteUserRequest(RestRequest request)
        {
            IRestResponse<ParseUser> response = ParseContext.API.ExecuteRequest<ParseUser>(request);
            return response.Data;
        }

        #endregion
    }
}
﻿#region License

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParseApi.cs">
// LINQ-to-Parse, a LINQ interface to the Parse.com REST API.
//  
// Copyright (C) 2013 Benjamin Ramey
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
// http://www.gnu.org/licenses/lgpl-2.1-standalone.html
// 
// You can contact me at ben.ramey@gmail.com.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#endregion

#region Usings

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System;
using System.Net;
using System.Reflection;
using GoodlyFere.Parse.Attributes;
using GoodlyFere.Parse.Interfaces;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Contrib;

#endregion

namespace GoodlyFere.Parse
{
    public class ParseApi
    {
        #region Constants and Fields

        private readonly IParseApiSettingsProvider _settingsProvider;

        #endregion

        #region Constructors and Destructors

        public ParseApi(IParseApiSettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        #endregion

        #region Public Methods

        public static void CheckForParseError<T>(IRestResponse<T> response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }

            "ParseApi".Log().Error(
                String.Format(
                    "Parse error: {0}, {1}",
                    response.StatusCode,
                    response.StatusDescription));

            throw new Exception(response.StatusDescription);
        }

        public static void CheckForResponseError<T>(IRestResponse<T> response)
        {
            if (response.ResponseStatus == ResponseStatus.Completed)
            {
                return;
            }

            "ParseApi".Log().Error(
                String.Format("Response error: {0}", response.ResponseStatus),
                response.ErrorException);

            throw response.ErrorException;
        }

        public IList<T> Query<T>(string queryString)
        {
            string uri = GetQueryRequestUri<T>();
            var request = GetDefaultRequest(uri);
            SetParameters<T>(queryString, request);

            IRestResponse<ParseQueryResults<T>> response = ExecuteRequest<ParseQueryResults<T>>(request);
            return GetResults(response);
        }

        public T Update<T>(T modelToSave) where T : BaseModel, new()
        {
            if (String.IsNullOrWhiteSpace(modelToSave.ObjectId))
            {
                throw new ArgumentException("ObjectId must be set to save.");
            }

            string uri = GetQueryRequestUri<T>();
            uri += "/" + modelToSave.ObjectId;
            RestRequest request = GetDefaultRequest(uri);
            request.Method = Method.PUT;
            request.AddBody(modelToSave);

            ExecuteRequest<T>(request);
            // response only contains updatedAt field, so we just return the same updated object
            return modelToSave;
        }

        #endregion

        #region Methods

        internal IRestResponse<T> ExecuteRequest<T>(IRestRequest request) where T : new()
        {
            RestClient client = new RestClient(_settingsProvider.ApiUrl);
            client.AddHandler("application/json", new ParseDeserializer());
            SetParseHeaders(request);

            IRestResponse<T> response = client.Execute<T>(request);
            CheckForResponseError(response);
            CheckForParseError(response);

            return response;
        }

        private static void SetParameters<T>(string queryString, RestRequest request)
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(queryString);

            foreach (string key in parameters.AllKeys)
            {
                request.AddParameter(key, parameters[key]);
            }
        }

        private RestRequest GetDefaultRequest(string uri)
        {
            RestRequest request = new RestRequest(uri)
                {
                    RequestFormat = DataFormat.Json,
                    JsonSerializer = new ParseSerializer()
                };
            return request;
        }

        private string GetQueryRequestUri<T>()
        {
            Type type = typeof(T);
            CustomAttributeData attr =
                type.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(ParseClassNameAttribute));

            if (attr == null)
            {
                return "classes/" + type.Name;
            }

            string name = (string)attr.ConstructorArguments[0].Value;
            if (name.Equals("users"))
            {
                return name;
            }

            return "classes/" + name;
        }

        private IList<T> GetResults<T>(IRestResponse<ParseQueryResults<T>> response)
        {
            ParseQueryResults<T> results = response.Data;

            if (results.Code > 0)
            {
                this.Log().Error("Parse query failed: {0}", results.Error);
                return new List<T>();
            }

            return results.Results;
        }

        private void SetParseHeaders(IRestRequest request)
        {
            request.AddHeader("X-Parse-Application-Id", _settingsProvider.ApplicationId);
            request.AddHeader("X-Parse-REST-API-Key", _settingsProvider.RestApiKey);
            request.AddHeader("Content-Type", "application/json");

            if (!string.IsNullOrWhiteSpace(_settingsProvider.CurrentUserSessionToken))
            {
                request.AddHeader("X-Parse-Session-Token", _settingsProvider.CurrentUserSessionToken);
            }
        }

        #endregion
    }
}
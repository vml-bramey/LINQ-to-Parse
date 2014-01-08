﻿#region License

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseModel.cs">
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

using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

#endregion

namespace GoodlyFere.Parse
{
    public interface IBaseModel
    {
        #region Public Properties

        DateTime CreatedAt { get; set; }
        string ObjectId { get; set; }
        DateTime UpdatedAt { get; set; }

        #endregion
    }

    [DataContract]
    public abstract class BaseModel : IBaseModel
    {
        #region Public Properties

        [DataMember(Name = "createdAt")]
        [JsonIgnore]
        public DateTime CreatedAt { get; set; }

        [DataMember(Name = "objectId")]
        public string ObjectId { get; set; }

        [DataMember(Name = "updatedAt")]
        [JsonIgnore]
        public DateTime UpdatedAt { get; set; }

        #endregion
    }
}
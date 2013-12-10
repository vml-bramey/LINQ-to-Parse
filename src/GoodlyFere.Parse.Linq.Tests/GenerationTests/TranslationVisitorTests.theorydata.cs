﻿#region License

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TranslationVisitorTests.theorydata.cs">
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
using System.Collections.Generic;
using System.Linq;
using GoodlyFere.Parse.Linq.Tests.Support;

#endregion

namespace GoodlyFere.Parse.Linq.Tests.GenerationTests
{
    public partial class TranslationVisitorTests
    {
        #region Public Properties

        public static IEnumerable<object[]> CompoundComparisons
        {
            get
            {
                return new[]
                    {
                        new object[]
                            {
                                (from to in ParseQueryFactory.Queryable<TestObject>()
                                 where to.Age > 31 && to.Age < 100
                                 select to),
                                "where={\"age\":{\"$gt\":31,\"$lt\":100}}"
                            },
                        new object[]
                            {
                                (from to in ParseQueryFactory.Queryable<TestObject>()
                                 where to.Age > 31 && to.Age < 100 && to.FirstName == "Ben"
                                 select to),
                                "where={\"age\":{\"$gt\":31,\"$lt\":100},\"firstName\":\"Ben\"}"
                            },
                        new object[]
                            {
                                (from to in ParseQueryFactory.Queryable<TestObject>()
                                 where to.Age < 100 && to.FirstName == "Ben"
                                 select to),
                                "where={\"age\":{\"$lt\":100},\"firstName\":\"Ben\"}"
                            },
                        new object[]
                            {
                                (from to in ParseQueryFactory.Queryable<TestObject>()
                                 where to.Age == 100 && to.FirstName == "Ben"
                                 select to),
                                "where={\"age\":100,\"firstName\":\"Ben\"}"
                            },
                    };
            }
        }

        public static IEnumerable<object[]> SimpleComparisons
        {
            get
            {
                return new[]
                    {
                        new object[]
                            {
                                (from to in ParseQueryFactory.Queryable<TestObject>()
                                 where to.Age == 31
                                 select to),
                                "where={\"age\":31}"
                            },
                        new object[]
                            {
                                (from to in ParseQueryFactory.Queryable<TestObject>()
                                 where 31 == to.Age
                                 select to),
                                "where={\"age\":31}"
                            },
                        new object[]
                            {
                                (from to in ParseQueryFactory.Queryable<TestObject>()
                                 where to.Age != 31
                                 select to),
                                "where={\"age\":{\"$ne\":31}}"
                            },
                        new object[]
                            {
                                (from to in ParseQueryFactory.Queryable<TestObject>()
                                 where to.Age > 31
                                 select to),
                                "where={\"age\":{\"$gt\":31}}"
                            },
                        new object[]
                            {
                                (from to in ParseQueryFactory.Queryable<TestObject>()
                                 where to.Age < 31
                                 select to),
                                "where={\"age\":{\"$lt\":31}}"
                            },
                        new object[]
                            {
                                (from to in ParseQueryFactory.Queryable<TestObject>()
                                 where to.Age <= 31
                                 select to),
                                "where={\"age\":{\"$lte\":31}}"
                            },
                        new object[]
                            {
                                (from to in ParseQueryFactory.Queryable<TestObject>()
                                 where to.Age >= 31
                                 select to),
                                "where={\"age\":{\"$gte\":31}}"
                            },
                    };
            }
        }

        #endregion
    }
}
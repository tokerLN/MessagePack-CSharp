﻿// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MessagePack.Tests
{
    public class ContractlessStandardResolverTest
    {
        public class Address
        {
            public string Street { get; set; }
        }

        public class Person
        {
            public string Name { get; set; }

            public object[] /*Address*/ Addresses { get; set; }
        }

        public class V1
        {
            public int ABCDEFG1 { get; set; }

            public int ABCDEFG3 { get; set; }
        }

        public class V2
        {
            public int ABCDEFG1 { get; set; }

            public int ABCDEFG2 { get; set; }

            public int ABCDEFG3 { get; set; }
        }

        public class Dup
        {
            public int ABCDEFGH { get; set; }

            public int ABCDEFGHIJKL { get; set; }
        }

        public class BinSearchSmall
        {
            public int MyP1 { get; set; }

            public int MyP2 { get; set; }

            public int MyP3 { get; set; }

            public int MyP4 { get; set; }

            public int MyP5 { get; set; }

            public int MyP6 { get; set; }

            public int MyP7 { get; set; }

            public int MyP8 { get; set; }

            public int MyP9 { get; set; }
        }

        public class BinSearchWithBranch
        {
            public int MyProperty1 { get; set; }

            public int MyProperty2 { get; set; }

            public int MyProperty3 { get; set; }

            public int MyProperty4 { get; set; }

            public int MyProperty5 { get; set; }

            public int MyProperty6 { get; set; }

            public int MyProperty7 { get; set; }

            public int MyProperty8 { get; set; }

            public int MyProperty9 { get; set; }
        }

        public class LongestString
        {
            public int MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1 { get; set; }

            public int MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty2 { get; set; }

            public int MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty2MyProperty { get; set; }

            public int OAFADFZEWFSDFSDFKSLJFWEFNWOZFUSEWWEFWEWFFFFFFFFFFFFFFZFEWBFOWUEGWHOUDGSOGUDSZNOFRWEUFWGOWHOGHWOG000000000000000000000000000000000000000HOGZ { get; set; }
        }

        [Fact]
        public void SimpleTest()
        {
            var p = new Person
            {
                Name = "John",
                Addresses = new[]
                {
                    new Address { Street = "St." },
                    new Address { Street = "Ave." },
                },
            };

            var result = MessagePackSerializer.Serialize(p, Resolvers.ContractlessStandardResolver.Options);

            MessagePackSerializer.ConvertToJson(result).Is(@"{""Name"":""John"",""Addresses"":[{""Street"":""St.""},{""Street"":""Ave.""}]}");

            Person p2 = MessagePackSerializer.Deserialize<Person>(result, Resolvers.ContractlessStandardResolver.Options);
            p2.Name.Is("John");
            var addresses = p2.Addresses as IList;
            var d1 = addresses[0] as IDictionary;
            var d2 = addresses[1] as IDictionary;
            (d1["Street"] as string).Is("St.");
            (d2["Street"] as string).Is("Ave.");
        }

        [Fact]
        public void Versioning()
        {
            var v1 = MessagePackSerializer.Serialize(new V1 { ABCDEFG1 = 10, ABCDEFG3 = 99 }, Resolvers.ContractlessStandardResolver.Options);
            var v2 = MessagePackSerializer.Serialize(new V2 { ABCDEFG1 = 350, ABCDEFG2 = 34, ABCDEFG3 = 500 }, Resolvers.ContractlessStandardResolver.Options);

            V1 v1_1 = MessagePackSerializer.Deserialize<V1>(v1, Resolvers.ContractlessStandardResolver.Options);
            V1 v1_2 = MessagePackSerializer.Deserialize<V1>(v2, Resolvers.ContractlessStandardResolver.Options);
            V2 v2_1 = MessagePackSerializer.Deserialize<V2>(v1, Resolvers.ContractlessStandardResolver.Options);
            V2 v2_2 = MessagePackSerializer.Deserialize<V2>(v2, Resolvers.ContractlessStandardResolver.Options);

            v1_1.ABCDEFG1.Is(10);
            v1_1.ABCDEFG3.Is(99);

            v1_2.ABCDEFG1.Is(350);
            v1_2.ABCDEFG3.Is(500);

            v2_1.ABCDEFG1.Is(10);
            v2_1.ABCDEFG2.Is(0);
            v2_1.ABCDEFG3.Is(99);

            v2_2.ABCDEFG1.Is(350);
            v2_2.ABCDEFG2.Is(34);
            v2_2.ABCDEFG3.Is(500);
        }

        [Fact]
        public void DuplicateAutomata()
        {
            var bin = MessagePackSerializer.Serialize(new Dup { ABCDEFGH = 10, ABCDEFGHIJKL = 99 }, Resolvers.ContractlessStandardResolver.Options);
            Dup v = MessagePackSerializer.Deserialize<Dup>(bin, Resolvers.ContractlessStandardResolver.Options);

            v.ABCDEFGH.Is(10);
            v.ABCDEFGHIJKL.Is(99);
        }

        [Fact]
        public void BinSearchSmallCheck()
        {
            var o = new BinSearchSmall
            {
                MyP1 = 1,
                MyP2 = 10,
                MyP3 = 1000,
                MyP4 = 100000,
                MyP5 = 32421,
                MyP6 = 52521,
                MyP7 = 46363631,
                MyP8 = 7373731,
                MyP9 = 73573731,
            };
            var bin = MessagePackSerializer.Serialize(o, Resolvers.ContractlessStandardResolver.Options);
            BinSearchSmall v = MessagePackSerializer.Deserialize<BinSearchSmall>(bin, Resolvers.ContractlessStandardResolver.Options);

            v.IsStructuralEqual(o);
        }

        [Fact]
        public void BinSearchWithBranchCheck()
        {
            var o = new BinSearchWithBranch
            {
                MyProperty1 = 1,
                MyProperty2 = 10,
                MyProperty3 = 1000,
                MyProperty4 = 100000,
                MyProperty5 = 32421,
                MyProperty6 = 52521,
                MyProperty7 = 46363631,
                MyProperty8 = 7373731,
                MyProperty9 = 73573731,
            };
            var bin = MessagePackSerializer.Serialize(o, Resolvers.ContractlessStandardResolver.Options);
            BinSearchWithBranch v = MessagePackSerializer.Deserialize<BinSearchWithBranch>(bin, Resolvers.ContractlessStandardResolver.Options);

            v.IsStructuralEqual(o);
        }

        [Fact]
        public void LongestStringCheck()
        {
            var o = new LongestString
            {
                MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1 = 431413,
                MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty2 = 352525252,
                MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty1MyProperty2MyProperty = 532525252,
                OAFADFZEWFSDFSDFKSLJFWEFNWOZFUSEWWEFWEWFFFFFFFFFFFFFFZFEWBFOWUEGWHOUDGSOGUDSZNOFRWEUFWGOWHOGHWOG000000000000000000000000000000000000000HOGZ = 3352666,
            };
            var bin = MessagePackSerializer.Serialize(o, Resolvers.ContractlessStandardResolver.Options);
            LongestString v = MessagePackSerializer.Deserialize<LongestString>(bin, Resolvers.ContractlessStandardResolver.Options);

            v.IsStructuralEqual(o);
        }
    }
}
